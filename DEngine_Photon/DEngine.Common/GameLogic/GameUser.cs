using DEngine.Common.Config;
using System;
using System.Linq;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

#if _SERVER
using DEngine.HeroServer.GameData;
using ExitGames.Logging;
using System.IO;
using DEngine.HeroServer;
using Photon.SocketServer;
#endif

namespace DEngine.Common.GameLogic
{
    public class PvPLog : IDataSerializable
    {
        public int LogId;
        public DateTime LogTime;
        public GameUser Opponent;
        public int Result;
        public int HonorAdd;
        public int HonorTotal;

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref LogId);
            serializer.Serialize(ref LogTime);
            serializer.Serialize(ref Opponent);
            serializer.Serialize(ref Result);
            serializer.Serialize(ref HonorAdd);
            serializer.Serialize(ref HonorTotal);
        }
    }

    public class PvALog : IDataSerializable
    {
        public int LogId;
        public DateTime LogTime;
        public GameUser Opponent;
        public int Mode;
        public int Result;
        public int Silver;

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref LogId);
            serializer.Serialize(ref LogTime);
            serializer.Serialize(ref Opponent);
            serializer.Serialize(ref Mode);
            serializer.Serialize(ref Result);
            serializer.Serialize(ref Silver);
        }
    }

    public class UserFriend : IDataSerializable
    {
        public GameUser Opponent;
        public int Mode;
        public DateTime Expiry;

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref Opponent);
            serializer.Serialize(ref Mode);
            serializer.Serialize(ref Expiry);
        }
    }

    public class UserMail : IDataSerializable
    {
        public int MailId;
        public int SenderId;
        public GameUser Sender;
        public string Title;
        public string Message;
        public GameObjList Items;
        public DateTime SendTime;
        public DateTime RecvTime;

        public UserMail()
        {
            Items = new GameObjList();
        }

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref MailId);
            serializer.Serialize(ref SenderId);
            serializer.Serialize(ref Sender);
            serializer.Serialize(ref Title);
            serializer.Serialize(ref Message);
            serializer.Serialize(ref Items);
            serializer.Serialize(ref SendTime);
            serializer.Serialize(ref RecvTime);
        }
    }

    public class UserHouse : IDataSerializable
    {
        public int UID;
        public int HouseId;
        public int PosX;
        public int PosY;
        public int SizeX;
        public int SizeY;

#if _SERVER
        public void InitData(HeroDBDataContext dbContext, House house)
        {
            UID = house.House_UID;
            HouseId = house.HouseId;
            PosX = house.PosX;
            PosY = house.PosY;
        }

        public void UpdateData(HeroDBDataContext dbContext, House house)
        {
            house.PosX = PosX;
            house.PosY = PosY;
        }
#endif

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref HouseId);
            serializer.Serialize(ref PosX);
            serializer.Serialize(ref PosY);
            serializer.Serialize(ref SizeX);
            serializer.Serialize(ref SizeY);
        }
    }

    public class UserLand : IDataSerializable
    {
        public GameUser GameUser;

        public int OpenPoint;
        public int ClosePoint;
        public int LandSizeX;
        public int LandSizeY;
        public byte[,] LandData;
        public int BankSilver;

        public DateTime lastCheck;

        public readonly List<UserHouse> Houses = new List<UserHouse>();
        public readonly List<UserHouse> DelHouses = new List<UserHouse>();

        public UserLand()
        {
            lastCheck = DateTime.Now;
            ExpandLand(12, 12);
        }

#if _SERVER
        public void InitData(HeroDBDataContext dbContext, Land land)
        {
            if (land == null)
                return;

            OpenPoint = land.ExpandPoint;
            ClosePoint = land.ShrinkPoint;
            LandSizeX = land.LandSizeX;
            LandSizeY = land.LandSizeY;
            LandData = new byte[LandSizeX, LandSizeY];

            if (land.LandData == null)
                return;

            byte[] landData = land.LandData.ToArray();
            if (landData.Length < LandSizeX * LandSizeY)
                return;

            int idx = 0;
            for (int i = 0; i < LandSizeX; i++)
            {
                for (int j = 0; j < LandSizeY; j++)
                {
                    LandData[i, j] = landData[idx++];
                }
            }

            Houses.Clear();
            foreach (var item in land.Houses)
            {
                if (!LandConfig.HOUSE_DATA.ContainsKey(item.HouseId))
                    continue;

                HouseData houseData = LandConfig.HOUSE_DATA[item.HouseId];

                UserHouse newHouse = new UserHouse();
                newHouse.InitData(dbContext, item);
                newHouse.SizeX = houseData.SizeX;
                newHouse.SizeY = houseData.SizeY;

                Houses.Add(newHouse);
            }
        }

        public void UpdateData(HeroDBDataContext dbContext, Land land)
        {
            land.ExpandPoint = OpenPoint;
            land.ShrinkPoint = ClosePoint;
            land.LandSizeX = LandSizeX;
            land.LandSizeY = LandSizeY;

            int idx = 0;
            byte[] landData = new byte[LandSizeX * LandSizeY];
            for (int i = 0; i < LandSizeX; i++)
            {
                for (int j = 0; j < LandSizeY; j++)
                {
                    landData[idx++] = LandData[i, j];
                }
            }

            land.LandData = landData;

            foreach (var item in DelHouses)
            {
                House house = dbContext.Houses.FirstOrDefault(h => h.House_UID == item.UID);
                if (house != null)
                    dbContext.Houses.DeleteOnSubmit(house);
            }

            DelHouses.Clear();

            foreach (var item in Houses)
            {
                House house = null;
                if (item.UID == 0)
                {
                    house = new House() { HouseId = item.HouseId };
                    land.Houses.Add(house);
                }
                else
                    house = dbContext.Houses.FirstOrDefault(h => h.House_UID == item.UID);

                if (house != null)
                    item.UpdateData(dbContext, house);
            }
        }

        public void GetBankSilver(int mode)
        {
            int bankCount = Houses.Count(h => h.HouseId == 1);
            if (bankCount < 1)
                return;

            int marketCount = Houses.Count(h => h.HouseId == 2);

            BankSilver += marketCount * (int)DateTime.Now.Subtract(lastCheck).TotalSeconds;
            if (BankSilver > 2000)
                BankSilver = 2000;

            lastCheck = DateTime.Now;

            if (mode > 0)
            {
                GameUser.AddCash(BankSilver, 0);
                BankSilver = 0;
            }
        }

        public bool OpenLandCell(int posX, int posY)
        {
            if (posX < LandSizeX && posY < LandSizeY)
            {
                LandData[posX, posY] = 1;
                return true;
            }

            return false;
        }

        public bool CloseLandCell()
        {
            if (ClosePoint < LandConfig.CLOSE_POINT)
                return false;

            int openCellCount = 0;
            for (int i = 0; i < LandSizeX; i++)
            {
                for (int j = 0; j < LandSizeY; j++)
                {
                    if (LandData[i, j] > 0)
                        openCellCount++;
                }
            }

            int closePos = Helpers.Random.Next(openCellCount);

            openCellCount = 0;
            for (int i = 0; i < LandSizeX; i++)
            {
                for (int j = 0; j < LandSizeY; j++)
                {
                    if (LandData[i, j] > 0)
                    {
                        if (openCellCount == closePos)
                        {
                            ClosePoint -= LandConfig.CLOSE_POINT;
                            LandData[i, j] = 0;
                            RefreshHouses();
                            return true;
                        }

                        openCellCount++;
                    }
                }
            }

            return false;
        }

        public ErrorCode BuildHouse(int houseId, int posX, int posY)
        {
            if (!LandConfig.HOUSE_DATA.ContainsKey(houseId))
                return  ErrorCode.InvalidParams;

            HouseData houseData = LandConfig.HOUSE_DATA[houseId];

            int curCount = Houses.Count(h => h.HouseId == houseId);
            if (curCount >= houseData.MaxCount && houseData.MaxCount > 0)
                return ErrorCode.TargetNotAvaiable;

            if (GameUser.Base.Silver < houseData.Silver)
                return ErrorCode.CashInsufficient;

            if (GameUser.Base.Gold < houseData.Gold)
                return ErrorCode.CashInsufficient;

            for (int i = 0; i < houseData.SizeX; i++)
            {
                for (int j = 0; j < houseData.SizeY; j++)
                {
                    if (LandData[posX + i, posY + j] != 1)
                        return ErrorCode.OperationDedined;
                }
            }

            for (int i = 0; i < houseData.SizeX; i++)
            {
                for (int j = 0; j < houseData.SizeY; j++)
                {
                    LandData[posX + i, posY + j] = 2;
                }
            }

            UserHouse newHouse = new UserHouse()
            {
                HouseId = houseId,
                PosX = posX,
                PosY = posY,
                SizeX = houseData.SizeX,
                SizeY = houseData.SizeY,
            };

            Houses.Add(newHouse);

            GameUser.AddCash(-houseData.Silver, -houseData.Gold);
            //GameUser.Base.Silver -= houseData.Silver;
            //GameUser.Base.Gold -= houseData.Gold;
           
            return ErrorCode.Success;
        }

        public ErrorCode DestroyHouse(UserHouse house)
        {
            if (!LandConfig.HOUSE_DATA.ContainsKey(house.HouseId))
                return ErrorCode.InvalidParams;

            HouseData houseData = LandConfig.HOUSE_DATA[house.HouseId];

            for (int i = 0; i < houseData.SizeX; i++)
            {
                for (int j = 0; j < houseData.SizeY; j++)
                {
                    if (LandData[house.PosX + i, house.PosY + j] == 2)
                        LandData[house.PosX + i, house.PosY + j] = 1;
                }
            }

            DelHouses.Add(house);
            Houses.Remove(house);

            return ErrorCode.Success;
        }

        private void RefreshHouses()
        {
            for (int i = Houses.Count - 1; i >= 0; i--)
            {
                UserHouse house = Houses[i];
                HouseData houseData = LandConfig.HOUSE_DATA[house.HouseId];

                int closeCount = 0;
                for (int xx = 0; xx < house.SizeX; xx++)
                {
                    for (int yy = 0; yy < house.SizeY; yy++)
                    {
                        if (LandData[house.PosX + xx, house.PosY + yy] == 0)
                            closeCount++;
                    }
                }

                if (closeCount >= houseData.Lost)
                    DestroyHouse(house);
            }
        }
#endif
        public void ExpandLand(int sizeX, int sizeY)
        {
            if (LandSizeX >= sizeX && LandSizeY >= sizeY)
                return;

            byte[,] newLandData = new byte[sizeX, sizeY];

            if (LandData != null)
            {
                for (int i = 0; i < LandSizeX; i++)
                {
                    for (int j = 0; j < LandSizeY; j++)
                    {
                        newLandData[i, j] = LandData[i, j];
                    }
                }
            }

            LandSizeX = sizeX;
            LandSizeY = sizeY;
            LandData = newLandData;
        }

        public void Serialize(BinSerializer serializer)
        {
            serializer.Serialize(ref OpenPoint);
            serializer.Serialize(ref ClosePoint);
            serializer.Serialize(ref LandSizeX);
            serializer.Serialize(ref LandSizeY);
            serializer.Serialize(ref BankSilver);

            if (serializer.Mode == SerializerMode.Read)
                LandData = new byte[LandSizeX, LandSizeY];

            for (int i = 0; i < LandSizeX; i++)
            {
                for (int j = 0; j < LandSizeY; j++)
                {
                    serializer.Serialize(ref LandData[i, j]);
                }
            }

            serializer.Serialize(Houses);
        }
    }

    public class GameUser : GameObj
    {
        [StructLayout(LayoutKind.Sequential)]
        public struct UserBase
        {
            [MarshalAs(UnmanagedType.ByValTStr, SizeConst = 32)]
            public string NickName;

            public int Avatar;
            public long Silver;
            public long Gold;
            public int Level;
            public int Exp;
            public int LevelRank;
            public int MissionLevel;
            public int Honor;
            public int HonorRank;
            public int TotalWon;
            public int TotalDraw;
            public int TotalLost;
            public int TutorStep;
            public int TodayAttack;
            public int TodayDefence;
            public int SilverWon;
            public int SilverLost;
            public int OnlineAwardStep;
            public int LastAwardTime;
            public int CurrentAwardTime;

            public void ClearArena()
            {
                TotalWon = 0;
                TotalDraw = 0;
                TotalLost = 0;
            }

            public void UpdateArena(int result)
            {
                if (result < 0)
                    TotalWon += 1;
                else if (result == 0)
                    TotalDraw += 1;
                else
                    TotalLost += 1;
            }
        }

        #region Fields

        public UserBase Base;

        public UserLand Land = new UserLand();

        public int DailyLoginCount;

        public GameAward DailyLoginAward = new GameAward();

        public readonly List<UserItem> UserItems = new List<UserItem>();

        public readonly List<UserItem> DelItems = new List<UserItem>();

        public readonly List<UserRole> UserRoles = new List<UserRole>();

        public readonly List<UserRoleHire> HireRoles = new List<UserRoleHire>();

        //list roles selected
        public readonly List<UserRole> ActiveRoles = new List<UserRole>();

        public readonly List<PvPLog> PvPLogs = new List<PvPLog>();

        public readonly List<PvALog> PvALogs = new List<PvALog>();

        public readonly List<UserMail> UserMails = new List<UserMail>();

        public readonly List<UserFriend> UserFriends = new List<UserFriend>();

        #endregion

#if _SERVER
        protected static ILogger Log = LogManager.GetCurrentClassLogger();

        public ZoneService Zone { get; set; }

        public GamePlayService GamePlay { get; set; }

        public UserStatus Status { get; set; }

        public GameUser EnemyUser { get; set; }

        public int MaxHeroLevel { get; set; }

        public int MaxHeroGrade { get; set; }

        public string RemoteIP { get; set; }

        public DateTime LoginTime { get; set; }

        public int OnlineTime { get { return (int)DateTime.Now.Subtract(LoginTime).TotalSeconds; } }

        public GameAward OnlineAward { get; set; }

        public int SlaveCount { get; set; }

        public int[] WizardCounts =  new int[(int)RoleClass.Count];
#endif

        #region Properties

        public Guid ClientId { get; set; }

        public int TargetId { get; set; }

        public int Position { get; set; }

        #endregion

        #region Methods

        public GameUser()
        {
            Base.NickName = "";
            Land.GameUser = this;
        }

        public override void Serialize(BinSerializer serializer)
        {
            base.Serialize(serializer);

            serializer.SerializeEx(ref Base);
            serializer.Serialize(ref Land);
            serializer.Serialize(ref DailyLoginCount);
            serializer.SerializeEx(ref DailyLoginAward);

            serializer.Serialize(UserItems);
            serializer.Serialize(UserRoles);
            serializer.Serialize(UserFriends);
        }

        public void AddExp(int exp)
        {
            if (Base.Level >= UserConfig.LEVEL_MAX)
                return;

            Base.Exp += exp;

            while (Base.Level < UserConfig.LEVEL_MAX)
            {
                if (Base.Exp < UserConfig.LEVELS_EXP[Base.Level])
                    break;

                Base.Exp -= UserConfig.LEVELS_EXP[Base.Level];
                Base.Level += 1;
            }

            if (Base.Level == UserConfig.LEVEL_MAX)
                Base.Exp = 0;
        }

        public bool AddHonor(int hornor)
        {
            int newHonor = Base.Honor + hornor;
            if (newHonor < 0)
                return false;

            Base.Honor = newHonor;
            return true;
        }

        public ErrorCode AddCash(long silver, long gold)
        {
            long newSilver = Base.Silver + silver;
            long newGold = Base.Gold + gold;

            if (newSilver < 0 || newGold < 0)
                return ErrorCode.CashInsufficient;

            Base.Silver = newSilver;
            Base.Gold = newGold;

            try
            {
                string websiteUrl = string.Format("{0}/set", DEngine.Common.Config.ServerConfig.WEBSITE_URL);
                var formData = new Dictionary<string, string>();
                formData.Add("k", Global.key);
                formData.Add("a", Base.Gold.ToString());
                var httpWebRes = DEngine.HeroServer.HttpService.GetResponse2(websiteUrl, formData);

                Log.InfoFormat(string.Format("set balance bg {0} ng {1} \n status: {2} res: {3} website balance {4}", Base.Gold, newGold, httpWebRes.Code, httpWebRes.Description, websiteUrl));
            } catch(Exception e) {
                Log.ErrorFormat(string.Format("ERROR! set balance {0}", e.Message));
            }
            return ErrorCode.Success;
        }

#if _SERVER
        public void AddDailyAwards(HeroDB heroDB)
        {
            if (DailyLoginCount < 1 || DailyLoginCount >= GameConfig.DAILY_AWARDS.Length)
                return;

            List<GameAward> curAwards = GameConfig.DAILY_AWARDS[DailyLoginCount];
            if (curAwards == null || curAwards.Count <= Base.Level)
                return;

            DailyLoginAward = curAwards[Base.Level];

            AddExp(DailyLoginAward.UserExp);
            AddCash(DailyLoginAward.Silver, DailyLoginAward.Gold);

            List<UserItem> addItems = new List<UserItem>();
            foreach (var item in DailyLoginAward.Items)
            {
                UserItem userItem = AddItem(item.Key, item.Value);
                if (userItem != null)
                    addItems.Add(userItem);
            }

            heroDB.UserAddItems(this, addItems, UserAction.DailyAward);
        }

        public void AddOnlineAwards(HeroDB heroDB, int param01)
        {
            Base.CurrentAwardTime = Base.LastAwardTime + OnlineTime;

            int nextStep = Base.OnlineAwardStep + 1;
            if (param01 == 0 || nextStep >= GameConfig.ONLINE_STEPS.Length)
                return;

            OnlineAward = null;
            if (Base.CurrentAwardTime > GameConfig.ONLINE_STEPS[nextStep])
            {
                List<GameAward> curAwards = GameConfig.ONLINE_AWARDS[nextStep];
                if (curAwards == null || curAwards.Count <= Base.Level)
                    return;

                OnlineAward = curAwards[Base.Level];

                AddExp(OnlineAward.UserExp);
                AddCash(OnlineAward.Silver, OnlineAward.Gold);

                List<UserItem> addItems = new List<UserItem>();
                foreach (var item in OnlineAward.Items)
                {
                    UserItem userItem = AddItem(item.Key, item.Value);
                    if (userItem != null)
                        addItems.Add(userItem);
                }

                Base.OnlineAwardStep = nextStep;
                Base.LastAwardTime = 0;
                Base.CurrentAwardTime = 0;

                heroDB.UserAddItems(this, addItems, UserAction.OnlinAward);
            }
        }
#endif

        public void RefeshRoleItems()
        {
            foreach (UserRole role in UserRoles)
                role.RoleItems.Clear();

            foreach (UserItem item in UserItems)
            {
                if (item.RoleUId == 0)
                    continue;

                UserRole role = UserRoles.Where(r => r.Id == item.RoleUId).FirstOrDefault();
                if (role != null)
                    role.RoleItems.Add(item);
            }
        }

        public UserItem AddItem(int itemId, int itemCount, int itemRank = 0)
        {
            UserItem newItem = null;

            // Check existing items
            foreach (UserItem userItem in UserItems)
            {
                if (userItem.ItemId != itemId)
                    continue;

                int freeCount = userItem.GameItem.Stack - userItem.Count;
                int addCount = (freeCount < itemCount) ? freeCount : itemCount;

                userItem.Count += addCount;
                itemCount -= addCount;

                newItem = userItem;
            }

#if _SERVER
            if (itemCount > 0)
            {
                newItem = new UserItem() { GameUser = this };
                newItem.CreateRandom(Id, itemId, itemCount, itemRank);
                return newItem;
            }
#endif

            return newItem;
        }

        public int GetFreeItemCount(int itemId)
        {
            int totalCount = (from item in UserItems
                              where item.ItemId == itemId && item.RoleUId == 0
                              select item.Count).Sum();
            return totalCount;
        }

        public bool RemoveItem(int itemId, int itemCount)
        {
            int totalCount = (from item in UserItems
                              where item.ItemId == itemId && item.RoleUId == 0
                              select item.Count).Sum();

            if (itemCount > totalCount)
                return false;

            for (int i = UserItems.Count - 1; i >= 0; i--)
            {
                UserItem userItem = UserItems[i];

                if (userItem.ItemId != itemId || userItem.RoleUId != 0)
                    continue;

                if (userItem.Count >= itemCount)
                {
                    userItem.Count -= itemCount;
                    itemCount = 0;
                }
                else
                {
                    itemCount -= userItem.Count;
                    userItem.Count = 0;
                }

                if (userItem.Count == 0)
                {
                    DelItems.Add(userItem);
                    UserItems.Remove(userItem);
                }

                if (itemCount <= 0)
                    break;
            }

            return true;
        }

        public ErrorCode SetItemForRole(UserRole userRole, UserItem userItem, int itemCount = 0)
        {
            if (userRole != null)
            {
                if (userItem.RoleUId > 0)
                {
                    string errorLog = string.Format("Item is on other role ({0})", userItem.RoleUId);
#if _SERVER
                    Log.Warn(errorLog);
#else
                    Debug.Log(errorLog);
#endif
                    return ErrorCode.ItemNotAvailable;
                }

                int itemKind = userItem.GameItem.Kind;
                if (itemKind >= (int)ItemKind.Ring && itemKind <= (int)ItemKind.Support)
                {
                    if (userItem.MinRoleLevel > userRole.Base.Level)
                        return ErrorCode.RoleLevelNotEnough;

                    UserItem curItem = userRole.RoleItems.Where(i => i.GameItem.Kind == itemKind).FirstOrDefault();
                    if (curItem != null)
                    {
                        userRole.RoleItems.Remove(curItem);
                        curItem.RoleUId = 0;
                    }

                    userItem.RoleUId = userRole.Id;
                    userRole.RoleItems.Add(userItem);
                    userRole.InitAttrib();
                }
                else if (itemKind == (int)ItemKind.Material || itemKind == (int)ItemKind.Consume)
                {
                    if (itemCount == 0)
                        itemCount = 1;

                    if (userItem.Count >= itemCount && userItem.Attribs.Count > 0)
                    {
                        AttribType attType = userItem.Attribs[0].Attrib;
                        int attValue = (int)(userItem.Attribs[0].Value * itemCount);

                        switch (attType)
                        {
                            case AttribType.RoleExpValue:
                                userRole.AddExp(attValue);
                                break;

                            case AttribType.RoleEngValue:
                                userRole.AddEnergy(attValue);
                                break;
                        }

                        userItem.Count -= itemCount;
                        if (userItem.Count <= 0)
                        {
                            DelItems.Add(userItem);
                            UserItems.Remove(userItem);
                        }
                    }
                    else
                    {
#if _SERVER
                        Log.WarnFormat("ItemId {0} is invalid", userItem.ItemId);
#endif
                        return ErrorCode.ItemNotAvailable;
                    }

                }
                else
                {
                    string errorLog = string.Format("ItemKind {0} is invalid", itemKind);
#if _SERVER
                    Log.Warn(errorLog);
#else
                    Debug.Log(errorLog);
#endif
                    return ErrorCode.ItemNotAvailable;
                }
            }
            else if (userItem.RoleUId > 0)
            {
                userRole = UserRoles.FirstOrDefault(r => r.Id == userItem.RoleUId);
                if (userRole != null)
                {
                    userRole.RoleItems.Remove(userItem);
                    userRole.InitAttrib();
                }

                userItem.RoleUId = 0;
            }

            return ErrorCode.Success;
        }

        public int GetAddExpFromRoles(List<UserRole> roles)
        {
            int addExp = 0;
            foreach (UserRole role in roles)
            {
                if (role.Base.Level > RoleConfig.LEVEL_MAX)
                    continue;

                int rate = RoleConfig.GRADES_EXP[role.Base.Grade];
                addExp += RoleConfig.LEVELS_EXP[role.Base.Level - 1] * rate / 100;

                if (role.Base.Level < RoleConfig.LEVEL_MAX)
                {
                    rate = rate * role.Base.Exp / RoleConfig.LEVELS_EXP[role.Base.Level];
                    addExp += role.Base.Exp * rate / 100;
                }
            }
            return addExp;
        }

        public bool UpgradeRoleFromRole(UserRole role1, UserRole role2)
        {
            if (role1.Base.Level >= RoleConfig.LEVEL_MAX)
                return false;

            int rate = RoleConfig.GRADES_EXP[role2.Base.Grade];
            int addExp = RoleConfig.LEVELS_EXP[role2.Base.Level - 1] * rate / 100;

            if (role2.Base.Level < RoleConfig.LEVEL_MAX)
            {
                rate = rate * role2.Base.Exp / RoleConfig.LEVELS_EXP[role2.Base.Level];
                addExp += role2.Base.Exp * rate / 100;
            }

            UserRoles.Remove(role2);

            role1.AddExp(addExp);
            return true;
        }

        public ErrorCode UpgradeRoleFromItems(UserRole userRole)
        {
            RoleUpgrade upgradeData = (from item in RoleConfig.ROLE_UPGRADE
                                       where item.RoleId == userRole.Base.RoleId && item.Grade == userRole.Base.Grade
                                       select item).FirstOrDefault();

            if (upgradeData == null || userRole.Base.Level < upgradeData.Level)
                return ErrorCode.RoleNotReady;

            ErrorCode errCode = AddCash(-upgradeData.Silver, -upgradeData.Gold);
            if (errCode != ErrorCode.Success)
                return errCode;

            foreach (var item in upgradeData.Items)
            {
                int itemId = item.Key;
                int itemCount = item.Value;

                if (!RemoveItem(itemId, itemCount))
                    return ErrorCode.UserNotReady;
            }

            if (userRole.Upgrade())
                return ErrorCode.Success;

            return ErrorCode.InvalidParams;
        }

        private ErrorCode UpgradeMaterial(UserItem userItem, ref UserItem newItem)
        {
            if (!RemoveItem(userItem.ItemId, 3))
                return ErrorCode.ItemsInsufficient;

            newItem = AddItem(userItem.ItemId + 1, 1);

            return ErrorCode.Success;
        }

        private ErrorCode UpgradeEquipment(UserItem userItem, int itemGrade, bool useGold, ref UserItem newItem)
        {
            RankUpgrade[] RANK_UPGRADES = null;
            switch (itemGrade)
            {
                case 1:
                    RANK_UPGRADES = ItemConfig.WHITE_UPGRADES;
                    break;
                case 2:
                    RANK_UPGRADES = ItemConfig.GREEN_UPGRADES;
                    break;
                case 3:
                    RANK_UPGRADES = ItemConfig.BLUE_UPGRADES;
                    break;
                case 4:
                    RANK_UPGRADES = ItemConfig.YELLOW_UPGRADES;
                    break;
                default:
                    return ErrorCode.InvalidParams;
            }

            int curLevel = userItem.GameItem.Level;
            int curRank = userItem.Ranks[itemGrade];
            int silverUse = RANK_UPGRADES[curLevel].RequiredSilver[curRank];
            int goldUse = useGold ? RANK_UPGRADES[curLevel].RequiredGold[curRank] : 0;

            // Check money
            if (Base.Silver < silverUse || Base.Gold < goldUse)
                return ErrorCode.CashInsufficient;

            // Check material Items
            var item = RANK_UPGRADES[curLevel].Materials[curRank];
            if (GetFreeItemCount(item.ItemId) < item.Count)
                return ErrorCode.ItemsInsufficient;

#if _SERVER
            if (!useGold)
            {
                int curRate = RANK_UPGRADES[curLevel].NoGoldRate[curRank];
                int randNo = Helpers.Random.Next(100);
                if (randNo > curRate)
                {
                    AddCash(-silverUse, 0);
                    RemoveItem(item.ItemId, 1);

                    return ErrorCode.ItemsUpgradeFailed;
                }
            }
#endif

            if (!RemoveItem(item.ItemId, item.Count))
                return ErrorCode.ItemsInsufficient;

            // UseMoney
            AddCash(-silverUse, -goldUse);

            userItem.UpgradeRank(itemGrade);
            return ErrorCode.Success;
        }

        public ErrorCode UpgradeItemFromItems(UserItem userItem, int itemGrade, bool useGold, ref UserItem newItem)
        {
            ItemKind itemKind = (ItemKind)userItem.GameItem.Kind;
            if (itemKind == ItemKind.Material)
                return UpgradeMaterial(userItem, ref newItem);
            else if (itemKind == ItemKind.Ring || itemKind == ItemKind.Armor || itemKind == ItemKind.Medal)
                return UpgradeEquipment(userItem, itemGrade, useGold, ref newItem);
            else
                return ErrorCode.InvalidParams;
        }

        public ErrorCode SellItem(UserItem userItem)
        {
            if (userItem == null)
                return ErrorCode.InvalidParams;

            if (userItem.RoleUId > 0)
                return ErrorCode.OperationDedined;

            int silver = userItem.Count * userItem.GameItem.SellPrice;
            ErrorCode errCode = AddCash(silver, 0);
            if (errCode == ErrorCode.Success)
            {
                DelItems.Add(userItem);
                UserItems.Remove(userItem);
            }

            return errCode;
        }

#if _SERVER
        public void InitData(HeroDBDataContext dbContext, UserEx user)
        {
            Base.NickName = user.NickName;
            Base.Avatar = user.Avatar;
            Base.Silver = user.Silver;
            Base.Gold = user.Gold;
            Base.Level = user.Level;
            Base.Exp = user.Exp;
            Base.LevelRank = user.LevelRank;
            Base.MissionLevel = user.MissionLevel;
            Base.Honor = user.Honor;
            Base.HonorRank = user.HonorRank;
            Base.TotalWon = user.TotalWon;
            Base.TotalDraw = user.TotalDraw;
            Base.TotalLost = user.TotalLost;
            Base.TutorStep = user.TutorStep;

            Base.OnlineAwardStep = user.OnlineAwardStep;
            Base.LastAwardTime = user.OnlineAwardTime;
            Base.CurrentAwardTime = user.OnlineAwardTime;

            Land.InitData(dbContext, user.Land);

            for (;;)
            {
                if (!Land.CloseLandCell())
                    break;
            }

            RefreshRoles();
        }

        public void UpdateData(HeroDBDataContext dbContext, UserEx user)
        {
            user.Avatar = Base.Avatar;
            user.Silver = Base.Silver;
            user.Gold = Base.Gold;
            user.Exp = Base.Exp;
            user.Level = Base.Level;
            user.MissionLevel = Base.MissionLevel;
            user.Honor = Base.Honor;
            user.TotalWon = Base.TotalWon;
            user.TotalDraw = Base.TotalDraw;
            user.TotalLost = Base.TotalLost;
            user.TutorStep = Base.TutorStep;

            user.OnlineAwardStep = Base.OnlineAwardStep;
            user.OnlineAwardTime = Base.LastAwardTime + OnlineTime;

            if (user.Land == null)
                user.Land = new Land();

            Land.UpdateData(dbContext, user.Land);
        }

        // add roles selected to list of Roles join battle for User
        public void RefreshRoles()
        {
            if (Status == UserStatus.InBattle)
                return;

            ActiveRoles.Clear();

            MaxHeroLevel = 0;
            MaxHeroGrade = 0;
            foreach (UserRole userRole in UserRoles)
            {
                //new update Energy
                userRole.Refresh();

                // add role is ready to list of Roles join battle
                if (userRole.IsReady)
                {
                    if (userRole.Base.Level > MaxHeroLevel)
                        MaxHeroLevel = userRole.Base.Level;

                    if (userRole.Base.Grade > MaxHeroGrade)
                        MaxHeroGrade = userRole.Base.Grade;

                    ActiveRoles.Add(userRole);
                }
            }

            Status = ActiveRoles.Count >= UserConfig.BATTLE_ROLES ? UserStatus.Ready : UserStatus.Default;
        }

        public void InitAvatarRoles()
        {
            List<UserRole> activeRoles = (from role in UserRoles
                                          orderby role.Base.Status descending, role.Base.Level descending
                                          select role).Take(3).ToList();

            ActiveRoles.Clear();
            ActiveRoles.AddRange(activeRoles);
        }

        public void BattleInit(int pos)
        {
            if (WizardCounts != null)
            {
                WizardCounts[(int)RoleClass.Warrior] = Land.Houses.Count(h => h.HouseId == 31);
                WizardCounts[(int)RoleClass.Tanker] = Land.Houses.Count(h => h.HouseId == 32);
                WizardCounts[(int)RoleClass.Ranger] = Land.Houses.Count(h => h.HouseId == 33);
                WizardCounts[(int)RoleClass.Mage] = Land.Houses.Count(h => h.HouseId == 34);
                WizardCounts[(int)RoleClass.Healer] = Land.Houses.Count(h => h.HouseId == 35);
            }

            for (int i = 0; i < ActiveRoles.Count; i++)
            {
                float xPos = (pos == 0) ? -4f : 4f;
                ActiveRoles[i].InitBattle(xPos, 0f, BattleConfig.ROLE_SPACE * (1 - i));
            }

            Status = UserStatus.InBattle;
            Position = pos;
        }

        public byte[] GetLogData()
        {
            using (MemoryStream memStream = new MemoryStream())
            {
                BinaryWriter writer = new BinaryWriter(memStream);
                writer.Write(Base.Silver);
                writer.Write(Base.Gold);
                writer.Write(Base.Level);
                writer.Write(Base.Exp);
                writer.Write(Base.Honor);

                writer.Flush();

                byte[] buffer = memStream.GetBuffer();
                Array.Resize(ref buffer, (int)memStream.Length);

                return buffer;
            }
        }

        public ErrorCode ExpandLand()
        {
            ErrorCode errCode = AddCash(-LandConfig.EXPAND_SILVER, -LandConfig.EXPAND_GOLD);
            if (errCode != ErrorCode.Success)
                return errCode;

            Land.ExpandLand(Land.LandSizeX + 2, Land.LandSizeY + 2);

            return ErrorCode.Success;
        }

        public ErrorCode OpenLandCell(int posX, int posY)
        {
            ErrorCode errCode = AddCash(-LandConfig.OPEN_SILVER, -LandConfig.OPEN_GOLD);
            if (errCode != ErrorCode.Success)
                return errCode;

            Land.OpenLandCell(posX, posY);

            return ErrorCode.Success;
        }

        public ErrorCode BuildHouse(int houseId, int posX, int posY)
        {
            return Land.BuildHouse(houseId, posX, posY);
        }

        public ErrorCode DestroyHouse(int houseUId, int posX, int posY)
        {
            UserHouse house = Land.Houses.FirstOrDefault(h => h.PosX == posX && h.PosY == posY);
            if (house == null)
                return ErrorCode.TargetNotFound;

            return Land.DestroyHouse(house);
        }

#endif
        #endregion
    }
}
