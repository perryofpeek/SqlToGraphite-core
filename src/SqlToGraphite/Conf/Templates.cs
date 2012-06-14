using System.Collections.Generic;

namespace SqlToGraphite.Conf
{
    public class Templates
    {
        private readonly List<SqlToGraphiteConfigTemplatesWorkItems> sqlToGraphiteConfigTemplates;

        public Templates(List<SqlToGraphiteConfigTemplatesWorkItems> sqlToGraphiteConfigTemplates)
        {
            this.sqlToGraphiteConfigTemplates = sqlToGraphiteConfigTemplates;
        }

        public List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet> GetTaskSetList(List<string> roles)
        {
            var rtn = new List<SqlToGraphiteConfigTemplatesWorkItemsTaskSet>();
            foreach (var workItem in sqlToGraphiteConfigTemplates)
            {
                if (roles.Contains(workItem.Role))
                {
                    rtn.AddRange(workItem.TaskSet);
                }
            }

            return rtn;
        }
    }
}