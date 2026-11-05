using System.Net.Sockets;
using System.Text;
using System.Text.Json;
using SeaBattle.Protocol;

namespace SeaBattleNet
{
    public class NetworkClient
    {
        private TcpClient? _tcpClient;
        private NetworkStream? _stream;
        private StreamReader? _reader;
        private StreamWriter? _writer;
        private readonly SemaphoreSlim _sendLock = new(1, 1);
        private bool _isConnected = false;

        public bool IsConnected => _isConnected;

        public event Action<NetworkMessage>? MessageReceived;
        public event Action? Connected;
        public event Action? Disconnected;
        public event Action<string>? ErrorOccurred;

        public async Task<bool> ConnectAsync(string ipAddress, int port)
        {
            try
            {
                Disconnect();

                _tcpClient = new TcpClient();
                await _tcpClient.ConnectAsync(ipAddress, port);
                _stream = _tcpClient.GetStream();
                _reader = new StreamReader(_stream, Encoding.UTF8);
                _writer = new StreamWriter(_stream, Encoding.UTF8) { AutoFlush = true };

                _isConnected = true;
                Connected?.Invoke();

                _ = ReceiveLoopAsync();
                return true;
            }
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Помилка підключення: {ex.Message}");
                Disconnect();
                return false;
            }
        }

        private async Task ReceiveLoopAsync()
        {
            try
            {
                while (_isConnected && _reader != null)
                {
                    string? line = await _reader.ReadLineAsync();
                    if (line == null) break;

                    if (string.IsNullOrWhiteSpace(line)) continue;

                    var message = JsonSerializer.Deserialize<NetworkMessage>(line);
                    if (message != null)
                    {
                        MessageReceived?.Invoke(message);
                    }
                }
            }
            catch (Exception)
            {
                // Disconnected
            }
            finally
            {
                Disconnect();
            }
        }

        public async Task SendAsync(string type, object? payloadObj = null)
        {
            if (!_isConnected || _writer == null) return;

            try
            {
                string payloadJson = payloadObj != null 
                    ? (payloadObj is string s ? s : JsonSerializer.Serialize(payloadObj))
                    : "";
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
            catch (Exception ex)
            {
                ErrorOccurred?.Invoke($"Помилка надсилання: {ex.Message}");
                Disconnect();
            }
        }

        public void Disconnect()
        {
            if (!_isConnected) return;
            _isConnected = false;

            try
            {
                _reader?.Dispose();
                _writer?.Dispose();
                _stream?.Dispose();
                _tcpClient?.Close();
            }
            catch { }

            Disconnected?.Invoke();
        }
    }
}
