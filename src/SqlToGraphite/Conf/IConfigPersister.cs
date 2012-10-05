using ConfigSpike.Config;

namespace SqlToGraphite.Conf
{
    public interface IConfigPersister
    {
        void Save(SqlToGraphiteConfig config);
    }
}