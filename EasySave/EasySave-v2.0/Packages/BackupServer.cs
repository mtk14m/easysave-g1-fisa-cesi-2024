using System;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace EasySave_v2._0.Packages
{
    internal class BackupServer
    {
        private const int Port = 8888;
        private static readonly ManualResetEvent ShutdownEvent = new ManualResetEvent(false);

        public void Start()
        {
            string hostName = Dns.GetHostName();
            IPAddress[] hostAddresses = Dns.GetHostAddresses(hostName);
            Socket listener = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
            IPAddress ipAddress = hostAddresses[0];

            try
            {
                
                listener.Bind(new IPEndPoint(ipAddress, Port));
                listener.Listen(10);

                while (!ShutdownEvent.WaitOne(0))
                {
                    Console.WriteLine("En attente de la connexion d'un client...");
                    Socket handler = listener.Accept();
                    Console.WriteLine($"Client connecté : {handler.RemoteEndPoint}");

                    
                    Thread clientThread = new Thread(() => HandleClient(handler));
                    clientThread.Start();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors du démarrage du serveur : {ex.Message}");
            }
            finally
            {
                listener.Close();
            }
        }

        private void HandleClient(Socket clientSocket)
        {
            try
            {
                // Boucle de communication avec le client
                while (true)
                {
                    byte[] buffer = new byte[1024];
                    int bytesRead = clientSocket.Receive(buffer);
                    string clientMessage = Encoding.UTF8.GetString(buffer, 0, bytesRead);

                    // Traiter la commande du client et exécuter le backup en conséquence
                    // Exemple : si clientMessage contient "start_backup", exécutez le backup
                    // Vous devez implémenter cette logique selon vos besoins

                    string response = "Backup exécuté avec succès";
                    clientSocket.Send(Encoding.UTF8.GetBytes(response));
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Erreur lors de la communication avec le client : {ex.Message}");
            }
            finally
            {
                clientSocket.Shutdown(SocketShutdown.Both);
                clientSocket.Close();
            }
        }

        public void Stop()
        {
            ShutdownEvent.Set();
        }
    }
}
