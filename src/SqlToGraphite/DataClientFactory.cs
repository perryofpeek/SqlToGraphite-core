using log4net;

namespace SqlToGraphite
{
    public class DataClientFactory : IDataClientFactory
    {
        private readonly ILog log;

        public DataClientFactory(ILog log)
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

            if (type == "oracle")
            {
                return new OracleClient(log, taskParams);
            }

            if (type == "sqlserver")
            {
                return new SqlServerClient(log, taskParams);    
            }

            throw new UnknownDataClientException(type);
        }
    }
}