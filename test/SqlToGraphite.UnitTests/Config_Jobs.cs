using ConfigSpike.Config;
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite;
using SqlToGraphite.Plugin.SqlServer;
using SqlToGraphite.UnitTests;
using GraphiteTcpClient = SqlToGraphite.Config.GraphiteTcpClient;

namespace ConfigSpike
{
    [TestFixture]
    // ReSharper disable InconsistentNaming
    public class Config_Jobs
    {
        private ILog log;

        private SqlToGraphiteConfig config;

        private IDirectory directory;

        [SetUp]
        public void SetUp()
        {
            directory = MockRepository.GenerateMock<IDirectory>();
            log = MockRepository.GenerateMock<ILog>();
            config = new SqlToGraphiteConfig(new AssemblyResolver(new DirectoryImpl()));
        }

        [Test]
        public void Should_not_validate_job_if_client_does_not_exist()
        {
            var name = "Name";
            var client = "SomeClient";

            config.Jobs.Add(new SqlServerClient { ClientName = client, Name = name });

            var ex = Assert.Throws<ClientNotDefinedException>(() => config.Validate());
            //Test
            Assert.That(ex.Message, Is.EqualTo(string.Format("The client {0} has not been defined", client)));
        }

        [Test]
        public void Should_validate_job_if_client_exist()
        {
            var name = "Name";
            var clientName = "SomeClient";
            var c = new GraphiteTcpClient { ClientName = clientName };
            config.Clients.Add(c);
            config.Jobs.Add(new SqlServerClient { ClientName = clientName, Name = name });
            //Test
            config.Validate();
        }

        [Test]
        public void Should_add_single_job()
        {
            var name = "Name";
            var client = "SomeClient";
            config.Jobs.Add(new SqlServerClient { ClientName = client, Name = name });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Jobs[0].ClientName, Is.EqualTo(client));
            Assert.That(sqlToGraphiteConfig.Jobs[0].Name, Is.EqualTo(name));
        }

        [Test]
        public void Should_add_multiple_jobs_of_same_type()
        {
            var name1 = "Name1";
            var client1 = "SomeClient1";
            var name2 = "Name2";
            var client2 = "SomeClient2";

            config.Jobs.Add(new SqlServerClient { ClientName = client1, Name = name1 });
            config.Jobs.Add(new SqlServerClient { ClientName = client2, Name = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Jobs[0].ClientName, Is.EqualTo(client1));
            Assert.That(sqlToGraphiteConfig.Jobs[0].Name, Is.EqualTo(name1));
            Assert.That(sqlToGraphiteConfig.Jobs[1].ClientName, Is.EqualTo(client2));
            Assert.That(sqlToGraphiteConfig.Jobs[1].Name, Is.EqualTo(name2));
        }

        [Test]
        public void Should_add_multiple_jobs_of_different_types()
        {
            var name1 = "Name1";
            var client1 = "SomeClient1";
            var name2 = "Name2";
            var client2 = "SomeClient2";

            config.Jobs.Add(new SqlServerClient { ClientName = client1, Name = name1 });
            config.Jobs.Add(new WmiPlugin { ClientName = client2, Name = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(config);
            //Assert
            Assert.That(sqlToGraphiteConfig.Jobs[0].ClientName, Is.EqualTo(client1));
            Assert.That(sqlToGraphiteConfig.Jobs[0].Name, Is.EqualTo(name1));
            Assert.That(sqlToGraphiteConfig.Jobs[0], Is.TypeOf<SqlServerClient>());
            Assert.That(sqlToGraphiteConfig.Jobs[1].ClientName, Is.EqualTo(client2));
            Assert.That(sqlToGraphiteConfig.Jobs[1].Name, Is.EqualTo(name2));
            Assert.That(sqlToGraphiteConfig.Jobs[1], Is.TypeOf<WmiPlugin>());
        }
    }
}