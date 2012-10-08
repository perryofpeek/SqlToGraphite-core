using log4net;

namespace SqlToGraphite
{
    public class Controller : IController
    {
        private readonly HostConfiguration config;

        private readonly IRunTaskSet runTaskSet;

        private readonly ISleep sleep;

        private readonly IStop stop;

        private readonly ILog log;

        public Controller(IRunTaskSet runTaskSet, ISleep sleep, IStop stop, ILog log)
        {
            //this.config = config;
            this.runTaskSet = runTaskSet;
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
                this.runTaskSet.Process();
                log.Debug(string.Format("sleep {0}", this.runTaskSet.Frequency));
                sleep.SleepSeconds(this.runTaskSet.Frequency);
            }
        }

        public void Stop()
        {
            log.Debug("Stop");
            stop.SetStop(true);
        }
    }
}