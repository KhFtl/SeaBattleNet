using System.Text.Json;

namespace Server
{
    public class ServerConfig
    {
        public string IpAddress { get; set; } = "127.0.0.1";
        public int Port { get; set; } = 8888;

        public static ServerConfig Load(string path = "settings.json")
        {
            try
            {
                if (File.Exists(path))
                {
                    string json = File.ReadAllText(path);
                    var config = JsonSerializer.Deserialize<ServerConfig>(json, new JsonSerializerOptions
                    {
                        PropertyNameCaseInsensitive = true
                    });
                    if (config != null)
                        return config;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [ERROR] Помилка завантаження settings.json: {ex.Message}");
            }

            Console.WriteLine($"[{DateTime.Now:HH:mm:ss}] [WARN] Використовуються налаштування за замовчуванням (127.0.0.1:8888)");
            return new ServerConfig();
        }
    }
}
