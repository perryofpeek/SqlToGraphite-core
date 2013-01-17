using System.Xml.Serialization;

using Graphite.StatsD;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    using System.Collections.Generic;

    public class StatsdClient : Client
    {
        [XmlAttribute]
        public override string Hostname { get; set; }

        [XmlAttribute]
        public override string ClientName { get; set; }

        [XmlAttribute]
        public override int Port { get; set; }

        private readonly StatsDClient pipe;

        public StatsdClient()
        {
        }

        public StatsdClient(string hostname, int port)
        {
            pipe = new StatsDClient(hostname, port);
        }

        public override void Send(IResult result)
        {
            pipe.Timing(result.FullPath, result.Value);
        }

        public override void Send(IList<IResult> result)
        {
            throw new System.NotImplementedException();
        }
    }
}