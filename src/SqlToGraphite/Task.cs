using System;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class Task : ITask
    {
        private readonly TaskParams taskParams;

        private readonly IDataClientFactory dataClientFactory;

        private readonly IGraphiteClientFactory graphiteClientFactory;

        private readonly GraphiteParams graphiteParams;

        private readonly ILog log;

        public Task(TaskParams taskParams, IDataClientFactory dataClientFactory, IGraphiteClientFactory graphiteClientFactory, GraphiteParams graphiteParams, ILog log)
        {
            this.taskParams = taskParams;
            this.dataClientFactory = dataClientFactory;
            this.graphiteClientFactory = graphiteClientFactory;
            this.graphiteParams = graphiteParams;
            this.log = log;
        }

        public void Process()
        {
            try
            {
                this.LogTaskParams();
                var dataClient = this.dataClientFactory.Create(taskParams);
                var graphiteClient = this.graphiteClientFactory.Create(graphiteParams, taskParams);
                var results = dataClient.Get();
                foreach (var result in results)
                {
                    graphiteClient.Send(result);
                }
            }
            catch (Exception ex)
            {
                // Catch all errors so we keep going. 
                log.Error(ex);
            }
        }

        private void LogTaskParams()
        {
            this.log.Debug(
                string.Format(
                    "CLient[{0}] ConStr[{1}] Path[{2}] Sql[{3}] Type[{4}]",
                    this.taskParams.Client,
                    this.taskParams.ConnectionString,
                    this.taskParams.Path,
                    this.taskParams.Sql,
                    this.taskParams.Type));
        }
    }
}