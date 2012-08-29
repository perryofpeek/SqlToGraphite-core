using System.Collections.Generic;

namespace SqlToGraphite.Conf
{
    public interface IConfigPersister
    {
        void Save(List<SqlToGraphiteConfigClientsClient> buildConfigObject, List<SqlToGraphiteConfigTemplatesWorkItems> templates, List<SqlToGraphiteConfigHostsHost> hosts);
    }
}