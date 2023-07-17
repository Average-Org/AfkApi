using System.Text.Json;
using System.Text.Json.Serialization;

namespace AfkApi
{
    public class Settings
    {
        [JsonPropertyName("KickForAFK")]
        public bool KickForAFK { get; set; } = false;

        [JsonPropertyName("KickThreshold")]
        public int KickThreshold { get; set; } = 1000;
        public static Settings Write(string path)
        {
            var settings = new Settings();
            File.WriteAllText(path, JsonSerializer.Serialize(settings, new JsonSerializerOptions { WriteIndented = true }));
            return settings;
        }
        public static Settings Read(string path)
        {
            var settings = new Settings();
            if (!File.Exists(path))
            {
                return Write(path);
            }
            else
            {
                return JsonSerializer.Deserialize<Settings>(File.ReadAllText(path));
            }
        }
    }
}
