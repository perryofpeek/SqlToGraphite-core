using System.Collections.Generic;
using log4net;
using SqlToGraphite.Conf;
using SqlToGraphite.Config;
using SqlToGraphiteInterfaces;

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

        public IList<IRunTaskSet> Map(List<TaskSet> list)
        {
            var taskSets = new List<IRunTaskSet>();
            foreach (TaskSet taskSet in list)
            {
                var tasks = this.MapTasks(hostname, taskSet);
                taskSets.Add(new RunTaskSetWithProcess(tasks, stop, taskSet.Frequency));
            }

            return taskSets;
        }

        private IList<IRunTask> MapTasks(string hostName, TaskSet workItem)
        {
            var tasks = new List<IRunTask>();
            foreach (var item in workItem.Tasks)
            {
                var job = repository.GetJob(item.JobName);
                var client = repository.GetClient(job.ClientName);
                tasks.Add(this.CreateTask(client, job));
            }

            return tasks;
        }

        private RunableRunTask CreateTask(Client client, Job job)
        {
            return new RunableRunTask(job, this.dataClientFactory, this.graphiteClientFactory, log, client);
        }
    }
}