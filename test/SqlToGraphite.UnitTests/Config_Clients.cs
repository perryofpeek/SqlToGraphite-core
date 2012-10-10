using System.Linq;

using Graphite;

using NUnit.Framework;
using SqlToGraphite;
using SqlToGraphite.Config;
using SqlToGraphite.UnitTests;

using GraphiteUdpClient = SqlToGraphite.GraphiteUdpClient;

namespace ConfigSpike
{
    [TestFixture]
// ReSharper disable InconsistentNaming
    public class Config_Clients
    {
        private SqlToGraphiteConfig config;

        private IAssemblyResolver assemblyResolver;

        [SetUp]
        public void SetUp()
        {
            assemblyResolver = new AssemblyResolver(new DirectoryImpl());
            config = new SqlToGraphiteConfig(assemblyResolver); 
        }

        [Test]
        public void Should_add_single_client()
        {
            var name = "Name";
            var port = 1234;
            var c1 = new LocalGraphiteTcpClient();
            c1.ClientName = name;
            c1.Port = port;
           
            config.Clients.Add(c1);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Clients.First().ClientName, Is.EqualTo(name));
            Assert.That(sqlToGraphiteConfig.Clients.First().Port, Is.EqualTo(port));
            Assert.That(sqlToGraphiteConfig.Clients.First(), Is.TypeOf<LocalGraphiteTcpClient>());
        }

        [Test]
        public void Should_not_add_multiple_clients_of_same_type()
        {            
            config.Clients.Add(new LocalGraphiteTcpClient { Port = 321, ClientName = "some name" });            
            //Test
            var ex = Assert.Throws<CannotAddAnotherInstanceOfTypeException>(() => config.Clients.Add(new LocalGraphiteTcpClient { Port = 54321, ClientName = "fred" }));
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
            
            config.Clients.Add(new LocalGraphiteTcpClient() { Port = port1, ClientName = name1 });
            config.Clients.Add(new GraphiteUdpClient { Port = port2, ClientName = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Clients.First().Port, Is.EqualTo(port1));
            Assert.That(sqlToGraphiteConfig.Clients.First().ClientName, Is.EqualTo(name1));
            Assert.That(sqlToGraphiteConfig.Clients.First(), Is.TypeOf<LocalGraphiteTcpClient>());
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1].Port, Is.EqualTo(port2));
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1].ClientName, Is.EqualTo(name2));
            Assert.That(sqlToGraphiteConfig.Clients.ToList()[1], Is.TypeOf<GraphiteUdpClient>());
        }        
    }
}