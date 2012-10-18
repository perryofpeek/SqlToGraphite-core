using System;
using System.Collections.Generic;

namespace Plugin.Oracle.Transactions
{
    public static class DataStore
    {
        static DataStore()
        {
            LastMaxId = 0;
            EntryPoints = new Dictionary<int, string>();
            LastRun = new DateTime(2000,1,1,1,1,1);
        }

        public static Int64 LastMaxId { get; set; }

        public static Dictionary<int, string> EntryPoints { get; set; }

        public static DateTime LastRun { get; set; }
    }
}
