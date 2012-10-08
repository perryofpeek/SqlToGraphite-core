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

        [Test]
        public void Should_get_result()
        {
            var name = Guid.NewGuid().ToString();
            //var param = new TaskParams("path", "SELECT COUNT(*) as count FROM [master].[dbo].[spt_monitor]", ConnectionString, "SqlServer", name, "Statsdudp");
            Job param = null;
            var log = MockRepository.GenerateMock<ILog>();
            var sqlServerClient = new SqlServerClient(log, param);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results.Count, Is.EqualTo(1));
        }

        [Test]
        public void Should_get_result_with_date()
        {
            var name = Guid.NewGuid().ToString();
            Job param = null;
            //var param = new TaskParams("path", "SELECT 234 , DATEADD(day,11,GETDATE())", ConnectionString, "SqlServer", name, "statsdudp");

            var log = MockRepository.GenerateMock<ILog>();
            var sqlServerClient = new SqlServerClient(log, param);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            var futureDate = DateTime.Now.Add(new TimeSpan(11, 0, 0, 0));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results[0].Value, Is.EqualTo(234));
            Assert.That(results[0].TimeStamp.Day, Is.EqualTo(futureDate.Day));
            Assert.That(results[0].TimeStamp.Hour, Is.EqualTo(futureDate.Hour));
            Assert.That(results[0].TimeStamp.Minute, Is.EqualTo(futureDate.Minute));
            Assert.That(results[0].TimeStamp.Year, Is.EqualTo(futureDate.Year));
        }

        [Test]
        public void Should_get_result_with_date_and_name_set_in_select()
        {
            var name = Guid.NewGuid().ToString();
            Job param = null;
            //var param = new TaskParams("path", "SELECT 234 , DATEADD(day,11,GETDATE()), 'someName'", ConnectionString, "SqlServer", name, "statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var sqlServerClient = new SqlServerClient(log, param);
            //Test
            var results = sqlServerClient.Get();
            //Asset            
            var futureDate = DateTime.Now.Add(new TimeSpan(11, 0, 0, 0));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo("someName"));
            Assert.That(results[0].Path, Is.EqualTo("path"));
            Assert.That(results[0].Value, Is.EqualTo(234));
        }

        [Test]
        public void Should_get_results()
        {
            Job param = null;
            //var param = new TaskParams("path", "SELECT [dbms_id] FROM [msdb].[dbo].[MSdbms]", ConnectionString, "SqlServer", "name", "statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var sqlServerClient = new SqlServerClient(log, param);
            var results = sqlServerClient.Get();
            Assert.That(results.Count, Is.EqualTo(8));
        }
    }
}