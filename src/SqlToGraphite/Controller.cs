using log4net;

namespace SqlToGraphite
{
    public class Controller : IController
    {
        private readonly HostConfiguration config;

        private readonly ITaskSet taskSet;

        private readonly ISleep sleep;

        private readonly IStop stop;

        private readonly ILog log;

        public Controller(ITaskSet taskSet, ISleep sleep, IStop stop, ILog log)
        {
            //this.config = config;
            this.taskSet = taskSet;
            this.sleep = sleep;
            this.stop = stop;
            this.log = log;
        }

        public Controller(HostConfiguration config, ILog log)
        {
            this.config = config;
            this.log = log;
        }

        public void Process()
        {
            log.Debug("Processing");
            while (!stop.ShouldStop())
            {                
                taskSet.Process();
                log.Debug(string.Format("sleep {0}", taskSet.Frequency));
                sleep.SleepSeconds(taskSet.Frequency);
            }
        }

        public void Stop()
        {
            log.Debug("Stop");
            stop.SetStop(true);
        }
    }
}