using System.Data;

namespace Plugin.Oracle.Transactions
{
    public interface IOracleRepository
    {
        DataSet ExecuteQuery(string connectionString, string sql);
    }
}