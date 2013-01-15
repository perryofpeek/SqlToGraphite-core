using System.Collections.Generic;

using SqlToGraphite.Config;

namespace SqlToGraphite.Conf
{
    public interface ITaskSetBuilder
    {
        List<TaskSet> BuildTaskSet(List<Template> templates, IRoleConfig roleList);
    }
}