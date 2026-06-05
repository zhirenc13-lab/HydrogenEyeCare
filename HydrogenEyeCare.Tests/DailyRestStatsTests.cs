namespace HydrogenEyeCare.Tests;

public sealed class DailyRestStatsTests
{
    [Fact]
    public void StartsAtZero()
    {
        using var tempDir = new TempDirectory();
        var today = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => today, Path.Combine(tempDir.Path, "daily-stats.json"));

        Assert.Equal(0, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void IncrementSuccessfulRestIncreasesTodayCount()
    {
        using var tempDir = new TempDirectory();
        var today = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => today, Path.Combine(tempDir.Path, "daily-stats.json"));

        stats.IncrementSuccessfulRest();
        stats.IncrementSuccessfulRest();

        Assert.Equal(2, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void ReadingCountResetsAfterMidnight()
    {
        using var tempDir = new TempDirectory();
        var currentDate = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => currentDate, Path.Combine(tempDir.Path, "daily-stats.json"));
        stats.IncrementSuccessfulRest();

        currentDate = new DateOnly(2026, 5, 29);

        Assert.Equal(0, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void IncrementAfterMidnightStartsNewDayAtOne()
    {
        using var tempDir = new TempDirectory();
        var currentDate = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => currentDate, Path.Combine(tempDir.Path, "daily-stats.json"));
        stats.IncrementSuccessfulRest();

        currentDate = new DateOnly(2026, 5, 29);
        stats.IncrementSuccessfulRest();

        Assert.Equal(1, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void LoadsSameDayCountFromStatsFile()
    {
        using var tempDir = new TempDirectory();
        var statsPath = Path.Combine(tempDir.Path, "daily-stats.json");
        File.WriteAllText(statsPath, """{"Date":"2026-06-05","SuccessfulRestsToday":3}""");

        var stats = new DailyRestStats(() => new DateOnly(2026, 6, 5), statsPath);

        Assert.Equal(3, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void OldStatsFileResetsForNewDayAndPersistsReset()
    {
        using var tempDir = new TempDirectory();
        var statsPath = Path.Combine(tempDir.Path, "daily-stats.json");
        File.WriteAllText(statsPath, """{"Date":"2026-06-04","SuccessfulRestsToday":9}""");

        var stats = new DailyRestStats(() => new DateOnly(2026, 6, 5), statsPath);

        Assert.Equal(0, stats.SuccessfulRestsToday);
        Assert.Contains("2026-06-05", File.ReadAllText(statsPath));
    }

    [Fact]
    public void CountsEachRestSessionOnlyOnce()
    {
        using var tempDir = new TempDirectory();
        var statsPath = Path.Combine(tempDir.Path, "daily-stats.json");
        var stats = new DailyRestStats(() => new DateOnly(2026, 6, 5), statsPath);

        stats.MarkRestSessionCompleted(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        stats.MarkRestSessionCompleted(Guid.Parse("aaaaaaaa-aaaa-aaaa-aaaa-aaaaaaaaaaaa"));
        stats.MarkRestSessionCompleted(Guid.Parse("bbbbbbbb-bbbb-bbbb-bbbb-bbbbbbbbbbbb"));

        Assert.Equal(2, stats.SuccessfulRestsToday);
    }

    private sealed class TempDirectory : IDisposable
    {
        public TempDirectory()
        {
            Path = System.IO.Path.Combine(
                System.IO.Path.GetTempPath(),
                $"{nameof(HydrogenEyeCare)}.{nameof(DailyRestStatsTests)}.{Guid.NewGuid():N}");
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
