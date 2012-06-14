using System;
using System.Threading;

using log4net;

namespace SqlToGraphite.host
{
    public class Application
    {
        private readonly TaskManager taskManager;

        private static ILog log;

        private Thread workerThread;

        public Application(TaskManager taskManager, ILog log)
        {
            this.taskManager = taskManager;            
            Application.log = log;
        }

        public void Start()
        {
            try
            {
                workerThread = new Thread(taskManager.Start);
                workerThread.Start();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(ex.Message);
            }
        }

        public void Stop()
        {
            try
            {
                workerThread.Abort();
            }
            catch (Exception ex)
            {
                log.Error(ex);
                Console.WriteLine(ex.Message);
            }
        }
    }
}