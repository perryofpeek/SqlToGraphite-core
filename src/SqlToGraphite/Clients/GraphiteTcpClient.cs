using System.Xml.Serialization;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class GraphiteTcpClient : Client
    {
        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }

        public GraphiteTcpClient()
        {
        }

        public GraphiteTcpClient(string hostname, int port)
        {
            this.Hostname = hostname;
            this.Port = port;
        }

        public override void Send(IResult result)
        {
            var client = new Graphite.GraphiteTcpClient(Hostname, Port);
            client.Send(result.FullPath, result.Value, result.TimeStamp);
        }
    }
}