namespace SqlToGraphite
{
    public interface IDataClientFactory
    {
        ISqlClient Create(TaskParams taskParams);
    }
}