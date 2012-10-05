using System.Collections.Generic;

using ConfigSpike.Config;

namespace SqlToGraphite
{
    public interface IConfigMapper
    {
        IList<ITaskSet> Map(List<TaskSet> list, GraphiteClients clients);
    }
}