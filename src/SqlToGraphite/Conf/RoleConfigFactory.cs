namespace SqlToGraphite.Conf
{
    public class RoleConfigFactory : IRoleConfigFactory
    {
        public IRoleConfig Create(IConfigRepository configRepository, IEnvironment environment)
        {
            return new RoleConfig(configRepository.GetHosts(), environment);
        }
    }
}

//Environment.MachineName