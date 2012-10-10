
using System.Xml.Serialization;

namespace SqlToGraphiteInterfaces
{
    public interface IStatsClient
    {
        [XmlAttribute]
        string ClientName { get; set; }

        [XmlAttribute]
        int Port { get; set; }

        void Send(IResult result);
    }
}
