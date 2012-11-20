using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using log4net;
using SqlToGraphiteInterfaces;

namespace SqlToGraphite.Plugin.SqlServer
{
    public class SqlServerClient : PluginBase
    {
        private string connectionString;
        
        [Help("Name of the metric")]
        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        [Encrypted]
        [Help("The connection string in sql server format.")]
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

        [Help("Namespace path for the metric in graphite, use %h to substitute the hostname")]
        public string Path { get; set; }

        [Help("The Sql query to run, use a string column and return multiple rows, the value of the column will be used at the end of the metric path, return a date time column the value will be used as the metric datetime")]
        public string Sql { get; set; }

        public SqlServerClient()
        {
        }

        public SqlServerClient(ILog log, Job job, IEncryption encryption) : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        public override IList<IResult> Get()
        {            
            var rtn = new List<IResult>();
            var connection = new SqlConnection();
            try
            {
                connection.ConnectionString = this.connectionString;
                this.Log.Debug(string.Format("running {0}", this.Sql));
                connection.Open();
                var records = GetRecords(this.Sql, connection);
                do
                {
                    while (records.Read())
                    {
                        rtn.Add(this.Map(records));
                    }
                }
                while (records.NextResult());
                records.Close();
            }
            catch (Exception ex)
            {
                this.Log.Error(ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }

            return rtn;
        }

        private Result Map(IDataRecord record)
        {
            var value = -1;
            var dateTime = DateTime.Now;
            var name = string.Empty;
            for (int i = 0; i < record.FieldCount; i++)
            {
                if (record[i] is string)
                {
                    name = Convert.ToString(record.GetValue(i));
                }

                if (record[i] is int)
                {
                    value = record.GetInt32(i);
                }

                if (record[i] is DateTime)
                {
                    dateTime = record.GetDateTime(i);
                }
            }
           
            this.Log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            return new Result(value, name, dateTime, this.Path);
        }

        private static SqlDataReader GetRecords(string sql, SqlConnection connection)
        {
            var myCMD = new SqlCommand(sql, connection);
            var myReader = myCMD.ExecuteReader();
            return myReader;
        }
    }
}