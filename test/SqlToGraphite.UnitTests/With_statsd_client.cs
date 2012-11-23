using NUnit.Framework;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_statsd_client
    {
        [Test]
        public void Should_send_message()
        {
            var statsdClient = new StatsdClient("localhost", 1234);
            var r = new Result("name", System.DateTime.Now, "path");
            r.SetValue(12);
            statsdClient.Send(r);
        }
    }
}