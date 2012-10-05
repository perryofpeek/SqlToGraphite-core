using System.Xml.Serialization;

namespace ConfigSpike
{
    public interface IClient
    {
        [XmlAttribute]
        string ClientName { get; set; }

        [XmlAttribute]
        int Port { get; set; }
    }
}