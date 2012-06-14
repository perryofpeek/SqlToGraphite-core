namespace SqlToGraphite
{
    public interface IStatsClient
    {
        void Send(IResult result);
    }
}