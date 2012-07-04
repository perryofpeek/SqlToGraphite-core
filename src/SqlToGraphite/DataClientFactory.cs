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
            var t = Type.GetType(taskParams.Type);
            if (t == null)
            {
                throw new UnknownDataClientException(taskParams.Type);
            }

            var obj = Activator.CreateInstance(t, new object[] { log, taskParams });
            return (ISqlClient)obj;
        }
    }
}