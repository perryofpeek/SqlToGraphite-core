namespace SqlToGraphite
{
    public interface IClientFactory
    {
        ISqlClient Create(TaskParams taskParams);
    }
}