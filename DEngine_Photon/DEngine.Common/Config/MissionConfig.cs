using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public class RuneData
    {
        public int Id;
        public int Grade;
        public int Faction;
        public float XPos;
        public float YPos;
        public float ZPos;

        public RuneData(string input)
        {
            int[] intArrary = Utilities.GetArrayInt(input);
            Id = intArrary[0];
            Grade = intArrary[1];
            Faction = intArrary[2];
            XPos = intArrary[3];
            YPos = intArrary[4];
            ZPos = intArrary[5];
        }
    }

    public class MobData
    {
        public int MinCount;
        public int MaxCount;
        public int[] MobList;

        public MobData()
        {
            // Used by MissionConfig
        }

        public MobData(string input)
        {
            // Used by MissionConfig
            if (input.Length > 0)
            {
                int[] intArrary = Utilities.GetArrayInt(input);
                MinCount = intArrary[0];
                MaxCount = intArrary[1];

                MobList = new int[intArrary.Length - 2];
                Array.Copy(intArrary, 2, MobList, 0, MobList.Length);
            }
        }

        public List<int> GetMobList()
        {
            List<int> retList = new List<int>();

            if (MobList != null)
            {
                int nCount = Utilities.Random.Next(MinCount, MaxCount + 1);
                for (int i = 0; i < nCount; i++)
                {
                    int randIdx = Utilities.Random.Next(MobList.Length);
                    retList.Add(MobList[randIdx]);
                }
            }

            return retList;
        }
    }

    public class WaveData
    {
        public int WaitTime;
        public int MobLevel;
        public int MobGrade;
        public MobData Mobs01;
        public MobData Mobs02;
        public MobData Mobs03;

        public WaveData()
        {
        }

        public List<int> GetMobList()
        {
            List<int> retList = new List<int>();
            retList.AddRange(Mobs01.GetMobList());
            retList.AddRange(Mobs02.GetMobList());
            retList.AddRange(Mobs03.GetMobList());
            return retList;
        }
    }

    public class DropItem
    {
        public int Id;
        public int Quantity;
        public float Rate;
    }

    public class MissionData
    {
        public int Level;

        public int Energy;

        public int UserExp;

        public int RoleExp;

        public int Silver;

        public int[] EquipItems;

        public RuneData[] Runes;

        public WaveData[] Waves;

        public int GerRandItem()
        {
            if (EquipItems.Length > 0)
            {
                int idx = Utilities.Random.Next(EquipItems.Length);
                return EquipItems[idx];
            }

            return 0;
        }
    }

    public static class MissionConfig
    {
        #region XmlSerialization

        [XmlType("WorldDrop")]
        public class WorldDropXml
        {
            [XmlAttribute]
            public int Level;

            public string DropItems;

            public string DropRates;
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

        [XmlType("Mission")]
        public struct MissionXml
        {
            [XmlAttribute]
            public int Level;

            [XmlAttribute]
            public int Energy;

            [XmlAttribute]
            public int UserExp;

            [XmlAttribute]
            public int RoleExp;

            [XmlAttribute]
            public int Silver;

            [XmlAttribute]
            public string EquipId;

            [XmlElement("Rune")]
            public string[] Runes;

            [XmlElement("Wave")]
            public WaveXml[] Waves;
        }

        [XmlRoot("MissionConfig")]
        public class MissionConfigXml
        {
            public WorldDropXml[] WorldDrops;

            public MissionXml[] Missions;
        }

        private static void InitConfig(MissionConfigXml xmlConfig)
        {
            DROP_ITEMS = new List<DropItem[]>();
            DROP_ITEMS.Add(null);

            foreach (var line in xmlConfig.WorldDrops)
            {
                int[] iValues = Utilities.GetArrayInt(line.DropItems);
                float[] fValues = Utilities.GetArrayFloat(line.DropRates);

                DropItem[] items = new DropItem[iValues.Length];
                for (int i = 0; i < iValues.Length; i++)
                    items[i] = new DropItem() { Id = iValues[i], Quantity = 1, Rate = fValues[i] };

                DROP_ITEMS.Add(items);
            }

            MISSIONS = new MissionData[xmlConfig.Missions.Length + 1];

            foreach (MissionXml mission in xmlConfig.Missions)
            {
                MissionData missionData = new MissionData()
                {
                    Level = mission.Level,
                    UserExp = mission.UserExp,
                    RoleExp = mission.RoleExp,
                    Energy = mission.Energy,
                    Silver = mission.Silver,
                    EquipItems = Utilities.GetArrayInt(mission.EquipId),
                };

                if (mission.Runes != null)
                {
                    missionData.Runes = new RuneData[mission.Runes.Length];
                    for (int i = 0; i < mission.Runes.Length; i++)
                        missionData.Runes[i] = new RuneData(mission.Runes[i]);
                }

                if (mission.Waves != null)
                {
                    missionData.Waves = new WaveData[mission.Waves.Length];
                    for (int i = 0; i < mission.Waves.Length; i++)
                    {
                        WaveData waveData = new WaveData()
                        {
                            MobLevel = mission.Level,
                            MobGrade = mission.Waves[i].Grade,
                            WaitTime = mission.Waves[i].Time,
                            Mobs01 = new MobData(mission.Waves[i].Mob1),
                            Mobs02 = new MobData(mission.Waves[i].Mob2),
                            Mobs03 = new MobData(mission.Waves[i].Mob3),
                        };
                        missionData.Waves[i] = waveData;
                    }
                }

                MISSIONS[mission.Level] = missionData;
            }
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                try
                {
                    XmlSerializer xs = new XmlSerializer(typeof(MissionConfigXml));
                    MissionConfigXml xmlConfig = (MissionConfigXml)xs.Deserialize(sr);
                    InitConfig(xmlConfig);
                }
                catch
                {
                }
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
                XmlSerializer xs = new XmlSerializer(typeof(MissionConfigXml));
                MissionConfigXml xmlConfig = (MissionConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static List<DropItem[]> DROP_ITEMS;
        public static MissionData[] MISSIONS;

        static MissionConfig()
        {
            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("MissionConfig");
#else
            LoadFile("Config\\MissionConfig.xml");
#endif
        }
    }
}
