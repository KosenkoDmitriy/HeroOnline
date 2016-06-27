using DEngine.Common.GameLogic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public class DungeonData
    {
        public int Id;
        public string Name;
        public int Energy;
        public int PvERate;
        public int PvARate;
        public int LootRate;
        public int StatusRate;

        public WaveData[] PvEWaves;
        public WaveData[] BossWaves;
        public DropItem[] PvEDrops;
        public DropItem[] LootDrops;
        public DropItem[] BossDrops;
    }

    public static class DungeonConfig
    {
        #region XmlSerialization

        [XmlType("WinAward")]
        public struct WinAwardXml
        {
            [XmlAttribute]
            public int Level;

            [XmlAttribute]
            public int Gold;

            [XmlAttribute]
            public int Silver;

            [XmlAttribute]
            public int UserExp;

            [XmlAttribute]
            public int RoleExp;
        }

        public struct DtopItemXml
        {
            public string Items;
            public string Quantity;
            public string Rates;
        }

        public class WaveXml
        {
            [XmlAttribute]
            public int Grade;

            [XmlAttribute]
            public int Time;

            [XmlAttribute]
            public string Mob1;

            [XmlAttribute]
            public string Mob2;

            [XmlAttribute]
            public string Mob3;
        }

        [XmlType("Dungeon")]
        public struct DungeonXml
        {
            [XmlAttribute]
            public int Id;

            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public int Energy;

            [XmlAttribute]
            public int PvERate;

            [XmlAttribute]
            public int PvARate;

            [XmlAttribute]
            public int LootRate;

            [XmlAttribute]
            public int StatusRate;

            [XmlArrayItem("Wave")]
            public WaveXml[] PvEWaves;

            [XmlArrayItem("Wave")]
            public WaveXml[] BossWaves;

            public DtopItemXml PvEDrops;

            public DtopItemXml LootDrops;

            public DtopItemXml BossDrops;
        }

        [XmlRoot("DungeonConfig")]
        public class DungeonConfigXml
        {
            public int EventMin;

            public int EventMax;

            public WinAwardXml[] WinAwards;

            public DungeonXml[] Dungeons;
        }

        private static void InitConfig(DungeonConfigXml xmlConfig)
        {
            EVENTS_MIN = xmlConfig.EventMin;
            
            EVENTS_MAX = xmlConfig.EventMax;

            AWARDS = new GameAward[xmlConfig.WinAwards.Length + 1];
            for (int j = 0; j < xmlConfig.WinAwards.Length; j++)
            {
                var item = xmlConfig.WinAwards[j];
                AWARDS[j + 1] = new GameAward() { UserExp = item.UserExp, RoleExp = item.RoleExp, Silver = item.Silver, Gold = item.Gold };
            }

            DUNGEONS = new DungeonData[xmlConfig.Dungeons.Length];
            for (int i = 0; i < DUNGEONS.Length; i++)
            {
                DungeonXml dungeon = xmlConfig.Dungeons[i];

                DungeonData curDungeon = new DungeonData();
                curDungeon.Id = dungeon.Id;
                curDungeon.Name = dungeon.Name;
                curDungeon.Energy = dungeon.Energy;
                curDungeon.PvERate = dungeon.PvERate;
                curDungeon.PvARate = dungeon.PvARate;
                curDungeon.LootRate = dungeon.LootRate;
                curDungeon.StatusRate = dungeon.StatusRate;

                curDungeon.PvEWaves = new WaveData[dungeon.PvEWaves.Length];
                for (int j = 0; j < curDungeon.PvEWaves.Length; j++)
                {
                    WaveData waveData = new WaveData()
                    {
                        MobLevel = 0,
                        MobGrade = dungeon.PvEWaves[j].Grade,
                        WaitTime = dungeon.PvEWaves[j].Time,
                        Mobs01 = new MobData(dungeon.PvEWaves[j].Mob1),
                        Mobs02 = new MobData(dungeon.PvEWaves[j].Mob2),
                        Mobs03 = new MobData(dungeon.PvEWaves[j].Mob3),
                    };
                    curDungeon.PvEWaves[j] = waveData;
                }

                curDungeon.BossWaves = new WaveData[dungeon.BossWaves.Length];
                for (int j = 0; j < curDungeon.BossWaves.Length; j++)
                {
                    WaveData waveData = new WaveData()
                    {
                        MobLevel = 0,
                        MobGrade = dungeon.BossWaves[j].Grade,
                        WaitTime = dungeon.BossWaves[j].Time,
                        Mobs01 = new MobData(dungeon.BossWaves[j].Mob1),
                        Mobs02 = new MobData(dungeon.BossWaves[j].Mob2),
                        Mobs03 = new MobData(dungeon.BossWaves[j].Mob3),
                    };
                    curDungeon.BossWaves[j] = waveData;
                }

                curDungeon.PvEDrops = GetDropItemArray(dungeon.PvEDrops);
                curDungeon.LootDrops = GetDropItemArray(dungeon.LootDrops);
                curDungeon.BossDrops = GetDropItemArray(dungeon.BossDrops);

                DUNGEONS[i] = curDungeon;
            }
        }

        private static DropItem[] GetDropItemArray(DtopItemXml dropItemXml)
        {
            int[] iValues = Utilities.GetArrayInt(dropItemXml.Items);
            int[] iCount = Utilities.GetArrayInt(dropItemXml.Quantity);
            float[] fValues = Utilities.GetArrayFloat(dropItemXml.Rates);

            if (iValues.Length != iCount.Length || iValues.Length != fValues.Length)
                return null;

            DropItem[] retArray = new DropItem[iValues.Length];
            for (int i = 0; i < retArray.Length; i++)
                retArray[i] = new DropItem { Id = iValues[i], Quantity = iCount[i], Rate = fValues[i] };

            return retArray;
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(DungeonConfigXml));
                DungeonConfigXml xmlConfig = (DungeonConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(DungeonConfigXml));
                DungeonConfigXml xmlConfig = (DungeonConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static GameAward[] AWARDS;

        public static DungeonData[] DUNGEONS;

        public static int EVENTS_MIN = 4;

        public static int EVENTS_MAX = 6;

        static DungeonConfig()
        {
            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("DungeonConfig");
#else
            LoadFile("Config\\DungeonConfig.xml");
#endif
        }
    }
}
