using System.Xml.Serialization;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class GraphiteUdpClient : Client
    {
        public GraphiteUdpClient()
        {
        }

        public GraphiteUdpClient(string hostname, int port)
        {
            this.Hostname  = hostname;
            this.Port = port;
        }

        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }

        public override void Send(IResult result)
        {
            var client = new Graphite.GraphiteUdpClient(Hostname, Port);
            client.Send(result.FullPath, result.Value, result.TimeStamp);
        }
    }
}