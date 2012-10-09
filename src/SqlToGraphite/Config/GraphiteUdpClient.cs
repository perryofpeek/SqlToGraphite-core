using System.Xml.Serialization;

namespace ConfigSpike
{
    public class GraphiteUdpClient : Client
    {
        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }
    }
}