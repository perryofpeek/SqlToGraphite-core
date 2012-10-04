using System;

public class WmiPlugin : Job
{
    public WmiPlugin()
    {
        this.ClientName = "graphiteUdp";        
        this.Sql = "Black Lab";
        this.Name = "GetTracsDeliveryTypes";
        this.Type = this.GetType().AssemblyQualifiedName;
        this.Hostname = "SomeConnection";
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

    public string Sql
    {
        get;
        set;
    }

    public string Hostname
    {
        get;
        set;
    }

    public override string Type 
    {
        get;
        set;
    }    
}