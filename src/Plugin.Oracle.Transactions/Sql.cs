namespace Plugin.Oracle.Transactions
{
    public class Sql
    {
        public const string GetEntryPoints = "select code,alias from Entry_point";

        public const string GetMaxIdSql = " Select max(id) from transactions";

        //select * from transactions where id > 537308183 and start_date_time > sysdate-60/(24*60*60)
        //private const string GetTransactionsCountSql = "select originating_system, , sum(total_cost_of_bookings), count(1) from transactions where where current_ts_code = 'FIRM' and id > {0} and start_date_time > sysdate-{1}/(24*60*60) group by originating_system";
        public const string GetTransactionsCountSql = "select originating_system, count(1) from transactions where current_ts_code = 'FIRM' and id > {0} and trunc(start_date_time) > trunc(sysdate-{1}/(24*60*60)) group by originating_system";    
    }
}