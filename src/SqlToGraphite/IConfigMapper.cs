using System.Collections.Generic;

using ConfigSpike.Config;

namespace SqlToGraphite
{
    public interface IConfigMapper
    {
        IList<IRunTaskSet> Map(List<TaskSet> list, GraphiteClients clients);
    }
}