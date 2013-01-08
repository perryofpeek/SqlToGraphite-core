using System;

using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    using System.Threading;

    public class RunableRunTask : IRunTask
    {
        private readonly Job job;

        private readonly IDataClientFactory dataClientFactory;

        private readonly IGraphiteClientFactory graphiteClientFactory;

        private readonly ILog log;

        private readonly Client client;

        public RunableRunTask(Job job, IDataClientFactory dataClientFactory, IGraphiteClientFactory graphiteClientFactory, ILog log, Client client)
        {
            this.job = job;
            this.dataClientFactory = dataClientFactory;
            this.graphiteClientFactory = graphiteClientFactory;
            this.log = log;
            this.client = client;
        }

        public void Process()
        {
            try
            {
                var dataClient = this.dataClientFactory.Create(this.job);
                var graphiteClient = this.graphiteClientFactory.Create(this.client);
                var results = dataClient.Get();
                foreach (var result in results)
                {
                    this.SendResult(graphiteClient, result);
                    SleepToPreventNetworkFlooding();
                }
            }
            catch (Exception ex)
            {
                // Catch all errors so we keep going. 
                log.Error(ex.Message);
                log.Error(ex);
            }
        }

        private static void SleepToPreventNetworkFlooding()
        {
            Thread.Sleep(1);
        }

        private void SendResult(IStatsClient graphiteClient, IResult result)
        {
            this.log.Debug(string.Format("{0} [{1}] @ {2} ({3}) {4}", result.FullPath, result.Value, result.TimeStamp, result.Name, graphiteClient.GetType().Name));
            try
            {
                graphiteClient.Send(result);
            }
            catch (Exception ex)
            {
                this.log.Error(ex.Message);
                this.log.Error(ex);
            }
        }
    }
}