using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer.Properties;
using ExitGames.Logging;
using System;
using System.Collections.Generic;
using System.Data.Linq;
using System.Linq;

namespace DEngine.HeroServer.GameData
{
    public enum UserAction
    {
        None,
        BuyItem,
        BuyRole,
        SellItem,
        SellRole,
        UpgradeItem,
        UpgradeRole,
        DailyAward,
        OnlinAward,
        TutorAward,
        BuyPack,
    }

    public class EnemyList : List<int>
    {
        public DateTime LastUpdate;
    }

    public class HeroDB
    {
        #region Fields

        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        protected static DateTime LastRankUpdate = DateTime.Today;

        #endregion

        #region Common Methods

        public HeroDB()
        {
            LoadGlobalTables();
        }

        private void LoadGameRoles(HeroDBDataContext dbContext)
        {
            Global.GameRoles.Clear();

            foreach (RoleBase role in dbContext.RoleBases)
            {
                GameRole gameRole = role.GetGameRole();
                Global.GameRoles.Add(gameRole);

                switch ((RoleType)gameRole.Type)
                {
                    case RoleType.Hostage:
                    case RoleType.Hero:
                        Global.GameHeroes.Add(gameRole);
                        break;

                    case RoleType.Mob:
                    case RoleType.Elite:
                    case RoleType.Boss:
                        Global.GameMobs.Add(gameRole);
                        break;
                }
            }
        }

        private void LoadGameItems(HeroDBDataContext dbContext)
        {
            Global.GameItems.Clear();

            foreach (ItemBase item in dbContext.ItemBases)
                Global.GameItems.Add(item.GetGameItem());
        }

        private void LoadGameSkills(HeroDBDataContext dbContext)
        {
            Global.GameSkills.Clear();

            foreach (SkillBase skill in dbContext.SkillBases)
                Global.GameSkills.Add(skill.GetGameSkill());
        }

        private void LoadChargeShop(HeroDBDataContext dbContext)
        {
            Global.ChargeShop.Clear();

            foreach (Shop shop in dbContext.Shops)
            {
                if (shop.ItemKind > 0)
                    Global.ChargeShop.Add(shop.GetShopItem());
            }
        }

        public void LoadGlobalTables()
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                LoadGameRoles(dbContext);
                LoadGameItems(dbContext);
                LoadGameSkills(dbContext);
                LoadChargeShop(dbContext);
            }
        }

        #endregion

        #region Private Methods

        private UserEx CreateUser(HeroDBDataContext dbContext, int userId, string userName, string nickName)
        {
            int userCount = dbContext.UserExes.Count() + 10000;

            UserEx newUser = new UserEx();
            dbContext.UserExes.InsertOnSubmit(newUser);

            newUser.UserId = userId;
            newUser.UserName = userName;
            newUser.NickName = nickName;
            newUser.Level = 1;
            newUser.MissionLevel = 1;
            newUser.CreateTime = DateTime.Now;
            newUser.LoginTime = DateTime.Now;

            newUser.Gold = 50;
            newUser.Silver = 10000;
            newUser.Status = 1;

            dbContext.SubmitChanges();
            return newUser;
        }

        private void AddAccountLog(HeroDBDataContext dbContext, int userId, string remoteIP, int action, int onlineTime)
        {
            AccountLog accLog = new AccountLog();
            accLog.LogTime = DateTime.Now;
            accLog.UserId = userId;
            accLog.RemoteIP = remoteIP;
            accLog.Action = action;
            accLog.OnlineTime = onlineTime;

            dbContext.AccountLogs.InsertOnSubmit(accLog);
            dbContext.SubmitChanges();
        }

        private void AddActionLog(HeroDBDataContext dbContext, GameUser gameUser, int roleId, int itemId, int action)
        {
            ActionLog log = new ActionLog();
            log.LogTime = DateTime.Now;
            log.UserId = gameUser.Id;
            log.RoleId = roleId;
            log.ItemId = itemId;
            log.Action = action;
            log.UserData = gameUser.GetLogData();
            dbContext.ActionLogs.InsertOnSubmit(log);
        }

        #endregion

        #region Public Methods

        public int CreateBot(int botId, string nickName)
        {
            string userName = string.Format("bot{0:D4}", botId);
            string password = string.Format("botPass{0:X8}", Helpers.Random.NextDouble().GetHashCode());
            string email = userName + "@gmnail.com";

            string serviceUrl = string.Format("{0}/Account/Register?userName={1}&password={2}&email={3}&nickname={4}", ServerConfig.SERVICE_URL,
                userName, password, email, nickName);

            int userId = HttpService.GetResponse(serviceUrl).Code;
            if (userId <= 0)
                return userId;

            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                int botLevel = 1 + Helpers.Random.Next(49);

                UserEx botUser = CreateUser(dbContext, userId, userName, nickName);
                botUser.Level = botLevel;

                int heroCount = 0;
                while (heroCount < 10)
                {
                    int idx = Helpers.Random.Next(Global.GameHeroes.Count);
                    GameRole gameRole = (GameRole)Global.GameHeroes[idx];
                    if (gameRole.Id > 10)
                        continue;

                    heroCount += 1;

                    int botGrade = 1 + Helpers.Random.Next(5);
                    int botElem = 1 + Helpers.Random.Next(5);

                    Role newRole = new Role()
                    {
                        RoleId = gameRole.Id,
                        Name = gameRole.Name,
                        Grade = botGrade,
                        Level = botLevel,
                        ElemId = botElem,
                        Energy = 10000,
                        UseTime = DateTime.Now,
                    };

                    botUser.Roles.Add(newRole);
                }

                dbContext.SubmitChanges();
            }

            return userId;
        }
        
        public GameUser UserGet(int userId)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == userId);
                    if (theUser == null)
                        return null;

                    GameUser gameUser = theUser.GetGameUser(dbContext, true);
                    gameUser.LoadArenaLogs(dbContext);
                    gameUser.LoadPillageLogs(dbContext);
                    gameUser.LoadUserFriends(dbContext);

                    return gameUser;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return null;
            }
        }

        public GameUser UserGet(string userName)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserName == userName);
                    if (theUser == null)
                        return null;

                    GameUser gameUser = theUser.GetGameUser(dbContext, true);
                    gameUser.LoadArenaLogs(dbContext);
                    gameUser.LoadPillageLogs(dbContext);
                    gameUser.LoadUserFriends(dbContext);

                    return gameUser;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return null;
            }
        }

        public void UserUpdate(GameUser gameUser)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == gameUser.Id);
                    if (theUser == null)
                        return;

                    theUser.UpdateGameUser(gameUser, dbContext);

                    dbContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public GameUser UserSignIn(int userId, string userName, string nickName, string remoteIp)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == userId);
                    if (theUser == null)
                    {
                        theUser = CreateUser(dbContext, userId, userName, nickName);
                        Log.InfoFormat("UserCreate IP = {0}, Id = {1}, name = {2}...", remoteIp, userId, userName);
                        AddAccountLog(dbContext, userId, remoteIp, 0, 0);
                    }
                    else
                    {
                        if (theUser.LoginTime.Date != DateTime.Today)
                        {
                            theUser.OnlineAwardStep = 0;
                            theUser.OnlineAwardTime = 0;
                        }

                        theUser.LoginTime = DateTime.Now;
                        theUser.Status = 1;
                        dbContext.SubmitChanges();

                        Log.InfoFormat("UserSignIn IP = {0}, Id = {1}, name = {2}...", remoteIp, userId, userName);
                        AddAccountLog(dbContext, userId, remoteIp, 1, 0);
                    }

                    GameUser gameUser = theUser.GetGameUser(dbContext, true);
                    gameUser.Base.NickName = nickName;

                    gameUser.RemoteIP = remoteIp;
                    gameUser.LoginTime = DateTime.Now;
                    gameUser.LoadArenaLogs(dbContext);
                    gameUser.LoadPillageLogs(dbContext);
                    gameUser.LoadUserFriends(dbContext);

                    int todayLogin = gameUser.LoadLoginLogs(dbContext);
                    if (todayLogin == 1)
                        gameUser.AddDailyAwards(this);

                    return gameUser;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return null;
            }
        }

        public void UserSignOut(GameUser gameUser)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == gameUser.Id);
                    if (theUser == null)
                        return;

                    DateTime curTime = DateTime.Now;

                    theUser.UpdateGameUser(gameUser, dbContext);
                    theUser.Status = 0;
                    theUser.LogoutTime = curTime;

                    dbContext.SubmitChanges();

                    Log.InfoFormat("UserSignOut Id = {0}, name = {1}...", theUser.UserId, theUser.UserName);
                    AddAccountLog(dbContext, gameUser.Id, gameUser.RemoteIP, 2, gameUser.OnlineTime);

                    if (LastRankUpdate.AddHours(3) < DateTime.Now)
                    {
                        Log.InfoFormat("Updating User ranking...");
                        LastRankUpdate = DateTime.Now;
                        dbContext.UpdateRank();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public GameObjList GetTopUsers(int listType, int listCount)
        {
            if (listType < 0 || listType >= Global.TopUsers.Length)
                listType = 0;

            GameObjList userList = Global.TopUsers[listType];
            DateTime lastUpdate = (DateTime)userList.Tag;
            if (lastUpdate.AddHours(1) >= DateTime.Now)
                return userList;

            try
            {
                userList.Clear();

                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    IEnumerable<UserEx> allUsers = null;

                    switch ((UserListType)listType)
                    {
                        case UserListType.TopLevel:
                            allUsers = (from item in dbContext.UserExes
                                        where item.LevelRank > 0
                                        orderby item.LevelRank
                                        select item).Take(listCount);
                            break;

                        case UserListType.TopHonor:
                            allUsers = (from item in dbContext.UserExes
                                        where item.Honor > 0
                                        orderby item.Honor descending
                                        select item).Take(listCount);

                            break;

                        case UserListType.TopGold:
                            allUsers = (from item in dbContext.UserExes
                                        orderby item.Gold descending
                                        select item).Take(listCount);
                            break;

                        case UserListType.TopSilver:
                            allUsers = (from item in dbContext.UserExes
                                        orderby item.Silver descending
                                        select item).Take(listCount);
                            break;
                    }

                    if (allUsers != null)
                    {
                        int honorRank = 1;
                        foreach (UserEx user in allUsers)
                        {
                            GameUser gameUser = user.GetGameUser(dbContext);
                            if (listType == 2)
                                gameUser.Base.HonorRank = honorRank++;

                            userList.Add(gameUser);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }

            return userList;
        }

        public ErrorCode PillageDefenceFailed(int userId, int silverLost)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = (from item in dbContext.UserExes
                                      where item.UserId == userId
                                      select item).FirstOrDefault();

                    if (theUser.Status != 0)
                        return ErrorCode.UserNotReady;

                    if (theUser.Land != null)
                    {
                        int towerCount = theUser.Land.Houses.Count(h => h.HouseId == LandConfig.TOWER_ID);
                        theUser.Land.ShrinkPoint += (10 - towerCount / 2);
                    }

                    if (theUser.Silver - silverLost < 0)
                        return ErrorCode.CashInsufficient;

                    theUser.Silver -= silverLost;

                    dbContext.SubmitChanges();
                    return ErrorCode.Success;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public ErrorCode UserAddCashOnline(int userId, int goldAdd, int silverAdd, bool directAdd = false)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    int resId = (from item in dbContext.UserExes
                                 where item.UserId == userId
                                 select item.UserId).FirstOrDefault();

                    if (resId == 0)
                        return ErrorCode.InvalidParams;

                    CashLog cashLog = new CashLog();
                    cashLog.LogTime = DateTime.Now;
                    cashLog.UserId = userId;
                    cashLog.GoldAdd = goldAdd;
                    cashLog.SilverAdd = silverAdd;

                    if (directAdd)
                        cashLog.UpdateTime = DateTime.Now;

                    dbContext.CashLogs.InsertOnSubmit(cashLog);
                    dbContext.SubmitChanges();

                    return ErrorCode.Success;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public ErrorCode UserSetCash(GameUser gameUser) {
            try {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx theUser = dbContext.UserExes.Where(u => u.UserId == gameUser.Id).FirstOrDefault();
                    if (theUser != null)
                    {
                        theUser.Silver = gameUser.Base.Silver;
                        theUser.Gold = gameUser.Base.Gold;
                    }

                    dbContext.SubmitChanges();
                    return ErrorCode.Success;
                }
            } catch (Exception ex) {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public ErrorCode UserGetCash(GameUser gameUser)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    IEnumerable<CashLog> allLogs = (from item in dbContext.CashLogs
                                                    where item.UserId == gameUser.Id && item.UpdateTime == null
                                                    select item);

                    foreach (CashLog cashLog in allLogs)
                    {
                        gameUser.AddCash(cashLog.SilverAdd, cashLog.GoldAdd);
                        cashLog.UpdateTime = DateTime.Now;
                        cashLog.GoldTotal = gameUser.Base.Gold;
                        cashLog.SilverTotal = gameUser.Base.Silver;
                    }

                    UserEx theUser = dbContext.UserExes.Where(u => u.UserId == gameUser.Id).FirstOrDefault();
                    if (theUser != null)
                    {
                        theUser.Silver = gameUser.Base.Silver;
                        theUser.Gold = gameUser.Base.Gold;
                    }

                    dbContext.SubmitChanges();
                    return ErrorCode.Success;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public ErrorCode UserAddFriend(GameUser gameUser, string friendName, int friendMode)
        {
            try
            {
                int userId = gameUser.Id;

                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    UserEx friendUser = dbContext.UserExes.FirstOrDefault(u => u.NickName == friendName);
                    if (friendUser == null || friendUser.UserId == userId)
                        return ErrorCode.TargetNotFound;

                    int friendId = friendUser.UserId;

                    if (friendMode == -2)
                    {
                        // free all slaves of gameUser
                        IEnumerable<Friend> slaves = dbContext.Friends.Where(f => f.User01Id == userId && f.User02Id == friendId && f.Mode == (int)UserRelation.Slave);
                        dbContext.Friends.DeleteAllOnSubmit(slaves);

                        // free all slaves of gameUser
                        IEnumerable<Friend> masters = dbContext.Friends.Where(f => f.User02Id == userId && f.User01Id == friendId && f.Mode == (int)UserRelation.Master);
                        dbContext.Friends.DeleteAllOnSubmit(masters);
                    }
                    else
                    {

                        Friend friend1 = (from item in dbContext.Friends
                                          where (item.User01Id == userId && item.User02Id == friendId && item.Mode <= (int)UserRelation.FriendTwo)
                                          select item).FirstOrDefault();

                        Friend friend2 = (from item in dbContext.Friends
                                          where (item.User02Id == userId && item.User01Id == friendId && item.Mode <= (int)UserRelation.FriendTwo)
                                          select item).FirstOrDefault();

                        if (friend1 == null)
                        {
                            friend1 = new Friend();
                            dbContext.Friends.InsertOnSubmit(friend1);

                            friend1.User01Id = userId;
                            friend1.User02Id = friendId;
                            friend1.Mode = friendMode;

                            if (friendMode == 1)
                            {
                                if (friend2 != null && friend2.Mode == (int)UserRelation.FriendOne)
                                {
                                    friend1.Mode = (int)UserRelation.FriendTwo;
                                    friend2.Mode = (int)UserRelation.FriendTwo;
                                }
                                else
                                    dbContext.UserSendEmail(userId, friendId, "Friend request!", string.Format("User {0} has just sent you a friend request.", gameUser.Base.NickName), 0, 0);
                            }
                        }
                        else
                        {
                            if (friendMode == -1)
                            {
                                dbContext.Friends.DeleteOnSubmit(friend1);

                                if (friend2 != null && friend2.Mode == (int)UserRelation.FriendTwo)
                                    friend2.Mode = (int)UserRelation.FriendOne;
                            }
                            else
                            {
                                friend1.Mode = friendMode;

                                if (friend2 != null)
                                {
                                    if (friend2.Mode == (int)UserRelation.FriendOne && friendMode == 1)
                                    {
                                        friend1.Mode = (int)UserRelation.FriendTwo;
                                        friend2.Mode = (int)UserRelation.FriendTwo;
                                    }

                                    if (friend2.Mode == (int)UserRelation.FriendTwo && friendMode != 1)
                                        friend2.Mode = (int)UserRelation.FriendOne;
                                }
                            }
                        }
                    }

                    dbContext.SubmitChanges();
                    gameUser.LoadUserFriends(dbContext);
                    return ErrorCode.Success;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public void UserAddItem(GameUser gameUser, UserItem userItem, UserAction action)
        {
            try
            {
                if (userItem.Id != 0)
                    return;

                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    GameItem gameItem = userItem.GameItem;
                    if (gameItem == null)
                        return;

                    AddActionLog(dbContext, gameUser, 0, userItem.ItemId, (int)action);

                    Item newItem = userItem.GetNewItem();
                    dbContext.Items.InsertOnSubmit(newItem);
                    dbContext.SubmitChanges();

                    userItem.InitData(newItem);
                    gameUser.UserItems.Add(userItem);
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void UserAddMasterSlave(GameUser masterUser, GameUser slaveUser)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    // slaveUser has a master
                    int masterCount = dbContext.Friends.Count(f => f.User01Id == slaveUser.Id && f.Mode == (int)UserRelation.Master);
                    if (masterCount > 0)
                        return;

                    // masterUser has a master
                    masterCount = dbContext.Friends.Count(f => f.User01Id == masterUser.Id && f.Mode == (int)UserRelation.Master);
                    if (masterCount > 0)
                    {
                        Friend fr01 = dbContext.Friends.FirstOrDefault(f => f.User01Id == masterUser.Id && f.Mode == (int)UserRelation.Master);
                        if (fr01 != null && fr01.User02Id == slaveUser.Id)
                        {
                            dbContext.Friends.DeleteOnSubmit(fr01);
                            Friend fr02 = dbContext.Friends.FirstOrDefault(f => f.User01Id == slaveUser.Id && f.Mode == (int)UserRelation.Slave);
                            if (fr02 != null)
                            {
                                dbContext.Friends.DeleteOnSubmit(fr02);
                            }

                            dbContext.SubmitChanges();
                        }

                        return;
                    }

                    // masterUser has 3 slaves
                    int slaveCount = dbContext.Friends.Count(f => f.User01Id == masterUser.Id && f.Mode == (int)UserRelation.Slave);
                    if (slaveCount >= 3)
                        return;

                    // free all slaves of slave User
                    IEnumerable<Friend> slaves = dbContext.Friends.Where(f => (f.User01Id == slaveUser.Id && f.Mode == (int)UserRelation.Slave) || (f.User02Id == slaveUser.Id && f.Mode == (int)UserRelation.Master));
                    dbContext.Friends.DeleteAllOnSubmit(slaves);

                    // add new relationships
                    Friend friend1 = new Friend();
                    dbContext.Friends.InsertOnSubmit(friend1);

                    friend1.User01Id = masterUser.Id;
                    friend1.User02Id = slaveUser.Id;
                    friend1.ExpiryTime = DateTime.Now.AddHours(24);
                    friend1.Mode = (int)UserRelation.Slave;

                    Friend friend2 = new Friend();
                    dbContext.Friends.InsertOnSubmit(friend2);

                    friend2.User01Id = slaveUser.Id;
                    friend2.User02Id = masterUser.Id;
                    friend2.ExpiryTime = DateTime.Now.AddHours(24);
                    friend2.Mode = (int)UserRelation.Master;

                    dbContext.SubmitChanges();
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void UserAddItems(GameUser gameUser, List<UserItem> userItems, UserAction action)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    foreach (UserItem userItem in userItems)
                    {
                        if (userItem.Id != 0)
                            continue;

                        GameItem gameItem = userItem.GameItem;
                        if (gameItem == null)
                            continue;

                        if (action != UserAction.None)
                            AddActionLog(dbContext, gameUser, 0, userItem.ItemId, (int)action);

                        Item newItem = userItem.GetNewItem();
                        dbContext.Items.InsertOnSubmit(newItem);
                        dbContext.SubmitChanges();

                        userItem.InitData(newItem);
                        gameUser.UserItems.Add(userItem);
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void UserAddRole(GameUser gameUser, UserRole userRole, UserAction action)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    GameRole gameRole = userRole.GameRole;
                    if (gameRole == null)
                        return;

                    AddActionLog(dbContext, gameUser, userRole.Base.RoleId, 0, (int)action);

                    Role newRole = userRole.GetNewRole();

                    dbContext.Roles.InsertOnSubmit(newRole);
                    dbContext.SubmitChanges();

                    userRole.InitData(newRole);
                    gameUser.UserRoles.Add(userRole);
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void UserDelRoles(GameUser gameUser, List<UserRole> userRoles, UserAction action)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    foreach (UserRole role in userRoles)
                    {
                        Role delRole = (from item in dbContext.Roles
                                        where item.Role_UID == role.Id
                                        select item).FirstOrDefault();

                        if (delRole == null)
                            continue;

                        AddActionLog(dbContext, gameUser, role.Id, 0, (int)action);

                        dbContext.Roles.DeleteOnSubmit(delRole);
                        dbContext.SubmitChanges();
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void UserRefreshArena(GameUser gameUser)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
                gameUser.LoadArenaLogs(dbContext);
        }

        public void UserRefreshPillages(GameUser gameUser)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
                gameUser.LoadPillageLogs(dbContext);
        }

        public void UserRefreshFriends(GameUser gameUser)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
                gameUser.LoadUserFriends(dbContext);
        }

        public void UserRefreshEmails(GameUser gameUser)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
                gameUser.LoadUserEmails(dbContext);
        }

        public void UserSendEmail(int senderId, int receiverId, string mailTitle, string mailContent, int silver, int gold, GameObjList attachedItems = null)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
                dbContext.UserSendEmail(senderId, receiverId, mailTitle, mailContent, silver, gold, attachedItems);
        }

        public ErrorCode UserReadEmail(GameUser gameUser, int emailId)
        {
            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    MailBox mail = dbContext.MailBoxes.FirstOrDefault(m => m.MailId == emailId);
                    if (mail == null || mail.ReceverId != gameUser.Id || mail.RecvTime != null)
                        return ErrorCode.TargetNotFound;

                    if (mail.AttachItems != null)
                    {
                        int[] itemIds = Serialization.LoadStructArray<int>(mail.AttachItems.ToArray());

                        foreach (int itemId in itemIds)
                        {
                            Item item = dbContext.Items.FirstOrDefault(i => i.Item_UID == itemId);
                            if (item == null || item.UserId != 0)
                                continue;

                            item.UserId = gameUser.Id;

                            UserItem userItem = item.GetUserItem(gameUser);
                            gameUser.UserItems.Add(userItem);
                        }
                    }

                    gameUser.AddCash(mail.Silver, mail.Gold);

                    mail.RecvTime = DateTime.Now;
                    dbContext.SubmitChanges();

                    gameUser.LoadUserEmails(dbContext);

                    return ErrorCode.Success;
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
                return ErrorCode.InvalidParams;
            }
        }

        public void AddArenaLog(GameUser gameUser1, GameUser gameUser2, int result, int honor1, int honor2)
        {
            int userId1 = gameUser1.Id;
            int userId2 = gameUser2.Id;

            try
            {
                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    ArenaLog aLog = new ArenaLog();
                    aLog.LogTime = DateTime.Now;
                    aLog.UserId1 = userId1;
                    aLog.UserId2 = userId2;
                    aLog.Result = result;
                    aLog.Honor1 = honor1;
                    aLog.Honor2 = honor2;
                    aLog.TotalHonor1 = gameUser1.Base.Honor;
                    aLog.TotalHonor2 = gameUser2.Base.Honor;
                    dbContext.ArenaLogs.InsertOnSubmit(aLog);
                    dbContext.SubmitChanges();

                    gameUser1.LoadArenaLogs(dbContext);
                    gameUser2.LoadArenaLogs(dbContext);
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void AddPillageLog(GameUser atkUser, GameUser defUser, int result, int silverWon, int silverLost)
        {
            try
            {
                int atkId = atkUser.Id;
                int defId = defUser.Id;

                using (HeroDBDataContext dbContext = new HeroDBDataContext())
                {
                    PillageLog plLog = new PillageLog();
                    plLog.LogTime = DateTime.Now;
                    plLog.AttackId = atkId;
                    plLog.DefenceId = defId;
                    plLog.Result = result;
                    plLog.SilverWon = silverWon;
                    plLog.SilverLost = silverLost;

                    dbContext.PillageLogs.InsertOnSubmit(plLog);
                    dbContext.SubmitChanges();

                    IEnumerable<PillageLog> pvpLogs = (from item in dbContext.PillageLogs
                                                       where item.AttackId == atkId && item.LogTime.Date == DateTime.Today
                                                       select item);

                    atkUser.Base.TodayAttack = pvpLogs.Count();

                    if (atkUser.Base.TodayAttack > 0)
                        atkUser.Base.SilverWon = pvpLogs.Sum(log => log.SilverWon);
                }
            }
            catch (Exception ex)
            {
                Log.Warn(ex.Message);
                Log.Warn(ex.StackTrace);
            }
        }

        public void AddDailyGift()
        {
        }

        public GameObjList GetRandUsers(ZoneService zoneService, GameUser gameUser, int listType)
        {
            GameObjList retList = new GameObjList();

            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    //GameUser testUser = UserGet("cc01");
                    //testUser.LoadUserFriends(dbContext);
                    //retList.Add(testUser);

                    EnemyCache enemyCache = (from item in dbContext.EnemyCaches
                                             where item.UserId == gameUser.Id
                                             select item).FirstOrDefault();

                    if (enemyCache == null || enemyCache.EnemyList == null || enemyCache.LastUpdate.Date < DateTime.Today)
                        listType = (int)UserListType.Pillage;

                    if (listType == (int)UserListType.Pillage)
                    {
                        int levelRank = gameUser.Base.LevelRank > 0 ? gameUser.Base.LevelRank : gameUser.Id;
                        ISingleResult<GetRandUsersResult> resultSet = dbContext.GetRandUsers(gameUser.Id, gameUser.Base.LevelRank);

                        List<int> userIdList = new List<int>();
                        foreach (GetRandUsersResult result in resultSet)
                        {
                            if (userIdList.Count >= 10)
                                break;

                            UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == result.UserId);
                            if (theUser == null)
                                continue;

                            // Online in other Zone
                            if (zoneService != null)
                            {
                                if (theUser.Status == 1 && !zoneService.AllUsers.Contains(result.UserId))
                                    continue;
                            }

                            userIdList.Add(result.UserId);

                            GameUser rivalUser = theUser.GetGameUser(dbContext, true);
                            rivalUser.LoadUserFriends(dbContext);
                            retList.Add(rivalUser);
                        }

                        // UpdateCache
                        if (enemyCache == null)
                        {
                            enemyCache = new EnemyCache() { UserId = gameUser.Id };
                            dbContext.EnemyCaches.InsertOnSubmit(enemyCache);
                        }

                        enemyCache.EnemyList = Helpers.GetByteArray(userIdList.ToArray());
                        enemyCache.LastUpdate = DateTime.Now;
                        dbContext.SubmitChanges();

                        return retList;
                    }

                    int[] intArray = Helpers.GetIntArray(enemyCache.EnemyList.ToArray());
                    if (intArray != null)
                    {
                        foreach (int userId in intArray)
                        {
                            UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == userId);
                            if (theUser == null)
                                continue;

                            GameUser rivalUser = theUser.GetGameUser(dbContext, true);
                            rivalUser.LoadUserFriends(dbContext);
                            retList.Add(rivalUser);
                        }
                    }

                    return retList;
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                }
            }

            return retList;
        }

        public GameUser GetRandomUser(GameUser gameUser)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    int levelRank = gameUser.Base.LevelRank > 0 ? gameUser.Base.LevelRank : gameUser.Id;
                    int avatarId = dbContext.GetAvatar(gameUser.Id, levelRank);

                    UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == avatarId);
                    if (theUser == null)
                        return null;

                    Log.InfoFormat("ID={0}, Name={1}, Roles={2}", theUser.UserId, theUser.NickName, theUser.Roles.Count());

                    GameUser rivalUser = theUser.GetGameUser(dbContext, true);
                    rivalUser.InitAvatarRoles();

                    return rivalUser;
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                }
            }
            return null;
        }

        public void UpdateUsersRank()
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    dbContext.UpdateRank();
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                }
            }
        }

        public UserRoleHire SetRoleForHire(UserRole userRole, int hireMode, int hireGold, int hireSilver)
        {
            if (hireGold < 0 || hireSilver < 0)
                return null;

            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    RoleHire roleHire = dbContext.RoleHires.FirstOrDefault(r => r.Role_UID == userRole.Id);
                    if (roleHire == null)
                    {
                        roleHire = new RoleHire();
                        roleHire.Role_UID = userRole.Id;
                        dbContext.RoleHires.InsertOnSubmit(roleHire);

                        roleHire.UserId = userRole.Base.UserId;
                        roleHire.RoleId = userRole.Base.RoleId;
                        roleHire.ElemId = (int)userRole.Base.ElemId;
                    }

                    roleHire.Level = userRole.Base.Level;
                    roleHire.Grade = userRole.Base.Grade;
                    roleHire.HireMode = hireMode;

                    if (hireMode > 0)
                    {
                        roleHire.HireGold = hireGold;
                        roleHire.HireSilver = hireSilver;
                    }

                    dbContext.SubmitChanges();

                    UserRoleHire userRoleHire = new UserRoleHire();
                    userRoleHire.InitData(roleHire);

                    return userRoleHire;
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                    return null;
                }
            }
        }

        public void GetHireRoles(GameUser gameUser)
        {
            gameUser.HireRoles.Clear();

            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    IEnumerable<RoleHire> hireRoles = dbContext.RoleHires.Where(r => r.UserId == gameUser.Id);
                    foreach (RoleHire hRole in hireRoles)
                    {
                        UserRoleHire userRoleHire = new UserRoleHire();
                        userRoleHire.InitData(hRole);
                        gameUser.HireRoles.Add(userRoleHire);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                }
            }
        }

        public List<UserRoleHire> GetHireRoles(int userLevel)
        {
            List<UserRoleHire> roleHireList = new List<UserRoleHire>();

            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    IEnumerable<RoleHire> hireRoles = dbContext.RoleHires.Where(r => r.HireMode > 0 && r.Level < userLevel + 5).OrderByDescending(r => r.Level).Take(20);
                    foreach (RoleHire hRole in hireRoles)
                    {
                        UserRoleHire userRoleHire = new UserRoleHire();
                        userRoleHire.InitData(hRole);
                        roleHireList.Add(userRoleHire);
                    }
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                }
            }

            return roleHireList;
        }

        public ErrorCode HireOneRole(GameUser gameUser, int roleUId)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                try
                {
                    Role theRole = dbContext.Roles.FirstOrDefault(r => r.Role_UID == roleUId);
                    if (theRole == null)
                        return ErrorCode.TargetNotFound;

                    if (theRole.UserId == gameUser.Id)
                        return ErrorCode.InvalidParams;

                    RoleHire roleHire = dbContext.RoleHires.FirstOrDefault(r => r.Role_UID == roleUId);
                    if (roleHire == null || roleHire.HireMode == 0)
                        return ErrorCode.TargetNotFound;

                    UserRole userRole = new UserRole()
                    {
                        GameUser = gameUser,
                        GameRole = (GameRole)Global.GameRoles[theRole.RoleId],
                    };

                    userRole.InitData(theRole);
                    userRole.Base.UserId = gameUser.Id;
                    userRole.Base.Energy = 10000;
                    userRole.Base.Type = RoleType.HiredHero;

                    ErrorCode errCode = gameUser.AddCash(-roleHire.HireSilver, -roleHire.HireGold);
                    if (errCode != ErrorCode.Success)
                        return errCode;

                    int mailSilver = roleHire.HireSilver * 90 / 100;
                    int mailGold = roleHire.HireGold * 90 / 100;

                    string mailTitle = "Hire a Hero";
                    string mailContent = string.Format("{0} hire your {1}. You get {2} Silver and {3} Gold (Tax = 10%).", gameUser.Base.NickName, theRole.Name, mailSilver, mailGold);
                    dbContext.UserSendEmail(0, roleHire.UserId, mailTitle, mailContent, mailSilver, mailGold);

                    gameUser.ActiveRoles.Add(userRole);
                    return ErrorCode.Success;
                }
                catch (Exception ex)
                {
                    Log.Warn(ex.Message);
                    Log.Warn(ex.StackTrace);
                    return ErrorCode.InvalidParams;
                }
            }
        }

        #endregion
    }
}
