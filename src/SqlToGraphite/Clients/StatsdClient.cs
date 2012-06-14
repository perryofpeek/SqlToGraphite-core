using Graphite.StatsD;

namespace SqlToGraphite
{
    public class StatsdClient : IStatsClient
    {
        private readonly StatsDClient pipe;

        public StatsdClient(string hostname, int port)
        {
            pipe = new StatsDClient(hostname, port);
        }

        public void Send(IResult result)
        {
            pipe.Timing(result.FullPath, result.Value);
        }
    }
}