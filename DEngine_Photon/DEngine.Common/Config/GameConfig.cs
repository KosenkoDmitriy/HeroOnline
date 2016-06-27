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
    public class GameEvent
    {
        public string Name;
        public string Schedule;
        public DateTime StartTime;
        public DateTime EndTime;
        public DateTime LastRunTime;
        public string EventData;
    }

    public class HeroXml
    {
        [XmlAttribute]
        public int Count;

        [XmlAttribute]
        public int Grade;
    }

    public class EquipXml
    {
        [XmlAttribute]
        public int Count;

        [XmlAttribute]
        public int MinLevel;

        [XmlAttribute]
        public int MaxLevel;

        [XmlAttribute]
        public string Grades;

        public float[] GradesList
        {
            get { return Utilities.GetArrayFloat(Grades); }
        }
    }

    public class ItemXml
    {
        [XmlAttribute]
        public int Count;

        [XmlAttribute]
        public string ItemId;

        [XmlAttribute]
        public string ItemRate;

        public int[] ItemIdList
        {
            get { return Utilities.GetArrayInt(ItemId); }
        }

        public float[] ItemRateList
        {
            get { return Utilities.GetArrayFloat(ItemRate); }
        }
    }

    public class PackageXml
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public int Price;

        public HeroXml Hero;

        public EquipXml Equipment;

        [XmlElement("Item")]
        public ItemXml[] Items;
    }

    public static class GameConfig
    {
        #region XmlSerialization

        public struct PillageXml
        {
            [XmlAttribute]
            public int RefreshUsers;

            [XmlAttribute]
            public int BuyAttack;

            [XmlAttribute]
            public int MaxAttack;

            [XmlAttribute]
            public int MaxDefence;
        }

        public class GameEventXml
        {
            [XmlAttribute]
            public int Id;

            [XmlAttribute]
            public string Name;

            public string Schedule;
            public DateTime StartTime;
            public DateTime EndTime;
            public string EventData;
        }

        [XmlRoot("GameConfig")]
        public class GameConfigXml
        {
            public int DungeonLevel;
            public int ArenaLevel;
            public int ArenaWinSilver;
            public int ArenaLostSilver;
            public int PillageLevel;
            public PillageXml Pillage;
            public string DailyAwards;
            public string OnlineAwards;

            [XmlArrayItem("Event")]
            public GameEventXml[] GameEvents;

            [XmlArrayItem("Package")]
            public PackageXml[] Promotions;
        }

        private static void InitConfig(GameConfigXml xmlConfig)
        {
            DUNGEONLEVEL = xmlConfig.DungeonLevel;
            ARENALEVEL = xmlConfig.ArenaLevel;
            ARENAWINSILVER = xmlConfig.ArenaWinSilver;
            ARENALOSTSILVER = xmlConfig.ArenaLostSilver;
            PILLAGELEVEL = xmlConfig.PillageLevel;
            PLE_REFRESSUSERS = xmlConfig.Pillage.RefreshUsers;
            PLE_BUYATTACK = xmlConfig.Pillage.BuyAttack;
            PLE_MAXATTACK = xmlConfig.Pillage.MaxAttack;
            PLE_MAXDEFENCE = xmlConfig.Pillage.MaxDefence;
            DAILY_STEPS = Utilities.GetArrayInt(xmlConfig.DailyAwards);
            ONLINE_STEPS = Utilities.GetArrayInt(xmlConfig.OnlineAwards);

            DAILY_AWARDS = new List<GameAward>[DAILY_STEPS.Length];
            ONLINE_AWARDS = new List<GameAward>[ONLINE_STEPS.Length];

            GAME_EVENTS = new Dictionary<int, GameEvent>();
            foreach (GameEventXml eventXml in xmlConfig.GameEvents)
            {
                GameEvent eventInfo = new GameEvent()
                {
                    Name = eventXml.Name,
                    Schedule = eventXml.Schedule,
                    StartTime = eventXml.StartTime,
                    EndTime = eventXml.EndTime,
                    LastRunTime = DateTime.Now,
                    EventData = eventXml.EventData
                };

                GAME_EVENTS[eventXml.Id] = eventInfo;
            }

            PROMOTION_PACKS = new List<PackageXml>(xmlConfig.Promotions);
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(GameConfigXml));
                GameConfigXml xmlConfig = (GameConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(GameConfigXml));
                GameConfigXml xmlConfig = (GameConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        private static void InitAwards(List<GameAward> awardList, string[] allLines)
        {
            for (int i = 1; i < allLines.Length; i++)
            {
                string curLine = allLines[i];
                if (curLine.Length < 2)
                    continue;
                curLine = curLine.Substring(1, curLine.Length - 2);

                string[] allFields = curLine.Split(Utilities.CsvSpliter, StringSplitOptions.RemoveEmptyEntries);
                if (allFields.Length < 6)
                    continue;

                GameAward award = new GameAward();
                award.Silver = Int32.Parse(allFields[1]);
                award.Gold = Int32.Parse(allFields[2]);
                award.UserExp = Int32.Parse(allFields[3]);
                award.Honor = Int32.Parse(allFields[4]);

                int[] itemArray = Utilities.GetArrayInt(allFields[5]);
                for (int j = 0; j < itemArray.Length / 2; j++)
                {
                    int itemId = itemArray[j * 2];
                    int itemCount = itemArray[j * 2 + 1];
                    award.Items.Add(itemId, itemCount);
                }

                awardList.Add(award);
            }
        }

        public static void LoadDailyAwards()
        {
            for (int i = 1; i < DAILY_AWARDS.Length; i++)
            {
                DAILY_AWARDS[i] = new List<GameAward>();
                DAILY_AWARDS[i].Add(new GameAward()); // Level0

                try
                {
#if _UNITY
                    string csvFile = string.Format("LoginAwards/Day{0}", i);
                    TextAsset textAsset = Resources.Load(csvFile) as TextAsset;
                    if (textAsset == null)
                        continue;

                    string[] allLines = textAsset.text.Split('\n');
                    InitAwards(DAILY_AWARDS[i], allLines);
#else
                    string csvFile = string.Format("Config\\LoginAwards\\Day{0}.csv", i);
                    string[] allLines = File.ReadAllLines(csvFile);
                    InitAwards(DAILY_AWARDS[i], allLines);
#endif
                }
                catch
                {
                }
            }
        }

        public static void LoadOnlineAwards()
        {
            for (int i = 1; i < ONLINE_AWARDS.Length; i++)
            {
                ONLINE_AWARDS[i] = new List<GameAward>();
                ONLINE_AWARDS[i].Add(new GameAward()); // Level0

                try
                {
#if _UNITY
                    string csvFile = string.Format("LoginAwards/Online{0}", i);
                    TextAsset textAsset = Resources.Load(csvFile) as TextAsset;
                    if (textAsset == null)
                        continue;

                    string[] allLines = textAsset.text.Split('\n');
                    InitAwards(ONLINE_AWARDS[i], allLines);
#else
                    string csvFile = string.Format("Config\\LoginAwards\\Online{0}.csv", i);
                    string[] allLines = File.ReadAllLines(csvFile);
                    InitAwards(ONLINE_AWARDS[i], allLines);
#endif
                }
                catch
                {
                }
            }
        }

        #endregion

        public static int DUNGEONLEVEL = 5;
        public static int ARENALEVEL = 5;
        public static int ARENAWINSILVER = 1800;
        public static int ARENALOSTSILVER = 2000;
        public static int PILLAGELEVEL = 5;
        public static int PLE_REFRESSUSERS = 3;
        public static int PLE_BUYATTACK = 3;
        public static int PLE_MAXATTACK = 5;
        public static int PLE_MAXDEFENCE = 3;

        public static ElemType[] ELEMS_OVER_FORWARD = new ElemType[(int)ElemType.Count];
        public static ElemType[] ELEMS_OVER_RESVERSE = new ElemType[(int)ElemType.Count];

        public static int[] DAILY_STEPS;
        public static List<GameAward>[] DAILY_AWARDS;

        public static int[] ONLINE_STEPS;
        public static List<GameAward>[] ONLINE_AWARDS;

        public static Dictionary<int, GameEvent> GAME_EVENTS;

        public static List<PackageXml> PROMOTION_PACKS;

        static GameConfig()
        {
            Reload();

            ELEMS_OVER_FORWARD[(int)ElemType.Metal] = ElemType.Wood;
            ELEMS_OVER_FORWARD[(int)ElemType.Wood] = ElemType.Earth;
            ELEMS_OVER_FORWARD[(int)ElemType.Water] = ElemType.Fire;
            ELEMS_OVER_FORWARD[(int)ElemType.Fire] = ElemType.Metal;
            ELEMS_OVER_FORWARD[(int)ElemType.Earth] = ElemType.Water;

            ELEMS_OVER_RESVERSE[(int)ElemType.Wood] = ElemType.Metal;
            ELEMS_OVER_RESVERSE[(int)ElemType.Earth] = ElemType.Wood;
            ELEMS_OVER_RESVERSE[(int)ElemType.Water] = ElemType.Earth;
            ELEMS_OVER_RESVERSE[(int)ElemType.Fire] = ElemType.Water;
            ELEMS_OVER_RESVERSE[(int)ElemType.Metal] = ElemType.Fire;
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("GameConfig");
#else
            LoadFile("Config\\GameConfig.xml");
#endif
            LoadDailyAwards();
            LoadOnlineAwards();
        }
    }
}
