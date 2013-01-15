namespace SqlToGraphite.Conf
{
    public class Environment : IEnvironment
    {
        public string GetMachineName()
        {
            return System.Environment.MachineName;
        }
    }
}