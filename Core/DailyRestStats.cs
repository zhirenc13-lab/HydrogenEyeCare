namespace HydrogenEyeCare;

public sealed class DailyRestStats
{
    private readonly Func<DateOnly> _currentDateProvider;
    private DateOnly _currentDate;
    private int _successfulRestsToday;

    public DailyRestStats()
        : this(() => DateOnly.FromDateTime(DateTime.Now))
    {
    }

    public DailyRestStats(Func<DateOnly> currentDateProvider)
    {
        _currentDateProvider = currentDateProvider;
        _currentDate = _currentDateProvider();
    }

    public int SuccessfulRestsToday
    {
        get
        {
            ResetIfDateChanged();
            return _successfulRestsToday;
        }
    }

    public void IncrementSuccessfulRest()
    {
        ResetIfDateChanged();
        _successfulRestsToday++;
    }

    private void ResetIfDateChanged()
    {
        var today = _currentDateProvider();
        if (today == _currentDate)
        {
            return;
        }

        _currentDate = today;
        _successfulRestsToday = 0;
    }
}
