
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

        private IDataClientFactory dataClientFactory;
        private const string SqlServerType = "SqlToGraphite.Plugin.SqlServer.SqlServerClient, SqlToGraphite.Plugin.SqlServer, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        private const string OracleType = "SqlToGraphite.Plugin.Oracle.OracleClient, SqlToGraphite.Plugin.Oracle, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";
        private const string WmiType = "SqlToGraphite.Plugin.Wmi.WmiClient, SqlToGraphite.Plugin.Wmi, Version=1.0.0.0, Culture=neutral, PublicKeyToken=null";

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            this.dataClientFactory = new DataClientFactory(log);
        }

        [Test]
        public void Should_throw_UnknownDataClientException()
        {
            var taskParams = new TaskParams("path", "sql", "cs", "unknown", "name", "client");
            Job job = new SqlServer();
            
            var ex = Assert.Throws<UnknownDataClientException>(() => this.dataClientFactory.Create(job));
            Assert.That(ex.Message, Is.EqualTo(string.Format("{0}", "unknown")));
        }

        [Test]
        public void Should_create_sql_server_client()
        {
            var taskParams = new TaskParams("path", "sql", "cs", SqlServerType, "name", "client");
            Job job = new SqlServer();
            var o = this.dataClientFactory.Create(job);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(SqlServerClient)));
            Assert.That(o, Is.TypeOf<SqlServerClient>());
        }

        [Test]
        public void Should_create_wmi_client()
        {
            Job job = new SqlServer();
            var taskParams = new TaskParams("path", "sql", "cs", WmiType, "name", "client");
            var o = this.dataClientFactory.Create(job);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }

        [Test]
        public void Should_create_oracle_client()
        {
            Job job = new SqlServer();
            var taskParams = new TaskParams("path", "sql", "cs", OracleType, "name", "client");
            var o = this.dataClientFactory.Create(job);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(OracleClient)));
            Assert.That(o, Is.TypeOf<OracleClient>());
        }

        [Test]
        public void Should_create_wmi_client_lowercase()
        {
            Job job = new SqlServer();
            var taskParams = new TaskParams("path", "sql", "cs", WmiType, "name", "client");
            var o = this.dataClientFactory.Create(job);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }
    }
}