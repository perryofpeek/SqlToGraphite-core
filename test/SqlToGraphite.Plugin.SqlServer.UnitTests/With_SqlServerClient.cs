using System;

using log4net;

using NUnit.Framework;

using Rhino.Mocks;

using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.SqlServer.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_SqlServerClient
    {
        private const string ConnectionString = "Data Source=localhost;Initial Catalog=Master;User Id=sa;Password=!bcde1234;";

        private const string SimpleQuery = "SELECT 234 , DATEADD(day,11,GETDATE())";

        private const string SimplePath = "Some.Path";

        private IEncryption encryption;

        private ILog log;

        private Encryption e;

        [SetUp]
        public void SetUp()
        {
            e = new Encryption();
            encryption = MockRepository.GenerateMock<IEncryption>();
            log = MockRepository.GenerateMock<ILog>();
            encryption.Expect(x => x.Decrypt(e.Encrypt(ConnectionString))).Return(ConnectionString);
        }

        [Test]
        public void Should_get_result()
        {
            var metricName = Guid.NewGuid().ToString();
            var serverClientParams = new SqlServerClient
                {
                    ConnectionString = e.Encrypt(ConnectionString),
                    Path = SimplePath,
                    Sql = SimpleQuery,
                    ClientName = "GraphiteTcpClient",                    
                };


            var sqlServerClient = new SqlServerClient(log, serverClientParams, encryption);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Path, Is.EqualTo(SimplePath));
            Assert.That(results[0].Value, Is.EqualTo("234"));
        }

        [Test]
        public void Should_get_result_with_date()
        {
            var metricName = Guid.NewGuid().ToString();
            var serverClientParams = new SqlServerClient
            {
                ConnectionString = e.Encrypt(ConnectionString),
                Path = SimplePath,
                Sql = SimpleQuery,
                ClientName = "GraphiteTcpClient",                
            };
            
            var sqlServerClient = new SqlServerClient(log, serverClientParams,encryption);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            var futureDate = DateTime.Now.Add(new TimeSpan(11, 0, 0, 0));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Path, Is.EqualTo(SimplePath));
            Assert.That(results[0].Value, Is.EqualTo("234"));
            Assert.That(results[0].TimeStamp.Day, Is.EqualTo(futureDate.Day));
            Assert.That(results[0].TimeStamp.Hour, Is.EqualTo(futureDate.Hour));
            Assert.That(results[0].TimeStamp.Minute, Is.EqualTo(futureDate.Minute));
            Assert.That(results[0].TimeStamp.Year, Is.EqualTo(futureDate.Year));
        }

        [Test]
        public void Should_get_result_with_date_and_name_set_in_select()
        {
            var metricName = Guid.NewGuid().ToString();
            var serverClientParams = new SqlServerClient
            {
                ConnectionString = e.Encrypt(ConnectionString),
                Path = SimplePath,
                Sql = "SELECT 234 , DATEADD(day,11,GETDATE()), 'someName'",
                ClientName = "GraphiteTcpClient",                
            };            

            var sqlServerClient = new SqlServerClient(log, serverClientParams, encryption);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            var futureDate = DateTime.Now.Add(new TimeSpan(11, 0, 0, 0));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("someName"));
            Assert.That(results[0].Path, Is.EqualTo(SimplePath));
            Assert.That(results[0].Value, Is.EqualTo("234"));
        }

        [Test]
        public void Should_get_results()
        {
            var metricName = Guid.NewGuid().ToString();
            var serverClientParams = new SqlServerClient
            {
                ConnectionString = e.Encrypt(ConnectionString),
                Path = SimplePath,
                Sql = "SELECT [dbms_id] FROM [msdb].[dbo].[MSdbms]",
                ClientName = "GraphiteTcpClient",                
            };
            
            var sqlServerClient = new SqlServerClient(log, serverClientParams,encryption);
            var results = sqlServerClient.Get();
            Assert.That(results.Count, Is.EqualTo(8));
        }
    }
}