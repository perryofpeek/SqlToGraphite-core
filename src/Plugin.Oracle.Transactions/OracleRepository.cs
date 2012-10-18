using System.Data;
using System.Data.OracleClient;

namespace Plugin.Oracle.Transactions
{
    public class OracleRepository : IOracleRepository
    {
        public DataSet ExecuteQuery(string connectionString, string sql)
        {
            var ds = new DataSet();
            using (var connection = new OracleConnection())
            {
                connection.ConnectionString = connectionString;
                connection.Open();
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