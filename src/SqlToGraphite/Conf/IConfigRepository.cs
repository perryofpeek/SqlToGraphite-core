using System.Collections.Generic;

using ConfigSpike;
using ConfigSpike.Config;

namespace SqlToGraphite.Conf
{
    public interface IConfigRepository
    {
        void Load();

        List<string> Errors { get; }

        ListOfUniqueType<Client> GetClients();

        List<Template> GetTemplates();

        List<Host> GetHosts();

        bool Validate();

        GraphiteClients GetClientList();

        void AddClient(Client client);

        void AddHost(string name, List<Role> roles);

        void AddWorkItem(WorkItems workItem);

        void AddTask(TaskDetails taskProperties);

        void Save();

        List<Job> GetJobs();

        Job GetJob(string jobName);

        Client GetClient(string clientName);
    }
}