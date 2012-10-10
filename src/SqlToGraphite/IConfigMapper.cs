using System.Collections.Generic;

using SqlToGraphite.Config;

namespace SqlToGraphite
{
    public interface IConfigMapper
    {
        IList<IRunTaskSet> Map(List<TaskSet> list);
    }
}