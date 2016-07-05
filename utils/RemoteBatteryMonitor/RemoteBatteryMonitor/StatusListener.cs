using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Net.Sockets;
using System.Net;
namespace RemoteBatteryMonitor
{
    class StatusListener
    {
        private int localPort;

        TcpClient client;

        public StatusListener(int localPort)
        {
            this.localPort = localPort;
            var localEp = new IPEndPoint(IPAddress.Any, localPort);
            client = new TcpClient(localEp);
        }

        public void ListenForStatusMessages()
        {
            while (true) { 

                NetworkStream stream = client.GetStream();

                // Receive the TcpServer.response.

                // Buffer to store the response bytes.
                var data = new Byte[256];

                // String to store the response ASCII representation.
                String responseData = String.Empty;

                // Read the first batch of the TcpServer response bytes.
                Int32 bytes = stream.Read(data, 0, data.Length);
                responseData = System.Text.Encoding.ASCII.GetString(data, 0, bytes);
                Console.WriteLine("Received: {0}", responseData);
            }
        }
    }
}
