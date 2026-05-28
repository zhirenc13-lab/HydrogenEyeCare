namespace HydrogenEyeCare;

public sealed class AppConfig
{
    public bool Muted { get; set; }

    public bool StartWithWindows { get; set; }

    public AppTheme Theme { get; set; } = AppTheme.ForestGreen;

    public void Normalize()
    {
        if (!Enum.IsDefined(Theme))
        {
            Theme = AppTheme.ForestGreen;
        }
    }
}
