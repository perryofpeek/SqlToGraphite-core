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
            statsdClient.Send(new Result(12, "name", System.DateTime.Now, "path"));
        }
    }
}