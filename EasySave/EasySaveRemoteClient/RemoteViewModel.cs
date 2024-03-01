using System;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace EasySaveRemoteClient
{
    class RemoteViewModel
    {
        private TcpClient client;
        private string serverIp; // Adresse IP du serveur
        private int port; // Port sur lequel le serveur écoute

        public RemoteViewModel(string serverIp, int port)
        {
            this.serverIp = serverIp;
            this.port = port;
        }

        public async Task ConnectToServerAsync()
        {
            try
            {
                client = new TcpClient();
                await client.ConnectAsync(serverIp, port);
                Console.WriteLine("Connected to server.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error connecting to server: {ex.Message}");
            }
        }

        public async Task SendDataAsync(string data)
        {
            try
            {
                if (client != null && client.Connected)
                {
                    byte[] buffer = Encoding.UTF8.GetBytes(data);
                    await client.GetStream().WriteAsync(buffer, 0, buffer.Length);
                    Console.WriteLine("Data sent to server.");
                }
                else
                {
                    Console.WriteLine("Client is not connected.");
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error sending data to server: {ex.Message}");
            }
        }
    }
}
