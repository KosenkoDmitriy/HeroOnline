using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using DEngine.HeroServer.Handlers;
using DEngine.HeroServer.Operations;
using DEngine.HeroServer.Properties;
using DEngine.PhotonFX.Common;
using DEngine.PhotonFX.Slave;
using Photon.SocketServer;
using System;
using System.Collections.Generic;
using System.Linq;

namespace DEngine.HeroServer
{
    public class ZoneService : SlaveServer
    {
        #region Properties

        protected override string MasterIP { get { return ServerConfig.MASTER_IP; } }
        protected override int MasterPort { get { return ServerConfig.MASTER_PORT; } }
        protected override int ZoneMaxCCU { get { return ServerConfig.ZONE_MAXCCU; } }

        public HeroDB HeroDatabase { get; private set; }

        public GameObjCollection AllUsers { get; private set; }

        public GameObjCollection PvPUsers { get; private set; }

        #endregion

        #region Constructors

        public ZoneService()
        {
            HeroDatabase = new HeroDB();

            AllUsers = new GameObjCollection();
            PvPUsers = new GameObjCollection();

            // 
            RegisterHandler(new SignInHandler(this));
            RegisterHandler(new SignOutHandler(this));
            RegisterHandler(new SendChatHandler(this));
            RegisterHandler(new ShopBuyHandler(this));
            RegisterHandler(new ChargeCashHandler(this));

            RegisterHandler(new UsersListHandler(this));
            RegisterHandler(new RolesListHandler(this));
            RegisterHandler(new UserUpdateHandler(this));
            RegisterHandler(new RoleUpdateHandler(this));
            RegisterHandler(new BattleHandler(this));
        }

        protected override void TearDown()
        {
            AllUsers.ForEach((GameObj obj) => { HeroDatabase.UserSignOut((GameUser)obj); });

            base.TearDown();
        }

        protected override IPhotonHandler CreateDefaultHandler()
        {
            return new DefaultHandlerZone(this);
        }

        #endregion

        #region Public Methods

        protected override void OnMasterDisconnected()
        {
            PvPUsers.Clear();

            AllUsers.ForEach((GameObj obj) => { HeroDatabase.UserSignOut((GameUser)obj); });
            AllUsers.Clear();

            base.OnMasterDisconnected();
        }

        //call in SigninHandler.cs
        public ErrorCode OnSignIn(string userName, string password, string remoteIp, out GameUser gameUser)
        {
            string serviceUrl = string.Format("{0}/Account/SignIn?username={1}&password={2}", ServerConfig.SERVICE_URL, userName, password);
            HttpResult httpRes = HttpService.GetResponse(serviceUrl);

            Log.InfoFormat("serviceUrl login " + serviceUrl);


            gameUser = null;

            int userId = httpRes.Code;
            if (userId > 0)
            {
                gameUser = (GameUser)AllUsers[userId];
                if (gameUser != null)
                    return ErrorCode.Success;

                string nickName = httpRes.Description;
                gameUser = HeroDatabase.UserSignIn(userId, userName, nickName, remoteIp);
                if (gameUser == null)
                {
                    Log.InfoFormat("HeroDatabase.UserSignIn return NULL");
                    return ErrorCode.InvalidParams;
                }

                gameUser.Zone = this;
                int userCount = AllUsers.LockAdd(gameUser);

                if (userCount >= 0)
                    Log.InfoFormat("{0}: ZoneCCU = {1}", ApplicationName, userCount);
                else
                    Log.InfoFormat("{0}: Duplicate SignIn for {1}", ApplicationName, userName);

                return ErrorCode.Success;
            }
            else
            {
                Log.InfoFormat("SignIn HttpService.GetResponse: {0}", userId);
                switch (userId)
                {
                    case -2:
                        return ErrorCode.UserNotFound;
                    case -1:
                        return ErrorCode.InvalidParams;
                    case 0:
                        return ErrorCode.InvalidPassword;
                    default:
                        return ErrorCode.InvalidParams;
                }
            }
        }

        public ErrorCode OnSignOut(int userId, string userName)
        {
            GameUser gameUser = (GameUser)AllUsers[userId];
            if (gameUser != null)
            {
                BattleService battle = gameUser.Tag as BattleService;
                if (battle != null && !battle.IsDisposed)
                    battle.OnUserLeave(gameUser);

                HeroDatabase.UserSignOut(gameUser);
            }

            PvPUsers.LockRemove(userId);

            int userCount = AllUsers.LockRemove(userId);

            if (userCount >= 0)
                Log.InfoFormat("{0}: ZoneCCU = {1}", ApplicationName, userCount);
            else
                Log.InfoFormat("{0}: Invalid SignOut for {1}", ApplicationName, userName);

            return ErrorCode.Success;
        }

       
        //event network , user require playerlist by ListType(top players arena , pillage)
        public ErrorCode GetUserList(int userId, int listType, ref GameObjList userList)
        {
            //top Hornor
            if (listType > (int)UserListType.Pillage)
            {
                userList = HeroDatabase.GetTopUsers(listType, 20);
                return ErrorCode.Success;
            }

            GameUser gameUser = (GameUser)AllUsers[userId];
            if (gameUser == null)
                return ErrorCode.InvalidParams;

            //top pillage
            if (listType > 0)
            {
                ErrorCode errCode = gameUser.AddCash(0, -GameConfig.PLE_REFRESSUSERS);
                if (errCode != ErrorCode.Success)
                    return errCode;
            }

            userList = HeroDatabase.GetRandUsers(this, gameUser, listType);
            return ErrorCode.Success;
        }

        public void OnSendChat(int senderId, int receiverId, string message)
        {
            if (senderId == receiverId || message.Length == 0)
                return;

            GameUser srcUser = (GameUser)AllUsers[senderId];
            if (srcUser == null)
                return;

            EventData eventData = new EventData((byte)EventCode.SendChat, new object());
            eventData[(byte)ParameterCode.UserId] = senderId;
            eventData[(byte)ParameterCode.Message] = message;

            switch (receiverId)
            {
                case -1: // WORLD CHANNEL
                    SendWorldEvent(eventData);
                    break;

                case 0: // ZONE CHANNEL
                    SendZoneEvent(eventData);
                    break;

                default: // PRIVATE CHANNEL
                    GameUser dstUser = (GameUser)AllUsers[receiverId];
                    if (dstUser != null)
                        SendUserEvent(dstUser, eventData);
                    break;
            }
        }


        //user requests to update their information
        public ErrorCode OnUserUpdate(UserUpdateOperation requestData, ref GameUser gameUser)
        {
            gameUser = (GameUser)AllUsers[requestData.UserId];
            if (gameUser == null)
                return ErrorCode.UserNotFound;

            if (gameUser.Status == UserStatus.InBattle)
                return ErrorCode.UserInBattle;

            gameUser.AddOnlineAwards(HeroDatabase, 0);

            HeroDatabase.UserGetCash(gameUser);

            UserRole userRole = gameUser.UserRoles.Where(r => r.Id == requestData.RoleUId).FirstOrDefault();
            UserItem userItem = gameUser.UserItems.Where(i => i.Id == requestData.ItemUId).FirstOrDefault();

            ErrorCode errCode = ErrorCode.Success;
            switch ((SubCode)requestData.SubCode)
            {
                case SubCode.UserAvatar:
                    errCode = gameUser.AddCash(0, -1);
                    if (errCode != ErrorCode.Success)
                        return errCode;
                    gameUser.Base.Avatar = requestData.AvatarId;
                    break;

                case SubCode.TutorStep:
                    if (gameUser.Base.TutorStep != requestData.TargetId)
                    {
                        gameUser.Base.TutorStep = requestData.TargetId;

                        if (gameUser.Base.TutorStep == 7)
                        {
                            UserItem newItem = new UserItem() { GameUser = gameUser, Grade = 3 };
                            newItem.CreateRandom(gameUser.Id, 11, 1, 3);
                            HeroDatabase.UserAddItem(gameUser, newItem, UserAction.TutorAward);
                        }
                    }
                    break;

                case SubCode.CheckCash:
                    HeroDatabase.UserGetCash(gameUser);
                    break;

                case SubCode.RoleSetItem:
                    if (userItem == null)
                        return ErrorCode.InvalidParams;

                    int itemCount = requestData.ItemCount;
                    return gameUser.SetItemForRole(userRole, userItem, itemCount);

                case SubCode.RoleUpExp:
                    if (userRole == null || requestData.UserRoles == null)
                        return ErrorCode.InvalidParams;

                    int reqSilver = requestData.UserRoles.Length * RoleConfig.EVOLVE_SIVLER;
                    errCode = gameUser.AddCash(-reqSilver, 0);
                    if (errCode != ErrorCode.Success)
                        return errCode;

                    List<UserRole> delRoles = new List<UserRole>();
                    foreach (int rId in requestData.UserRoles)
                    {
                        UserRole delRole = gameUser.UserRoles.Where(r => r.Id == rId).FirstOrDefault();
                        if (delRole != null && gameUser.UpgradeRoleFromRole(userRole, delRole))
                            delRoles.Add(delRole);
                    }

                    HeroDatabase.UserDelRoles(gameUser, delRoles, UserAction.UpgradeRole);
                    break;

                case SubCode.RoleUpStar:
                    if (userRole == null)
                        return ErrorCode.InvalidParams;

                    errCode = gameUser.UpgradeRoleFromItems(userRole);
                    if (errCode == ErrorCode.Success)
                        SendWorldMessage(gameUser, WorldMessage.RoleUpgrade, string.Format("{0}\n{1}", userRole.Name, userRole.Base.Grade));

                    return errCode;

                case SubCode.ItemUpgrade:
                    {
                        UserItem newItem = null;
                        int itemGrade = requestData.Param01;
                        bool useGold = requestData.Param02 > 0;
                        errCode = gameUser.UpgradeItemFromItems(userItem, itemGrade, useGold, ref newItem);
                        if (newItem != null)
                            HeroDatabase.UserAddItem(gameUser, newItem, UserAction.UpgradeItem);
                        return errCode;
                    }

                case SubCode.ItemSell:
                    return gameUser.SellItem(userItem);

                case SubCode.AddFriend:
                    return HeroDatabase.UserAddFriend(gameUser, requestData.FriendName, requestData.FriendMode);

                case SubCode.GetFriend:
                    HeroDatabase.UserRefreshFriends(gameUser);
                    break;

                case SubCode.CheckEmail:
                    HeroDatabase.UserRefreshEmails(gameUser);
                    break;

                case SubCode.ReadEmail:
                    return HeroDatabase.UserReadEmail(gameUser, requestData.TargetId);

                case SubCode.CheckReport:
                    HeroDatabase.UserRefreshArena(gameUser);
                    HeroDatabase.UserRefreshPillages(gameUser);
                    break;

                case SubCode.OnlineAward:
                    gameUser.AddOnlineAwards(HeroDatabase, requestData.Param01);
                    break;

                case SubCode.ExpandLand:
                    return gameUser.ExpandLand();

                case SubCode.OpenLandCell:
                    return gameUser.OpenLandCell(requestData.Param01, requestData.Param02);

                case SubCode.BuildHouse:
                    return gameUser.BuildHouse(requestData.TargetId, requestData.Param01, requestData.Param02);

                case SubCode.DestroyHouse:
                    return gameUser.DestroyHouse(requestData.TargetId, requestData.Param01, requestData.Param02);

                case SubCode.CheckBank:
                    gameUser.Land.GetBankSilver(requestData.Param01);
                    break;

                case SubCode.SetHire:
                    if (userRole == null)
                    {
                        HeroDatabase.GetHireRoles(gameUser);
                        return ErrorCode.Success;
                    }

                    UserRoleHire hireRole = HeroDatabase.SetRoleForHire(userRole, requestData.TargetId, requestData.Param01, requestData.Param02);
                    if (hireRole == null)
                        return ErrorCode.TargetNotFound;
                    else
                    {
                        UserRoleHire curRole = gameUser.HireRoles.FirstOrDefault(r => r.Id == hireRole.Id);
                        if (curRole != null)
                            gameUser.HireRoles.Remove(curRole);

                        gameUser.HireRoles.Add(hireRole);
                    }
                    break;

                case SubCode.GetHires:
                    break;

                default:
                    return ErrorCode.InvalidParams;
            }

            return ErrorCode.Success;
        }

        //require roles list for inventory from user , add data to byte 
        public ErrorCode OnRolesList(int userId, ref byte[] roleData, ref byte[] itemData)
        {
            GameUser gameUser = (GameUser)AllUsers[userId];
            if (gameUser == null)
                return ErrorCode.UserNotFound;

            gameUser.RefreshRoles();
            roleData = Serialization.SaveArray(gameUser.UserRoles.ToArray(), true);
            itemData = Serialization.SaveArray(gameUser.UserItems.ToArray(), true);

            return ErrorCode.Success;
        }

        //update a role status from inventory
        public ErrorCode OnRoleUpdate(RoleUpdateOperation requestData)
        {
            GameUser gameUser = (GameUser)AllUsers[requestData.UserId];
            if (gameUser == null)
                return ErrorCode.UserNotFound;

            if (gameUser.Status == UserStatus.InBattle)
                return ErrorCode.UserInBattle;

            UserRole userRole = gameUser.UserRoles.Where(r => r.Id == requestData.RoleUId).FirstOrDefault();
            if (userRole == null)
                return ErrorCode.RoleNotFound;

            if (userRole.Base.Level < 1)
                return ErrorCode.RoleNotReady;

            userRole.Base.Status = (RoleStatus)requestData.Status;
            userRole.Base.AIMode = requestData.AIMode;

            return ErrorCode.Success;
        }

        //buy items , summon heroes 
        public ErrorCode OnShopBuy(ShopBuyOperation requestData, ref GameUser gameUser, out GameObj gameObj)
        {
            gameObj = null;
            ErrorCode errCode = ErrorCode.Success;

            gameUser = (GameUser)AllUsers[requestData.UserId];
            if (gameUser == null)
                return ErrorCode.UserNotFound;

            HeroDatabase.UserGetCash(gameUser);

            if (requestData.ShopId >= 1000)
            {
                int pckId = requestData.ShopId - 1000;
                PackageXml thePack = GameConfig.PROMOTION_PACKS[pckId];

                errCode = gameUser.AddCash(0, -thePack.Price);
                if (errCode != ErrorCode.Success)
                    return errCode;

                // Add Heroes
                for (int i = 0; i < thePack.Hero.Count; i++)
                {
                    UserRole userRole = new UserRole() { GameUser = gameUser };
                    userRole.CreateRandom(gameUser.Id, 2);
                    userRole.Base.Grade = thePack.Hero.Grade;
                    HeroDatabase.UserAddRole(gameUser, userRole, UserAction.BuyPack);
                }

                // Add Equipts
                for (int i = 0; i < thePack.Equipment.Count; i++)
                {
                    int itemLevel = Helpers.Random.Next(thePack.Equipment.MinLevel, thePack.Equipment.MaxLevel + 1);
                    List<GameItem> gameItems = Global.GameItems.Select(r => (GameItem)r).Where(r => r.SubKind == (int)ItemSubKind.Equipment && r.Level == itemLevel).ToList();
                    int[] itemList = gameItems.Select(r => r.Id).ToArray();

                    int itemId = Helpers.GetRandomInt(itemList);

                    int itemGrade = Helpers.GetRandomIndex(thePack.Equipment.GradesList);

                    UserItem newItem = new UserItem() { GameUser = gameUser, Grade = itemGrade };
                    newItem.CreateRandom(gameUser.Id, itemId, 1, itemGrade);
                    HeroDatabase.UserAddItem(gameUser, newItem, UserAction.BuyPack);
                }

                // Add Items
                foreach (var packItem in thePack.Items)
                {
                    int itemIdx = Helpers.GetRandomIndex(packItem.ItemRateList);
                    int itemId = packItem.ItemIdList[itemIdx];
                    UserItem newItem = new UserItem() { GameUser = gameUser, Grade = 0 };
                    newItem.CreateRandom(gameUser.Id, itemId, packItem.Count, 1);
                    HeroDatabase.UserAddItem(gameUser, newItem, UserAction.BuyPack);
                }

                return errCode;
            }

            ShopItem shopItem = (ShopItem)Global.ChargeShop[requestData.ShopId];
            if (shopItem == null)
                return ErrorCode.TargetNotFound;

            if (gameUser.Base.Level < shopItem.UserLevel)
                return ErrorCode.UserNotReady;

            switch (shopItem.ItemKind)
            {
                case ItemKind.Hero:
                    errCode = gameUser.AddCash(-shopItem.PriceSilverSale, -shopItem.PriceGoldSale);
                    if (errCode != ErrorCode.Success)
                        return errCode;

                    UserRole userRole = new UserRole() { GameUser = gameUser };
                    userRole.CreateRandom(requestData.UserId, shopItem.ItemId, gameUser.UserRoles.Count);
                    HeroDatabase.UserAddRole(gameUser, userRole, UserAction.BuyRole);
                    gameObj = userRole;

                    if (userRole.Base.Grade >= 3)
                        SendWorldMessage(gameUser, WorldMessage.Sommon, string.Format("{0}\n{1}", userRole.Name, userRole.Base.Grade));

                    return ErrorCode.Success;

                case ItemKind.Support:
                case ItemKind.Material:
                case ItemKind.Consume:
                    errCode = gameUser.AddCash(-shopItem.PriceSilverSale, -shopItem.PriceGoldSale);
                    if (errCode != ErrorCode.Success)
                        return errCode;

                    UserItem userItem = gameUser.AddItem(shopItem.ItemId, 1, 0);
                    if (userItem != null)
                        HeroDatabase.UserAddItem(gameUser, userItem, UserAction.BuyItem);

                    gameObj = userItem;
                    return ErrorCode.Success;

                case ItemKind.Gold:
                    break;

                case ItemKind.Silver:
                    break;
            }

            return ErrorCode.InvalidParams;
        }

       
        //events of battle
        public ErrorCode OnBattle(BattleOperation operation)
        {
            SubCode subCode = (SubCode)operation.SubCode;

            GameUser gameUser = (GameUser)AllUsers[operation.UserId];
            if (gameUser == null)
                return ErrorCode.UserNotFound;

            
            switch (subCode)
            {
                // user require create battle with opponent in PVP list (mode pvp in Arena)
                case SubCode.RandomPvP:
                    return OnBattleRandomPvP(gameUser, operation);

               // user require create battle with AI opponent (selected user top in Arena , social)
                case SubCode.RandomPvA:
                    return OnBattleRandomPvA(gameUser, operation);

                case SubCode.RandomPvE:
                    return OnBattleRandomPvE(gameUser, operation);


                //send invite opponent join a battle
                case SubCode.Invite:
                    return OnBattleInvite(gameUser, operation);

                //send Accept join a battle
                case SubCode.Accept:
                    return OnBattleAccept(gameUser, operation);

                //send quit battle in Arena mode
                case SubCode.Abort:
                    return OnBattleAbort(gameUser, operation);

                //when battle was created , user init  battle completed , user send Ready status , check count if all user ready , zone send start battle
                case SubCode.Ready:
                    return OnBattleReady(gameUser, operation);

                // user require create a match for mission
                case SubCode.MissionBegin:
                    return OnMissionBegin(gameUser, operation);

                // when Lose (mission) , user require Resume game or end game.
                case SubCode.MissionResume:
                    return OnMissionResume(gameUser, operation);              

              

              
            }


            //get Battle of this User 
            BattleService battle = gameUser.Tag as BattleService;

            if (battle != null)
                return battle.OnRequest(operation, gameUser);

            Log.WarnFormat("OnBattle Invalid SubCode {0}", subCode);
            return ErrorCode.OperationDedined;
        }

        public ErrorCode OnCardCharge(int userId, int goldAdd, int silverAdd, ref GameUser gameUser)
        {
            gameUser = (GameUser)AllUsers[userId];
            if (gameUser != null)
            {
                HeroDatabase.UserAddCashOnline(userId, goldAdd, silverAdd, true);
                return gameUser.AddCash(silverAdd, goldAdd);
            }

            return HeroDatabase.UserAddCashOnline(userId, goldAdd, silverAdd);
        }

        public void SendResponse(OperationResponse response)
        {
            MasterPeer.SendOperationResponse(response, new SendParameters());
        }

        public void SendSystemMsg(GameUser gameUser, string message)
        {
            EventData eventData = new EventData((byte)EventCode.SendChat, new object());
            eventData[(byte)ParameterCode.UserId] = 0;
            eventData[(byte)ParameterCode.Message] = message;

            SendUserEvent(gameUser, eventData);
        }

        public void SendUserSync(GameUser gameUser, GameAward battleAward = null)
        {
            EventData eventData = new EventData((byte)EventCode.UserSync, new object());
            eventData[(byte)ParameterCode.UserData] = Serialization.Save(gameUser, true);
            eventData[(byte)ParameterCode.GameAward] = (battleAward == null) ? null : Serialization.SaveStruct(battleAward);
            SendUserEvent(gameUser, eventData);
        }

        //send event to client
        public void SendUserEvent(GameUser gameUser, EventData eventData)
        {
            //Set ID client reciever
            eventData[(byte)ParameterCode.ClientId] = gameUser.ClientId.ToByteArray();
            MasterPeer.SendEvent(eventData, new SendParameters());
        }

        public void SendZoneEvent(EventData eventData)
        {
            eventData[(byte)ParameterCode.Channel] = ServerEventCode.ZoneEvent;
            MasterPeer.SendEvent(eventData, new SendParameters());
        }

        public void SendWorldEvent(EventData eventData)
        {
            eventData[(byte)ParameterCode.Channel] = ServerEventCode.WorldEvent;
            MasterPeer.SendEvent(eventData, new SendParameters());
        }

        public void SendWorldMessage(GameUser gameUser, WorldMessage msg, string msgParams)
        {
            EventData eventData = new EventData((byte)EventCode.SendChat, new object());
            eventData[(byte)ParameterCode.UserId] = 0;
            eventData[(byte)ParameterCode.Message] = string.Format("{0}\n{1}\n{2}", (int)msg, gameUser.Base.NickName, msgParams);

            eventData[(byte)ParameterCode.Channel] = ServerEventCode.WorldEvent;
            MasterPeer.SendEvent(eventData, new SendParameters());
        }

        #endregion

        #region Battle Arrangement

        private GameObj FindPvPUser(GameObj obj, GameUser gameUser)
        {
            GameUser user = obj as GameUser;
            if (user == null || user == gameUser)
                return null;

            //add roles selected to list of Roles join battle
            user.RefreshRoles();

            if (user.Status == UserStatus.Ready)
                return user;

            return null;
        }


        //Create Pvp Match (arena mode)
        private ErrorCode OnBattleRandomPvP(GameUser gameUser, BattleOperation operation)
        {
            if (gameUser.Base.Level < GameConfig.ARENALEVEL)
                return ErrorCode.UserLevelNotEnough;

            if (gameUser.Base.Silver < GameConfig.ARENALOSTSILVER)
                return ErrorCode.CashInsufficient;

            //add roles selected to list of Roles join battle
            gameUser.RefreshRoles();

            if (gameUser.Status != UserStatus.Ready)
                return ErrorCode.UserNotReady;

            //get a opponent in PvPUsers list 
            GameUser targetUser = (GameUser)PvPUsers.ForEachRet((GameObj obj) => { return FindPvPUser(obj, gameUser); });
            if (targetUser != null)
            {
                //connection successful match for 2 users,remove 2 users in list 
                PvPUsers.LockRemove(gameUser);
                PvPUsers.LockRemove(targetUser);

                BattleService.Create(this, BattleMode.RandomPvP, gameUser, targetUser);
            }
            else
            {
                PvPUsers.LockAdd(gameUser);

                EventData eventData = new EventData((byte)EventCode.PvPSearch, new object());
                eventData[(byte)ParameterCode.UserName] = gameUser.Name;

                SendZoneEvent(eventData);
            }

            return ErrorCode.Success;
        }

        //Create a match with AI opponent
        private ErrorCode OnBattleRandomPvA(GameUser gameUser, BattleOperation operation)
        {
            if (gameUser.Base.Level < GameConfig.PILLAGELEVEL)
                return ErrorCode.UserLevelNotEnough;

            //add roles selected to list of Roles join battle
            gameUser.RefreshRoles();

            if (gameUser.Status != UserStatus.Ready)
                return ErrorCode.UserNotReady;

            GameUser targetUser = null;
            if (operation.TargetId == 0)
            {
                targetUser = HeroDatabase.GetRandomUser(gameUser);
            }
            else
            {
                targetUser = HeroDatabase.UserGet(operation.TargetId);
                if (targetUser == null)
                    return ErrorCode.InvalidParams;

                targetUser.InitAvatarRoles();
            }

            PvPUsers.LockRemove(gameUser);

            foreach (UserRole userRole in gameUser.ActiveRoles)
                userRole.Base.Energy -= RoleConfig.ENERGY_MIN;

            BattleService.Create(this, BattleMode.RandomPvA, gameUser, targetUser);
            return ErrorCode.Success;
        }

        private ErrorCode OnBattleRandomPvE(GameUser gameUser, BattleOperation operation)
        {
            return ErrorCode.Success;
        }

        //forward Invite to target
        private ErrorCode OnBattleInvite(GameUser gameUser, BattleOperation operation)
        {
            //add roles selected to list of Roles join battle
            gameUser.RefreshRoles();

            if (gameUser.Status != UserStatus.Ready)
                return ErrorCode.UserNotReady;

            if (operation.UserId == operation.TargetId)
                return ErrorCode.OperationDedined;

            GameUser targetUser = (GameUser)AllUsers[operation.TargetId];
            if (targetUser == null)
                return ErrorCode.TargetNotFound;

            //add roles selected to list of Roles join battle
            targetUser.RefreshRoles();

            if (targetUser.Status != UserStatus.Ready)
                return ErrorCode.TargetNotReady;

            gameUser.TargetId = operation.TargetId;

            Dictionary<byte, object> parameters = new Dictionary<byte, object>();
            parameters[(byte)ParameterCode.SubCode] = operation.SubCode;
            parameters[(byte)ParameterCode.UserId] = operation.UserId;
            parameters[(byte)ParameterCode.TargetId] = operation.TargetId;
            SendUserEvent(targetUser, new EventData((byte)EventCode.Battle, parameters));

            return ErrorCode.Success;
        }


        //target Accept and create battle
        private ErrorCode OnBattleAccept(GameUser gameUser, BattleOperation operation)
        {
            //add roles selected to list of Roles join battle
            gameUser.RefreshRoles();

            if (gameUser.Status != UserStatus.Ready)
                return ErrorCode.UserNotReady;

            if (operation.UserId == operation.TargetId)
                return ErrorCode.OperationDedined;

            GameUser targetUser = (GameUser)AllUsers[operation.TargetId];
            if (targetUser == null)
                return ErrorCode.TargetNotFound;

            //add roles selected to list of Roles join battle
            targetUser.RefreshRoles();

            if (targetUser.Status != UserStatus.Ready)
                return ErrorCode.TargetNotReady;

            if (targetUser.TargetId != operation.UserId)
                return ErrorCode.OperationDedined;

            PvPUsers.LockRemove(gameUser);
            PvPUsers.LockRemove(targetUser);
            BattleService.Create(this, BattleMode.Challenge, gameUser, targetUser);

            return ErrorCode.Success;
        }

        //User exit Battle in Arena
        private ErrorCode OnBattleAbort(GameUser gameUser, BattleOperation operation)
        {
            PvPUsers.LockRemove(gameUser);
            return ErrorCode.Success;
        }

        private ErrorCode OnBattleReady(GameUser gameUser, BattleOperation operation)
        {
            BattleService userBattle = gameUser.Tag as BattleService;
            if (userBattle != null)
            {
                if (userBattle.Mode == BattleMode.RandomPvP)
                    userBattle.ReadyCount += 1;
                else
                    userBattle.ReadyCount = 2;

                return ErrorCode.Success;
            }
            else
            {
                return ErrorCode.BattleNotAvailable;
            }
        }

        //check conditions to create mission and create a match for mission
        private ErrorCode OnMissionBegin(GameUser gameUser, BattleOperation operation)
        {
            if (gameUser.Base.MissionLevel < operation.TargetId)
                return ErrorCode.UserLevelNotEnough;

            //add roles selected to list of Roles join battle
            gameUser.RefreshRoles();

            if (gameUser.ActiveRoles.Count < 1)
                return ErrorCode.UserNotReady;

            if (!MissionService.UseEnergy(gameUser, operation.TargetId, operation.Difficulty, true))
                return ErrorCode.EnergyInsufficient;

            int hireRoleId = operation.RoleId;
            if (hireRoleId > 0)
            {
                ErrorCode errCode = HeroDatabase.HireOneRole(gameUser, hireRoleId);
                if (errCode != ErrorCode.Success)
                    return errCode;
            }

            PvPUsers.LockRemove(gameUser);

            MissionService mission = new MissionService(this, gameUser, operation.TargetId, operation.Difficulty);

            return ErrorCode.Success;
        }

        //player lose, player require reborn to continue playing  
        private ErrorCode OnMissionResume(GameUser gameUser, BattleOperation operation)
        {
            if (gameUser.GamePlay == null || gameUser.GamePlay.Mode != PlayMode.Mission)
                return ErrorCode.OperationDedined;

            if (operation.TargetId == 1)
            {
                ErrorCode errCode = gameUser.AddCash(0, -5);
                if (errCode != ErrorCode.Success)
                {
                    gameUser.GamePlay.OnGamePlayEnd(1);
                    return errCode;
                }

                //user call OnGamePlayResume of gameplay service (mission service)
                gameUser.GamePlay.OnGamePlayResume();
            }
            else
                gameUser.GamePlay.OnGamePlayEnd(1);

            return ErrorCode.Success;
        }
        
        //check conditions to create Dungeon and create Dungeon
        private ErrorCode OnDungeonBegin(GameUser gameUser, BattleOperation operation)
        {
           
            return ErrorCode.Success;
        }

        //create a event for dungeon
        private ErrorCode OnDungeonResume(GameUser gameUser, BattleOperation operation)
        {
            if (gameUser.GamePlay == null || gameUser.GamePlay.Mode != PlayMode.Dungeon)
                return ErrorCode.OperationDedined;

            //user call OnGamePlayResume of gameplay service (dungeon service)
            gameUser.GamePlay.OnGamePlayResume();

            return ErrorCode.Success;
        }



        #endregion
    }
}
