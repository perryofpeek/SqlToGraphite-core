
using log4net;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphite.Plugin.SqlServer;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_DataClientFactory
    {
        private ILog log;

        private IDataClientFactory dataClientFactory;

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
            var ex = Assert.Throws<UnknownDataClientException>(() => this.dataClientFactory.Create(taskParams));
            Assert.That(ex.Message, Is.EqualTo(string.Format("{0}", "unknown")));
        }

        [Test]
        public void Should_create_sql_server_client()
        {
            //string[] files = Directory.GetFiles(Directory.GetCurrentDirectory(), "SqlToGraphite.Plugin.SqlServer.dll");
            //foreach (string file in files)
            //{
            //    Assembly assembly = Assembly.LoadFile(file);
            //}
            var taskParams = new TaskParams("path", "sql", "cs", "SqlServer", "name", "client");
            var o = this.dataClientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(SqlServerClient)));
            Assert.That(o, Is.TypeOf<SqlServerClient>());
        }

        [Test]
        public void Should_create_wmi_client()
        {
            var taskParams = new TaskParams("path", "sql", "cs", "Wmi", "name", "client");
            var o = this.dataClientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }

        [Test]
        public void Should_create_oracle_client()
        {
            var taskParams = new TaskParams("path", "sql", "cs", "Oracle", "name", "client");
            var o = this.dataClientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(OracleClient)));
            Assert.That(o, Is.TypeOf<OracleClient>());
        }

        [Test]
        public void Should_create_wmi_client_lowercase()
        {
            var taskParams = new TaskParams("path", "sql", "cs", "wmi", "name", "client");
            var o = this.dataClientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }
    }
}