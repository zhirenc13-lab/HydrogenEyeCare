namespace HydrogenEyeCare.Tests;

public sealed class ErrorLoggerTests : IDisposable
{
    private readonly string _directory;
    private readonly string _logPath;

    public ErrorLoggerTests()
    {
        _directory = Path.Combine(Path.GetTempPath(), "HydrogenEyeCareTests", Guid.NewGuid().ToString("N"));
        Directory.CreateDirectory(_directory);
        _logPath = Path.Combine(_directory, "error.log");
    }

    [Fact]
    public void LogClearsExistingFileWhenItExceedsFiveMb()
    {
        File.WriteAllBytes(_logPath, new byte[(5 * 1024 * 1024) + 1]);
        var logger = new ErrorLogger(_logPath);

        logger.Log(new InvalidOperationException("rotation marker"));

        var text = File.ReadAllText(_logPath);
        Assert.Contains("rotation marker", text);
        Assert.True(new FileInfo(_logPath).Length < 1024 * 1024);
    }

    [Fact]
    public void LogAppendsWhenFileIsBelowLimit()
    {
        File.WriteAllText(_logPath, "existing");
        var logger = new ErrorLogger(_logPath);

        logger.Log(new InvalidOperationException("append marker"));

        var text = File.ReadAllText(_logPath);
        Assert.Contains("existing", text);
        Assert.Contains("append marker", text);
    }

    public void Dispose()
    {
        if (Directory.Exists(_directory))
        {
            Directory.Delete(_directory, recursive: true);
        }
    }
}
