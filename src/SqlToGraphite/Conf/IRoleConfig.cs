namespace SqlToGraphite.Conf
{
    using System.Collections.Generic;

    public interface IRoleConfig
    {
        List<string> GetRoleListToRunOnThisMachine();
    }
}