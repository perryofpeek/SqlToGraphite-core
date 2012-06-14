using log4net;

namespace SqlToGraphite
{
    public class ClientFactory : IClientFactory
    {
        private readonly ILog log;

        public ClientFactory(ILog log)
        {
            this.log = log;
        }

        public ISqlClient Create(TaskParams taskParams)
        {
            var type = taskParams.Type.ToLower();
            if (type == "wmi")
            {
                return new WmiClient(log, taskParams);
            }

            return new SqlServerClient(log, taskParams);
        }
    }
}