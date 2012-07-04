using System.Collections.Generic;
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

        public IList<ITaskSet> Map(List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet> list, GraphiteClients clients)
        {
            var taskSets = new List<ITaskSet>();
            foreach (SqlToGraphiteConfigTemplatesWorkItemsTaskSet taskSet in list)
            {
                var fequency = int.Parse(taskSet.frequency);
                var tasks = this.MapTasks(hostname, taskSet, clients);
                taskSets.Add(new TaskSet(tasks, stop, fequency));
            }

            return taskSets;
        }

        private IList<ITask> MapTasks(string hostName, SqlToGraphiteConfigTemplatesWorkItemsTaskSet workItem, GraphiteClients clients)
        {
            var tasks = new List<ITask>();
            foreach (var item in workItem.Task)
            {
                var c = clients.Get(item.client);
                tasks.Add(CreateTask(hostName, c, item));
            }

            return tasks;
        }

        private Task CreateTask(string hostName, GraphiteClient client, SqlToGraphiteConfigTemplatesWorkItemsTaskSetTask item)
        {
            var graphiteParams = new GraphiteParams(hostName, client.Port);
            var taskParams = new TaskParams(item.path, item.sql, item.connectionstring, item.type, item.name, item.client);
            var task = new Task(taskParams, this.dataClientFactory, this.graphiteClientFactory, graphiteParams, log);
            return task;
        }        
    }
}