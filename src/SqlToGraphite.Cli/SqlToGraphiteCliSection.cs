using System.Configuration;

namespace SqlToGraphite.Cli
{
    public class SqlToGraphiteCliSection : ConfigurationSection
    {
        public const string SectionName = "SqlToGraphiteCli";

        [ConfigurationProperty("hostname", IsRequired = true)]
        public string Hostname
        {
            get { return (string)this["hostname"]; }
            set { this["hostname"] = value; }
        }

        [ConfigurationProperty("port", IsRequired = true)]
        public int Port
        {
            get { return (int)this["port"]; }
            set { this["port"] = value; }
        }       
    }
}