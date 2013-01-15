using System.Collections.Generic;

namespace SqlToGraphite
{
    public class TaskBag : ITaskBag
    {
        public IList<IThread> Threads { get; private set; }

        public TaskBag(IList<IThread> threads)
        {
            Threads = threads;                        
        }
       
        public void Start()
        {
            foreach (var thread in Threads)
            {                
                thread.Start();
            }
        }

        public void Stop()
        {
            foreach (var thread in Threads)
            {
                thread.Abort();
            }
        }
    }
}