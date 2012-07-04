
namespace SqlToGraphiteInterfaces
{
    public interface IStatsClient
    {
        void Send(IResult result);
    }
}
