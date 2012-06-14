using System;
using System.Collections.Generic;
using System.Threading;

namespace SqlToGraphite
{
    public class TaskSet : ITaskSet
    {
        private readonly IStop stop;

        public TaskSet(IList<ITask> tasks, IStop stop, int frequency)
        {
            this.stop = stop;
            this.Tasks = tasks;
            Frequency = frequency;
        }

        public IList<ITask> Tasks { get; private set; }

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