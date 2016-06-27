using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEngine.Test
{
    class ItemTable
    {
        static string[] csvSpliter = new string[] { "\",\"" };

        static void UpdateData(string csvFile)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                dbContext.ItemBases.DeleteAllOnSubmit(dbContext.ItemBases);
                dbContext.SubmitChanges();

                string[] allLines = File.ReadAllLines(csvFile);

                for (int i = 1; i < allLines.Length; i++)
                {
                    string curLine = allLines[i];
                    if (curLine.Length < 2)
                        continue;

                    curLine = curLine.Substring(1, curLine.Length - 2);

                    string[] allFields = curLine.Split(csvSpliter, StringSplitOptions.None);
                    if (allFields.Length < 12)
                        continue;

                    ItemBase itemBase = new ItemBase();
                    dbContext.ItemBases.InsertOnSubmit(itemBase);

                    int f = 0;
                    itemBase.ItemId = Int32.Parse(allFields[f++]);
                    itemBase.Name = allFields[f++];
                    itemBase.Desc = allFields[f++];
                    itemBase.Kind = Int32.Parse(allFields[f++]);
                    itemBase.SubKind = Int32.Parse(allFields[f++]);
                    itemBase.Level = Int32.Parse(allFields[f++]);
                    itemBase.Stack = Int32.Parse(allFields[f++]);
                    itemBase.Attrib01 = Single.Parse(allFields[f++]);
                    itemBase.Step01 = Single.Parse(allFields[f++]);
                    itemBase.MinVal01 = Single.Parse(allFields[f++]);
                    itemBase.MaxVal01 = Single.Parse(allFields[f++]);
                    itemBase.Attrib02 = Single.Parse(allFields[f++]);
                    itemBase.Step02 = Single.Parse(allFields[f++]);
                    itemBase.MinVal02 = Single.Parse(allFields[f++]);
                    itemBase.MaxVal02 = Single.Parse(allFields[f++]);
                    itemBase.SellPrice = Int32.Parse(allFields[f++]);
                }

                dbContext.SubmitChanges();
            }
        }

        public static void Update()
        {
            UpdateData(@"D:\0-HeroGame\0-Documents\ItemBase.csv");
        }
    }
}
