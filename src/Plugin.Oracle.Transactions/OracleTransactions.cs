using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using System.Linq;

using log4net;
using SqlToGraphiteInterfaces;

namespace Plugin.Oracle.Transactions
{
    public class OracleTransactions : PluginBase
    {
        private IOracleRepository oracleRepository;

        public OracleTransactions()
        {
            oracleRepository = new OracleRepository();
        }

        public OracleTransactions(ILog log, Job job)
            : base(log, job)
        {
            oracleRepository = new OracleRepository();
            this.WireUpProperties(job, this);
        }

        public OracleTransactions(ILog log, Job job, IOracleRepository oracleRepository)
            : base(log, job)
        {
            this.oracleRepository = oracleRepository;
            this.WireUpProperties(job, this);
        }

        public string MetricName { get; set; }

        public string ConnectionString { get; set; }

        public string Path { get; set; }

        public int NumberOfSecondsInThePast { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public override IList<IResult> Get()
        {
            InitialiseEntryPointsOnFirstRun();
            InitialiseLastIdOnFirstRun();

            var rtn = this.GetTransactionCounts();


            this.SetLastId();
            //Need to have a safty net for exception where lastId is reset 
            return rtn;
        }

        private void InitialiseLastIdOnFirstRun()
        {
            if (DataStore.LastMaxId == 0)
            {
                Log.Info("Initialising last id on first run");
                this.SetLastId();
            }
        }

        private void InitialiseEntryPointsOnFirstRun()
        {
            if (DataStore.EntryPoints.Count == 0)
            {
                Log.Info(string.Format("Setting up entry points"));
                this.PopulateEntryPoints();
            }
        }

        private void PopulateEntryPoints()
        {
            var list = this.oracleRepository.ExecuteQuery(this.ConnectionString, Sql.GetEntryPoints);
            foreach (DataRow row in list.Tables[0].Rows)
            {
                Log.Debug(string.Format("Adding Entry point: {0} {1}", row[0], row[1]));
                DataStore.EntryPoints.Add(Convert.ToInt32(row[0]), row[1].ToString());
            }
        }

        private List<IResult> GetTransactionCounts()
        {
            var now = DateTime.Now;
            var sql = string.Format(Sql.GetTransactionsCountSql, DataStore.LastMaxId, this.NumberOfSecondsInThePast);
            var dataSet = this.oracleRepository.ExecuteQuery(this.ConnectionString, sql);
            var resultDictionary = this.LoadResponseIntoDictionary(dataSet);
            return CreateResponseList(now, resultDictionary); ;
        }

        private List<IResult> CreateResponseList(DateTime now, Dictionary<string, int> resultDictionary)
        {
           
            var responseList = this.CreateResultSetForEveryEntryPoint(now, resultDictionary);
            this.AddResultsThatAreNotListedAsEntryPoints(now, resultDictionary, responseList);
            this.AddTotalTransactionCount(now, responseList);

            return responseList;
        }

        private void AddTotalTransactionCount(DateTime now, List<IResult> responseList)
        {
            var total = responseList.Sum(result => result.Value);
            responseList.Add(new Result(total, "Total", now, this.Path));
        }

        private void AddResultsThatAreNotListedAsEntryPoints(DateTime now, Dictionary<string, int> resultDictionary, List<IResult> responseList)
        {
            foreach (var oddResult in resultDictionary)
            {
                responseList.Add(new Result(oddResult.Value, this.ReplaceDotsAndSpaces(oddResult.Key), now, this.Path));
            }
        }

        private List<IResult> CreateResultSetForEveryEntryPoint(DateTime now, Dictionary<string, int> resultDictionary)
        {
            var responseList = new List<IResult>();
            foreach (var entryPoint in DataStore.EntryPoints)
            {
                var value = SetEntrypointValueIfReturnedInResultSet(resultDictionary, entryPoint);
                this.Log.Debug(string.Format("Adding response {0} {1} {2} {3}", value, this.ReplaceDotsAndSpaces(entryPoint.Value), now, this.Path));
                responseList.Add(new Result(value, this.ReplaceDotsAndSpaces(entryPoint.Value), now, this.Path));
            }
            return responseList;
        }

        private static int SetEntrypointValueIfReturnedInResultSet(Dictionary<string, int> resultDictionary, KeyValuePair<int, string> entryPoint)
        {
            int value = 0;
            if (resultDictionary.ContainsKey(entryPoint.Key.ToString()))
            {
                value = resultDictionary[entryPoint.Key.ToString()];
                resultDictionary.Remove((entryPoint.Key.ToString()));
            }
            return value;
        }

        private Dictionary<string, int> LoadResponseIntoDictionary(DataSet dataSet)
        {
            var resultDictionary = new Dictionary<string, int>();
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                Log.Debug(string.Format("Orig_system [{0}] Value[{1}]", row[0], row[1]));
                AddResultToDictionary(resultDictionary, this.CleanOrigSystem(row), Convert.ToInt32(row[1]));               
            }
            return resultDictionary;
        }

        private static void AddResultToDictionary(Dictionary<string, int> resultDictionary, string origSystem, int count)
        {
            if (resultDictionary.ContainsKey(origSystem))
            {
                resultDictionary[origSystem] += count;
            }
            else
            {
                resultDictionary.Add(origSystem, count);
            }
        }

        private string CleanOrigSystem(DataRow row)
        {
            var origSystem = row[0].ToString();
            if (origSystem.Contains("-"))
            {
                origSystem = origSystem.Substring(0, origSystem.IndexOf("-"));
            }
            return origSystem;
        }

        private string ReplaceDotsAndSpaces(string input)
        {
            return input.Replace(".", "_").Replace(" ", "_");
        }

        private void SetLastId()
        {
            Log.Debug("Setting last id");
            var ds = this.oracleRepository.ExecuteQuery(this.ConnectionString, Sql.GetMaxIdSql);
            var row = ds.Tables[0].Rows[0];
            Log.Debug(string.Format("Last Id is: {0}", row[0]));
            DataStore.LastMaxId = Convert.ToInt64(row[0]);
        }
    }
}