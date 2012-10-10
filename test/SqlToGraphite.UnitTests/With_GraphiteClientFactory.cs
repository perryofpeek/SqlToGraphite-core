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
            var client = new LocalGraphiteTcpClient { Hostname = "hostname", Port = 8125, ClientName = "GraphiteTcpClient" };
            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);            
            Assert.That(o, Is.TypeOf<LocalGraphiteTcpClient>());
        }

        [Test]
        public void Should_create_native_graphiteudp_client()
        {
            var client = new LocalGraphiteTcpClient { Hostname = "hostname", Port = 8125, ClientName = "GraphiteUdpClient" };
            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);            
            Assert.That(o, Is.TypeOf<GraphiteUdpClient>());
        }

        [Test]
        public void Should_create_statsd_client()
        {
            var client = new StatsdClient { Hostname = "localhost", Port = 8125, ClientName = "Statsdclient" };

            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(StatsdClient)));
            Assert.That(o, Is.TypeOf<StatsdClient>());
        }

        [Test]
        public void Should_create_statsd_client_uppercase()
        {
            var client = new StatsdClient { Hostname = "localhost", Port = 8125, ClientName = "STATSDCLIENT" };

            var o = clientFactory.Create(client);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(StatsdClient)));
            Assert.That(o, Is.TypeOf<StatsdClient>());
        }

        [Test]
        public void Should_throw_exception_if_unknown_type()
        {
            var client = new LocalGraphiteTcpClient { Hostname = "localhost", Port = 8125, ClientName = "unknown" };
            var ex = Assert.Throws<UnknownGraphiteClientTypeException>(() => clientFactory.Create(client));
            Assert.That(ex.Message, Is.EqualTo("Unknown Graphite Client Type: unknown"));
        }
    }
}