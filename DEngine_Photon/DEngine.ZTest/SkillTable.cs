using DEngine.Common.GameLogic;
using DEngine.HeroServer.GameData;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DEngine.Test
{
    class SkillTable
    {
        static string[] csvSpliter = new string[] { "\",\"" };

        static void UpdateData(string csvFile)
        {
            using (HeroDBDataContext dbContext = new HeroDBDataContext())
            {
                dbContext.SkillBases.DeleteAllOnSubmit(dbContext.SkillBases);
                dbContext.SubmitChanges();

                string[] allLines = File.ReadAllLines(csvFile);

                for (int i = 1; i < allLines.Length; i++)
                {
                    string curLine = allLines[i];
                    if (curLine.Length < 2)
                        continue;

                    curLine = curLine.Substring(1, curLine.Length - 2);

                    string[] allFields = curLine.Split(csvSpliter, StringSplitOptions.None);
                    if (allFields.Length < 32)
                        continue;

                    SkillBase skillBase = new SkillBase();
                    dbContext.SkillBases.InsertOnSubmit(skillBase);

                    int f = 0;
                    skillBase.SkillId = Int32.Parse(allFields[f++]);
                    skillBase.Name = allFields[f++];
                    skillBase.Desc = allFields[f++];
                    skillBase.SkillType = Int32.Parse(allFields[f++]);
                    skillBase.TargetType = Int32.Parse(allFields[f++]);
                    skillBase.CostType = Int32.Parse(allFields[f++]);
                    skillBase.CostValue = Int32.Parse(allFields[f++]);
                    skillBase.CoolTime = Single.Parse(allFields[f++]);
                    skillBase.CastRange = Single.Parse(allFields[f++]);
                    skillBase.EffectType = Int32.Parse(allFields[f++]);
                    skillBase.EffectRange = Single.Parse(allFields[f++]);
                    skillBase.Duration = Single.Parse(allFields[f++]);
                    skillBase.SrcAtt01 = Int32.Parse(allFields[f++]);
                    skillBase.DstAtt01 = Int32.Parse(allFields[f++]);
                    skillBase.Value01 = Int32.Parse(allFields[f++]);
                    skillBase.SrcAtt02 = Int32.Parse(allFields[f++]);
                    skillBase.DstAtt02 = Int32.Parse(allFields[f++]);
                    skillBase.Value02 = Int32.Parse(allFields[f++]);
                    skillBase.SrcAtt03 = Int32.Parse(allFields[f++]);
                    skillBase.DstAtt03 = Int32.Parse(allFields[f++]);
                    skillBase.Value03 = Int32.Parse(allFields[f++]);
                    skillBase.SrcAtt04 = Int32.Parse(allFields[f++]);
                    skillBase.DstAtt04 = Int32.Parse(allFields[f++]);
                    skillBase.Value04 = Int32.Parse(allFields[f++]);
                    skillBase.SrcAtt05 = Int32.Parse(allFields[f++]);
                    skillBase.DstAtt05 = Int32.Parse(allFields[f++]);
                    skillBase.Value05 = Int32.Parse(allFields[f++]);

                    skillBase.EffectMask = 0;
                    for (int j = 0; j < 5; j++)
                    {
                        int iValue = Int32.Parse(allFields[f + j]) << j;
                        skillBase.EffectMask += iValue;
                    }

                    EffectMask effMask = (EffectMask)skillBase.EffectMask;
                }

                dbContext.SubmitChanges();
            }
        }

        public static void Update()
        {
            UpdateData(@"D:\0-HeroGame\0-Documents\SkillBase.csv");
        }
    }
}
