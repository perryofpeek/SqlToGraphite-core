using System.Collections.Generic;

using ConfigSpike;
using ConfigSpike.Config;

namespace SqlToGraphite.Conf
{
    public interface IConfigRepository
    {
        void Load();

        List<string> Errors { get; }

        ListOfUniqueType<IClient> GetClients();

        List<Template> GetTemplates();

        List<Host> GetHosts();

        bool Validate();

        GraphiteClients GetClientList();

        void AddClient(IClient client);

        void AddHost(string name, List<Role> roles);

        void AddWorkItem(WorkItems workItem);

        void AddTask(TaskDetails taskProperties);

        void Save();

        List<IJob> GetJobs();
    }
}