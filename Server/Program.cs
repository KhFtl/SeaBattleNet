using System.Net;
using System.Net.Sockets;
using System.Text;
using SeaBattle.Protocol;
using Server.GameLogic;

namespace Server
{
    internal class Program
    {
        private static TcpListener? _listener;
        private static readonly Lobby _lobby = new();
        private static bool _isRunning = true;

        static async Task Main(string[] args)
        {
            Console.OutputEncoding = Encoding.UTF8;
            Console.Title = "SeaBattle Server";

            Console.WriteLine("==================================================");
            Console.WriteLine("   СЕРВЕР МЕРЕЖЕВОЇ ГРИ «МОРСЬКИЙ БІЙ»           ");
            Console.WriteLine("==================================================");

            var config = ServerConfig.Load();
            var ip = IPAddress.Parse(config.IpAddress);
            int port = config.Port;

            try
            {
                _listener = new TcpListener(ip, port);
                _listener.Start();
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [SERVER] Сервер успішно запущено на {config.IpAddress}:{port}");
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [SERVER] Очікування підключення гравців...\n");

                _ = AcceptClientsAsync();

                Console.WriteLine("Натисніть Enter або 'q' для зупинки сервера.\n");
                var key = Console.ReadLine();
                _isRunning = false;
                _listener.Stop();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [FATAL] Помилка запуску сервера: {ex.Message}");
            }
        }

        private static async Task AcceptClientsAsync()
        {
            while (_isRunning && _listener != null)
            {
                try
                {
                    var tcpClient = await _listener.AcceptTcpClientAsync();
                    var clientHandler = new ClientHandler(tcpClient);

                    Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [CONNECT] Нове підключення з {tcpClient.Client.RemoteEndPoint}. Призначено ID: {clientHandler.Id}");

                    clientHandler.MessageReceived += OnInitialClientMessage;
                    clientHandler.Disconnected += OnClientDisconnected;
                    clientHandler.Start();
                }
                catch (ObjectDisposedException)
                {
                    break;
                }
                catch (Exception ex)
                {
                    if (_isRunning)
                    {
                        Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] Помилка прийняття клієнта: {ex.Message}");
                    }
                }
            }
        }

        private static void OnInitialClientMessage(ClientHandler client, NetworkMessage message)
        {
            if (message.Type == MessageTypes.JoinLobby)
            {
                if (!string.IsNullOrWhiteSpace(message.Payload))
                {
                    client.Name = message.Payload.Trim();
                }

                client.MessageReceived -= OnInitialClientMessage;
                _lobby.AddPlayer(client);
            }
        }

        private static void OnClientDisconnected(ClientHandler client)
        {
            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [DISCONNECT] Клієнт {client.Name} ({client.Id}) відключився.");
        }
    }
}
