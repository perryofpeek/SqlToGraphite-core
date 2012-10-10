using System.Xml.Serialization;

namespace SqlToGraphiteInterfaces
{
    public abstract class Client : IStatsClient
    {
        [XmlAttribute]
        public abstract string Hostname { get; set; }

        [XmlAttribute]
        public abstract string ClientName { get; set; }

        [XmlAttribute]
        public abstract int Port { get; set; }

        public abstract void Send(IResult result);
    }
}