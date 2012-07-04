using System;

using log4net;
using SqlToGraphiteInterfaces;

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
                Type t = Type.GetType("SqlToGraphite.Plugin.SqlServer.SqlServerClient, SqlToGraphite.Plugin.SqlServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null"); 
                var obj = Activator.CreateInstance(t, new object[] { log, taskParams });
                return (ISqlClient) obj;
                //return new SqlServerClient(log, taskParams);    
            }

            throw new UnknownDataClientException(type);
        }
    }
}