using SqlToGraphite.Config;

namespace SqlToGraphite.Conf
{
    public interface IConfigPersister
    {
        void Save(SqlToGraphiteConfig config);

        void Save(SqlToGraphiteConfig config, string path);
    }
}