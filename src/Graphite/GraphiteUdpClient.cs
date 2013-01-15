using System;
using System.Net.Sockets;

namespace Graphite
{
    using System.Text;

    public class GraphiteUdpClient : IGraphiteClient, IDisposable
    {
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string KeyPrefix { get; private set; }

        private readonly UdpClient _udpClient;

        public GraphiteUdpClient(string hostname, int port = 2003, string keyPrefix = null)
        {
            Hostname = hostname;
            Port = port;
            KeyPrefix = keyPrefix;

            _udpClient = new UdpClient(Hostname, Port);
        }

        public void Send(string path, string value, DateTime timeStamp)
        {
            var message = new PlaintextMessage(SetKeyPrefix(path), value, timeStamp).ToByteArray();
            _udpClient.Send(message, message.Length);
        }

        public void Send(string path, int value, DateTime timeStamp)
        {
            var message = new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray();
            _udpClient.Send(message, message.Length);
        }

        public void SendList()
        {
            var line1 = makeLine("Test.p1", 10, DateTime.Now);

            var line2 = makeLine("Test.p2", 20, DateTime.Now);
            var line3 = makeLine("Test.p3", 30, DateTime.Now);
            var msg = string.Format("{0} {1} {2}", line1, line2, line3);
            var v = Encoding.UTF8.GetBytes(msg);
            
            _udpClient.Send(v, v.Length);
        }

        private static string makeLine(string path1, int value1, DateTime timeStamp1)
        {
            var line1 = string.Format("{0} {1} {2}\n", path1, value1, timeStamp1.ToUnixTime());
            return line1;
        }

        private string SetKeyPrefix(string path)
        {
            if (!string.IsNullOrWhiteSpace(this.KeyPrefix))
            {
                path = this.KeyPrefix + "." + path;
            }
            return path;
        }

        public void Send(string path, long value, DateTime timeStamp)
        {
            var message = new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray();
            _udpClient.Send(message, message.Length);
        }

        public void Send(string path, decimal value, DateTime timeStamp)
        {
            var message = new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray();
            _udpClient.Send(message, message.Length);
        }

        public void Send(string path, float value, DateTime timeStamp)
        {
            var message = new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray();
            _udpClient.Send(message, message.Length);
        }

        #region IDisposable

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposing) return;

            if (_udpClient != null)
            {
                _udpClient.Close();
            }
        }

        #endregion
    }
}