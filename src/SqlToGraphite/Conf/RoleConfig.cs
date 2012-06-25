using System;
using System.Collections.Generic;
using System.Linq;

namespace SqlToGraphite.Conf
{
    public class RoleConfig
    {
        private List<SqlToGraphiteConfigHostsHost> sqlToGraphiteConfigHosts;

        private readonly string hostname;

        public RoleConfig(List<SqlToGraphiteConfigHostsHost> sqlToGraphiteConfigHosts, string hostname)
        {
            this.sqlToGraphiteConfigHosts = sqlToGraphiteConfigHosts;
            this.hostname = hostname;
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
                else if (System.Text.RegularExpressions.Regex.IsMatch(hostname, h.name, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    rtn.AddRange(h.role.Select(role => role.name));
                }
            }

            return rtn;
        }
    }
}