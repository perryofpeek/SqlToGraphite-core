using System;
using System.Collections.Generic;

using ConfigSpike.Config;

using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class ConfigMapper : IConfigMapper
    {
        private readonly string hostname;

        private readonly IStop stop;

        private readonly IDataClientFactory dataClientFactory;

        private readonly IGraphiteClientFactory graphiteClientFactory;

        private static ILog log;

        public ConfigMapper(string hostname, IStop stop, IDataClientFactory dataClientFactory, IGraphiteClientFactory graphiteClientFactory, ILog log)
        {
            this.hostname = hostname;
            this.stop = stop;
            this.dataClientFactory = dataClientFactory;
            this.graphiteClientFactory = graphiteClientFactory;
            ConfigMapper.log = log;
        }

        public IList<ITaskSet> Map(List<ConfigSpike.Config.TaskSet> list, GraphiteClients clients)
        {
            var taskSets = new List<ITaskSet>();
            foreach (ConfigSpike.Config.TaskSet taskSet in list)
            {
                var fequency = taskSet.Frequency;

                var tasks = this.MapTasks(hostname, taskSet, clients);
                taskSets.Add(new TaskSetWithProcess(tasks, stop, fequency));
                throw new ApplicationException("this is not right");
            }

            return taskSets;
        }

        private IList<ITask> MapTasks(string hostName, ConfigSpike.Config.TaskSet workItem, GraphiteClients clients)
        {
            var tasks = new List<ITask>();
            throw new ApplicationException("this needs to get the job");
            foreach (var item in workItem.Tasks)
            {
                var c = clients.Get(item.JobName);
                //    tasks.Add(CreateTask(hostName, c, item));
            }

            return tasks;
        }

        private Task CreateTask(string hostName, GraphiteClient client, ConfigSpike.Config.Task item)
        {
            var graphiteParams = new GraphiteParams(hostName, client.Port);
            //var taskParams = new TaskParams(item.path, item.sql, item.connectionstring, item.type, item.name, item.client);
            //var task = new Task(taskParams, this.dataClientFactory, this.graphiteClientFactory, graphiteParams, log);
            throw new ApplicationException("this needs to be fixed to work.");
            return null;
            // task;
        }
    }
}