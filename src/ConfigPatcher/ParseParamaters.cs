using System.Collections.Generic;

namespace ConfigPatcher
{
    public class ParseParamaters
    {
        private readonly Dictionary<string, string> dictionary;

        public string Password { get; private set; }
        public string Username { get; private set; }
        public string Hostname { get; private set; }
        public string ConfigUpdate { get; private set; }
        public string ConfigRetry { get; private set; }
        public string CacheLength { get; private set; }
        public string Path { get; private set; }
        public string ConfigUri { get; private set; }

        public ParseParamaters(IEnumerable<string> input)
        {
            this.SetDefaults();
            this.dictionary = new Dictionary<string, string>();


            foreach (var param in input)
            {
                if (param != null)
                {
                    var parts = param.Split('=');
                    if (parts.Length >= 2)
                    {
                        dictionary.Add(parts[0], parts[1]);
                    }
                }
            }

            this.AssignProperties();
        }

        private void AssignProperties()
        {
            if (this.dictionary.ContainsKey("username"))
            {
                this.Username = this.dictionary["username"];
            }

            if (this.dictionary.ContainsKey("password"))
            {
                this.Password = this.dictionary["password"];
            }

            if (this.dictionary.ContainsKey("path"))
            {
                this.Path = this.dictionary["path"];
            }

            if (this.dictionary.ContainsKey("hostname"))
            {
                this.Hostname = this.dictionary["hostname"];
            }

            if (this.dictionary.ContainsKey("configupdate"))
            {
                this.ConfigUpdate = this.dictionary["configupdate"];
            }

            if (this.dictionary.ContainsKey("configretry"))
            {
                this.ConfigRetry = this.dictionary["configretry"];
            }

            if (this.dictionary.ContainsKey("configuri"))
            {
                this.ConfigUri = this.dictionary["configuri"];
            }

            if (this.dictionary.ContainsKey("cachelength"))
            {
                this.CacheLength = this.dictionary["cachelength"];
            }
        }

        private void SetDefaults()
        {
            this.Password = string.Empty;
            this.Username = string.Empty;
            this.Hostname = string.Empty;
            this.ConfigUpdate = "10";
            this.ConfigRetry = "10";
            this.CacheLength = "10";
            this.Path = "SqlToGraphite.exe.config";
        }
    }
}