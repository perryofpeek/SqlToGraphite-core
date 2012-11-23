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
            var result = new Result("name", new DateTime(2012, 11, 10, 9, 8, 7), "path");
            result.SetValue(1);
            var graphiteClient = new GraphiteTcpClient("metrics.london.ttldev.local", 2003);
            graphiteClient.Send(result);
        }
    }
}