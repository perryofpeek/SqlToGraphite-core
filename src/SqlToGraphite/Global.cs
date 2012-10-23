namespace SqlToGraphite
{
    public class Global
    {
        public const string NameSpace = "SqlToGraphite";

        public const string Version = "0.0.0.1";

        public static string GetNameSpace()
        {
            return string.Format("{0}_{1}", NameSpace, Version);
        }
    }
}