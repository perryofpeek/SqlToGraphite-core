using System;
using System.Net.Sockets;

namespace Graphite
{
    public class GraphiteTcpClient : IGraphiteClient, IDisposable
    {
        public string Hostname { get; private set; }
        public int Port { get; private set; }
        public string KeyPrefix { get; private set; }

        private readonly TcpClient _tcpClient;

        public GraphiteTcpClient(string hostname, int port = 2003, string keyPrefix = null)
        {
            Hostname = hostname;
            Port = port;
            KeyPrefix = keyPrefix;

            _tcpClient = new TcpClient(Hostname, Port);
        }

        public void Send(string path, string value, DateTime timeStamp)
        {
            this.WriteToStream(new PlaintextMessage(SetKeyPrefix(path), value, timeStamp).ToByteArray());
        }

        public void Send(string path, int value, DateTime timeStamp)
        {
            this.WriteToStream(new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray());
        }

        private void WriteToStream(byte[] message)
        {
            this._tcpClient.GetStream().Write(message, 0, message.Length);
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
            this.WriteToStream(new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray());
        }

        public void Send(string path, decimal value, DateTime timeStamp)
        {
            this.WriteToStream(new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray());
        }

        public void Send(string path, float value, DateTime timeStamp)
        {
            this.WriteToStream(new PlaintextMessage(SetKeyPrefix(path), value.ToString(), timeStamp).ToByteArray());
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

            if (_tcpClient != null)
            {
                _tcpClient.Close();
            }
        }

        #endregion
    }
}