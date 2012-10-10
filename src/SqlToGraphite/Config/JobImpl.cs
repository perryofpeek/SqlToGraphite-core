using System;
using System.Collections.Generic;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Config
{
    public class JobImpl : Job
    {
        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public override IList<IResult> Get()
        {
            throw new NotImplementedException();
        }
    }
}