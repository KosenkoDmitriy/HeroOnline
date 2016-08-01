using System;
using System.IO;
using System.Xml.Serialization;

namespace DEngine.Common.Config
{
    public static class ServerConfig
    {
        #region XmlSerialization

        [XmlRoot("ServerConfig")]
        public class ServerConfigXml
        {
            public string MasterIP;
            public int MasterPort;
            public int ZoneMaxCCU;
            public int WorldId;
            public string ServiceUrl;
            public string WebsiteUrl;
        }

        private static void InitConfig(ServerConfigXml xmlConfig)
        {
            MASTER_IP = xmlConfig.MasterIP;
            MASTER_PORT = xmlConfig.MasterPort;
            ZONE_MAXCCU = xmlConfig.ZoneMaxCCU;
            WORLD_ID = xmlConfig.WorldId;
            SERVICE_URL = xmlConfig.ServiceUrl;
            WEBSITE_URL = xmlConfig.WebsiteUrl;
        }

        public static void LoadFile(string xmlFileName)
        {
            using (StreamReader sr = new StreamReader(xmlFileName))
            {
                XmlSerializer xs = new XmlSerializer(typeof(ServerConfigXml));
                ServerConfigXml xmlConfig = (ServerConfigXml)xs.Deserialize(sr);
                InitConfig(xmlConfig);
            }
        }

        #endregion

        public static string MASTER_IP;
        public static int MASTER_PORT;
        public static int ZONE_MAXCCU;
        public static int WORLD_ID;
        public static string SERVICE_URL;
        public static string WEBSITE_URL;

        static ServerConfig()
        {
            Reload();
        }

        public static void Reload()
        {
            LoadFile("Config\\ServerConfig.xml");
        }
    }
}
