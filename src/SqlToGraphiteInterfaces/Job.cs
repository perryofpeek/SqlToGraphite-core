using System;

public abstract class Job : IJob
{
    public abstract string Name { get; set; }

    public abstract string ClientName { get; set; }
     
    public abstract string Type { get; set; }
}