namespace SqlToGraphite
{
    public class GraphiteUdpClient : IStatsClient
    {
        private readonly string hostname;

        private readonly int port;

        public GraphiteUdpClient(string hostname, int port)
        {
            this.hostname = hostname;
            this.port = port;
        }

        public void Send(IResult result)
        {
            var client = new Graphite.GraphiteUdpClient(hostname, port);
            client.Send(result.FullPath, result.Value, result.TimeStamp);
        }
    }
}