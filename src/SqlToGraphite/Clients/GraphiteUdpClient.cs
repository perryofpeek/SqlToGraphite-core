using System.Xml.Serialization;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    using System.Collections.Generic;

    using Graphite;

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

        public override void Send(IList<IResult> result)
        {
            var client = new Graphite.GraphiteUdpClient(Hostname, Port);
            var gs = new GraphiteMetrics(result);
            client.Send(gs);
        }
    }
}