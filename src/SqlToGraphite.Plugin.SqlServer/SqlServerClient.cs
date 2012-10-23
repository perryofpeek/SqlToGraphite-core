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
        public string MetricName { get; set; }
            
        public override string Name { get; set; }

        public override string ClientName { get; set; }

        public override string Type { get; set; }

        public string ConnectionString
        {
            get
            {
                if (string.IsNullOrEmpty(connectionString))
                {
                    return string.Empty;
                }
                return this.Encrypt(this.connectionString);
            }
            set
            {
                if (!string.IsNullOrEmpty(value))
                {
                    this.connectionString = this.Decrypt(value);
                }
                else
                {
                    this.connectionString = string.Empty;
                }
            }
        }

        public string Path { get; set; }

        public string  Sql { get; set; }

        private static int count;

        private string connectionString;

        public SqlServerClient()
        {
        }

        public SqlServerClient(ILog log, Job job, IEncryption encryption)
            : base(log, job, encryption)
        {
            this.WireUpProperties(job, this);
        }

        public override IList<IResult> Get()
        {
            count++;           
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

            if (this.MetricName != string.Empty && name == string.Empty)
            {
                name = this.MetricName;
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