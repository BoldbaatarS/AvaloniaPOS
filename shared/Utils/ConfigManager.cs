using System;
using System.IO;
using System.Text.Json;

namespace RestaurantPOS.Utils
{
    public class AppConfig
    {
        public Guid? DefaultHallId { get; set; }
    }

    public static class ConfigManager
    {
        private static readonly string ConfigPath = 
            Path.Combine(AppContext.BaseDirectory, "config.json");

        public static AppConfig Load()
        {
            if (!File.Exists(ConfigPath))
                return new AppConfig();

            var json = File.ReadAllText(ConfigPath);
            return JsonSerializer.Deserialize<AppConfig>(json) ?? new AppConfig();
        }

        public static void Save(AppConfig config)
        {
            var json = JsonSerializer.Serialize(config, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(ConfigPath, json);
        }
    }
}
