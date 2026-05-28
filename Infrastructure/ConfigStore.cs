using System.Text.Json;

namespace HydrogenEyeCare;

public sealed class ConfigStore
{
    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true
    };

    public AppConfig Load()
    {
        try
        {
            AppPaths.EnsureAppDataDirectory();
            if (!File.Exists(AppPaths.ConfigFilePath))
            {
                return CreateDefaultConfig();
            }

            var json = File.ReadAllText(AppPaths.ConfigFilePath);
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
        AppPaths.EnsureAppDataDirectory();
        config.Normalize();
        var json = JsonSerializer.Serialize(config, JsonOptions);
        File.WriteAllText(AppPaths.ConfigFilePath, json);
    }

    private static AppConfig CreateDefaultConfig()
    {
        var config = new AppConfig();
        config.Normalize();
        return config;
    }
}
