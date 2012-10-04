using System;

public class SqlServer : Job
{
    public SqlServer()
    {
        this.Type = this.GetType().AssemblyQualifiedName;
        //this.Client = "graphiteTcp";
        //this.HasTail = false;
        //this.Name = "GetCpuLoadAverage";
    }

    public override string Name
    {
        get;
        set;
    }

    public override string ClientName
    {
        get;
        set;
    }

    public override string Type { get; set; }    
}