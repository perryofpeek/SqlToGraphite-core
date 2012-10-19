using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

using SqlToGraphite;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;

using SqlToGraphiteInterfaces;

using log4net;

namespace Configurator.code
{
    public class Controller
    {
        private ILog log;

        private ConfigRepository repository;

        private AssemblyResolver assemblyResolver;

        public Controller()
        {
            this.Initialise(Directory.GetCurrentDirectory());
        }

        public void Initialise(string path)
        {
            log = LogManager.GetLogger("log");
            log4net.Config.XmlConfigurator.Configure();

            var sleepTime = 0;
            assemblyResolver = new AssemblyResolver(new DirectoryImpl());
            var config = new SqlToGraphiteConfig(assemblyResolver);
            var reader = new ConfigHttpReader(path, "", "");
            var cache = new Cache(new TimeSpan(0, 0, 1, 0), log);
            var sleep = new Sleeper();
            var genericSerializer = new GenericSerializer();
            var configPersister = new ConfigPersister(new ConfigFileWriter(path), genericSerializer);

            repository = new ConfigRepository(reader, cache, sleep, this.log, sleepTime, configPersister, genericSerializer);
        }

        public void LoadConfig(string path)
        {
            this.Initialise(path);
            repository.Load();
        }

        public List<SqlToGraphite.Config.Host> GetHosts()
        {
            return repository.GetHosts();
        }

        public List<Job> GetJobs()
        {
            return repository.GetJobs();
        }

        public ISqlClient GetTypedJob(string name)
        {
            var dataClientFactory = new DataClientFactory(log, assemblyResolver);
            return dataClientFactory.Create(repository.GetJob(name));
        }

        public ListOfUniqueType<Client> GetClientTypes()
        {
            return repository.GetClients();
        }

        public void AddJob(ISqlClient client)
        {
            repository.AddJob((Job)client);
        }

        public void Save(string fileName)
        {
            repository.Save(fileName);
        }

        public List<string> GetRoles()
        {
            var templates = repository.GetTemplates();
            return templates[0].WorkItems.Select(workItem => workItem.RoleName).ToList();
        }

        public void GetJobsInRole(string name)
        {
            //var templates = repository.GetTemplates();
            //foreach (var wi in templates[0].WorkItems)
            //{
            //    if(wi.RoleName == name)
            //    {
            //        foreach (var taskSet in wi.TaskSet)
            //        {
            //            taskSet.
            //        }
            //        wi.TaskSet
            //    }
            //}                       
        }

        public List<WorkItems> GetWorkItems()
        {
            var templates = repository.GetTemplates();
            if (templates.Count > 0)
            {
                return templates[0].WorkItems;
            }
            return new List<WorkItems>();
        }


        public List<int> GetTasksFrequencyInRole(string name)
        {
            var rtn = new List<int>();
            foreach (var taskSet in GetRoleTaskSet(name))
            {
                rtn.Add(taskSet.Frequency);
            }
            return rtn;
        }

        private IEnumerable<TaskSet> GetRoleTaskSet(string name)
        {
            var templates = repository.GetTemplates();
            foreach (var wi in templates[0].WorkItems)
            {
                if (wi.RoleName == name)
                {
                    return wi.TaskSet;
                }
            }
            return null;
        }

        public List<string> GetTasksWithFrequencyInRole(int selectedFrequency, string name)
        {
            var rtn = new List<string>();
            var templates = repository.GetTemplates();
            foreach (var taskSet in GetRoleTaskSet(name))
            {
                if (taskSet.Frequency == selectedFrequency)
                {
                    foreach (var task in taskSet.Tasks)
                    {
                        rtn.Add(task.JobName);
                    }
                }
            }
            return rtn;
        }

        public void AddJobToRoleAndFrequency(string role, int frequency, string jobName)
        {
            repository.AddTask(new TaskDetails(role,frequency,jobName));
        }

        public void DeleteJobFromRole(string jobName, int frequency, string roleName)
        {
            repository.DeleteJobFromRole(jobName, frequency, roleName);
        }
    }
}