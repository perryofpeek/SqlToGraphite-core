using Graphite;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class GraphiteTcpClient : IStatsClient
    {
        private readonly string hostname;

        private readonly int port;

        public GraphiteTcpClient(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public void Send(IResult result)
        {
            var client = new Graphite.GraphiteTcpClient(hostname, port);
            client.Send(result.FullPath, result.Value, result.TimeStamp);
        }
    }
}