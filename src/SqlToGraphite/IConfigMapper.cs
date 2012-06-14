using System.Collections.Generic;

namespace SqlToGraphite
{
    public interface IConfigMapper
    {
        IList<ITaskSet> Map(List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet> list, GraphiteClients clients);
    }
}