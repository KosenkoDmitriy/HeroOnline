using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEngine.Test
{
    class RoleTable
    {

        static string[] csvSpliter = new string[] { "\",\"" };

        static void UpdateData(string csvFile)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                dbContext.RoleBases.DeleteAllOnSubmit(dbContext.RoleBases);
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
                    if (allFields.Length < 11)
                    {
                        Console.WriteLine("Line {0} format is invalid...", i);
                        break;
                    }

                    RoleBase roleBase = new RoleBase();
                    dbContext.RoleBases.InsertOnSubmit(roleBase);

                    int f = 0;
                    roleBase.RoleId = Int32.Parse(allFields[f++]);
                    roleBase.Name = allFields[f++];
                    roleBase.Desc = allFields[f++];
                    roleBase.Type = Int32.Parse(allFields[f++]);
                    roleBase.Class = Int32.Parse(allFields[f++]);
                    roleBase.AssetPath = allFields[f++];
                    roleBase.Strength = Int32.Parse(allFields[f++]);
                    roleBase.Agility = Int32.Parse(allFields[f++]);
                    roleBase.Intelligent = Int32.Parse(allFields[f++]);
                    roleBase.MoveSpeed = Int32.Parse(allFields[f++]);
                    roleBase.AttackRate = Int32.Parse(allFields[f++]);

                    if (allFields[f] != "0")
                        roleBase.Skills = allFields[f++];

                    if (allFields[f] != "0")
                        roleBase.Items = allFields[f++];
                }

                dbContext.SubmitChanges();
            }
        }

        public static void Update()
        {
            UpdateData(@"D:\0-HeroGame\0-Documents\RoleBase.csv");
        }
    }
}
