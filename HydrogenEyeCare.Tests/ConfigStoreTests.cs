using System.Text.Json;

namespace HydrogenEyeCare;

public sealed class ConfigStoreTests
{
    [Fact]
    public void LoadReturnsForestGreenWhenConfigFileContainsInvalidTheme()
    {
        using var tempDir = new TempDirectory();
        var configFilePath = Path.Combine(tempDir.Path, "config.json");
        File.WriteAllText(configFilePath, """{"Theme":999}""");

        var store = new ConfigStore(configFilePath, _ => { });

        var config = store.Load();

        Assert.Equal(AppTheme.ForestGreen, config.Theme);
    }

    [Fact]
    public void SaveWritesNormalizedForestGreenWhenConfigThemeIsInvalid()
    {
        using var tempDir = new TempDirectory();
        var configFilePath = Path.Combine(tempDir.Path, "config.json");
        var store = new ConfigStore(configFilePath, path => Directory.CreateDirectory(path));
        var config = new AppConfig
        {
            Theme = (AppTheme)999
        };

        store.Save(config);

        var savedConfig = JsonSerializer.Deserialize<AppConfig>(File.ReadAllText(configFilePath));
        Assert.NotNull(savedConfig);
        Assert.Equal(AppTheme.ForestGreen, savedConfig.Theme);
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"{nameof(HydrogenEyeCare)}.{nameof(ConfigStoreTests)}.{Guid.NewGuid():N}");
            Directory.CreateDirectory(Path);
        }

        public string Path { get; }

        public void Dispose()
        {
            if (Directory.Exists(Path))
            {
                Directory.Delete(Path, recursive: true);
            }
        }
    }
}
