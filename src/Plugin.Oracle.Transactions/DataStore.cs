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
        }

        public static Int64 LastMaxId;

        public static Dictionary<int, string> EntryPoints;
    }
}
