using System;
using System.Collections.Generic;
using System.Data;
using NUnit.Framework;
using Rhino.Mocks;
using SqlToGraphiteInterfaces;
using log4net;

namespace Plugin.Oracle.Transactions.Test
{
    // ReSharper disable InconsistentNaming
    [TestFixture]
    public class Should_map_entrypoints
    {
        private ILog log = MockRepository.GenerateMock<ILog>();
        private IOracleRepository oracleRepository = MockRepository.GenerateMock<IOracleRepository>();
        private string connectionString;

        private IEncryption encryption;

        [SetUp]
        public void SetUp()
        {
            log = MockRepository.GenerateMock<ILog>();
            encryption = MockRepository.GenerateMock<IEncryption>();
            oracleRepository = MockRepository.GenerateMock<IOracleRepository>();
            connectionString = "someString";
            DataStore.EntryPoints = new Dictionary<int, string>();
            DataStore.LastMaxId = 0;
            DataStore.LastRun = DateTime.Now;
        }

        [Test]
        public void Should_get_last_max_id_and_use_it_in_the_query_and_get_results_for_each_entry_point()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            const int NewMaxId = 555777;
            DataStore.LastMaxId = 0;
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId)).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(NewMaxId)).Repeat.Once();
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository,encryption);
            //Test
            var result = oracleTransactions.Get();
            //Assert
            Assert.That(result[0].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "trainline")));
            Assert.That(result[1].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "tracs")));
            Assert.That(result[2].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "10_20_40_50")));
            Assert.That(result[0].Value, Is.EqualTo(333));
            Assert.That(result[1].Value, Is.EqualTo(444));
            Assert.That(result[2].Value, Is.EqualTo(0));
            Assert.That(DataStore.LastMaxId, Is.EqualTo(NewMaxId));
            oracleRepository.VerifyAllExpectations();
        }

        [Test]
        public void Should_get_last_max_id_because_last_run_is_greater_than_timedrift_timeout()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            const int NewMaxId = 555777;
            DataStore.LastMaxId = 1000;
            DataStore.LastRun = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, NumberOfSecondsInThePast * 3));
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId)).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(NewMaxId)).Repeat.Once();
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository,encryption);
            //Test
           oracleTransactions.Get();
            //Assert           
            Assert.That(DataStore.LastMaxId, Is.EqualTo(NewMaxId));
            oracleRepository.VerifyAllExpectations();
        }

        [Test]
        public void Should_set_last_max_id_to_zero_on_exception()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            const int NewMaxId = 555777;
            DataStore.LastMaxId = 1000;
            DataStore.LastRun = DateTime.Now.Subtract(new TimeSpan(0, 0, 0, NumberOfSecondsInThePast * 3));
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId)).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResult()).Throw(new ApplicationException());
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository, encryption);
            //Test
            Assert.Throws<ApplicationException>(() => oracleTransactions.Get());
            //Assert           
            Assert.That(DataStore.LastMaxId, Is.EqualTo(0));
            oracleRepository.VerifyAllExpectations();
        }


        [Test]
        public void Should_deal_with_odd_data_in_entry_point_and_total_count()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            const int NewMaxId = 555777;
            DataStore.LastMaxId = 0;
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId)).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResultOddData()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(NewMaxId)).Repeat.Once();
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository, encryption);
            //Test
            var result = oracleTransactions.Get();
            //Assert
            Assert.That(result.Count, Is.EqualTo(5));
            Assert.That(result[0].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "trainline")));
            Assert.That(result[1].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "tracs")));
            Assert.That(result[2].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "10_20_40_50")));
            Assert.That(result[3].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "SomeString")));
            Assert.That(result[4].FullPath, Is.EqualTo(string.Format("{0}.{1}", "Path", "Total")));
            Assert.That(result[0].Value, Is.EqualTo(333));
            Assert.That(result[1].Value, Is.EqualTo(999));
            Assert.That(result[2].Value, Is.EqualTo(0));
            Assert.That(result[3].Value, Is.EqualTo(666));
            Assert.That(result[4].Value, Is.EqualTo(1998));
            Assert.That(DataStore.LastMaxId, Is.EqualTo(NewMaxId));
            oracleRepository.VerifyAllExpectations();
        }

        [Test]
        public void Should_get_from_Entry_points_and_create_hash_only_once()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            DataStore.LastMaxId = 0;
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId));
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResult());
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository, encryption);
            //Test
            oracleTransactions.Get();
            oracleTransactions.Get();
            //Assert           
            oracleRepository.VerifyAllExpectations();
        }

        [Test]
        public void Should_get_from_Entry_points_table_and_create_hash()
        {
            int NumberOfSecondsInThePast = 123;
            var job = new OracleTransactions();
            job.Path = "Path";
            job.ConnectionString = connectionString;
            job.NumberOfSecondsInThePast = NumberOfSecondsInThePast;
            const int MaxId = 123456;
            const int NewMaxId = 555777;
            DataStore.LastMaxId = 0;
            var sql = string.Format(Sql.GetTransactionsCountSql, MaxId, NumberOfSecondsInThePast);
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetEntryPoints)).Return(CreateEntryPointQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(MaxId)).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, sql)).Return(CreateQueryResult()).Repeat.Once();
            oracleRepository.Expect(x => x.ExecuteQuery(connectionString, Sql.GetMaxIdSql)).Return(CreateMaxIdResult(NewMaxId)).Repeat.Once();
            var oracleTransactions = new OracleTransactions(log, job, oracleRepository, encryption);
            //Test
            oracleTransactions.Get();
            //Assert
            Assert.That(DataStore.EntryPoints.Count, Is.EqualTo(3));
            oracleRepository.VerifyAllExpectations();
        }

        private DataSet CreateEntryPointQueryResult()
        {
            var maxIdDs = new DataSet();
            var dt = new DataTable();
            maxIdDs.Tables.Add(dt);
            var dc0 = new DataColumn("code", typeof(string));
            var dc1 = new DataColumn("alias", typeof(string));
            maxIdDs.Tables[0].Columns.Add(dc0);
            maxIdDs.Tables[0].Columns.Add(dc1);
            AddEntryRow(dt, 11, "trainline");
            AddEntryRow(dt, 12, "tracs");
            AddEntryRow(dt, 13, "10.20.40.50");
            return maxIdDs;
        }

        private DataSet CreateQueryResult()
        {
            var maxIdDs = new DataSet();
            var dt = new DataTable();
            maxIdDs.Tables.Add(dt);
            var dc0 = new DataColumn("originating_system");
            var dc1 = new DataColumn("Count(1)", typeof(int));
            maxIdDs.Tables[0].Columns.Add(dc0);
            maxIdDs.Tables[0].Columns.Add(dc1);
            AddRow(dt, "11", 333);
            AddRow(dt, "12", 444);
            return maxIdDs;
        }

        private DataSet CreateQueryResultOddData()
        {
            var maxIdDs = new DataSet();
            var dt = new DataTable();
            maxIdDs.Tables.Add(dt);
            var dc0 = new DataColumn("originating_system");
            var dc1 = new DataColumn("Count(1)", typeof(int));
            maxIdDs.Tables[0].Columns.Add(dc0);
            maxIdDs.Tables[0].Columns.Add(dc1);
            AddRow(dt, "11", 333);
            AddRow(dt, "12", 444);
            AddRow(dt, "12-509", 555);
            AddRow(dt, "SomeString", 666);
            return maxIdDs;
        }

        private void AddEntryRow(DataTable dt, int code, string alias)
        {
            var row = dt.NewRow();
            row.ItemArray = new object[2];
            row["code"] = code;
            row["alias"] = alias;
            dt.Rows.Add(row);
        }

        private void AddRow(DataTable dt, string code, int value)
        {
            var row = dt.NewRow();
            row.ItemArray = new object[2];
            row["originating_system"] = code;
            row["Count(1)"] = value;
            dt.Rows.Add(row);
        }

        private DataSet CreateMaxIdResult(int MaxId)
        {
            var maxIdDs = new DataSet();
            var dt = new DataTable();
            maxIdDs.Tables.Add(dt);
            var dc = new DataColumn("id");
            maxIdDs.Tables[0].Columns.Add(dc);

            var row = dt.NewRow();
            row.ItemArray = new object[1];
            row["id"] = MaxId;
            dt.Rows.Add(row);
            return maxIdDs;
        }
    }
}