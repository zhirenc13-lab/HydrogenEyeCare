namespace HydrogenEyeCare;

public sealed class DailyRestStats
{
    private readonly Func<DateOnly> _currentDateProvider;
    private readonly string _statsFilePath;
    private readonly HashSet<Guid> _completedSessionIds = [];
    private DateOnly _currentDate;
    private int _successfulRestsToday;

    public DailyRestStats()
        : this(() => DateOnly.FromDateTime(DateTime.Now), AppPaths.DailyStatsFilePath)
    {
    }

    public DailyRestStats(Func<DateOnly> currentDateProvider)
        : this(currentDateProvider, AppPaths.DailyStatsFilePath)
    {
    }

    public DailyRestStats(Func<DateOnly> currentDateProvider, string statsFilePath)
    {
        _currentDateProvider = currentDateProvider;
        _statsFilePath = statsFilePath;
        LoadOrReset();
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
        MarkRestSessionCompleted(Guid.NewGuid());
    }

    public bool MarkRestSessionCompleted(Guid sessionId)
    {
        ResetIfDateChanged();
        if (!_completedSessionIds.Add(sessionId))
        {
            return false;
        }

        _successfulRestsToday++;
        Save();
        return true;
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
        _completedSessionIds.Clear();
        Save();
    }

    private void LoadOrReset()
    {
        _currentDate = _currentDateProvider();

        try
        {
            if (File.Exists(_statsFilePath))
            {
                var json = File.ReadAllText(_statsFilePath);
                var savedStats = System.Text.Json.JsonSerializer.Deserialize<SavedDailyRestStats>(json);
                if (savedStats?.Date == _currentDate)
                {
                    _successfulRestsToday = Math.Max(0, savedStats.SuccessfulRestsToday);
                    _completedSessionIds.Clear();
                    foreach (var sessionId in savedStats.CompletedSessionIds ?? [])
                    {
                        _completedSessionIds.Add(sessionId);
                    }

                    return;
                }
            }
        }
        catch
        {
            _successfulRestsToday = 0;
            _completedSessionIds.Clear();
            Save();
            return;
        }

        _successfulRestsToday = 0;
        _completedSessionIds.Clear();
        Save();
    }

    private void Save()
    {
        var directoryPath = Path.GetDirectoryName(_statsFilePath);
        if (!string.IsNullOrWhiteSpace(directoryPath))
        {
            Directory.CreateDirectory(directoryPath);
        }

        var savedStats = new SavedDailyRestStats
        {
            Date = _currentDate,
            SuccessfulRestsToday = _successfulRestsToday,
            CompletedSessionIds = [.. _completedSessionIds]
        };
        var json = System.Text.Json.JsonSerializer.Serialize(
            savedStats,
            new System.Text.Json.JsonSerializerOptions { WriteIndented = true });
        File.WriteAllText(_statsFilePath, json);
    }

    private sealed class SavedDailyRestStats
    {
        public DateOnly Date { get; set; }

        public int SuccessfulRestsToday { get; set; }

        public List<Guid>? CompletedSessionIds { get; set; }
    }
}
