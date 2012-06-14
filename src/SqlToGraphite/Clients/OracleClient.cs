using System;
using System.Collections.Generic;
using System.Data;
using System.Data.OracleClient;

using log4net;

namespace SqlToGraphite
{
    public class OracleClient : ISqlClient
    {
        private readonly ILog log;

        private readonly TaskParams taskParams;

        public OracleClient(ILog log, TaskParams taskParams)
        {
            this.log = log;
            this.taskParams = taskParams;
        }

        public IList<IResult> Get()
        {
            var rtn = new List<IResult>();
            var dataSet = this.ExecuteQuery(taskParams.ConnectionString, taskParams.Sql);            
            foreach (DataRow row in dataSet.Tables[0].Rows)
            {
                rtn.Add(Map(row));
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

            if (taskParams.Name != string.Empty && name == string.Empty)
            {
                name = taskParams.Name;
            }

            this.log.Debug(string.Format("Got [{1}] {0}", value, dateTime));
            return new Result(value, name, dateTime, this.taskParams.Path);
        }

        public DataSet ExecuteQuery(string connectionString, string sql)
        {
            var ds = new DataSet();
            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
                Console.WriteLine("State: {0}", connection.State);
                Console.WriteLine("ConnectionString: {0}", connection.ConnectionString);
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