using System.Text.Json;

namespace HydrogenEyeCare;

public sealed class ConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configFilePath;
    public ConfigStore()
        : this(AppPaths.ConfigFilePath)
    {
    }

    public ConfigStore(string configFilePath)
    {
        _configFilePath = configFilePath;
    }

    public AppConfig Load()
    {
        try
        {
            EnsureConfigDirectory();
            if (!File.Exists(_configFilePath))
            {
                return CreateDefaultConfig();
            }

            var json = File.ReadAllText(_configFilePath);
            var config = JsonSerializer.Deserialize<AppConfig>(json) ?? CreateDefaultConfig();
            config.Normalize();
            return config;
        }
        catch
        {
            return CreateDefaultConfig();
        }
    }

    public void Save(AppConfig config)
    {
        EnsureConfigDirectory();
        config.Normalize();
        var json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(_configFilePath, json);
    }

    private void EnsureConfigDirectory()
    {
        var directoryPath = Path.GetDirectoryName(_configFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }
    }

    private static AppConfig CreateDefaultConfig()
    {
        var config = new AppConfig();
        config.Normalize();
        return config;
    }
}
