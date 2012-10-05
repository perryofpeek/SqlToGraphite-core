using System.Xml.Serialization;

namespace ConfigSpike
{
    public abstract class Client : IClient
    {
        [XmlAttribute]
        public abstract string ClientName { get; set; }

        [XmlAttribute]
        public abstract int Port { get; set; }
    }
}