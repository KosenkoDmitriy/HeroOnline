using DEngine.Common;
using DEngine.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace DEngine.HeroServer.GameData
{
    public static class Helpers
    {
        public static readonly Random Random;

        static Helpers()
        {
            Random = new Random();
        }

        #region Game Data

        public static GameRole GetGameRole(this RoleBase role)
        {
            GameRole gameRole = new GameRole()
            {
                Id = role.RoleId,
                Name = role.Name,
            };

            gameRole.InitData(role);
            return gameRole;
        }

        public static GameItem GetGameItem(this ItemBase item)
        {
            GameItem gameItem = new GameItem()
            {
                Id = item.ItemId,
                Name = item.Name,
            };

            gameItem.InitData(item);
            return gameItem;
        }

        public static GameSkill GetGameSkill(this SkillBase skill)
        {
            GameSkill gameSkill = new GameSkill()
            {
                Id = skill.SkillId,
                Name = skill.Name,
            };

            gameSkill.InitData(skill);
            return gameSkill;
        }

        public static ShopItem GetShopItem(this Shop shop)
        {
            ShopItem gameShop = new ShopItem()
            {
                Id = shop.ShopId,
                Name = shop.ItemName,
            };

            gameShop.InitData(shop);
            return gameShop;
        }

        #endregion

        #region UserEx Data

        public static GameUser GetGameUser(this HeroDBDataContext dbContext, int userId, bool loadFull = false)
        {
            UserEx user = (from item in dbContext.UserExes
                           where item.UserId == userId
                           select item).FirstOrDefault();

            if (user == null)
                return null;

            return user.GetGameUser(dbContext, loadFull);
        }

        public static GameUser GetGameUser(this UserEx user, HeroDBDataContext dbContext, bool loadFull = false)
        {
            GameUser gameUser = new GameUser()
            {
                Id = user.UserId,
                Name = user.UserName,
            };

            if (loadFull)
            {
                // Add UserItems
                foreach (Item item in user.Items)
                {
                    UserItem userItem = item.GetUserItem(gameUser);
                    if (userItem != null)
                        gameUser.UserItems.Add(userItem);
                }

                // Add UserRoles
                foreach (Role role in user.Roles)
                {
                    UserRole userRole = role.GetUserRole(gameUser);
                    if (userRole != null)
                        gameUser.UserRoles.Add(userRole);
                }
            }

            gameUser.InitData(dbContext, user);

            return gameUser;
        }

        public static UserItem GetUserItem(this Item item, GameUser gameUser)
        {
            GameItem gameItem = (GameItem)Global.GameItems[item.ItemId];
            if (gameItem == null)
                return null;

            UserItem userItem = new UserItem()
            {
                GameUser = gameUser,
                GameItem = gameItem,
            };

            userItem.InitData(item);
            return userItem;
        }

        public static UserRole GetUserRole(this Role role, GameUser gameUser)
        {
            GameRole gameRole = (GameRole)Global.GameRoles[role.RoleId];
            if (gameRole == null)
                return null;

            UserRole userRole = new UserRole()
            {
                GameUser = gameUser,
                GameRole = (GameRole)Global.GameRoles[role.RoleId],
            };

            // Add RoleItems
            foreach (UserItem item in gameUser.UserItems)
            {
                if (item.RoleUId == role.Role_UID)
                    userRole.RoleItems.Add(item);
            }

            userRole.InitData(role);
            return userRole;
        }

        public static void LoadArenaLogs(this GameUser gameUser, HeroDBDataContext dbContext)
        {
            int userId = gameUser.Id;
            DateTime monday = Helpers.GetMonday(DateTime.Now);

            IEnumerable<ArenaLog> arLogs = (from item in dbContext.ArenaLogs
                                            where item.LogTime >= monday && item.UserId1 == userId || item.UserId2 == userId
                                            orderby item.LogId descending
                                            select item).Take(10);

            gameUser.PvPLogs.Clear();
            gameUser.Base.ClearArena();

            foreach (ArenaLog log in arLogs)
            {
                PvPLog curLog = new PvPLog();
                curLog.LogId = log.LogId;
                curLog.LogTime = log.LogTime;

                if (log.UserId1 == userId)
                {
                    curLog.Result = log.Result;
                    curLog.HonorAdd = log.Honor1;
                    curLog.HonorTotal = log.TotalHonor1;
                }
                else
                {
                    curLog.Result = -log.Result;
                    curLog.HonorAdd = log.Honor2;
                    curLog.HonorTotal = log.TotalHonor2;
                }

                gameUser.Base.UpdateArena(curLog.Result);

                int oppId = (log.UserId1 == userId) ? log.UserId2 : log.UserId1;
                curLog.Opponent = dbContext.GetGameUser(oppId);

                gameUser.PvPLogs.Add(curLog);
            }
        }

        public static void LoadPillageLogs(this GameUser gameUser, HeroDBDataContext dbContext)
        {
            int userId = gameUser.Id;
            DateTime dtToday = DateTime.Today;

            gameUser.PvALogs.Clear();

            IEnumerable<PillageLog> atkLogs = (from item in dbContext.PillageLogs
                                               where item.AttackId == userId && item.LogTime.Date == dtToday
                                               select item);

            foreach (PillageLog log in atkLogs)
            {
                PvALog curLog = new PvALog();
                curLog.LogId = log.LogId;
                curLog.LogTime = log.LogTime;
                curLog.Opponent = dbContext.GetGameUser(log.DefenceId);
                curLog.Mode = 0;
                curLog.Result = log.Result;
                curLog.Silver = log.SilverWon;

                gameUser.PvALogs.Add(curLog);
            }

            gameUser.Base.TodayAttack = atkLogs.Count();
            gameUser.Base.SilverWon = atkLogs.Sum(log => log.SilverWon);

            IEnumerable<PillageLog> defLogs = (from item in dbContext.PillageLogs
                                               where item.DefenceId == userId && item.LogTime.Date == dtToday
                                               select item);

            foreach (PillageLog log in defLogs)
            {
                PvALog curLog = new PvALog();
                curLog.LogId = log.LogId;
                curLog.LogTime = log.LogTime;
                curLog.Opponent = dbContext.GetGameUser(log.AttackId);
                curLog.Mode = 1;
                curLog.Result = -log.Result;
                curLog.Silver = -log.SilverLost;

                gameUser.PvALogs.Add(curLog);
            }

            gameUser.Base.TodayDefence = defLogs.Count();
            gameUser.Base.SilverLost = defLogs.Sum(log => log.SilverLost);
        }

        public static void LoadUserFriends(this GameUser gameUser, HeroDBDataContext dbContext)
        {
            gameUser.UserFriends.Clear();
            gameUser.SlaveCount = 0;

            IEnumerable<Friend> friends = (from item in dbContext.Friends
                                           where (item.User01Id == gameUser.Id && item.Mode >= 0)
                                           select item);

            foreach (var fr in friends)
            {
                if (fr.ExpiryTime != null && fr.ExpiryTime < DateTime.Now)
                {
                    dbContext.Friends.DeleteOnSubmit(fr);
                    continue;
                }

                UserFriend uFriend = new UserFriend();
                uFriend.Opponent = dbContext.GetGameUser(fr.User02Id);
                uFriend.Mode = fr.Mode;
                uFriend.Expiry = fr.ExpiryTime != null ? fr.ExpiryTime.Value : new DateTime();
                gameUser.UserFriends.Add(uFriend);

                if (uFriend.Mode == (int)UserRelation.Master)
                    gameUser.SlaveCount = -1;
                else if (uFriend.Mode == (int)UserRelation.Slave && gameUser.SlaveCount >= 0)
                    gameUser.SlaveCount++;
            }

            dbContext.SubmitChanges();
        }

        public static void LoadUserEmails(this GameUser gameUser, HeroDBDataContext dbContext)
        {
            gameUser.UserMails.Clear();

            IEnumerable<MailBox> mailboxes = (from item in dbContext.MailBoxes
                                              where item.ReceverId == gameUser.Id && item.RecvTime == null
                                              orderby item.MailId descending
                                              select item);

            foreach (var mail in mailboxes)
            {
                UserMail userMail = new UserMail();
                userMail.MailId = mail.MailId;
                userMail.SenderId = mail.SenderId;

                if (mail.SenderId > 0)
                    userMail.Sender = dbContext.GetGameUser(mail.SenderId);
                else
                    userMail.Sender = new GameUser() { Id = 0, Name = "System" };

                if (userMail.Sender == null)
                    continue;

                userMail.Title = mail.Title;
                userMail.Message = mail.Message;
                if (mail.AttachItems != null)
                {
                    int[] itemList = Serialization.LoadStructArray<int>(mail.AttachItems.ToArray());
                    foreach (int itemId in itemList)
                    {
                        Item item = dbContext.Items.Where(i => i.Item_UID == itemId).FirstOrDefault();
                        if (item == null)
                            continue;

                        UserItem userItem = item.GetUserItem(gameUser);
                        userMail.Items.Add(userItem);
                    }
                }
                userMail.SendTime = mail.SendTime != null ? mail.SendTime.Value : new DateTime();
                userMail.RecvTime = mail.RecvTime != null ? mail.RecvTime.Value : new DateTime();
                gameUser.UserMails.Add(userMail);
            }
        }

        public static int LoadLoginLogs(this GameUser gameUser, HeroDBDataContext dbContext)
        {
            IEnumerable<AccountLog> loginLogs = dbContext.AccountLogs.Where(log => log.UserId == gameUser.Id && log.Action < 2);
            int loginCount = loginLogs.GroupBy(log => log.LogTime.Date).Count();
            gameUser.DailyLoginCount = (loginCount - 1) % 7 + 1;

            int todayLogin = loginLogs.Count(log => log.LogTime.Date == DateTime.Today);
            return todayLogin;
        }

        public static void UpdateGameUser(this UserEx user, GameUser gameUser, HeroDBDataContext dbContext)
        {
            gameUser.UpdateData(dbContext, user);

            // Update Items
            foreach (UserItem userItem in gameUser.UserItems)
            {
                Item item = dbContext.Items.FirstOrDefault(i => i.Item_UID == userItem.Id);
                if (item == null)
                    continue;

                userItem.UpdateData(item);
            }

            // Delete Items
            foreach (UserItem userItem in gameUser.DelItems)
            {
                Item item = dbContext.Items.FirstOrDefault(i => i.Item_UID == userItem.Id);
                if (item == null)
                    continue;

                dbContext.Items.DeleteOnSubmit(item);
            }
            gameUser.DelItems.Clear();

            // Update Roles
            foreach (UserRole userRole in gameUser.UserRoles)
            {
                Role role = dbContext.Roles.FirstOrDefault(r => r.Role_UID == userRole.Id);
                if (role == null)
                    continue;

                userRole.UpdateData(role);
            }
        }

        public static void UserSendEmail(this HeroDBDataContext dbContext, int senderId, int receiverId, string mailTitle, string mailContent, int silver, int gold, GameObjList attachedItems = null)
        {
            MailBox newMail = new MailBox();
            newMail.SenderId = senderId;
            newMail.ReceverId = receiverId;
            newMail.Title = mailTitle;
            newMail.Message = mailContent;
            newMail.Silver = silver;
            newMail.Gold = gold;
            newMail.SendTime = DateTime.Now;
            if (attachedItems != null)
            {
                List<int> addItems = new List<int>();
                foreach (var item in attachedItems)
                {
                    UserItem userItem = item as UserItem;
                    if (item == null)
                        continue;

                    if (userItem.Id == 0)
                    {
                        Item newItem = userItem.GetNewItem();
                        dbContext.Items.InsertOnSubmit(newItem);
                        dbContext.SubmitChanges();
                        userItem.Id = newItem.Item_UID;
                    }

                    addItems.Add(userItem.Id);
                }

                if (addItems.Count > 0)
                    newMail.AttachItems = Serialization.SaveStructArray(addItems.ToArray());
            }
            dbContext.MailBoxes.InsertOnSubmit(newMail);
            dbContext.SubmitChanges();
        }

        #endregion

        #region Others

        public static DateTime GetMonday(DateTime curTime)
        {
            int delta = DayOfWeek.Monday - curTime.DayOfWeek;
            if (delta > 0)
                delta -= 7;

            DateTime monday = curTime.Date.AddDays(delta);
            return monday;
        }

        public static int GetWeekNo(DateTime curTime)
        {
            return CultureInfo.CurrentCulture.Calendar.GetWeekOfYear(curTime, CalendarWeekRule.FirstDay, DayOfWeek.Monday);
        }

        public static int GetRandomInt(int[] values)
        {
            if (values == null || values.Length < 1)
                return 0;

            int idx = Random.Next(values.Length);
            return values[idx];
        }

        public static int GetRandomIndex(float[] values, int startIndex = 0)
        {
            float total = values.Sum();
            float rank = (float)Helpers.Random.NextDouble() * total;

            for (int i = startIndex; i < values.Length; i++)
            {
                rank -= values[i];
                if (rank < 0)
                    return i;
            }

            return startIndex;
        }

        public static int[] GetIntArray(byte[] byteArray)
        {
            if (byteArray.Length < 4)
                return null;

            int[] result = new int[(byteArray.Length / 4)];
            Buffer.BlockCopy(byteArray, 0, result, 0, result.Length * 4);
            return result;
        }

        public static byte[] GetByteArray(int[] intArray)
        {
            if (intArray.Length < 1)
                return null;

            byte[] result = new byte[intArray.Length * 4];
            Buffer.BlockCopy(intArray, 0, result, 0, result.Length);
            return result;
        }

        #endregion
    }
}
