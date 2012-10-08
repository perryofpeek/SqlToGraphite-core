using System;
using System.Collections.Generic;
using log4net;
using SqlToGraphiteInterfaces;

public class SqlServer : PluginBase
{
    public SqlServer(ILog log, Job taskParams) : base(log, taskParams)
    {
        this.WireUpProperties(taskParams, this);
    }

    public SqlServer() : base()
    {
    }

    public string ConnectionString { get; set; }

    public string Sql { get; set; }

    public override IList<IResult> Get()
    {
        return new List<IResult>();
    }
}