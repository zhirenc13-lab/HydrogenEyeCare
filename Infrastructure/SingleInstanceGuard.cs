namespace HydrogenEyeCare;

public sealed class SingleInstanceGuard : IDisposable
{
    private readonly Mutex _mutex;
    private bool _hasHandle;

    public SingleInstanceGuard(string name)
    {
        _mutex = new Mutex(false, $@"Local\{name}");
    }

    public bool TryAcquire()
    {
        try
        {
            _hasHandle = _mutex.WaitOne(TimeSpan.Zero, false);
            return _hasHandle;
        }
        catch (AbandonedMutexException)
        {
            _hasHandle = true;
            return true;
        }
    }

    public static void ShowAlreadyRunningNotice()
    {
        using var notifyIcon = new NotifyIcon
        {
            Icon = SystemIcons.Information,
            Text = "氢护眼已在后台运行",
            Visible = true
        };

        notifyIcon.ShowBalloonTip(2500, "氢护眼", "护眼工具已在后台运行中", ToolTipIcon.Info);
        Application.DoEvents();
        Thread.Sleep(1200);
    }

    public void Dispose()
    {
        if (_hasHandle)
        {
            _mutex.ReleaseMutex();
        }

        _mutex.Dispose();
    }
}
