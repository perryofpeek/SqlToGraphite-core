using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Plugin.Oracle;
using SqlToGraphite.Plugin.SqlServer;
using SqlToGraphite.Plugin.Wmi;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_DataClientFactory
    {
        private ILog log;

        private IAssemblyResolver assemblyResolver;

        private IDataClientFactory dataClientFactory;

        private IEncryption encryption;

        [SetUp]
        public void SetUp()
        {
            encryption = new Encryption();
            log = MockRepository.GenerateMock<ILog>();
            assemblyResolver = MockRepository.GenerateMock<IAssemblyResolver>();
            this.dataClientFactory = new DataClientFactory(log, assemblyResolver, encryption);
        }

        [Test]
        public void Should_throw_UnknownDataClientException()
        {
            Job job = new SqlServerClient();
            assemblyResolver.Expect(x => x.ResolveType(job)).Return(null);
            var ex = Assert.Throws<UnknownDataClientException>(() => this.dataClientFactory.Create(job));
            Assert.That(ex.Message, Is.EqualTo(string.Format("{0}", job.GetType().FullName)));
            assemblyResolver.VerifyAllExpectations();
        }

        [Test]
        public void Should_create_sql_server_client()
        {
            var e = new Encryption();
            var job = new SqlServerClient { ConnectionString = "WnCLk1lIq9i4/tb9MQlRFQ==", ClientName = "clientName" };
            //encryption.Expect(x => e.Decrypt("WnCLk1lIq9i4/tb9MQlRFQ==")).Return("someValue");
            assemblyResolver.Expect(x => x.ResolveType(job)).Return(job.GetType());
            //Test
            var o = (SqlServerClient)this.dataClientFactory.Create(job);
            //Assert
            Assert.That(o, Is.Not.Null);
            Assert.That(o.ConnectionString, Is.EqualTo(job.ConnectionString));
            Assert.That(o.ClientName, Is.EqualTo(job.ClientName));
            Assert.That(o.Sql, Is.EqualTo(job.Sql));
            Assert.That(o, Is.TypeOf<SqlServerClient>());
            assemblyResolver.VerifyAllExpectations();
        }

        [Test]
        public void Should_create_wmi_client()
        {
            var job = new WmiClient { Sql = "someSql", Name = "someName", Username = "someUser", Password = "pass", Hostname = "hostname" };
            assemblyResolver.Expect(x => x.ResolveType(job)).Return(job.GetType());
            //Test
            var o = (WmiClient)this.dataClientFactory.Create(job);
            //Assert
            Assert.That(o.Sql, Is.EqualTo(job.Sql));
            Assert.That(o.Name, Is.EqualTo(job.Name));
            Assert.That(o.Path, Is.EqualTo(job.Path));
            Assert.That(o.Username, Is.EqualTo(job.Username));
            Assert.That(o.Password, Is.EqualTo(job.Password));
            Assert.That(o.Hostname, Is.EqualTo(job.Hostname));
            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf<WmiClient>());
            assemblyResolver.VerifyAllExpectations();
        }

        [Test]
        public void Should_create_oracle_client()
        {
            OracleClient job = new OracleClient();
            job.ConnectionString = "WnCLk1lIq9i4/tb9MQlRFQ==";
            assemblyResolver.Expect(x => x.ResolveType(job)).Return(job.GetType());
            var o = this.dataClientFactory.Create(job);
            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(OracleClient)));
            Assert.That(o, Is.TypeOf<OracleClient>());
            assemblyResolver.VerifyAllExpectations();
        }
    }
}