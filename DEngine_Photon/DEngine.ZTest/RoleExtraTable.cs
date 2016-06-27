using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEngine.Test
{
    class RoleExtraTable
    {
        static string[] csvSpliter = new string[] { "\",\"" };

        static void UpdateData(string csvFile)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                dbContext.RoleExtras.DeleteAllOnSubmit(dbContext.RoleExtras);
                dbContext.SubmitChanges();

                string[] allLines = File.ReadAllLines(csvFile);

                for (int i = 1; i < allLines.Length; i++)
                {
                    string curLine = allLines[i];
                    if (curLine.Length < 2)
                    {
                        Console.WriteLine("Line {0} is too short...", i);
                        continue;
                    }

                    curLine = curLine.Substring(1, curLine.Length - 2);

                    string[] allFields = curLine.Split(csvSpliter, StringSplitOptions.None);
                    if (allFields.Length < 3)
                    {
                        Console.WriteLine("Line {0} format is invalid...", i);
                        break;
                    }

                    RoleExtra roleBase = new RoleExtra();
                    dbContext.RoleExtras.InsertOnSubmit(roleBase);

                    int f = 1;
                    roleBase.RoleId = Int32.Parse(allFields[f++]);
                    roleBase.ElemId = Int32.Parse(allFields[f++]);

                    if (allFields[f] != "0")
                        roleBase.Skills = allFields[f++];
                }

                dbContext.SubmitChanges();
            }
        }

        public static void Update()
        {
            UpdateData(@"D:\0-HeroGame\0-Documents\RoleExtra.csv");
        }
    }
}
