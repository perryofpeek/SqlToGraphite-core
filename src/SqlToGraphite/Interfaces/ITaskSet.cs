using System.Collections.Generic;

namespace SqlToGraphite
{
    public interface ITaskSet
    {
        IList<ITask> Tasks { get; }

        int Frequency { get; set; }

        void Process();
    }
}