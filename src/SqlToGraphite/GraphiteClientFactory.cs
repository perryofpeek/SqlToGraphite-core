using Graphite;

using log4net;

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

        public IStatsClient Create(GraphiteParams graphiteParams, TaskParams taskParams)
        {
            var clientType = taskParams.Client.ToLower();
            log.Debug(string.Format("Creating a graphite client of type: {0} with values {1} {2}", taskParams.Client, graphiteParams.Hostname, graphiteParams.Port));
            if (clientType == "graphitetcp")
            {
                return new GraphiteTcpClient(graphiteParams.Hostname, graphiteParams.Port);
            }

            if (clientType == "graphiteudp")
            {
                return new GraphiteUdpClient(graphiteParams.Hostname, graphiteParams.Port);
            }

            if (clientType == "statsdudp")
            {
                return new StatsdClient(graphiteParams.Hostname, graphiteParams.Port);
            }

            throw new UnknownGraphiteClientTypeException(string.Format("{0} {1}", ErrorMessage, clientType));
        }
    }
}