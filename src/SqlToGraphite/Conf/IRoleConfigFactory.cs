namespace SqlToGraphite.Conf
{
    public interface IRoleConfigFactory
    {
        IRoleConfig Create(IConfigRepository configRepository, IEnvironment environment);
    }
}