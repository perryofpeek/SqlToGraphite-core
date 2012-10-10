using System.Xml.Serialization;

using ConfigSpike;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class LocalGraphiteTcpClient : Client
    {
        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }

        public LocalGraphiteTcpClient()
        {
        }

        public LocalGraphiteTcpClient(string hostname, int port)
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