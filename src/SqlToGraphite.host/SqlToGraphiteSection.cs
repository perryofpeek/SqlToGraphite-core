using System.Configuration;

namespace SqlToGraphite.host
{
    public class SqlToGraphiteSection : ConfigurationSection
    {
        public const string SectionName = "SqlToGraphite";

        [ConfigurationProperty("Hostname", IsRequired = true)]
        public string Hostname
        {
            get { return (string)this["Hostname"]; }
            set { this["Hostname"] = value; }
        }

        [ConfigurationProperty("ConfigUri", IsRequired = true)]
        public string ConfigUri
        {
            get { return (string)this["ConfigUri"]; }
            set { this["ConfigUri"] = value; }
        }

        [ConfigurationProperty("ConfigUsername", IsRequired = false)]
        public string ConfigUsername
        {
            get { return (string)this["ConfigUsername"]; }
            set { this["ConfigUsername"] = value; }
        }

        [ConfigurationProperty("ConfigPassword", IsRequired = false)]
        public string ConfigPassword
        {
            get { return (string)this["ConfigPassword"]; }
            set { this["ConfigPassword"] = value; }
        }

        [ConfigurationProperty("CheckConfigUpdatedEveryMinutes", IsRequired = true)]
        public int CheckConfigUpdatedEveryMinutes
        {
            get { return (int)this["CheckConfigUpdatedEveryMinutes"]; }
            set { this["CheckConfigUpdatedEveryMinutes"] = value; }
        }

        [ConfigurationProperty("MinutesBetweenRetryToGetConfigOnError", IsRequired = true)]
        public int MinutesBetweenRetryToGetConfigOnError
        {
            get { return (int)this["MinutesBetweenRetryToGetConfigOnError"]; }
            set { this["MinutesBetweenRetryToGetConfigOnError"] = value; }
        }

        [ConfigurationProperty("ConfigCacheLengthMinutes", IsRequired = true)]
        public int ConfigCacheLengthMinutes
        {
            get { return (int)this["ConfigCacheLengthMinutes"]; }
            set { this["ConfigCacheLengthMinutes"] = value; }
        }
    }  
}