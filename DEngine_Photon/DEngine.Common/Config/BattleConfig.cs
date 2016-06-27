using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

#if _UNITY
using UnityEngine;
#endif

namespace DEngine.Common.Config
{
    public static class BattleConfig
    {
        #region XmlSerialization

        [XmlRoot("BattleConfig")]
        public class BattleConfigXml
        {
            public float RoleSpace;

            public float MaxTime;

            public int ItemTake;

            public int ItemDelay;
        }

        private static void InitConfig(BattleConfigXml xmlConfig)
        {
            ROLE_SPACE = xmlConfig.RoleSpace;

            BATTLE_DURATION = xmlConfig.MaxTime;

            ITEM_TAKEWITH = xmlConfig.ItemTake;

            ITEM_USEDELAY = xmlConfig.ItemDelay;
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(BattleConfigXml));
                BattleConfigXml xmlConfig = (BattleConfigXml)xs.Deserialize(sr);
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
                XmlSerializer xs = new XmlSerializer(typeof(BattleConfigXml));
                BattleConfigXml xmlConfig = (BattleConfigXml)xs.Deserialize(tr);
                InitConfig(xmlConfig);
            }
#endif
        }

        #endregion

        public static float ROLE_SPACE;

        public static float BATTLE_DURATION;

        public static float ITEM_TAKEWITH;

        public static float ITEM_USEDELAY;

        static BattleConfig()
        {
            ROLE_SPACE = 2.5f;

            BATTLE_DURATION = 300f; // 5 minutes

            Reload();
        }

        public static void Reload()
        {
#if _UNITY
            LoadResource("BattleConfig");
#else
            LoadFile("Config\\BattleConfig.xml");
#endif
        }
    }
}
