namespace HydrogenEyeCare;

public sealed class TrayAppContext : ApplicationContext
{
    private readonly ErrorLogger _logger;
    private readonly ConfigStore _configStore = new();
    private readonly StartupRegistry _startupRegistry = new();
    private readonly SoundPlayerService _soundPlayer = new();
    private readonly EyeCareController _controller = new();
    private readonly SystemEventsWatcher _systemEventsWatcher = new();
    private readonly NotifyIcon _notifyIcon;
    private readonly Icon _workingIcon;
    private readonly Icon _pausedIcon;
    private readonly ToolStripMenuItem _startupMenuItem;
    private readonly ToolStripMenuItem _mutedMenuItem;
    private readonly ToolStripMenuItem _pauseMenuItem;
    private readonly ToolStripMenuItem _restNowMenuItem;
    private readonly ToolStripMenuItem _exitMenuItem;
    private AppConfig _config;
    private RestReminderForm? _restForm;
    private bool _syncingStartupMenu;

    public TrayAppContext(ErrorLogger logger)
    {
        _logger = logger;
        _config = _configStore.Load();
        _config.StartWithWindows = _startupRegistry.IsEnabled();

        _workingIcon = TrayIconFactory.CreateWorkingIcon();
        _pausedIcon = TrayIconFactory.CreatePausedIcon();

        _startupMenuItem = new ToolStripMenuItem("开机自启") { CheckOnClick = true };
        _startupMenuItem.Checked = _config.StartWithWindows;
        _startupMenuItem.CheckedChanged += (_, _) => ToggleStartup();

        _mutedMenuItem = new ToolStripMenuItem("静音") { CheckOnClick = true };
        _mutedMenuItem.Checked = _config.Muted;
        _mutedMenuItem.CheckedChanged += (_, _) => ToggleMuted();

        _pauseMenuItem = new ToolStripMenuItem("暂停计时");
        _pauseMenuItem.Click += (_, _) => TogglePause();

        _restNowMenuItem = new ToolStripMenuItem("立即休息");
        _restNowMenuItem.Click += (_, _) => _controller.StartRestNow();

        _exitMenuItem = new ToolStripMenuItem("退出程序");
        _exitMenuItem.Click += (_, _) => ExitThread();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange(
        [
            _startupMenuItem,
            _mutedMenuItem,
            new ToolStripSeparator(),
            _pauseMenuItem,
            _restNowMenuItem,
            new ToolStripSeparator(),
            _exitMenuItem
        ]);

        _notifyIcon = new NotifyIcon
        {
            Icon = _workingIcon,
            Text = "氢护眼",
            ContextMenuStrip = menu,
            Visible = true
        };
        _notifyIcon.MouseMove += (_, _) => UpdateTrayStatus();

        _controller.StatusChanged += (_, _) => UpdateTrayStatus();
        _controller.RestRequested += (_, e) => ShowRestReminder(e);
        _systemEventsWatcher.UserReturnedOrRested += (_, _) => ResetAfterSystemRest();

        _controller.Start();
        UpdateTrayStatus();
    }

    protected override void ExitThreadCore()
    {
        _restForm?.Close();
        _notifyIcon.Visible = false;
        _notifyIcon.Dispose();
        _workingIcon.Dispose();
        _pausedIcon.Dispose();
        _controller.Dispose();
        _systemEventsWatcher.Dispose();
        base.ExitThreadCore();
    }

    private void ToggleStartup()
    {
        if (_syncingStartupMenu)
        {
            return;
        }

        try
        {
            _config.StartWithWindows = _startupMenuItem.Checked;
            _startupRegistry.SetEnabled(_config.StartWithWindows);
            _configStore.Save(_config);
        }
        catch (Exception exception)
        {
            _logger.Log(exception);
            _syncingStartupMenu = true;
            _startupMenuItem.Checked = !_startupMenuItem.Checked;
            _syncingStartupMenu = false;
            _notifyIcon.ShowBalloonTip(3000, "氢护眼", "开机自启设置失败", ToolTipIcon.Warning);
        }
    }

    private void ToggleMuted()
    {
        _config.Muted = _mutedMenuItem.Checked;
        _configStore.Save(_config);
    }

    private void TogglePause()
    {
        if (_controller.State == AppState.Paused)
        {
            _controller.Resume();
        }
        else
        {
            _controller.Pause();
        }
    }

    private void ShowRestReminder(RestRequestedEventArgs e)
    {
        _restForm?.Close();

        if (!_config.Muted)
        {
            _soundPlayer.PlayRestStarted();
        }

        var form = new RestReminderForm(e.Duration, e.CanDelay, e.RemainingDelays);
        form.RestCompleted += (_, _) =>
        {
            if (!_config.Muted)
            {
                _soundPlayer.PlayRestEnded();
            }

            _controller.CompleteRest();
        };
        form.DelayRequested += (_, _) => _controller.DelayRest();
        form.FormClosed += (_, _) =>
        {
            if (ReferenceEquals(_restForm, form))
            {
                _restForm = null;
            }
        };

        _restForm = form;
        form.Show();
    }

    private void ResetAfterSystemRest()
    {
        _restForm?.Close();
        _controller.ResetWorkCycle();
    }

    private void UpdateTrayStatus()
    {
        _notifyIcon.Icon = _controller.State == AppState.Paused ? _pausedIcon : _workingIcon;
        _notifyIcon.Text = TruncateTooltip($"氢护眼 - {_controller.GetStatusText()}");
        _pauseMenuItem.Text = _controller.State == AppState.Paused ? "恢复计时" : "暂停计时";
        _pauseMenuItem.Enabled = _controller.State != AppState.Resting;
        _restNowMenuItem.Enabled = _controller.State != AppState.Resting;
    }

    private static string TruncateTooltip(string text)
    {
        return text.Length <= 63 ? text : text[..63];
    }
}
