using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public class HouseData
    {
        public string Name;
        public int Id;
        public int Gold;
        public int Silver;
        public int SizeX;
        public int SizeY;
        public int Lost;
        public int MaxCount;
    }

    public static class LandConfig
    {
        #region XmlSerialization

        [XmlType("House")]
        public class HouseXml
        {
            [XmlAttribute]
            public int Id;

            [XmlAttribute]
            public string Name;

            [XmlAttribute]
            public int Gold;

            [XmlAttribute]
            public int Silver;

            [XmlAttribute]
            public int SizeX;

            [XmlAttribute]
            public int SizeY;

            [XmlAttribute]
            public int Lost;

            [XmlAttribute]
            public int MaxCount;
        }

        [XmlRoot("LandConfig")]
        public class LandConfigXml
        {
            public string ExpandLand;
            public string OpenLand;
            public HouseXml[] Houses;
        }

        private static void InitConfig(LandConfigXml xmlConfig)
        {
            int[] intArr = Utilities.GetArrayInt(xmlConfig.ExpandLand);
            EXPAND_GOLD = intArr[0];
            EXPAND_SILVER = intArr[1];

            intArr = Utilities.GetArrayInt(xmlConfig.OpenLand);
            OPEN_GOLD = intArr[0];
            OPEN_SILVER = intArr[1];

            HOUSE_DATA.Clear();
            foreach (var item in xmlConfig.Houses)
            {
                HouseData data = new HouseData()
                {
                    Id = item.Id,
                    Name = item.Name,
                    Gold = item.Gold,
                    Silver = item.Silver,
                    SizeX = item.SizeX,
                    SizeY = item.SizeY,
                    Lost = item.Lost,
                    MaxCount = item.MaxCount,
                };

                HOUSE_DATA[item.Id] = data;
            }
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(LandConfigXml));
                LandConfigXml xmlConfig = (LandConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(LandConfigXml));
                LandConfigXml xmlConfig = (LandConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static Dictionary<int, HouseData> HOUSE_DATA;
        public static int EXPAND_GOLD = 20;
        public static int EXPAND_SILVER = 0;
        public static int OPEN_GOLD = 0;
        public static int OPEN_SILVER = 300;
        public static int OPEN_POINT = 50;
        public static int CLOSE_POINT = 30;
        public static int TOWER_ID = 3;

        static LandConfig()
        {
            HOUSE_DATA = new Dictionary<int, HouseData>();
            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("LandConfig");
#else
            LoadFile("Config\\LandConfig.xml");
#endif
        }
    }
}
