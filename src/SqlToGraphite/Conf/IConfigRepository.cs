using System.Collections.Generic;

namespace SqlToGraphite.Conf
{
    public interface IConfigRepository
    {
        void Load();

        List<string> Errors { get; }

        List<SqlToGraphiteConfigClientsClient> GetClients();

        List<SqlToGraphiteConfigTemplatesWorkItems> GetTemplates();

        List<SqlToGraphiteConfigHostsHost> GetHosts();

        bool Validate();

        GraphiteClients GetClientList();
    }
}