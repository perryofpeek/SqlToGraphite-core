using System;
using System.Data;
using System.Data.SqlClient;

using log4net;

namespace SqlToGraphite
{
    public class SqlGetter
    {
        private readonly string connectionString;
        private readonly string hostname;
        private readonly int port;
        private readonly ILog log;

        public SqlGetter(string connectionString, string hostname, int port, ILog log)
        {
            this.connectionString = connectionString;
            this.hostname = hostname;
            this.port = port;
            this.log = log;
        }

        public void Process(string path, string sql)
        {
            var connection = new SqlConnection { ConnectionString = this.connectionString };
            try
            {
                log.Debug(string.Format("running {0}", sql));
                connection.Open();
                var myReader = GetRecords(sql, connection);
                SendAllRecordsToGraphite(path, myReader);
                myReader.Close();
            }
            catch (Exception ex)
            {
                log.Error(ex.Message, ex);
            }
            finally
            {
                connection.Close();
            }
        }

        private static SqlDataReader GetRecords(string sql, SqlConnection connection)
        {
            var myCMD = new SqlCommand(sql, connection);
            var myReader = myCMD.ExecuteReader();
            return myReader;
        }

        private void SendAllRecordsToGraphite(string path, SqlDataReader myReader)
        {
            do
            {
                this.SendRecordToGraphite(path, myReader);
            }
            while (myReader.NextResult());
        }

        private void SendRecordToGraphite(string path, IDataReader myReader)
        {
            using (var client = new Graphite.GraphiteTcpClient(this.hostname, this.port, string.Empty))
            {
                while (myReader.Read())
                {
                    var value = myReader.GetInt32(1);
                    var datetime = myReader.GetDateTime(0);
                    this.log.Debug(string.Format("Sending {2} [{0}] {1}", value, datetime, path));
                    client.Send(path, myReader.GetInt32(1), myReader.GetDateTime(0));
                }
            }
        }
    }
}