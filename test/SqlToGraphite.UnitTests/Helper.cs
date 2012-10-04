using System.IO;
using System.Xml;

using ConfigSpike.Config;

namespace SqlToGraphite.UnitTests
{
    public class Helper
    {
        public static StreamReader GetStreamReader(string xml)
        {
            var ms = new MemoryStream();
            var sw = new StreamWriter(ms);
            sw.WriteLine(xml);
            sw.Flush();
            ms.Position = 0;
            var sr = new StreamReader(ms);
            return sr;
        }

        public static XmlDocument GetXmlDocument(string configXml)
        {
            var rtn = new XmlDocument();
            rtn.LoadXml(configXml);
            return rtn;
        }

        public static ConfigSpike.Config.SqlToGraphiteConfig SerialiseDeserialise(ConfigSpike.Config.SqlToGraphiteConfig hh)
        {
            var xml = GenericSerializer.Serialize<ConfigSpike.Config.SqlToGraphiteConfig>(hh);
            var sqlToGraphiteConfig = GenericSerializer.Deserialize<ConfigSpike.Config.SqlToGraphiteConfig>(xml);
            return sqlToGraphiteConfig;
        }
    }
}