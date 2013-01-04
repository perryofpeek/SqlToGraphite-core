using System;
using System.Collections.Generic;
using System.Linq;
using log4net;
using SqlToGraphite.Config;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Conf
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IConfigPersister configPersister;

        private readonly IConfigReader configReader;

        private readonly ICache cache;

        private readonly ISleep sleep;

        private readonly ILog log;

        private readonly int errorReadingConfigSleepTime;

        private readonly IGenericSerializer genericSerializer;

        private List<string> errors;
        private GraphiteClients clientList;
        public const string FailedToLoadAnyConfiguration = "Failed to load any configuration";
        public const string FailedToFindClients = "Failed to find any clients defined";
        public const string FailedToFindHosts = "Failed to find any hosts defined";
        public const string FailedToFindTemplates = "Failed to find any templates defined";
        public const string UnknownClient = "unknown client defined";

        private SqlToGraphiteConfig masterConfig;

        public ConfigRepository(IConfigReader configReader, ICache cache, ISleep sleep, ILog log, int errorReadingConfigSleepTime, IGenericSerializer genericSerializer)
        {
            this.configReader = configReader;
            this.cache = cache;
            this.sleep = sleep;
            this.log = log;
            this.errorReadingConfigSleepTime = errorReadingConfigSleepTime;
            this.genericSerializer = genericSerializer;
            clientList = new GraphiteClients();
            var dir = new DirectoryImpl();
            this.masterConfig = new SqlToGraphiteConfig(new AssemblyResolver(dir, log), log);
        }

        public ConfigRepository(IConfigReader configReader, ICache cache, ISleep sleep, ILog log, int errorReadingConfigSleepTime, IConfigPersister configPersister, IGenericSerializer genericSerializer)
            : this(configReader, cache, sleep, log, errorReadingConfigSleepTime, genericSerializer)
        {
            this.configPersister = configPersister;
        }

        public void Load()
        {
            if (cache.HasExpired())
            {
                log.Debug("Cache has expired");
                SqlToGraphiteConfig graphiteConfig = null;
                while (graphiteConfig == null)
                {
                    try
                    {
                        graphiteConfig = GetConfig(configReader);
                        if (graphiteConfig == null)
                        {
                            log.Error(FailedToLoadAnyConfiguration);
                            this.errors.Add(FailedToLoadAnyConfiguration);
                            this.RestForAwhile();
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex);
                        this.RestForAwhile();
                    }
                }

                this.masterConfig = graphiteConfig;
                Init();
                cache.ResetCache();
            }
        }

        private void RestForAwhile()
        {
            log.Debug(string.Format("sleeping for {0} mins", errorReadingConfigSleepTime));
            this.sleep.Sleep(errorReadingConfigSleepTime);
        }

        private SqlToGraphiteConfig GetConfig(IConfigReader configurationReader)
        {
            var xml = configurationReader.GetXml();
            return genericSerializer.Deserialize<SqlToGraphiteConfig>(xml);
        }

        private void Init()
        {
            this.errors = new List<string>();
            clientList = new GraphiteClients();
        }

        public List<string> Errors
        {
            get
            {
                return errors;
            }
        }

        public ListOfUniqueType<Client> GetClients()
        {
            return this.masterConfig.Clients;
        }

        public List<Template> GetTemplates()
        {
            return this.masterConfig.Templates;
        }

        public List<Host> GetHosts()
        {
            return this.masterConfig.Hosts;
        }

        public bool Validate()
        {
            this.masterConfig.Validate();
            var rtn = true;
            if (masterConfig.Templates.Count == 0)
            {
                errors.Add(FailedToFindTemplates);
                rtn = false;
            }

            if (masterConfig.Hosts.Count == 0)
            {
                errors.Add(FailedToFindHosts);
                rtn = false;
            }

            if (masterConfig.Clients.Count == 0)
            {
                errors.Add(FailedToFindClients);
                rtn = false;
            }

            return rtn;
        }

        public GraphiteClients GetClientList()
        {
            return this.clientList;
        }

        public void AddClient(Client client)
        {
            this.masterConfig.Clients.Add(client);
        }

        public void AddHost(string name, List<Role> roles)
        {
            var host = new Host { Name = name, Roles = roles };
            this.masterConfig.Hosts.Add(host);
        }

        public void AddWorkItem(WorkItems workItem)
        {
            this.masterConfig.Templates[0].WorkItems.Add(workItem);
        }

        public void AddJob(Job job)
        {
            this.masterConfig.Jobs.Add(job);
        }

        public void AddTask(TaskDetails taskProperties)
        {
            var added = false;
            foreach (var template in this.masterConfig.Templates)
            {
                foreach (var wi in template.WorkItems)
                {
                    added = this.AddIfRolesAreTheSame(taskProperties, added, wi);
                }
            }

            if (!added)
            {
                var taskSetItem = CreateTaskSetItem(taskProperties);
                if (masterConfig.Templates.Count == 0)
                {
                    masterConfig.Templates.Add(new Template());
                }

                var workItem = new WorkItems { RoleName = taskProperties.Role, TaskSet = new List<TaskSet> { taskSetItem } };
                this.masterConfig.Templates[0].WorkItems.Add(workItem);
            }
        }

        public void Save()
        {
            this.configPersister.Save(this.masterConfig);
        }

        public void Save(string path)
        {
            this.configPersister.Save(this.masterConfig, path);
        }

        private bool AddIfRolesAreTheSame(TaskDetails taskProperties, bool added, WorkItems template)
        {
            if (template.RoleName == taskProperties.Role)
            {
                foreach (var t in template.TaskSet)
                {
                    added = AddTaskIfFrequencyIsTheSame(taskProperties, t);
                }

                if (!added)
                {
                    added = this.AddTaskToNewFrequency(taskProperties, template);
                }
            }

            return added;
        }

        private bool AddTaskToNewFrequency(TaskDetails taskProperties, WorkItems template)
        {
            template.TaskSet.Add(this.CreateTaskSetItem(taskProperties));
            return true;
        }

        private static bool AddTaskIfFrequencyIsTheSame(TaskDetails taskProperties, TaskSet t)
        {
            bool added = false;
            if (t.Frequency == taskProperties.Frequency)
            {
                t.Tasks.Add(CreateTask(taskProperties));
                added = true;
            }

            return added;
        }

        private TaskSet CreateTaskSetItem(TaskDetails taskProperties)
        {
            var tasks = new List<Task> { CreateTask(taskProperties) };
            var taskSetItem = new TaskSet { Frequency = taskProperties.Frequency, Tasks = tasks };
            return taskSetItem;
        }

        private static Task CreateTask(TaskDetails taskProperties)
        {
            return new Task { JobName = taskProperties.JobName };
        }

        public List<Job> GetJobs()
        {
            return this.masterConfig.Jobs;
        }

        public Job GetJob(string jobName)
        {
            foreach (var job in this.masterConfig.Jobs)
            {
                if (job.Name == jobName)
                {
                    return job;
                }
            }

            throw new JobNotFoundException();
        }

        public Client GetClient(string clientName)
        {
            foreach (var client in this.masterConfig.Clients)
            {
                if (client.ClientName == clientName)
                {
                    return client;
                }
            }

            throw new ClientNotFoundException(string.Format("Client {0} is not found", clientName));
        }

        public void DeleteJobFromRole(string jobName, int frequency, string roleName)
        {
            var found = false;
            foreach (var template in masterConfig.Templates)
            {
                foreach (var wi in template.WorkItems)
                {
                    if (wi.RoleName == roleName)
                    {
                        foreach (var ts in wi.TaskSet)
                        {
                            if (ts.Frequency == frequency)
                            {
                                found = RemoveJob(jobName, ts);
                            }
                        }
                    }
                }
            }

            if (!found)
            {
                throw new JobNotFoundException();
            }
        }

        public TaskSet GetTaskSet(string roleName, int frequency)
        {
            var wi = this.GetRole(roleName);
            return null;
        }

        public WorkItems GetRole(string rolename)
        {
            foreach (var template in masterConfig.Templates)
            {
                foreach (var wi in template.WorkItems)
                {
                    if (wi.RoleName == rolename)
                    {
                        return wi;
                    }
                }
            }

            return EmptyRole();
        }

        private WorkItems EmptyRole()
        {
            return new WorkItems();
        }

        /*
        This needs to be better should should be able to not have to loop so much. 
        */
        private static bool RemoveJob(string jobName, TaskSet ts)
        {
            bool found = false;
            Task foundTask = null;
            foreach (var t in ts.Tasks)
            {
                if (t.JobName == jobName)
                {
                    found = true;
                    foundTask = t;
                }
            }

            if (found)
            {
                ts.Tasks.Remove(foundTask);
            }

            return found;
        }

        public void DeleteRole(string roleName)
        {
            var role = this.GetRole(roleName);
            if (!role.IsEmpty())
            {
                masterConfig.Templates[0].WorkItems.Remove(role);
            }
        }

        public void DeleteRoleFrequency(string roleName, int frequency)
        {
            var role = this.GetRole(roleName);
            if (!role.IsEmpty())
            {
                TaskSet taskToDelete = null;

                foreach (var ts in role.TaskSet)
                {
                    if (ts.Frequency == frequency)
                    {
                        taskToDelete = ts;
                    }
                }

                if (taskToDelete != null)
                {
                    role.TaskSet.Remove(taskToDelete);
                }
            }
        }

        public void AddRoleFrequency(int frequency, string roleName)
        {
            var role = this.GetRole(roleName);
            var ts = new TaskSet { Frequency = frequency, Tasks = new List<Task>() };
            role.TaskSet.Add(ts);
        }

        public void AddNewRole(string roleName)
        {
            var wi = new WorkItems { RoleName = roleName, TaskSet = new List<TaskSet>() };
            this.masterConfig.Templates[0].WorkItems.Add(wi);
        }

        public void DeleteHost(string hostname)
        {
            masterConfig.Hosts.Remove(this.GetHostByName(hostname));
        }

        private Host GetHostByName(string hostname)
        {
            Host foundHost = null;
            foreach (var host in this.masterConfig.Hosts.Where(host => host.Name == hostname))
            {
                foundHost = host;
            }

            ThrowExceptionIfHostIsNotFound(hostname, foundHost);

            return foundHost;
        }

        private static void ThrowExceptionIfHostIsNotFound(string hostname, Host foundHost)
        {
            if (foundHost == null)
            {
                throw new HostNotFoundException(string.Format("Host {0} has not been found", hostname));
            }
        }

        public void AddRoleToHost(string roleName, string hostname)
        {
            var host = this.GetHostByName(hostname);
            host.Roles.Add(new Role() { Name = roleName });
        }

        public void DeleteRoleFromHost(string roleName, string hostname)
        {
            Role foundRole = null;
            var host = this.GetHostByName(hostname);
            foreach (var role in host.Roles)
            {
                if (role.Name == roleName)
                {
                    foundRole = role;
                }
            }

            if (foundRole != null)
            {
                host.Roles.Remove(foundRole);
            }
            else
            {
                throw new RoleNotFoundException(string.Format("Role {0} is not found", roleName));
            }
        }
    }
}