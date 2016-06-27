using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace DEngine.Common.Services
{
    public class AssetInfo
    {
        [XmlAttribute]
        public int Id;

        [XmlAttribute]
        public string Name;

        [XmlAttribute]
        public string DownloadUrl;

        [XmlAttribute]
        public int Version;
    }

    [XmlRoot("WorldList")]
    public class AssetInfoList
    {
        [XmlElement("World")]
        public List<AssetInfo> Worlds;

        public AssetInfoList()
        {
            Worlds = new List<AssetInfo>();
        }

        public static AssetInfoList FromXML(string xmlText)
        {
            using (StringReader strReader = new StringReader(xmlText))
            {
                XmlSerializer xs = new XmlSerializer(typeof(AssetInfoList));
                AssetInfoList sheet = (AssetInfoList)xs.Deserialize(strReader);
                return sheet;
            }
        }

        public string ToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(AssetInfoList));
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
