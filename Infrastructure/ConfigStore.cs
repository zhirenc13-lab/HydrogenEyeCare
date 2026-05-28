using System.Text.Json;

namespace HydrogenEyeCare;

public sealed class ConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    private readonly string _configFilePath;
    private readonly Action<string> _ensureDirectory;

    public ConfigStore()
        : this(AppPaths.ConfigFilePath, path => Directory.CreateDirectory(path))
    {
    }

    public ConfigStore(string configFilePath, Action<string> ensureDirectory)
    {
        _configFilePath = configFilePath;
        _ensureDirectory = ensureDirectory;
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
            _ensureDirectory(directoryPath);
        }
    }

    private static AppConfig CreateDefaultConfig()
    {
        var config = new AppConfig();
        config.Normalize();
        return config;
    }
}
