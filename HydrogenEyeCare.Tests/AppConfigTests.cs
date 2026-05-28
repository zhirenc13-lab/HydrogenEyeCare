using System.Text.Json;

namespace HydrogenEyeCare.Tests;

public sealed class AppConfigTests
{
    [Fact]
    public void DefaultThemeIsForestGreen()
    {
        var config = new AppConfig();

        Assert.Equal(AppTheme.ForestGreen, config.Theme);
    }

    [Fact]
    public void MissingThemeInExistingConfigFallsBackToForestGreen()
    {
        var config = JsonSerializer.Deserialize<AppConfig>("""{"Muted":true,"StartWithWindows":false}""");

        Assert.NotNull(config);
        Assert.Equal(AppTheme.ForestGreen, config.Theme);
    }
}
