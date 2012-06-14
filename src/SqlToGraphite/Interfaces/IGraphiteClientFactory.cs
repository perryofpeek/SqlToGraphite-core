using Graphite;

namespace SqlToGraphite
{
    public interface IGraphiteClientFactory
    {
        IStatsClient Create(GraphiteParams graphiteParams, TaskParams taskParams);
    }
}