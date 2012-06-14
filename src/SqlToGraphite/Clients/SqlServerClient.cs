using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using log4net;

namespace SqlToGraphite
{
    public class SqlServerClient : ISqlClient
    {
        private readonly ILog log;

        private readonly TaskParams taskParams;

        public SqlServerClient(ILog log, TaskParams taskParams)
        {
            this.log = log;
            this.taskParams = taskParams;
        }

        public IList<IResult> Get()
        {
            var rtn = new List<IResult>();
            var connection = new SqlConnection();
            try
            {
                connection.ConnectionString = taskParams.ConnectionString;
                log.Debug(string.Format("running {0}", taskParams.Sql));
                connection.Open();
                var records = GetRecords(taskParams.Sql, connection);
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
                log.Error(ex.Message, ex);
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

            if (taskParams.Name != string.Empty && name == string.Empty)
            {
                name = taskParams.Name;
            }

            this.log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            return new Result(value, name, dateTime, this.taskParams.Path);
        }

        private static SqlDataReader GetRecords(string sql, SqlConnection connection)
        {
            var myCMD = new SqlCommand(sql, connection);
            var myReader = myCMD.ExecuteReader();
            return myReader;
        }
    }
}