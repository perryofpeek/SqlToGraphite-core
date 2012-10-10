using System;

using NUnit.Framework;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GraphiteClient
    {
        [Test, Explicit]
        public void Should_call_graphite_using_TCP()
        {
            var result = new Result(1, "name", new DateTime(2012, 11, 10, 9, 8, 7), "path");
            var graphiteClient = new LocalGraphiteTcpClient("metrics.london.ttldev.local", 2003);
            graphiteClient.Send(result);
        }
    }
}