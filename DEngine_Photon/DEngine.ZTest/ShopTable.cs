using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEngine.Test
{
    class ShopTable
    {
        static string[] csvSpliter = new string[] { "\",\"" };

        static void UpdateData(string csvFile)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                dbContext.Shops.DeleteAllOnSubmit(dbContext.Shops);
                dbContext.SubmitChanges();

                string[] allLines = File.ReadAllLines(csvFile);

                for (int i = 1; i < allLines.Length; i++)
                {
                    string curLine = allLines[i];
                    if (curLine.Length < 2)
                        continue;

                    curLine = curLine.Substring(1, curLine.Length - 2);

                    string[] allFields = curLine.Split(csvSpliter, StringSplitOptions.None);
                    if (allFields.Length < 7)
                        continue;

                    Shop shop = new Shop();
                    dbContext.Shops.InsertOnSubmit(shop);

                    int f = 0;
                    shop.ShopId = Int32.Parse(allFields[f++]);
                    shop.ItemName = allFields[f++];
                    shop.ItemKind = Int32.Parse(allFields[f++]);
                    shop.ItemId = Int32.Parse(allFields[f++]);
                    shop.UserLevel = Int32.Parse(allFields[f++]);
                    shop.Promotion = Int32.Parse(allFields[f++]);
                    shop.PriceSilver = Int32.Parse(allFields[f++]);
                    shop.PriceGold = Int32.Parse(allFields[f++]);
                    shop.PriceUSD = Single.Parse(allFields[f++]);
                    shop.PriceVND = Single.Parse(allFields[f++]);
                    shop.Discount = Int32.Parse(allFields[f++]);
                }

                dbContext.SubmitChanges();
            }
        }

        public static void Update()
        {
            UpdateData(@"D:\0-HeroGame\0-Documents\ChargeShop.csv");
        }
    }
}
