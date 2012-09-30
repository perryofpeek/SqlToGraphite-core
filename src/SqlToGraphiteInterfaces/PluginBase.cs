using System;
using System.Collections.Generic;

using SqlToGraphite;

using log4net;

namespace SqlToGraphiteInterfaces
{
    public abstract class PluginBase : ISqlClient
    {
        protected ILog Log { get; set; }

        protected ITaskParams TaskParams { get; set; }        

        protected PluginBase(ILog log, ITaskParams taskParams)
        {
            this.Log = log;
            this.TaskParams = taskParams;           
        }

        protected PluginBase()
        {
            throw new NotImplementedException();
        }

        public abstract IList<IResult> Get();
    }
}