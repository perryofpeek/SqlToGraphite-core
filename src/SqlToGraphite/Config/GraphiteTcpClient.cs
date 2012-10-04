using System.Xml.Serialization;

namespace ConfigSpike
{
    public class GraphiteTcpClient : Client
    {
        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }
    }
}