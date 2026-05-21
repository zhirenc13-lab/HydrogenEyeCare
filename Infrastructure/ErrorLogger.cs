namespace HydrogenEyeCare;

public sealed class ErrorLogger
{
    public void Log(Exception exception)
    {
        try
        {
            AppPaths.EnsureAppDataDirectory();
            var entry = $"[{DateTimeOffset.Now:O}]{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
            File.AppendAllText(AppPaths.ErrorLogPath, entry);
        }
        catch
        {
            // Logging must never crash the reminder app.
        }
    }
}
