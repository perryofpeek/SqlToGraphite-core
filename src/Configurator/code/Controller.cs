using System;
using System.Collections;
using System.Collections.Generic;

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
            Job j = (Job)client;
            repository.AddJob(j);
        }

        public void Save(string fileName)
        {
            repository.Save(fileName);
        }
    }
}