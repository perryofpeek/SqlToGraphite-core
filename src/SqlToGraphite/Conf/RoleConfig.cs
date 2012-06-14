using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlToGraphite.Conf
{
    public class RoleConfig
    {
        private List<SqlToGraphiteConfigHostsHost> sqlToGraphiteConfigHosts;

        public RoleConfig(List<SqlToGraphiteConfigHostsHost> sqlToGraphiteConfigHosts)
        {
            this.sqlToGraphiteConfigHosts = sqlToGraphiteConfigHosts;
        }

        public List<string> GetRoleList()
        {
            var rtn = new List<string>();
            foreach (var h in sqlToGraphiteConfigHosts)
            {
                if (h.name.ToLower() == "default")
                {
                    rtn.AddRange(h.role.Select(role => role.name));
                }
                else if (h.name.ToLower() == Environment.MachineName.ToLower())
                {
                    rtn.AddRange(h.role.Select(role => role.name));
                }
            }

            return rtn;
        }
    }
}