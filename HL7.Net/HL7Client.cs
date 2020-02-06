using System;
using System.Net.Sockets;

namespace HL7.Net
{
    public class HL7Client : IDisposable
    {
        TcpClient m_Client;

        public void Connect(string address, int port)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (port <= 0 || port >= 0x10000)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (m_Client != null)
            {
                throw new InvalidOperationException("Already connected");
            }

            Address = address;
            Port = port;
            m_Client = new TcpClient(address, port);
        }

        public bool IsConnected => m_Client != null;

        public void Close()
        {
            if (m_Client == null)
            {
                return;
            }

            m_Client.Close();
            m_Client = null;
        }

        public void Dispose()
        {
            Close();
        }

        public string Address { get; private set; }

        public int Port { get; private set; }

        public string SendMessage(string message)
        {
            var stream = m_Client.GetStream();
            Llp.WriteMessage(stream, message);
            var ackMessage = Llp.ReadMessage(stream);
            return ackMessage;
        }
    }
}
