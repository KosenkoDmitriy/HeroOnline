using DEngine.Common.GameLogic;
using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public class GradeRate
    {
        public float[] GreenRate;
        public float[] BlueRate;
        public float[] YellowRate;
    }

    public class SlotOption
    {
        public int Kind;
        public int Grade;
        public int Attrib;
        public float Rate;
        public float Step;
        public float MinValue;
        public float MaxValue;

        public SlotOption(string input)
        {
            float[] values = Utilities.GetArrayFloat(input);
            Kind = (int)values[0];
            Grade = (int)values[1];
            Attrib = (int)values[2];
            Rate = values[3];
            Step = values[4];
            MinValue = values[5];
            MaxValue = values[6];
        }
    }

    public class RankUpgrade
    {
        public struct ItemMat
        {
            public int ItemId;
            public int Count;
        }

        public ItemMat[] Materials;
        public int[] NoGoldRate;
        public int[] RequiredSilver;
        public int[] RequiredGold;

        public RankUpgrade(string matDesc, string nogoldRate, string silverDesc, string goldDesc)
        {
            int[] items = Utilities.GetArrayInt(matDesc);
            Materials = new ItemMat[items.Length / 2];
            for (int i = 0; i < Materials.Length; i++)
            {
                Materials[i].ItemId = items[2 * i];
                Materials[i].Count = items[2 * i + 1];
            }

            NoGoldRate = Utilities.GetArrayInt(nogoldRate);
            RequiredSilver = Utilities.GetArrayInt(silverDesc);
            RequiredGold = Utilities.GetArrayInt(goldDesc);
        }
    }

    public static class ItemConfig
    {
        #region XmlSerialization

        [XmlType("GradeRates")]
        public struct GradeRatesXml
        {
            public string GreenRate;

            public string BlueRate;

            public string YellowRate;
        }

        public struct UpgradeFeeXml
        {
            public string Silver;
            public string Gold;
        }

        public struct ItemUpgradesXml
        {
            [XmlAttribute]
            public int LevelCount;

            public string NoGoldRate;

            [XmlElement("Material")]
            public string[] Materials;

            [XmlElement("UpgradeFee")]
            public UpgradeFeeXml[] UpgradeFees;
        }

        [XmlRoot("ItemConfig")]
        public class ItemConfigXml
        {
            public GradeRatesXml GradeRates;

            public ItemUpgradesXml ItemUpgrades;

            [XmlArrayItem("GradeRank")]
            public string[] GradeRanks;

            [XmlArrayItem("SlotOption")]
            public string[] SlotOptions;
        }

        private static void InitConfig(ItemConfigXml xmlConfig)
        {
            GRADE_RANKS = new List<float[]>();
            foreach (string rank in xmlConfig.GradeRanks)
                GRADE_RANKS.Add(Utilities.GetArrayFloat(rank));

            GRADE_RATES = new GradeRate();
            GRADE_RATES.GreenRate = Utilities.GetArrayFloat(xmlConfig.GradeRates.GreenRate);
            GRADE_RATES.BlueRate = Utilities.GetArrayFloat(xmlConfig.GradeRates.BlueRate);
            GRADE_RATES.YellowRate = Utilities.GetArrayFloat(xmlConfig.GradeRates.YellowRate);

            SLOT_OPTIONS = new SlotOption[xmlConfig.SlotOptions.Length];
            for (int i = 0; i < SLOT_OPTIONS.Length; i++)
                SLOT_OPTIONS[i] = new SlotOption(xmlConfig.SlotOptions[i]);

            WHITE_UPGRADES = new RankUpgrade[xmlConfig.ItemUpgrades.LevelCount + 1];
            GREEN_UPGRADES = new RankUpgrade[xmlConfig.ItemUpgrades.LevelCount + 1];
            BLUE_UPGRADES = new RankUpgrade[xmlConfig.ItemUpgrades.LevelCount + 1];
            YELLOW_UPGRADES = new RankUpgrade[xmlConfig.ItemUpgrades.LevelCount + 1];

            for (int i = 1; i <= xmlConfig.ItemUpgrades.LevelCount; i++)
            {
                WHITE_UPGRADES[i] = new RankUpgrade(xmlConfig.ItemUpgrades.Materials[0], xmlConfig.ItemUpgrades.NoGoldRate,
                    xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Silver, xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Gold);

                GREEN_UPGRADES[i] = new RankUpgrade(xmlConfig.ItemUpgrades.Materials[1], xmlConfig.ItemUpgrades.NoGoldRate,
                    xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Silver, xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Gold);

                BLUE_UPGRADES[i] = new RankUpgrade(xmlConfig.ItemUpgrades.Materials[2], xmlConfig.ItemUpgrades.NoGoldRate,
                    xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Silver, xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Gold);

                YELLOW_UPGRADES[i] = new RankUpgrade(xmlConfig.ItemUpgrades.Materials[3], xmlConfig.ItemUpgrades.NoGoldRate,
                    xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Silver, xmlConfig.ItemUpgrades.UpgradeFees[i - 1].Gold);
            }
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ItemConfigXml));
                ItemConfigXml xmlConfig = (ItemConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(ItemConfigXml));
                ItemConfigXml xmlConfig = (ItemConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static List<float[]> GRADE_RANKS;
        public static GradeRate GRADE_RATES;
        public static SlotOption[] SLOT_OPTIONS;

        public static RankUpgrade[] WHITE_UPGRADES;
        public static RankUpgrade[] GREEN_UPGRADES;
        public static RankUpgrade[] BLUE_UPGRADES;
        public static RankUpgrade[] YELLOW_UPGRADES;

        static ItemConfig()
        {
            Reload();
        }
        
        public static void Reload()
        {
#if _UNITY
            LoadResource("ItemConfig");
#else
            LoadFile("Config\\ItemConfig.xml");
#endif
        }
    }
}
