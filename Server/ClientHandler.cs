using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SeaBattle.Protocol;

namespace Server
{
    public class ClientHandler
    {
        private readonly TcpClient _tcpClient;
        private readonly NetworkStream _stream;
        private readonly StreamReader _reader;
        private readonly StreamWriter _writer;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private bool _isDisconnected = false;

        public string Id { get; }
        public string Name { get; set; }
        public string? RoomId { get; set; }
        public bool IsInLobby { get; set; } = false;

        public event Action<ClientHandler, NetworkMessage>? MessageReceived;
        public event Action<ClientHandler>? Disconnected;

        public ClientHandler(TcpClient tcpClient)
        {
            _tcpClient = tcpClient;
            _stream = tcpClient.GetStream();
            _reader = new StreamReader(_stream, Encoding.UTF8);
            _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

            Id = Guid.NewGuid().ToString("N")[..8];
            Name = $"Гравець_{Id}";
        }

        public void Start()
        {
            Task.Run(ReceiveLoopAsync);
        }

        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (!_isDisconnected)
                {
                    string? line = await _reader.ReadLineAsync();
                    if (line == null)
                    {
                        break;
                    }

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var message = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (message != null)
                    {
                        MessageReceived?.Invoke(this, message);
                    }
                }
            }
            catch (Exception)
            {
                // Disconnected or connection aborted
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task SendAsync(string type, object? payloadObj = null)
        {
            if (_isDisconnected) return;

            try
            {
                string payloadJson = payloadObj != null ? JsonSerializer.Serialize(payloadObj) : "";
                var message = new NetworkMessage(type, payloadJson);
                string jsonLine = JsonSerializer.Serialize(message);

                await _sendLock.WaitAsync();
                try
                {
                    await _writer.WriteLineAsync(jsonLine);
                }
                finally
                {
                    _sendLock.Release();
                }
            }
            catch (Exception)
            {
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (_isDisconnected) return;
            _isDisconnected = true;

            try
            {
                _reader.Dispose();
                _writer.Dispose();
                _stream.Dispose();
                _tcpClient.Close();
            }
            catch { }

            Disconnected?.Invoke(this);
        }
    }
}
