using System;
using System.Collections.Generic;
using System.Threading;

namespace SqlToGraphite
{
    public class RunTaskSetWithProcess : IRunTaskSet
    {
        private readonly IStop stop;

        public RunTaskSetWithProcess(IList<IRunTask> tasks, IStop stop, int frequency)
        {
            this.stop = stop;
            this.Tasks = tasks;
            Frequency = frequency;
        }

        public IList<IRunTask> Tasks { get; private set; }

        public int Frequency { get; set; }

        public void Process()
        {
            while (!stop.ShouldStop())
            {
                RunAllTasks();
                SleepTillNextSchedualedTime();
            }
        }

        private void SleepTillNextSchedualedTime()
        {
            var ms = Convert.ToInt32(this.Frequency * 1000);
            Thread.Sleep(ms);
        }

        private void RunAllTasks()
        {
            foreach (var task in this.Tasks)
            {
                task.Process();
            }
        }
    }
}