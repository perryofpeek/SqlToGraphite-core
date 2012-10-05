using System;
using System.Collections.Generic;

using ConfigSpike.Config;

namespace SqlToGraphite.Conf
{
    public class Templates1
    {
        private readonly List<WorkItems> sqlToGraphiteConfigTemplates;

        public Templates1(List<WorkItems> sqlToGraphiteConfigTemplates)
        {
            this.sqlToGraphiteConfigTemplates = sqlToGraphiteConfigTemplates;
        }

        public List<ConfigSpike.Config.TaskSet> GetTaskSetList(List<string> roles)
        {
            var rtn = new List<ConfigSpike.Config.TaskSet>();
            foreach (var workItem in sqlToGraphiteConfigTemplates)
            {
                if (roles.Contains(workItem.RoleName))
                {
                    rtn.AddRange(workItem.TaskSet.ToArray());
                }
            }
//            throw new ApplicationException("this is not right");
            return rtn;
        }
    }
}