using System;
using System.Net.Sockets;

namespace Graphite
{
    using System.Collections.Generic;
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

        public void Send(GraphiteMetrics graphiteMetrics)
        {
            var msg = graphiteMetrics.ToGraphiteMessageList();
            var v = Encoding.UTF8.GetBytes(msg);
            _udpClient.Send(v, v.Length);
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