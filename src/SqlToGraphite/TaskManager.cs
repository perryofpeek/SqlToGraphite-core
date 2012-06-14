using System.Collections.Generic;
using System.Linq;
using log4net;
using SqlToGraphite.Conf;

namespace SqlToGraphite
{
    public class TaskManager
    {
        public ITaskBag TaskBag { get; set; }

        private readonly ILog log;

        private readonly IConfigController configController;

        private readonly string path;

        private readonly IStop stop;

        private readonly ISleep sleep;

        private readonly int configurationReReadTime;

        public IList<ITaskSet> TaskSets { get; set; }

        public TaskManager(ILog log, IConfigController configController, string path, IStop stop, ISleep sleep, int configurationReReadTime)
        {
            this.log = log;
            this.configController = configController;
            this.path = path;
            this.stop = stop;
            this.sleep = sleep;
            this.configurationReReadTime = configurationReReadTime;
            TaskBag = new TaskBag(new List<IThread>());
        }

        public void Start()
        {
            while (!stop.ShouldStop())
            {
                var newTaskBag = configController.GetTaskBag(path);
                this.IfConfigurationHasBeenReloadedStopThreads();
                TaskBag = newTaskBag;
                this.StartTasks();
                sleep.Sleep(this.configurationReReadTime);
            }
        }

        private void StartTasks()
        {
            this.log.Info("Starting");
            this.TaskBag.Start();
        }

        private void IfConfigurationHasBeenReloadedStopThreads()
        {
            if (this.configController.IsNewConfig())
            {
                this.log.Debug(string.Format("New Configuration detected stopping threads"));
                this.TaskBag.Stop();
            }

            this.TaskBag = null;
        }

        public void Stop()
        {
            log.Info("Stopping");
            TaskBag.Stop();
            TaskBag = null;
        }
    }
}