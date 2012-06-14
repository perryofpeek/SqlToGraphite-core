using System;
using System.Threading;

namespace SqlToGraphite
{
    public class Sleeper : ISleep
    {
        private int sleepPeriod = 100;

        void ISleep.Sleep(int lengthInMinutes)
        {
            var wakeUpTime = DateTime.Now.Add(new TimeSpan(0, 0, lengthInMinutes, 0));
            while (DateTime.Now <= wakeUpTime)
            {
                Thread.Sleep(sleepPeriod);
            }
        }

        public void SleepSeconds(int lengthInSeconds)
        {
            var wakeUpTime = DateTime.Now.Add(new TimeSpan(0, 0, lengthInSeconds, 0));
            while (DateTime.Now <= wakeUpTime)
            {
                Thread.Sleep(sleepPeriod);
            }
        }
    }
}