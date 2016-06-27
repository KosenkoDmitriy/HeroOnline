using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DEngine.Common.Services
{
    public class WorldInfo
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string ServiceAddress;

        [XmlAttribute]
        public int Version;

        [XmlAttribute]
        public string GameDB;
    }

    [XmlRoot("WorldList")]
    public class WorldInfoList
    {
        [XmlElement("World")]
        public List<WorldInfo> Worlds;

        public WorldInfoList()
        {
            Worlds = new List<WorldInfo>();
        }

        public static WorldInfoList FromXML(string xmlText)
        {
            using (StringReader strReader = new StringReader(xmlText))
            {
                XmlSerializer xs = new XmlSerializer(typeof(WorldInfoList));
                WorldInfoList sheet = (WorldInfoList)xs.Deserialize(strReader);
                return sheet;
            }
        }

        public string ToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(WorldInfoList));
            XmlSerializerNamespaces xn = new XmlSerializerNamespaces();
            xn.Add("", "");

            using (StringWriter strWriter = new StringWriter())
            {
                xs.Serialize(strWriter, this, xn);
                return strWriter.ToString();
            }
        }
    }
}
