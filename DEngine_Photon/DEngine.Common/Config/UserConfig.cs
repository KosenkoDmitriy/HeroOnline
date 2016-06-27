using System;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public static class UserConfig
    {
        #region XmlSerialization

        [XmlRoot("UserConfig")]
        public class UserConfigXml
        {
            public int LevelMin;
            public int LevelMax;
            public string LevelExp;
            public int BattleRoles;
        }

        private static void InitConfig(UserConfigXml xmlConfig)
        {
            BATTLE_ROLES = xmlConfig.BattleRoles;
            LEVEL_MIN = xmlConfig.LevelMin;
            LEVEL_MAX = xmlConfig.LevelMax;
            LEVELS_EXP = Utilities.GetArrayInt(xmlConfig.LevelExp);
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(UserConfigXml));
                UserConfigXml xmlConfig = (UserConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(UserConfigXml));
                UserConfigXml xmlConfig = (UserConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static int BATTLE_ROLES = 3;
        public static int LEVEL_MIN = 1;
        public static int LEVEL_MAX = 10;
        public static int[] LEVELS_EXP = new int[] { 40, 240, 720, 1600, 3000, 5040, 7840, 11520, 16200, 22000 };

        static UserConfig()
        {
            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("UserConfig");
#else
            LoadFile("Config\\UserConfig.xml");
#endif
        }
    }
}
