using System;

using ConfigSpike;

using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class RunableRunTask : IRunTask
    {
        private readonly Job job;

        private readonly IDataClientFactory dataClientFactory;

        private readonly IGraphiteClientFactory graphiteClientFactory;

        private readonly GraphiteParams graphiteParams;

        private readonly ILog log;

        private readonly Client client;

        public RunableRunTask(Job job, IDataClientFactory dataClientFactory, IGraphiteClientFactory graphiteClientFactory, GraphiteParams graphiteParams, ILog log, Client client)
        {
            this.job = job;
            this.dataClientFactory = dataClientFactory;
            this.graphiteClientFactory = graphiteClientFactory;
            this.graphiteParams = graphiteParams;
            this.log = log;
            this.client = client;
        }

        public void Process()
        {
            try
            {
                //this.LogTaskParams();
                var dataClient = this.dataClientFactory.Create(this.job);
                var graphiteClient = this.graphiteClientFactory.Create(this.client);
                var results = dataClient.Get();
                foreach (var result in results)
                {
                    log.Debug(string.Format("{0} [{1}] @ {2} ({3}) {4}", result.FullPath, result.Value, result.TimeStamp, result.Name, graphiteClient.GetType().Name));
                    graphiteClient.Send(result);
                }
            }
            catch (Exception ex)
            {
                var s = ex.Message;
                // Catch all errors so we keep going. 
                log.Error(ex);
            }
        }

        //private void LogTaskParams()
        //{
        //    this.log.Debug(
        //        string.Format(
        //            "CLient[{0}] ConStr[{1}] Path[{2}] Sql[{3}] Type[{4}]",
        //            this.job.Client,
        //            this.job.ConnectionString,
        //            this.job.Path,
        //            this.job.Sql,
        //            this.job.Type));
        //}
    }
}