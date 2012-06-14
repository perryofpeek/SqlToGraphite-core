using System;

using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SqlToGraphite.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_OracleClient
    {
        private const string ConnectionString = "Data Source=localhost:1521/XE;User Id=owain;Password=!bcde1234;";

        [Test]
        public void Should_get_result()
        {
            var name = Guid.NewGuid().ToString();
            var sql = string.Format("SELECT level, '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') FROM    dual CONNECT BY  level <= 100", name);
            var param = new TaskParams("path", sql, ConnectionString, "Oracle", name, "Statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var client = new OracleClient(log, param);
            //Test
            var results = client.Get();
            //Asset            
            Assert.That(results.Count, Is.EqualTo(100));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results[0].TimeStamp, Is.EqualTo(new DateTime(2012, 02, 03, 00, 00, 01)));
        }

        [Test]
        public void Should_get_result_with_different_order()
        {
            var name = Guid.NewGuid().ToString();
            var sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS'),level FROM    dual CONNECT BY  level <= 100", name);
            var param = new TaskParams("path", sql, ConnectionString, "Oracle", name, "Statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var client = new OracleClient(log, param);
            //Test
            var results = client.Get();
            //Asset            
            Assert.That(results.Count, Is.EqualTo(100));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results[0].TimeStamp, Is.EqualTo(new DateTime(2012, 02, 03, 00, 00, 01)));
        }

        [Test]
        public void Should_get_result_with_date()
        {
            var name = Guid.NewGuid().ToString();
            var sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') , 234 FROM    dual CONNECT BY  level <= 1", name);
            var param = new TaskParams("path", sql, ConnectionString, "Oracle", name, "Statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var client = new OracleClient(log, param);
            //Test
            var results = client.Get();
            //Asset            
            var futureDate = DateTime.Now.Add(new TimeSpan(11, 0, 0, 0));
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results[0].Value, Is.EqualTo(234));
            Assert.That(results[0].TimeStamp, Is.EqualTo(new DateTime(2012, 02, 03, 00, 00, 01)));
        }

        [Test]
        public void Should_get_result_with_date_and_name_set_in_select()
        {
            var name = Guid.NewGuid().ToString();
            var sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') , 234 FROM    dual CONNECT BY  level <= 1", name);
            var param = new TaskParams("path", sql, ConnectionString, "Oracle", name, "Statsdudp");            
            var log = MockRepository.GenerateMock<ILog>();
            var client = new OracleClient(log, param);
            //Test
            var results = client.Get();
            //Asset                        
            Assert.That(results.Count, Is.EqualTo(1));
            Assert.That(results[0].Name, Is.EqualTo(name));
            Assert.That(results[0].Path, Is.EqualTo("path"));
            Assert.That(results[0].Value, Is.EqualTo(234));
        }

        [Test]
        public void Should_get_multiple_results()
        {
            var param = new TaskParams("path", "SELECT  23 , 'name' FROM    dual CONNECT BY  level <= 8", ConnectionString, "SqlServer", "name", "statsdudp");
            var log = MockRepository.GenerateMock<ILog>();
            var client = new OracleClient(log, param);
            var results = client.Get();
            Assert.That(results.Count, Is.EqualTo(8));
        }
    }
}