using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;

using ConfigSpike;
using ConfigSpike.Config;

using log4net;
using SqlToGraphite.Clients;

namespace SqlToGraphite.Conf
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IConfigPersister configPersister;

        private readonly IConfigReader configReader;

        private readonly IKnownGraphiteClients knownGraphiteClients;

        private readonly ICache cache;

        private readonly ISleep sleep;

        private readonly ILog log;

        private readonly int errorReadingConfigSleepTime;

        private readonly IGenericSerializer genericSerializer;

        //private List<Client> clients;
        //private List<WorkItems> templates;
        //private List<Host> hosts;
        private List<string> errors;
        private GraphiteClients clientList;
        public const string FailedToLoadAnyConfiguration = "Failed to load any configuration";
        public const string FailedToFindClients = "Failed to find any clients defined";
        public const string FailedToFindHosts = "Failed to find any hosts defined";
        public const string FailedToFindTemplates = "Failed to find any templates defined";
        public const string UnknownClient = "unknown client defined";

        private SqlToGraphiteConfig masterConfig;

        public ConfigRepository(IConfigReader configReader, IKnownGraphiteClients knownGraphiteClients, ICache cache, ISleep sleep, ILog log, int errorReadingConfigSleepTime, IGenericSerializer genericSerializer)
        {
            this.configReader = configReader;
            this.knownGraphiteClients = knownGraphiteClients;
            this.cache = cache;
            this.sleep = sleep;
            this.log = log;
            this.errorReadingConfigSleepTime = errorReadingConfigSleepTime;
            this.genericSerializer = genericSerializer;
            //    clients = new List<Client>();
            clientList = new GraphiteClients();
            //    hosts = new List<Host>();
            //    this.templates = new List<WorkItems>();
            this.masterConfig = new SqlToGraphiteConfig();
        }

        public ConfigRepository(IConfigReader configReader, IKnownGraphiteClients knownGraphiteClients, ICache cache, ISleep sleep, ILog log, int errorReadingConfigSleepTime, IConfigPersister configPersister, IGenericSerializer genericSerializer)
            : this(configReader, knownGraphiteClients, cache, sleep, log, errorReadingConfigSleepTime, genericSerializer)
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
                //this.ParseConfigItems(graphiteConfig);
                cache.ResetCache();
            }
        }

        private void RestForAwhile()
        {
            log.Debug(string.Format("sleeping for {0} mins", errorReadingConfigSleepTime));
            this.sleep.Sleep(errorReadingConfigSleepTime);
        }

        //private void ParseConfigItems(SqlToGraphiteConfig graphiteConfig)
        //{
        //    foreach (var setting in graphiteConfig.Items)
        //    {
        //        this.AddTypes(setting);
        //    }
        //}

        //private void AddTypes(object setting)
        //{
        //    this.AddClients(setting);
        //    this.AddTemplates(setting);
        //    this.AddHosts(setting);
        //}

        //private void AddHosts(object setting)
        //{
        //    if (setting.GetType() == typeof(SqlToGraphiteConfigHosts))
        //    {
        //        var x = (SqlToGraphiteConfigHosts)setting;
        //        this.hosts.AddRange(x.host);
        //    }
        //}

        //private void AddTemplates(object setting)
        //{
        //    if (setting.GetType() == typeof(SqlToGraphiteConfigTemplates))
        //    {
        //        var x = (SqlToGraphiteConfigTemplates)setting;
        //        this.templates.AddRange(x.WorkItems);
        //    }
        //}

        //private void AddClients(object setting)
        //{
        //    if (setting.GetType() == typeof(SqlToGraphiteConfigClients))
        //    {
        //        var x = (SqlToGraphiteConfigClients)setting;
        //        this.clients.AddRange(x.Client);
        //        this.AddAllClients();
        //    }
        //}
        //private static string SerializeConfig(SqlToGraphiteConfig config)
        //{
        //    var stringStream = new StringWriter();
        //    var ser = new System.Xml.Serialization.XmlSerializer(typeof(SqlToGraphiteConfig));
        //    ser.Serialize(stringStream, config);
        //    return stringStream.ToString();
        //}
        private SqlToGraphiteConfig GetConfig(IConfigReader configReader)
        {
            var xml = configReader.GetXml();
            return genericSerializer.Deserialize<SqlToGraphiteConfig>(xml);
        }

        //private XmlReader ReadConfig(IConfigReader configReader)
        //{

        //    return XmlReader.Create(new StringReader(xml));
        //}
        private void Init()
        {
            //this.clients = new List<SqlToGraphiteConfigClientsClient>();
            //this.templates = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            //this.hosts = new List<SqlToGraphiteConfigHostsHost>();
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

        public ListOfUniqueType<IClient> GetClients()
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

        //private void AddAllClients()
        //{
        //    foreach (var client in this.GetClients())
        //    {
        //        this.AddKnownClient(knownGraphiteClients, client);
        //    }
        //}
        //private void AddKnownClient(IKnownGraphiteClients knownGraphiteClients, SqlToGraphiteConfigClientsClient client)
        //{
        //    if (knownGraphiteClients.IsKnown(client.name))
        //    {
        //        this.clientList.Add(client.name, client.port);
        //    }
        //    else
        //    {
        //        Errors.Add(string.Format("{0} {1}", UnknownClient, client.name));
        //    }
        //}
        public GraphiteClients GetClientList()
        {
            return this.clientList;
        }

        public void AddClient(IClient client)
        {
            this.masterConfig.Clients.Add(client);
        }

        public void AddHost(string name, List<Role> roles)
        {
            var host = new Host();
            host.Name = name;
            host.Roles = roles;
            //var i = 0;
            //foreach (var role in roles)
            //{
            //    host.role[i] = new SqlToGraphiteConfigHostsHostRole();
            //    host.role[i].name = role;
            //    i++;
            //}
            this.masterConfig.Hosts.Add(host);
        }

        public void AddWorkItem(WorkItems workItem)
        {
            this.masterConfig.Templates[0].WorkItems.Add(workItem);
            // throw new ApplicationException("this is not right at all needs to be a proper list thing");
        }

        public void AddJob(IJob job)
        {
            this.masterConfig.Jobs.Add(job);
        }

        public void AddTask(TaskDetails taskProperties)
        {
            //findRole. 
            //var tasks = new List<ConfigSpike.Config.Task>();
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
                //var tasks = new List<Task>(t.Tasks);
                //tasks.Add();
                added = true;
                //t.Task = tasks.ToArray();
            }

            return added;
        }

        private TaskSet CreateTaskSetItem(TaskDetails taskProperties)
        {
            var tasks = new List<ConfigSpike.Config.Task> { CreateTask(taskProperties) };
            var taskSetItem = new TaskSet { Frequency = taskProperties.Frequency, Tasks = tasks };
            return taskSetItem;
        }

        private static ConfigSpike.Config.Task CreateTask(TaskDetails taskProperties)
        {
            return new ConfigSpike.Config.Task { JobName = taskProperties.JobName };
        }

        public List<IJob> GetJobs()
        {
            return this.masterConfig.Jobs;
        }
    }
}