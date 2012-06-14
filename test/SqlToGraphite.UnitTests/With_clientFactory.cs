using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_clientFactory
    {
        private ILog log;

        private IClientFactory clientFactory;

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            clientFactory = new ClientFactory(log);
        }

        [Test]
        public void Should_create_sql_server_client()
        {
            var taskParams = new TaskParams { Type = "SqlServer", Sql = "sql", ConnectionString = "cs", Path = "path" };           
            var o = clientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(SqlServerClient)));
            Assert.That(o, Is.TypeOf<SqlServerClient>());
        }

        [Test]
        public void Should_create_wmi_client()
        {
            var taskParams = new TaskParams { Type = "Wmi", Sql = "sql", ConnectionString = "cs", Path = "path" };
            var o = clientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }

        [Test]
        public void Should_create_wmi_client_lowercase()
        {
            var taskParams = new TaskParams { Type = "wmi", Sql = "sql", ConnectionString = "cs", Path = "path" };            
            var o = clientFactory.Create(taskParams);

            Assert.That(o, Is.Not.Null);
            Assert.That(o, Is.TypeOf(typeof(WmiClient)));
            Assert.That(o, Is.TypeOf<WmiClient>());
        }
    }   
}