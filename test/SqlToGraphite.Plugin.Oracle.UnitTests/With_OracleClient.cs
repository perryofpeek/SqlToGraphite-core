using System;

using SqlToGraphiteInterfaces;

using log4net;
using NUnit.Framework;
using Rhino.Mocks;

namespace SqlToGraphite.Plugin.Oracle.UnitTests
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class With_OracleClient
    {
        private const string Cs = "Data Source=localhost:1521/XE;User Id=owain;Password=abcd1234;";

        private const string SimpleQuery = "SELECT 234 , DATEADD(day,11,GETDATE())";

        private const string SimplePath = "Some.Path";

        private OracleClient param;

        private ILog log;

        private IEncryption encryption;

        private OracleClient client;

        [SetUp]
        public void SetUp()
        {
            var e = new Encryption();

            param = new OracleClient { Name = "TestName", ClientName = "TestClientName", ConnectionString = "", Path = "path" };
            param.ConnectionString = e.Encrypt(Cs);            
            log = MockRepository.GenerateMock<ILog>();
            encryption = MockRepository.GenerateMock<IEncryption>();
            encryption.Expect(x => x.Decrypt(e.Encrypt(Cs))).Return(Cs);
            //encryption.Expect(x => x.Encrypt("abc")).Return(ConnectionString);

        }

        [Test]
        public void Should_get_result()
        {
            var name = Guid.NewGuid().ToString();
            param.Sql = string.Format("SELECT level, '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') FROM    dual CONNECT BY  level <= 100", name);
            client = new OracleClient(log, param, encryption);

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
            param.Sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS'),level FROM    dual CONNECT BY  level <= 100", name);
            client = new OracleClient(log, param, encryption);
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
            param.Sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') , 234 FROM    dual CONNECT BY  level <= 1", name);
            client = new OracleClient(log, param, encryption);
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
            param.Sql = string.Format("SELECT  '{0}',  to_date('2012-FEB-03 00:00:01', 'YYYY-MON-DD HH24:MI:SS') , 234 FROM    dual CONNECT BY  level <= 1", name);
            client = new OracleClient(log, param, encryption);
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
            param.Sql = "SELECT  23 , 'name' FROM    dual CONNECT BY  level <= 8";
            client = new OracleClient(log, param, encryption);
            var results = client.Get();
            Assert.That(results.Count, Is.EqualTo(8));
        }
    }
}