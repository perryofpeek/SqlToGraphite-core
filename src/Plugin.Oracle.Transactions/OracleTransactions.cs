using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using log4net;
using SqlToGraphiteInterfaces;

namespace Plugin.Oracle.Transactions
{
    public class OracleTransactions : PluginBase
    {
        private readonly IOracleRepository oracleRepository;

        private string cs;

        private const int TimeDrift = 2;

        public OracleTransactions()
        {
            oracleRepository = new OracleRepository();
        }

        public OracleTransactions(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            oracleRepository = new OracleRepository();
            this.WireUpProperties(job, this);
        }

        public OracleTransactions(ILog log, Job job, IOracleRepository oracleRepository, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.oracleRepository = oracleRepository;
            this.WireUpProperties(job, this);
        }

        public string MetricName { get; set; }

        [Encrypted]
        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(this.cs))
                {
                    return string.Empty;
                }
                return this.Encrypt(this.cs);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.cs = this.Decrypt(value);
                }
                else
                {
                    this.cs = string.Empty;
                }
            }
        }


        public string Path { get; set; }

        public int NumberOfSecondsInThePast { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public override IList<IResult> Get()
        {
            try
            {
                InitialiseEntryPointsOnFirstRun();
                InitialiseLastIdOnFirstRun();
                this.ResetLastIdIfTimeDrifted();
                var rtn = this.GetTransactionCounts();
                this.SetLastId();
                UpdateLastRunTime();
                return rtn;
            }
            catch (Exception ex)
            {
                Log.Error(ex);
                DataStore.LastMaxId = 0;
                throw;
            }
        }

        private static void UpdateLastRunTime()
        {
            DataStore.LastRun = DateTime.Now;
        }

        private void ResetLastIdIfTimeDrifted()
        {
            if (DataStore.LastRun < DateTime.Now.Subtract(new TimeSpan(0, 0, 0, this.NumberOfSecondsInThePast * TimeDrift)))
            {
                Log.Error(string.Format("Time drift has occured last run was {0}", DataStore.LastRun));
                this.SetLastId();
            }
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
            var list = this.oracleRepository.ExecuteQuery(this.cs, Sql.GetEntryPoints);
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
            var dataSet = this.oracleRepository.ExecuteQuery(this.cs, sql);
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
                var name = string.Format("{0}_{1}", this.ReplaceDotsAndSpaces(entryPoint.Value), entryPoint.Key);
                this.Log.Debug(string.Format("Adding response {0} {1} {2} {3}", value, name, now, this.Path));
                responseList.Add(new Result(value, name, now, this.Path));
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
                AddToExistingValue(resultDictionary, origSystem, count);
            }
            else
            {
                resultDictionary.Add(origSystem, count);
            }
        }

        private static void AddToExistingValue(Dictionary<string, int> resultDictionary, string origSystem, int count)
        {
            resultDictionary[origSystem] += count;
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
            return input.Replace(".", "_").Replace(" ", "_").Replace("/", "_").Replace(@"\", "_");
        }

        private void SetLastId()
        {
            Log.Debug("Setting last id");
            var ds = this.oracleRepository.ExecuteQuery(this.cs, Sql.GetMaxIdSql);
            var row = ds.Tables[0].Rows[0];
            Log.Debug(string.Format("Last Id is: {0}", row[0]));
            DataStore.LastMaxId = Convert.ToInt64(row[0]);
        }
    }
}