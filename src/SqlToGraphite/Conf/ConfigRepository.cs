using System;
using System.Collections.Generic;
using System.IO;
using System.Xml;
using log4net;
using SqlToGraphite.Clients;

namespace SqlToGraphite.Conf
{
    public class ConfigRepository : IConfigRepository
    {
        private readonly IConfigReader configReader;

        private readonly KnownGraphiteClients knownGraphiteClients;

        private readonly ICache cache;

        private readonly ISleep sleep;

        private readonly ILog log;

        private readonly int errorReadingConfigSleepTime;

        private List<SqlToGraphiteConfigClientsClient> clients;
        private List<SqlToGraphiteConfigTemplatesWorkItems> templates;
        private List<SqlToGraphiteConfigHostsHost> hosts;
        private List<string> errors;
        private GraphiteClients clientList;
        public const string FailedToLoadAnyConfiguration = "Failed to load any configuration";
        public const string FailedToFindClients = "Failed to find any clients defined";
        public const string FailedToFindHosts = "Failed to find any hosts defined";
        public const string FailedToFindTemplates = "Failed to find any templates defined";
        public const string UnknownClient = "unknown client defined";

        public ConfigRepository(IConfigReader configReader, KnownGraphiteClients knownGraphiteClients, ICache cache, ISleep sleep, ILog log, int errorReadingConfigSleepTime)
        {
            this.configReader = configReader;
            this.knownGraphiteClients = knownGraphiteClients;
            this.cache = cache;
            this.sleep = sleep;
            this.log = log;
            this.errorReadingConfigSleepTime = errorReadingConfigSleepTime;
        }

        public void Load()
        {
            if (cache.HasExpired())
            {
                log.Debug("Cache has expired");
                var graphiteConfig = new SqlToGraphiteConfig();
                while (graphiteConfig.Items == null)
                {
                    try
                    {
                        graphiteConfig = GetConfig(configReader);
                        if (graphiteConfig.Items == null)
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

                Init();
                this.ParseConfigItems(graphiteConfig);
                cache.ResetCache();
            }
        }

        private void RestForAwhile()
        {
            log.Debug(string.Format("sleeping for {0} mins", errorReadingConfigSleepTime));
            this.sleep.Sleep(errorReadingConfigSleepTime);
        }

        private void ParseConfigItems(SqlToGraphiteConfig graphiteConfig)
        {
            foreach (var setting in graphiteConfig.Items)
            {
                this.AddTypes(setting);
            }
        }

        private void AddTypes(object setting)
        {
            this.AddClients(setting);
            this.AddTemplates(setting);
            this.AddHosts(setting);
        }

        private void AddHosts(object setting)
        {
            if (setting.GetType() == typeof(SqlToGraphiteConfigHosts))
            {
                var x = (SqlToGraphiteConfigHosts)setting;
                this.hosts.AddRange(x.host);
            }
        }

        private void AddTemplates(object setting)
        {
            if (setting.GetType() == typeof(SqlToGraphiteConfigTemplates))
            {
                var x = (SqlToGraphiteConfigTemplates)setting;
                this.templates.AddRange(x.WorkItems);
            }
        }

        private void AddClients(object setting)
        {
            if (setting.GetType() == typeof(SqlToGraphiteConfigClients))
            {
                var x = (SqlToGraphiteConfigClients)setting;
                this.clients.AddRange(x.Client);
                this.AddAllClients();
            }
        }

        private static SqlToGraphiteConfig GetConfig(IConfigReader configReader)
        {
            var ser = new System.Xml.Serialization.XmlSerializer(typeof(SqlToGraphiteConfig));
            var rdr = XmlReader.Create(new StringReader(configReader.GetXml()));
            var o = ser.Deserialize(rdr);
            var c = (SqlToGraphiteConfig)o;
            return c;
        }

        private void Init()
        {
            this.clients = new List<SqlToGraphiteConfigClientsClient>();
            this.templates = new List<SqlToGraphiteConfigTemplatesWorkItems>();
            this.hosts = new List<SqlToGraphiteConfigHostsHost>();
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

        public List<SqlToGraphiteConfigClientsClient> GetClients()
        {
            return clients;
        }

        public List<SqlToGraphiteConfigTemplatesWorkItems> GetTemplates()
        {
            return templates;
        }

        public List<SqlToGraphiteConfigHostsHost> GetHosts()
        {
            return hosts;
        }

        public bool Validate()
        {
            var rtn = true;
            if (templates.Count == 0)
            {
                errors.Add(FailedToFindTemplates);
                rtn = false;
            }

            if (hosts.Count == 0)
            {
                errors.Add(FailedToFindHosts);
                rtn = false;
            }

            if (clients.Count == 0)
            {
                errors.Add(FailedToFindClients);
                rtn = false;
            }

            return rtn;
        }

        private void AddAllClients()
        {
            foreach (var client in this.GetClients())
            {
                this.AddKnownClient(knownGraphiteClients, client);
            }
        }

        private void AddKnownClient(KnownGraphiteClients knownGraphiteClients, SqlToGraphiteConfigClientsClient client)
        {
            if (knownGraphiteClients.IsKnown(client.name))
            {
                this.clientList.Add(client.name, client.port);
            }
            else
            {
                Errors.Add(string.Format("{0} {1}", UnknownClient, client.name));
            }
        }

        public GraphiteClients GetClientList()
        {
            return this.clientList;
        }
    }
}