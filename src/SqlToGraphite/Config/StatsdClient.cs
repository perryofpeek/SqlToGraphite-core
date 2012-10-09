using System.Xml.Serialization;

using ConfigSpike;

namespace SqlToGraphite.Config
{
    public class StatsdClient : Client
    {
        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }
    }
}