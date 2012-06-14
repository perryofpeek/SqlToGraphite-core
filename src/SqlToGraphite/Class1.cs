using System.Collections.Generic;

using log4net;

namespace SqlToGraphite
{
    public class SqlClientBase : ISqlClient
    {
        protected readonly ILog Log;

        protected readonly TaskParams TaskParams;

        public SqlClientBase(ILog log, TaskParams taskParams)
        {
            this.Log = log;
            this.TaskParams = taskParams;
        }

        public IList<IResult> Get()
        {
            throw new System.NotImplementedException();
        }
    }
}