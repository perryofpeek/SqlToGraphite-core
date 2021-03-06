using System.Collections.Generic;
using System.Linq;

using SqlToGraphite.Config;

namespace SqlToGraphite.Conf
{
    public class RoleConfig : IRoleConfig
    {
        private List<Host> sqlToGraphiteConfigHosts;

        private readonly string hostname;

        public RoleConfig(List<Host> sqlToGraphiteConfigHosts, IEnvironment environment)
        {
            this.sqlToGraphiteConfigHosts = sqlToGraphiteConfigHosts;
            this.hostname = environment.GetMachineName();
        }

        public List<string> GetRoleListToRunOnThisMachine()
        {
            var rtn = new List<string>();
            foreach (var h in sqlToGraphiteConfigHosts)
            {
                if (h.Name.ToLower() == "default")
                {
                    rtn.AddRange(h.Roles.Select(role => role.Name));
                }
                else if (System.Text.RegularExpressions.Regex.IsMatch(hostname, h.Name, System.Text.RegularExpressions.RegexOptions.IgnoreCase))
                {
                    rtn.AddRange(h.Roles.Select(role => role.Name));
                }
            }

            return rtn;
        }
    }
}