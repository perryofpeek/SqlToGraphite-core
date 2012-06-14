using System.IO;
using System.Xml;

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
    }
}