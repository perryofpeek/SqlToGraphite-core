using System;
using System.Collections.Generic;

using ConfigSpike;

using log4net;
using SqlToGraphite.Conf;

namespace SqlToGraphite
{
    public class ConfigMapper : IConfigMapper
    {
        private readonly string hostname;

        private readonly IStop stop;

        private readonly IDataClientFactory dataClientFactory;

        private readonly IGraphiteClientFactory graphiteClientFactory;

        private readonly IConfigRepository repository;

        private static ILog log;

        public ConfigMapper(string hostname, IStop stop, IDataClientFactory dataClientFactory, IGraphiteClientFactory graphiteClientFactory, ILog log, IConfigRepository repository)
        {
            this.hostname = hostname;
            this.stop = stop;
            this.dataClientFactory = dataClientFactory;
            this.graphiteClientFactory = graphiteClientFactory;
            this.repository = repository;
            ConfigMapper.log = log;
        }

        public IList<IRunTaskSet> Map(List<ConfigSpike.Config.TaskSet> list, GraphiteClients clients)
        {
            var taskSets = new List<IRunTaskSet>();
            foreach (ConfigSpike.Config.TaskSet taskSet in list)
            {
                var fequency = taskSet.Frequency;

                var tasks = this.MapTasks(hostname, taskSet, clients);
                taskSets.Add(new RunTaskSetWithProcess(tasks, stop, fequency));
                //throw new ApplicationException("this is not right");
            }

            return taskSets;
        }

        private IList<IRunTask> MapTasks(string hostName, ConfigSpike.Config.TaskSet workItem, GraphiteClients clients)
        {
            var tasks = new List<IRunTask>();           
            foreach (var item in workItem.Tasks)
            {
                var job = repository.GetJob(item.JobName);
                var client = repository.GetClient(job.ClientName);
                //var c = clients.Get(item.JobName);
                tasks.Add(CreateTask(hostName, client, item, job));
            }

            return tasks;            
        }

        private RunableRunTask CreateTask(string hostName, Client client, ConfigSpike.Config.Task item, Job job)
        {            
            var graphiteParams = new GraphiteParams(hostName, client.Port);
            //var taskParams = new TaskParams(item.path, item.sql, item.connectionstring, item.type, item.name, item.client);
            var task = new RunableRunTask(job, this.dataClientFactory, this.graphiteClientFactory, graphiteParams, log, client);
            //throw new ApplicationException("this needs to be fixed to work.");
            return task;
            // task;
        }
    }
}