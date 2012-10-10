using System.IO;
using System.Xml;

using SqlToGraphite.Config;

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

        public static SqlToGraphiteConfig SerialiseDeserialise(SqlToGraphiteConfig hh)
        {
            var genericSerializer = new GenericSerializer();
            var xml = genericSerializer.Serialize<SqlToGraphiteConfig>(hh);
            var sqlToGraphiteConfig = genericSerializer.Deserialize<SqlToGraphiteConfig>(xml);
            return sqlToGraphiteConfig;
        }
    }
}