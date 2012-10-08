using System.Collections.Generic;

namespace SqlToGraphite
{
    public interface IRunTaskSet
    {
        IList<IRunTask> Tasks { get; }

        int Frequency { get; set; }

        void Process();
    }
}