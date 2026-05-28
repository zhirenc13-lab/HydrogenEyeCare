namespace HydrogenEyeCare.Tests;

public sealed class DailyRestStatsTests
{
    [Fact]
    public void StartsAtZero()
    {
        var today = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => today);

        Assert.Equal(0, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void IncrementSuccessfulRestIncreasesTodayCount()
    {
        var today = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => today);

        stats.IncrementSuccessfulRest();
        stats.IncrementSuccessfulRest();

        Assert.Equal(2, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void ReadingCountResetsAfterMidnight()
    {
        var currentDate = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => currentDate);
        stats.IncrementSuccessfulRest();

        currentDate = new DateOnly(2026, 5, 29);

        Assert.Equal(0, stats.SuccessfulRestsToday);
    }

    [Fact]
    public void IncrementAfterMidnightStartsNewDayAtOne()
    {
        var currentDate = new DateOnly(2026, 5, 28);
        var stats = new DailyRestStats(() => currentDate);
        stats.IncrementSuccessfulRest();

        currentDate = new DateOnly(2026, 5, 29);
        stats.IncrementSuccessfulRest();

        Assert.Equal(1, stats.SuccessfulRestsToday);
    }
}
