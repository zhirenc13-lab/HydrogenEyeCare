using System.Diagnostics;

namespace HydrogenEyeCare;

public sealed class EyeCareController : IDisposable
{
    private static readonly TimeSpan WorkDuration = TimeSpan.FromMinutes(20);
    private static readonly TimeSpan RestDelayDuration = TimeSpan.FromMinutes(5);
    private const int MaxConsecutiveDelays = 2;

    private readonly System.Windows.Forms.Timer _timer;
    private readonly Stopwatch _stopwatch = new();
    private TimeSpan _elapsedOffset = TimeSpan.Zero;
    private AppState _stateBeforePause = AppState.Working;
    private TimeSpan _elapsedBeforePause = TimeSpan.Zero;
    private int _consecutiveDelays;

    public EyeCareController()
    {
        _timer = new System.Windows.Forms.Timer
        {
            Interval = 500
        };
        _timer.Tick += (_, _) => Tick();
    }

    public AppState State { get; private set; } = AppState.Working;

    public event EventHandler? StatusChanged;

    public event EventHandler<RestRequestedEventArgs>? RestRequested;

    public void Start()
    {
        EnterState(AppState.Working);
        _timer.Start();
    }

    public void Pause()
    {
        if (State is AppState.Paused or AppState.Resting)
        {
            return;
        }

        _stateBeforePause = State;
        _elapsedBeforePause = ElapsedInCurrentState;
        _consecutiveDelays = 0;
        _stopwatch.Stop();
        State = AppState.Paused;
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Resume()
    {
        if (State != AppState.Paused)
        {
            return;
        }

        State = _stateBeforePause;
        _elapsedOffset = _elapsedBeforePause;
        _stopwatch.Restart();
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public void StartRestNow()
    {
        if (State == AppState.Resting)
        {
            return;
        }

        StartRest();
    }

    public void ResetWorkCycle()
    {
        _consecutiveDelays = 0;
        EnterState(AppState.Working);
    }

    public void CompleteRest()
    {
        if (State != AppState.Resting)
        {
            return;
        }

        _consecutiveDelays = 0;
        EnterState(AppState.Working);
    }

    public void DelayRest()
    {
        if (State != AppState.Resting || _consecutiveDelays >= MaxConsecutiveDelays)
        {
            return;
        }

        _consecutiveDelays++;
        EnterState(AppState.Delayed);
    }

    public string GetStatusText()
    {
        return State switch
        {
            AppState.Working => $"距离下次休息还剩约 {Math.Max(1, (int)Math.Ceiling((WorkDuration - ElapsedInCurrentState).TotalMinutes))} 分钟",
            AppState.Delayed => $"已延迟提醒，还剩约 {Math.Max(1, (int)Math.Ceiling((RestDelayDuration - ElapsedInCurrentState).TotalMinutes))} 分钟",
            AppState.Paused => "计时已暂停",
            AppState.Resting => "正在休息",
            AppState.Suspended => "等待用户返回",
            _ => "氢护眼"
        };
    }

    private TimeSpan ElapsedInCurrentState => _elapsedOffset + (_stopwatch.IsRunning ? _stopwatch.Elapsed : TimeSpan.Zero);

    private void Tick()
    {
        if (State == AppState.Working && ElapsedInCurrentState >= WorkDuration)
        {
            StartRest();
            return;
        }

        if (State == AppState.Delayed && ElapsedInCurrentState >= RestDelayDuration)
        {
            StartRest();
            return;
        }

        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    private void StartRest()
    {
        EnterState(AppState.Resting);
        RestRequested?.Invoke(
            this,
            new RestRequestedEventArgs(
                TimeSpan.FromSeconds(20),
                canDelay: _consecutiveDelays < MaxConsecutiveDelays,
                remainingDelays: Math.Max(0, MaxConsecutiveDelays - _consecutiveDelays)));
    }

    private void EnterState(AppState state)
    {
        State = state;
        _elapsedOffset = TimeSpan.Zero;
        _stopwatch.Restart();
        StatusChanged?.Invoke(this, EventArgs.Empty);
    }

    public void Dispose()
    {
        _timer.Dispose();
        _stopwatch.Stop();
    }
}

public sealed class RestRequestedEventArgs : EventArgs
{
    public RestRequestedEventArgs(TimeSpan duration, bool canDelay, int remainingDelays)
    {
        Duration = duration;
        CanDelay = canDelay;
        RemainingDelays = remainingDelays;
    }

    public TimeSpan Duration { get; }

    public bool CanDelay { get; }

    public int RemainingDelays { get; }
}
