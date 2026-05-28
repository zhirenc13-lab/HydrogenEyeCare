namespace HydrogenEyeCare;

public sealed class AppConfig
{
    public bool Muted { get; set; }

    public bool StartWithWindows { get; set; }

    public AppTheme Theme { get; set; } = AppTheme.ForestGreen;
}
