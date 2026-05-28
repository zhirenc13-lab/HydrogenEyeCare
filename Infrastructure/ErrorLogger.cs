namespace HydrogenEyeCare;

public sealed class ErrorLogger
{
    private const long MaxLogSizeBytes = 5L * 1024L * 1024L;
    private readonly string _logPath;

    public ErrorLogger()
        : this(AppPaths.ErrorLogPath)
    {
    }

    public ErrorLogger(string logPath)
    {
        _logPath = logPath;
    }

    public void Log(Exception exception)
    {
        try
        {
            EnsureLogDirectory();
            RotateIfNeeded();
            var entry = $"[{DateTimeOffset.Now:O}]{Environment.NewLine}{exception}{Environment.NewLine}{Environment.NewLine}";
            File.AppendAllText(_logPath, entry);
        }
        catch
        {
            // Logging must never crash the reminder app.
        }
    }

    private void EnsureLogDirectory()
    {
        var directory = Path.GetDirectoryName(_logPath);
        if (!string.IsNullOrWhiteSpace(directory))
        {
            Directory.CreateDirectory(directory);
        }
    }

    private void RotateIfNeeded()
    {
        var fileInfo = new FileInfo(_logPath);
        if (!fileInfo.Exists || fileInfo.Length <= MaxLogSizeBytes)
        {
            return;
        }

        File.WriteAllText(_logPath, string.Empty);
    }
}
