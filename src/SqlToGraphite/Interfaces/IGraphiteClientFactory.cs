using Graphite;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public interface IGraphiteClientFactory
    {
        IStatsClient Create(Client client);
    }
}