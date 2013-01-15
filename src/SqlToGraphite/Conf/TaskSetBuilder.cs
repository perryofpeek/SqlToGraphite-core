using System.Collections.Generic;
using SqlToGraphite.Config;

//TODO Write some tests for this class
namespace SqlToGraphite.Conf
{
    public class TaskSetBuilder : ITaskSetBuilder
    {
        public List<TaskSet> BuildTaskSet(List<Template> templates, IRoleConfig roleConfig)
        {
            var roleList = roleConfig.GetRoleListToRunOnThisMachine();
            var setList = new List<TaskSet>();
            foreach (var template in templates)
            {
                foreach (var wi in template.WorkItems)
                {
                    if (roleList.Contains(wi.RoleName))
                    {
                        setList.AddRange(wi.TaskSet.ToArray());
                    }
                }
            }

            return setList;
        }
    }
}