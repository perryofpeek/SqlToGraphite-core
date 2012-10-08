using System;
using System.Collections.Generic;

using SqlToGraphiteInterfaces;

public abstract class Job : ISqlClient
{
    public abstract string Name { get; set; }

    public abstract string ClientName { get; set; }
     
    public abstract string Type { get; set; }

    public abstract IList<IResult> Get();
}