using System.Xml.Serialization;

namespace ConfigSpike
{
    public interface IClient_delete
    {
        [XmlAttribute]
        string ClientName { get; set; }

        [XmlAttribute]
        int Port { get; set; }
    }
}