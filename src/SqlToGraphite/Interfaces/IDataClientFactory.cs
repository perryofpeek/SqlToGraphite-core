using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public interface IDataClientFactory
    {
        ISqlClient Create(Job job);
    }
}