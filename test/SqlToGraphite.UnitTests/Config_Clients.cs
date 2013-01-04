using System.Linq;

using log4net;

using NUnit.Framework;

using Rhino.Mocks;

using SqlToGraphite.Config;

namespace SqlToGraphite.UnitTests
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class Config_Clients
    {
        private SqlToGraphiteConfig config;

        private IAssemblyResolver assemblyResolver;

        private ILog log;

        [SetUp]
        public void SetUp()
        {
            this.log = MockRepository.GenerateMock<ILog>();
            this.assemblyResolver = new AssemblyResolver(new DirectoryImpl(), log);
            this.config = new SqlToGraphiteConfig(this.assemblyResolver, log);
        }

        [Test]
        public void Should_add_single_client()
        {
            var name = "Name";
            var port = 1234;
            var c1 = new GraphiteTcpClient();
            c1.ClientName = name;
            c1.Port = port;

            this.config.Clients.Add(c1);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Clients.First().ClientName, Is.EqualTo(name));
            Assert.That(sqlToGraphiteConfig.Clients.First().Port, Is.EqualTo(port));
            Assert.That(sqlToGraphiteConfig.Clients.First(), Is.TypeOf<GraphiteTcpClient>());
        }

        [Test]
        public void Should_not_add_multiple_clients_of_same_type()
        {
            this.config.Clients.Add(new GraphiteTcpClient { Port = 321, ClientName = "some name" });
            //Test
            var ex = Assert.Throws<CannotAddAnotherInstanceOfTypeException>(() => this.config.Clients.Add(new GraphiteTcpClient { Port = 54321, ClientName = "fred" }));
            //Assert
            Assert.That(ex.Message, Is.EqualTo("Cannot add another instance of GraphiteTcpClient"));
        }

        [Test]
        public void Should_add_multiple_clients_of_different_types()
        {
            var name1 = "Name1";
            var port1 = 54321;
            var name2 = "Name2";
            var port2 = 12345;

            this.config.Clients.Add(new GraphiteTcpClient() { Port = port1, ClientName = name1 });
            this.config.Clients.Add(new GraphiteUdpClient { Port = port2, ClientName = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Clients.First().Port, Is.EqualTo(port1));
            Assert.That(sqlToGraphiteConfig.Clients.First().ClientName, Is.EqualTo(name1));
            Assert.That(sqlToGraphiteConfig.Clients.First(), Is.TypeOf<GraphiteTcpClient>());
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1].Port, Is.EqualTo(port2));
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1].ClientName, Is.EqualTo(name2));
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1], Is.TypeOf<GraphiteUdpClient>());
        }
    }
}