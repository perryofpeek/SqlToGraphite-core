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
        public SqlServerClient(ILog log, ITaskParams taskParams)
            : base(log, taskParams)
        {           
        }

        public override IList<IResult> Get()
        {
            var rtn = new List<IResult>();
            var connection = new SqlConnection();
            try
            {
                connection.ConnectionString = this.TaskParams.ConnectionString;
                this.Log.Debug(string.Format("running {0}", this.TaskParams.Sql));
                connection.Open();
                var records = GetRecords(this.TaskParams.Sql, connection);
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

            if (this.TaskParams.Name != string.Empty && name == string.Empty)
            {
                name = this.TaskParams.Name;
            }

            this.Log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            return new Result(value, name, dateTime, this.TaskParams.Path);
        }

        private static SqlDataReader GetRecords(string sql, SqlConnection connection)
        {
            var myCMD = new SqlCommand(sql, connection);
            var myReader = myCMD.ExecuteReader();
            return myReader;
        }
    }
}