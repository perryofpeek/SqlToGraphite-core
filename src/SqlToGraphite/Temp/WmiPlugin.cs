using System;
using System.Collections.Generic;
using log4net;
using SqlToGraphiteInterfaces;

public class WmiPlugin : PluginBase
{
    public WmiPlugin(ILog log, Job taskParams)
        : base(log, taskParams)
    {
        this.WireUpProperties(taskParams, this);
    }

    public WmiPlugin()
    {
    }

    public override string Name { get; set; }

    public override string ClientName { get; set; }

    public string Sql { get; set; }

    public string Hostname { get; set; }

    public override string Type { get; set; }

    public override IList<IResult> Get()
    {
        throw new NotImplementedException();
    }
}