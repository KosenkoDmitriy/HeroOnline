using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public class RoleMaxData
    {
        public int Level;
        public int MaxHP;
        public int MaxMP;
        public int Attack;
        public int Defence;
        public int AtkSpeed;
        public float CritRate;
        public float CritPower;

        public RoleMaxData(int level)
        {
            Level = level;

            int curRate = (level + 10) * (level + 10);
            int maxRate = (RoleConfig.LEVEL_MAX + 10) * (RoleConfig.LEVEL_MAX + 10);

            MaxHP = RoleConfig.HEALTH_MIN + RoleConfig.HEALTH_MAX * curRate / maxRate;
            MaxMP = RoleConfig.MANA_MIN + RoleConfig.MANA_MAX * curRate / maxRate;
            Attack = RoleConfig.ATTACK_MIN + RoleConfig.ATTACK_MAX * curRate / maxRate;
            Defence = RoleConfig.DEFENCE_MIN + RoleConfig.DEFENCE_MAX * curRate / maxRate;
            AtkSpeed = RoleConfig.ATKSPEED_MIN + RoleConfig.ATKSPEED_MAX * curRate / maxRate;

            CritRate = (float)RoleConfig.CRITRATE_MIN + (float)RoleConfig.CRITRATE_MAX * level / RoleConfig.LEVEL_MAX;
            CritPower = (float)RoleConfig.CRITPOWER_MIN + (float)RoleConfig.CRITPOWER_MAX * level / RoleConfig.LEVEL_MAX;
        }
    }

    public class RoleGrade
    {
        public int[] Grades;
        public float[] Rates;

        public RoleGrade(int rank)
        {
            Grades = new int[] { rank, rank + 1 };
            Rates = new float[] { 50f, 50f };
        }
    }

    public class RoleUpgrade
    {
        public int RoleId;
        public int Grade;
        public int Level;
        public int Silver;
        public int Gold;
        public Dictionary<int, int> Items;

        public RoleUpgrade(string input, int[] items)
        {
            int[] iValues = Utilities.GetArrayInt(input);
            RoleId = iValues[0];
            Grade = iValues[1];
            Level = iValues[2];

            Items = new Dictionary<int, int>();
            for (int i = 3; i < iValues.Length && i < items.Length + 3; i++)
            {
                int itemId = items[i - 3];
                int itemCount = iValues[i];
                if (itemCount > 0)
                {
                    if (Items.ContainsKey(itemId))
                        Items[itemId] += itemCount;
                    else
                        Items[itemId] = itemCount;
                }
            }
        }
    }

    public static class RoleConfig
    {
        public static RoleGrade GetGradeData(int rank)
        {
            if (rank == 3)
                return SHOP_RANK03;
            else if (rank == 2)
                return SHOP_RANK02;
            return SHOP_RANK01;
        }

        #region XmlSerialization

        public struct MinMaxXml
        {
            [XmlAttribute]
            public int Min;

            [XmlAttribute]
            public int Max;
        }

        [XmlType("General")]
        public struct GeneralXml
        {
            public int HPReg;
            public int MPReg;
            public int EPReg;
            public int EvasRate;
            public int HitRate;
            public int CritRateDef;
            public int CritPowerDef;
            public int EvolveSivler;
            public MinMaxXml Energy;
            public MinMaxXml Level;
            public MinMaxXml Health;
            public MinMaxXml Mana;
            public MinMaxXml Attack;
            public MinMaxXml Defence;
            public MinMaxXml AtkSpeed;
            public MinMaxXml CritRate;
            public MinMaxXml CritPower;
            public string LevelExp;
            public string GradeExp;
            public string GradeAttrib;
        }

        [XmlType("ShopRank")]
        public struct ShopRankXml
        {
            [XmlAttribute]
            public int id;
            public string Grade;
            public string Rate;
        }

        [XmlType("Upgrade")]
        public struct UpgradeXml
        {
            public string Items;

            public string Silver;

            public string Gold;

            [XmlElement("UpData")]
            public string[] UpDatas;
        }

        [XmlRoot("RoleConfig")]
        public class RoleConfigXml
        {
            public GeneralXml General;

            public ShopRankXml[] ShopRanks;

            public UpgradeXml Upgrade;
        }

        private static void InitConfig(RoleConfigXml xmlConfig)
        {
            HP_REG = xmlConfig.General.HPReg;
            MP_REG = xmlConfig.General.MPReg;
            EP_REG = xmlConfig.General.EPReg;
            EVAS_RATE = xmlConfig.General.EvasRate;
            HIT_RATE = xmlConfig.General.HitRate;
            CRIT_RATE_DEF = xmlConfig.General.CritRateDef;
            CRIT_POWER_DEF = xmlConfig.General.CritPowerDef;
            EVOLVE_SIVLER = xmlConfig.General.EvolveSivler;

            ENERGY_MIN = xmlConfig.General.Energy.Min;
            ENERGY_MAX = xmlConfig.General.Energy.Max;
            LEVEL_MIN = xmlConfig.General.Level.Min;
            LEVEL_MAX = xmlConfig.General.Level.Max;
            HEALTH_MIN = xmlConfig.General.Health.Min;
            HEALTH_MAX = xmlConfig.General.Health.Max;
            MANA_MIN = xmlConfig.General.Mana.Min;
            MANA_MAX = xmlConfig.General.Mana.Max;
            ATTACK_MIN = xmlConfig.General.Attack.Min;
            ATTACK_MAX = xmlConfig.General.Attack.Max;
            DEFENCE_MIN = xmlConfig.General.Defence.Min;
            DEFENCE_MAX = xmlConfig.General.Defence.Max;
            ATKSPEED_MIN = xmlConfig.General.AtkSpeed.Min;
            ATKSPEED_MAX = xmlConfig.General.AtkSpeed.Max;
            CRITRATE_MIN = xmlConfig.General.CritRate.Min;
            CRITRATE_MAX = xmlConfig.General.CritRate.Max;
            CRITPOWER_MIN = xmlConfig.General.CritPower.Min;
            CRITPOWER_MAX = xmlConfig.General.CritPower.Max;

            LEVELS_EXP = Utilities.GetArrayInt(xmlConfig.General.LevelExp);
            GRADES_EXP = Utilities.GetArrayInt(xmlConfig.General.GradeExp);
            GRADES_ATTRIB = Utilities.GetArrayInt(xmlConfig.General.GradeAttrib);

            foreach (var item in xmlConfig.ShopRanks)
            {
                if (item.id == 1)
                {
                    SHOP_RANK01.Grades = Utilities.GetArrayInt(item.Grade);
                    SHOP_RANK01.Rates = Utilities.GetArrayFloat(item.Rate);
                }
                else if (item.id == 2)
                {
                    SHOP_RANK02.Grades = Utilities.GetArrayInt(item.Grade);
                    SHOP_RANK02.Rates = Utilities.GetArrayFloat(item.Rate);
                }
                else if (item.id == 3)
                {
                    SHOP_RANK03.Grades = Utilities.GetArrayInt(item.Grade);
                    SHOP_RANK03.Rates = Utilities.GetArrayFloat(item.Rate);
                }
            }

            ROLE_UPGRADE = new List<RoleUpgrade>();

            int[] silverArr = Utilities.GetArrayInt(xmlConfig.Upgrade.Silver);
            int[] goldArr = Utilities.GetArrayInt(xmlConfig.Upgrade.Gold);

            int[] items = Utilities.GetArrayInt(xmlConfig.Upgrade.Items);
            foreach (string input in xmlConfig.Upgrade.UpDatas)
            {
                RoleUpgrade roleUp = new RoleUpgrade(input, items);
                roleUp.Silver = silverArr[roleUp.Grade];
                roleUp.Gold = goldArr[roleUp.Grade];

                ROLE_UPGRADE.Add(roleUp);
            }

            ROLE_MAXDATA = new RoleMaxData[LEVEL_MAX + 1];
            for (int i = 0; i <= LEVEL_MAX; i++)
            {
                RoleMaxData maxData = new RoleMaxData(i);
                ROLE_MAXDATA[i] = maxData;
            }
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(RoleConfigXml));
                RoleConfigXml xmlConfig = (RoleConfigXml)xs.Deserialize(sr);
                InitConfig(xmlConfig);
            }
        }

        public static void LoadResource(string assetName)
        {
#if _UNITY
            TextAsset textAsset = Resources.Load(assetName) as TextAsset;
            if (textAsset == null)
                return;

            using (TextReader tr = new StringReader(textAsset.text))
            {
                XmlSerializer xs = new XmlSerializer(typeof(RoleConfigXml));
                RoleConfigXml xmlConfig = (RoleConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static int HP_REG = 1;
        public static int MP_REG = 1;
        public static int EP_REG = 1;
        public static int EVAS_RATE = 5;
        public static int HIT_RATE = 85;
        public static int CRIT_RATE_DEF = 5;
        public static int CRIT_POWER_DEF = 150;
        public static int EVOLVE_SIVLER = 200;

        public static int ENERGY_MIN;
        public static int ENERGY_MAX;
        public static int LEVEL_MIN;
        public static int LEVEL_MAX;
        public static int HEALTH_MIN;
        public static int HEALTH_MAX;
        public static int MANA_MIN;
        public static int MANA_MAX;
        public static int ATTACK_MIN;
        public static int ATTACK_MAX;
        public static int DEFENCE_MIN;
        public static int DEFENCE_MAX;
        public static int ATKSPEED_MIN;
        public static int ATKSPEED_MAX;
        public static int CRITRATE_MIN;
        public static int CRITRATE_MAX;
        public static int CRITPOWER_MIN;
        public static int CRITPOWER_MAX;

        public static int[] LEVELS_EXP = new int[] { 80, 240, 480, 800, 1200, 1680, 2240, 2880, 3600, 4400 };
        public static int[] GRADES_EXP = new int[] { 20, 30, 40, 50, 60, 75 };
        public static int[] GRADES_ATTRIB = new int[] { 50, 60, 70, 80, 90, 100 };

        public static RoleGrade SHOP_RANK01 = new RoleGrade(1);
        public static RoleGrade SHOP_RANK02 = new RoleGrade(2);
        public static RoleGrade SHOP_RANK03 = new RoleGrade(3);

        public static List<RoleUpgrade> ROLE_UPGRADE;

        public static RoleMaxData[] ROLE_MAXDATA;

        static RoleConfig()
        {
            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("RoleConfig");
#else
            LoadFile("Config\\RoleConfig.xml");
#endif
        }
    }
}
