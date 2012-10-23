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

        public OracleClient(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        public string MetricName { get; set; }

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

        public string Path { get; set; }

        public string Sql { get; set; }

        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

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

            if (this.MetricName != string.Empty && name == string.Empty)
            {
                name = this.MetricName;
            }

            this.Log.Debug(string.Format("Got [{1}] {0}", value, dateTime));

            return new Result(value, name, dateTime, this.Path);
        }

        public DataSet ExecuteQuery(string connectionString, string sql)
        {
            var ds = new DataSet();
            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = connectionString;
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