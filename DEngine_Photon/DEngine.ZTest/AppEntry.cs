using DEngine.Common;
using DEngine.Common.Config;
using DEngine.Common.GameLogic;
using DEngine.HeroServer;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;

namespace DEngine.Test
{
    class AppEntry
    {
        private static HeroDB HeroDatabase = new HeroDB();

        private static char[] cmdSplit = new char[] { ' ' };

        static void CreateFullItems(string userName)
        {
            GameUser gameUser = HeroDatabase.UserGet(userName);
            if (gameUser == null)
                return;

            List<UserItem> addItems = new List<UserItem>();

            for (int i = 2; i <= 4; i++)
            {
                int itemId = (5 + (i - 2) * 10);
                for (int j = 0; j < 10; j++)
                {
                    UserItem userItem = new UserItem() { GameUser = gameUser, Grade = 4 };
                    userItem.CreateRandom(gameUser.Id, itemId, 1, 4);
                    addItems.Add(userItem);
                }
            }

            for (int i = 52; i <= 116; i++)
            {
                UserItem userItem = new UserItem() { GameUser = gameUser, Grade = 1 };
                userItem.CreateRandom(gameUser.Id, i, 90, 4);
                addItems.Add(userItem);
            }

            HeroDatabase.UserAddItems(gameUser, addItems, UserAction.None);
        }

        static void CreateFullRoles(string userName)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                UserEx theUser = dbContext.UserExes.FirstOrDefault(u => u.UserName == userName);
                if (theUser == null)
                {
                    string serviceUrl = string.Format("http://home.blueskysoft.vn/lolservices/Account/Register?userName={0}&password={1}&email={2}", userName, "123", "123@123.com");
                    int userId = HttpService.GetResponse(serviceUrl).Code;
                    if (userId < 0)
                        return;

                    theUser = dbContext.UserExes.FirstOrDefault(u => u.UserId == userId);
                    if (theUser == null)
                        return;
                }

                theUser.Silver = 100000;
                theUser.Gold = 10000;

                dbContext.Roles.DeleteAllOnSubmit(theUser.Roles);

                for (int i = 0; i < Global.GameHeroes.Count; i++)
                {
                    GameRole gameRole = (GameRole)Global.GameHeroes[i];
                    if (gameRole.Id > 10)
                        continue;

                    for (int j = 1; j <= 5; j++)
                    {
                        Role newRole = new Role()
                        {
                            RoleId = gameRole.Id,
                            Name = gameRole.Name,
                            Grade = 2,
                            Level = 1,
                            ElemId = j,
                            Energy = 10000,
                            UseTime = DateTime.Now,
                        };

                        theUser.Roles.Add(newRole);
                    }
                }

                dbContext.SubmitChanges();
            }
        }

        static void WeeklyArenaAward(GameEvent gameEvent)
        {
            HeroDatabase.UpdateUsersRank();
            GameObjList topUsers = HeroDatabase.GetTopUsers((int)UserListType.TopHonor, 1000);

            GameUser gameUser1 = (GameUser)topUsers[0];
            UserItem userItem = new UserItem();
            userItem.CreateRandomeEquip(0, gameUser1.Base.Level, 4);
            GameObjList emailAttachments = new GameObjList();
            emailAttachments.Add(userItem);
            HeroDatabase.UserSendEmail(0, gameUser1.Id, "Arena Weekly Award", "Arena award for rank: 1", 0, 0, emailAttachments);

            int[] goldValues = Utilities.GetArrayInt(gameEvent.EventData);
            for (int i = 2; i < 1000 && i <= topUsers.Count; i++)
            {
                int goldValue = 0;
                if (i <= 10)
                    goldValue = goldValues[i];
                else if (i <= 100)
                    goldValue = goldValues[11];
                else if (i <= 1000)
                    goldValue = goldValues[12];

                string mailContent = string.Format("Arena award for rank: {0}. Gold = {1}", i, goldValue);
                HeroDatabase.UserSendEmail(0, topUsers[i].Id, "Arena Weekly Award", mailContent, 0, goldValue);
            }
        }

        static void CreateBots()
        {
            string[] allLines = File.ReadAllLines("BotNames.txt");

            for (int i = 0; i < allLines.Length; i++)
            {
                int botUserId = HeroDatabase.CreateBot(i + 3001, allLines[i]);
                Console.WriteLine("BotUserId = {0}", botUserId);

                if (botUserId <= 0)
                    break;
            }
        }

        static void TestDBContext()
        {
            HeroDBDataContext dbContext1 = new HeroDBDataContext();
            HeroDBDataContext dbContext2 = new HeroDBDataContext();

            try
            {
                UserEx myUser1 = dbContext1.UserExes.FirstOrDefault(u => u.UserId == 1);
                UserEx myUser2 = dbContext2.UserExes.FirstOrDefault(u => u.UserId == 1);

                myUser1.Gold = 2000;
                dbContext1.SubmitChanges();

                System.Threading.Thread.Sleep(3000);

                myUser2.Gold += 100;
                dbContext2.SubmitChanges();

                Console.WriteLine("SubmitChanges OK!");
            }
            catch (Exception ex)
            {
                Console.Write("SubmitChanges Error: ");
                Console.WriteLine(ex.Message);
            }
            finally
            {
                dbContext1.Dispose();
                dbContext2.Dispose();
            }
        }

        static ErrorCode TestPack(GameUser gameUser, int pckId)
        {
            PackageXml thePack = GameConfig.PROMOTION_PACKS[pckId];

            ErrorCode errCode = gameUser.AddCash(0, -thePack.Price);
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

        static void VerifyIOSReceipts(string receiptData)
        {
            string debugLog = "";

            try
            {
                string verifyURL = "https://buy.itunes.apple.com/verifyReceipt";//"https://sandbox.itunes.apple.com/verifyReceipt";
                string password = "4d1143219cf14aefb089b6a901656e49";
                string jSonContent = string.Format("{{ \"receipt-data\": \"{0}\", \"password\": \"{1}\" }}", receiptData, password);

                WebRequest webRequest = WebRequest.Create(verifyURL);
                webRequest.Method = "POST";
                webRequest.ContentType = "application/json; charset=utf-8";
                webRequest.ContentLength = jSonContent.Length;

                using (StreamWriter oneStreamWriter = new StreamWriter(webRequest.GetRequestStream(), Encoding.ASCII))
                {
                    oneStreamWriter.Write(jSonContent);
                }

                using (WebResponse webResponse = webRequest.GetResponse())
                {
                    using (StreamReader sr = new StreamReader(webResponse.GetResponseStream()))
                    {
                        string respose = sr.ReadToEnd().Trim();
                        JSONNode jSon = JSON.Parse(respose);

                        string status = jSon["status"].Value;

                        JSONNode receipt = jSon["receipt"];
                        JSONNode in_app = receipt["in_app"];
                        JSONArray in_app_arr = in_app.AsArray;
                        JSONNode in_app_0 = in_app_arr[0];

                        JSONNode productId = in_app_0["product_id"];
                    }
                }
            }
            catch (Exception ex)
            {
                debugLog += "\nException: " + ex.Message;
            }
        }

        static void Main(string[] args)
        {
            //RoleTable.Update();
            //RoleExtraTable.Update();
            //SkillTable.Update();
            //ItemTable.Update();
            //ShopTable.Update();

            //GameUser gameUser = HeroDatabase.UserGet(1);
            //TestPack(gameUser, 0);
            //TestPack(gameUser, 1);
            //TestPack(gameUser, 2);

            Dictionary<string, string> postData = new Dictionary<string, string>();
            postData.Add("worldid", ServerConfig.WORLD_ID.ToString());
            postData.Add("userid", "1");
            postData.Add("product", "gold100");
            postData.Add("receipt", "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");

            string serviceUrl = "http://localhost:3974/Account/AppleStore";
            int cardAmount = HttpService.GetResponse(serviceUrl, postData).Code;

            //VerifyIOSReceipts("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");

            Console.Write("Done...");
            Console.ReadLine();
        }
    }
}
