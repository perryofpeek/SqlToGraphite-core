using System.Collections.Generic;

using log4net;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.SqlServer
{
    public class SqlServer1 : PluginBase
    {
        public SqlServer1(ILog log, Job job) : base(log, job)
        {
            this.WireUpProperties(job, this);
        }

        public SqlServer1() : base()
        {
        }

        public string ConnectionString { get; set; }

        public string Sql { get; set; }

        public override IList<IResult> Get()
        {
            return new List<IResult>();
        }
    }
}