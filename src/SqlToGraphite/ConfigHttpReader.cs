using System.IO;
using System.Net;

namespace SqlToGraphite
{
    public class ConfigHttpReader : IConfigReader
    {
        private const string UserAgent = "Mozilla/4.0 (compatible; MSIE 6.0; Windows NT 5.2; .NET CLR 1.0.3705;)";

        private readonly string uri;

        private readonly string username;

        private readonly string password;

        private string currentConfig;

        public ConfigHttpReader(string uri, string username, string password)
        {
            this.uri = uri;
            this.username = username;
            this.password = password;
        }

        private StreamReader Get()
        {
            var client = new WebClient();
            this.SetUsernameAndPassword(client);
            client.Headers.Add("user-agent", UserAgent);
            return new StreamReader(client.OpenRead(uri));
        }

        private void SetUsernameAndPassword(WebClient client)
        {
            if (!string.IsNullOrEmpty(this.username) && !string.IsNullOrEmpty(password))
            {
                client.Credentials = new NetworkCredential(this.username, this.password);
            }
        }

        private long GetHead()
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.Method = "HEAD";
            var resp = (HttpWebResponse)req.GetResponse();
            return resp.ContentLength;
        }

        public string GetXml()
        {
            //GetHead();
            var reader = this.Get();
            var s = reader.ReadToEnd();
            hasChanged = false;
            if (this.currentConfig != s)
            {
                hasChanged = true;
                currentConfig = s;
            }

            reader.Close();
            return s;
        }

        private bool hasChanged;

        public bool HasChanged()
        {
            return hasChanged;
        }
    }
}