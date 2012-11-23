using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.Oracle
{
    public class OracleClient : PluginBase
    {
        private string connectionString;

        public OracleClient()
        {
        }

        public OracleClient(ILog log, Job job, IEncryption encryption) : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        [Help("Name of the metric")]
        public override string Name { get; set; }

        [Help("The connection string in oracle format.")]
        [Encrypted]
        public string ConnectionString
        {
            get
            {
                return this.Encrypt(this.connectionString);
            }

            set
            {
                this.connectionString = this.Decrypt(value);
            }
        }
        
        public override string ClientName { get; set; }

        public override string Type { get; set; }

        [Help("Namespace path for the metric in graphite, use %h to substitute the hostname")]
        public string Path { get; set; }

        [Help("The Sql query to run, use a string column and return multiple rows, the value of the column will be used at the end of the metric path, return a date time column the value will be used as the metric datetime")]       
        public string Sql { get; set; }
       
        public override IList<IResult> Get()
        {
            var rtn = new List<IResult>();
            var dataSet = this.ExecuteQuery(this.connectionString, this.Sql);
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                rtn.Add(this.Map(row));
            }

            return rtn;
        }

        private Result Map(DataRow record)
        {
            var value = -1;
            var dateTime = DateTime.Now;
            var name = string.Empty;
            for (int i = 0; i < record.ItemArray.Length; i++)
            {
                if (record[i] is string)
                {
                    name = Convert.ToString(record[i]);
                }

                if (record[i] is int || record[i] is decimal)
                {
                    value = Convert.ToInt32(record[i]);
                }

                if (record[i] is DateTime)
                {
                    dateTime = Convert.ToDateTime(record[i]);
                }
            }

            this.Log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            var r = new Result(name, dateTime, this.Path);
            r.SetValue(value);
            return r;
        }

        public DataSet ExecuteQuery(string cs, string sql)
        {
            var ds = new DataSet();
            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = cs;
                connection.Open();
                //Console.WriteLine("State: {0}", connection.State);
                //Console.WriteLine("ConnectionString: {0}", connection.ConnectionString);
                var command = connection.CreateCommand();
                command.CommandText = sql;
                command.ExecuteReader();
                var da = new OracleDataAdapter(command);
                da.Fill(ds);
            }

            return ds;
        }
    }
}