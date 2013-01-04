using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class GraphiteClientFactory : IGraphiteClientFactory
    {
        private const string ErrorMessage = "Unknown Graphite Client Type:";

        private readonly ILog log;

        public GraphiteClientFactory(ILog log)
        {
            this.log = log;
        }

        public IStatsClient Create(Client client)
        {
            var clientType = client.ClientName.ToLower();
            log.Debug(string.Format("Creating a graphite client of type: {0} with values {1} {2}", client.ClientName, client.Hostname, client.Port));
            if (clientType == "GraphiteTcpClient".ToLower())
            {
                return new GraphiteTcpClient(client.Hostname, client.Port);
            }

            if (clientType == "GraphiteUdpClient".ToLower())
            {
                return new GraphiteUdpClient(client.Hostname, client.Port);
            }

            if (clientType == "StatsdClient".ToLower())
            {
                return new StatsdClient(client.Hostname, client.Port);
            }

            throw new UnknownGraphiteClientTypeException(string.Format("{0} {1}", ErrorMessage, clientType));
        }
    }
}