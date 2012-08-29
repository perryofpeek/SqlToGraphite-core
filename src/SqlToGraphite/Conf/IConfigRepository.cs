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

        void AddClient(string name, string port);

        void AddHost(string name, List<string> roles);

        void AddWorkItem(SqlToGraphiteConfigTemplatesWorkItems workItem);

        void AddTask(TaskProperties taskProperties);

        void Save();
    }
}