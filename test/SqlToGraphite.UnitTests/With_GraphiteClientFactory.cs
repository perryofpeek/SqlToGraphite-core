using ConfigSpike;

using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_GraphiteClientFactory
    {
        private ILog log;

        private IGraphiteClientFactory clientFactory;

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            clientFactory = new GraphiteClientFactory(log);
        }

        [Test]
        public void Should_create_native_graphitetcp_client()
        {
            var client = new ConfigSpike.GraphiteTcpClient { Hostname = "hostname", Port = 8125 };
            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(GraphiteTcpClient)));
            Assert.That(o, Is.TypeOf<GraphiteTcpClient>());
        }

        [Test]
        public void Should_create_statsd_client()
        {
            var client = new ConfigSpike.GraphiteTcpClient { Hostname = "localhost", Port = 8125 };

            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(StatsdClient)));
            Assert.That(o, Is.TypeOf<StatsdClient>());
        }

        [Test]
        public void Should_create_statsd_client_uppercase()
        {
            //var graphiteParams = new GraphiteParams("localhost", 8125);
            //var taskParams = new TaskParams("path", "sql", "cs", "wmi", "name", "STATSduDp");
            var client = new ConfigSpike.GraphiteTcpClient { Hostname = "localhost", Port = 8125 };
            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(StatsdClient)));
            Assert.That(o, Is.TypeOf<StatsdClient>());
        }

        [Test]
        public void Should_throw_exception_if_unknown_type()
        {
            var client = new ConfigSpike.GraphiteTcpClient { Hostname = "localhost", Port = 8125 };
            var graphiteParams = new GraphiteParams("hostname", 8125);
            var taskParams = new TaskParams("path", "sql", "cs", "wmi", "name", "unknown");
            var ex = Assert.Throws<UnknownGraphiteClientTypeException>(() => clientFactory.Create(client));
            Assert.That(ex.Message, Is.EqualTo("Unknown Graphite Client Type: unknown"));
        }
    }
}