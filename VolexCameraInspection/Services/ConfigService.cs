using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using VolexCameraInspection.Models;

namespace VolexCameraInspection.Services
{
    public class ConfigService
    {
        private const string ConfigFile = "appsettings.json";

        public ConfigModel Config { get; private set; } = new ConfigModel(string.Empty, string.Empty, string.Empty,string.Empty);

        public void Load()
        {
            string path = Path.Combine(ConfigFile);
            if (!File.Exists(path))
                using (_ = File.Create(path)) { }

            string text = File.ReadAllText(path);
            if (string.IsNullOrEmpty(text))
                Save();
            ConfigModel? _config = JsonSerializer.Deserialize<ConfigModel>(text);
            if (_config is not null)
            {
                if (string.IsNullOrEmpty(_config.dbpath))
                    Config = new ConfigModel("Host=localhost;Username=autoscrewing_usr;Password=autoscrewing_usr;Database=camerainspection", "192.168.10.1","8080","ftp\\");
                else
                    Config = _config;
            }
        }

        public void Save()
        {

            string path = Path.Combine(ConfigFile);
            string json = JsonSerializer.Serialize(Config, new JsonSerializerOptions() { WriteIndented = true });
            File.WriteAllText(path, json);
        }
    }
}
