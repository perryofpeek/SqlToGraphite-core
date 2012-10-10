using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Config;
using SqlToGraphite.Plugin.SqlServer;

namespace SqlToGraphite.UnitTests
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
            this.directory = MockRepository.GenerateMock<IDirectory>();
            this.log = MockRepository.GenerateMock<ILog>();
            this.config = new SqlToGraphiteConfig(new AssemblyResolver(new DirectoryImpl()));
        }

        [Test]
        public void Should_not_validate_job_if_client_does_not_exist()
        {
            var name = "Name";
            var client = "SomeClient";

            this.config.Jobs.Add(new SqlServerClient { ClientName = client, Name = name });

            var ex = Assert.Throws<ClientNotDefinedException>(() => this.config.Validate());
            //Test
            Assert.That(ex.Message, Is.EqualTo(string.Format("The client {0} has not been defined", client)));
        }

        [Test]
        public void Should_validate_job_if_client_exist()
        {
            var name = "Name";
            var clientName = "SomeClient";
            var c = new LocalGraphiteTcpClient { ClientName = clientName };
            this.config.Clients.Add(c);
            this.config.Jobs.Add(new SqlServerClient { ClientName = clientName, Name = name });
            //Test
            this.config.Validate();
        }

        [Test]
        public void Should_add_single_job()
        {
            var name = "Name";
            var client = "SomeClient";
            this.config.Jobs.Add(new SqlServerClient { ClientName = client, Name = name });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
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

            this.config.Jobs.Add(new SqlServerClient { ClientName = client1, Name = name1 });
            this.config.Jobs.Add(new SqlServerClient { ClientName = client2, Name = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
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

            this.config.Jobs.Add(new SqlServerClient { ClientName = client1, Name = name1 });
            this.config.Jobs.Add(new WmiPlugin { ClientName = client2, Name = name2 });
            //Test
            var sqlToGraphiteConfig = Helper.SerialiseDeserialise(this.config);
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