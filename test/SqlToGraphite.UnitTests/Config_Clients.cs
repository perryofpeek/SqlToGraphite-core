using System.Linq;

using ConfigSpike.Config;

using NUnit.Framework;

using SqlToGraphite.UnitTests;

namespace ConfigSpike
{
    [TestFixture]
// ReSharper disable InconsistentNaming
    public class Config_Clients
    {
        [Test]
        public void Should_add_single_client()
        {
            var name = "Name";
            var port = 1234;
            var c1 = new GraphiteTcpClient();
            c1.ClientName = name;
            c1.Port = port;

            var config = new ConfigSpike.Config.SqlToGraphiteConfig();
            config.Clients.Add(c1);
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Clients.First().ClientName, Is.EqualTo(name));
            Assert.That(sqlToGraphiteConfig.Clients.First().Port, Is.EqualTo(port));
            Assert.That(sqlToGraphiteConfig.Clients.First(), Is.TypeOf<GraphiteTcpClient>());
        }

        [Test]
        public void Should_not_add_multiple_clients_of_same_type()
        {
            var config = new ConfigSpike.Config.SqlToGraphiteConfig();
            config.Clients.Add(new GraphiteTcpClient { Port = 321, ClientName = "some name" });            
            //Test
            var ex = Assert.Throws<CannotAddAnotherInstanceOfTypeException>(() => config.Clients.Add(new GraphiteTcpClient { Port = 54321, ClientName = "fred" }));
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

            var config = new ConfigSpike.Config.SqlToGraphiteConfig();
            config.Clients.Add(new GraphiteTcpClient() { Port = port1, ClientName = name1 });
            config.Clients.Add(new GraphiteUdpClient { Port = port2, ClientName = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
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