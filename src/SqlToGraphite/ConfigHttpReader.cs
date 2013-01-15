using System.IO;
using System.Net;

namespace SqlToGraphite
{
    using System.Security.Cryptography;
    using System.Text;

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
            hash = this.CalculateMD5Hash(s);
            return s;
        }

        private string hash;

        public string GetHash()
        {
            return hash;
        }

        private string CalculateMD5Hash(string input)
        {
            var md5 = MD5.Create();
            var inputBytes = Encoding.ASCII.GetBytes(input);
            var hash = md5.ComputeHash(inputBytes);
            var sb = new StringBuilder();
            for (int i = 0; i < hash.Length; i++)
            {
                sb.Append(hash[i].ToString("X2"));
            }

            return sb.ToString();
        }

        private bool hasChanged;

        public bool HasChanged()
        {
            return hasChanged;
        }
    }
}