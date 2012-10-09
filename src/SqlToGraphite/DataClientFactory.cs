using System;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite
{
    public class DataClientFactory : IDataClientFactory
    {
        private readonly ILog log;

        private readonly IAssemblyResolver assemblyResolver;

        public DataClientFactory(ILog log, IAssemblyResolver assemblyResolver)
        {
            this.log = log;
            this.assemblyResolver = assemblyResolver;            
        }
        
        public ISqlClient Create(Job job)
        {                        
            var t1 = assemblyResolver.ResolveType(job);

            if (t1 == null)
            {
                throw new UnknownDataClientException(job.Type);
            }
         
            var obj = Activator.CreateInstance(t1, new object[] { log, job });
            return (ISqlClient)obj;
        }        
    }
}