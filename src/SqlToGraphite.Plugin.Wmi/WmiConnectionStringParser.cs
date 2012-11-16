namespace SqlToGraphite.Plugin.Wmi
{
    public class WmiConnectionStringParser1
    {
        public string Username { get; private set; }

        public string Password { get; private set; }

        public object Hostname { get; private set; }

        public WmiConnectionStringParser1(string connectionString)
        {
            this.Username = string.Empty;
            this.Password = string.Empty;
            var parts = connectionString.Split(';');
            foreach (var part in parts)
            {
                var nameValuePairs = part.Split('=');
                if (nameValuePairs[0].ToLower() == "username")
                {
                    this.Username = nameValuePairs[1];
                }

                if (nameValuePairs[0].ToLower() == "password")
                {
                    this.Password = nameValuePairs[1];
                }

                if (nameValuePairs[0].ToLower() == "hostname")
                {
                    this.Hostname = nameValuePairs[1];
                }
            }
        }
    }
}