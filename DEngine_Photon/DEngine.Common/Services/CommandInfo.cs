using System;
using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace LOLServices.Models
{
    public enum CommandCode
    {
        None,
        ZoneReload,
        ZoneRestart,
        ZoneMessage,
        UserMessage,
        UserKick,
        UserBlock,
        UserAddCash,
        Count
    }

    public class CommandInfo
    {
        [XmlAttribute]
        public int Code;

        [XmlAttribute]
        public int UserId;

        [XmlAttribute]
        public string UserName;

        [XmlAttribute]
        public string Params;

        public override string ToString()
        {
            return string.Format("{0}\t{1}\t{2}", Code, UserId, UserName);
        }
    }

    [XmlRoot("CommandList")]
    public class CommandInfoList
    {
        [XmlElement("World")]
        public List<CommandInfo> Commands;

        public CommandInfoList()
        {
            Commands = new List<CommandInfo>();
        }

        public static CommandInfoList FromXML(string xmlText)
        {
            using (StringReader strReader = new StringReader(xmlText))
            {
                XmlSerializer xs = new XmlSerializer(typeof(CommandInfoList));
                CommandInfoList sheet = (CommandInfoList)xs.Deserialize(strReader);
                return sheet;
            }
        }

        public string ToXML()
        {
            XmlSerializer xs = new XmlSerializer(typeof(CommandInfoList));
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