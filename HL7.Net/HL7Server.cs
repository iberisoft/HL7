using System;
using System.Net;
using System.Net.Sockets;
using System.Threading.Tasks;

namespace HL7.Net
{
    public class HL7Server : IDisposable
    {
        TcpListener m_Listener;

        public void Listen(string address, int port, int maxConnections)
        {
            if (address == null)
            {
                throw new ArgumentNullException(nameof(address));
            }
            if (port <= 0 || port >= 0x10000)
            {
                throw new ArgumentOutOfRangeException(nameof(port));
            }
            if (maxConnections < 1)
            {
                throw new ArgumentException("Less than 1", nameof(maxConnections));
            }
            if (m_Listener != null)
            {
                throw new InvalidOperationException("Already listening");
            }

            Address = address;
            Port = port;
            m_Listener = new TcpListener(address != "" ? IPAddress.Parse(address) : IPAddress.Any, port);
            m_Listener.Start();

            for (var i = 0; i < maxConnections; ++i)
            {
                Task.Factory.StartNew(ListenConnection);
            }
        }

        public void Close()
        {
            if (m_Listener == null)
            {
                return;
            }

            m_Listener.Stop();
            m_Listener = null;
        }

        public void Dispose()
        {
            Close();
        }

        public string Address { get; private set; }

        public int Port { get; private set; }

        private void ListenConnection()
        {
            while (true)
            {
                TcpClient client;
                try
                {
                    client = m_Listener.AcceptTcpClient();
                }
                catch
                {
                    break;
                }
                try
                {
                    ProcessConnection(client);
                }
                catch (Exception ex)
                {
                    Console.WriteLine(ex);
                }
                finally
                {
                    client.Close();
                }
            }
        }

        private void ProcessConnection(TcpClient client)
        {
            using (var stream = client.GetStream())
            {
                while (true)
                {
                    var message = Llp.ReadMessage(stream);
                    if (message == null)
                    {
                        break;
                    }
                    var e = new HL7MessageEventArgs();
                    e.Message = message;
                    MessageReceived?.Invoke(this, e);
                    if (e.AckMessage != null)
                    {
                        Llp.WriteMessage(stream, e.AckMessage);
                    }
                }
            }
        }

        public event EventHandler<HL7MessageEventArgs> MessageReceived;
    }
}
