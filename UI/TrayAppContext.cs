using System.Reflection;

namespace HydrogenEyeCare;

public sealed class TrayAppContext : ApplicationContext
{
    private readonly ErrorLogger _logger;
    private readonly ConfigStore _configStore = new();
    private readonly StartupRegistry _startupRegistry = new();
    private readonly SoundPlayerService _soundPlayer = new();
    private readonly EyeCareController _controller = new();
    private readonly DailyRestStats _dailyRestStats = new();
    private readonly SystemEventsWatcher _systemEventsWatcher = new();
    private readonly NotifyIcon _notifyIcon;
    private readonly Icon _workingIcon;
    private readonly Icon _pausedIcon;
    private readonly ToolStripMenuItem _startupMenuItem;
    private readonly ToolStripMenuItem _mutedMenuItem;
    private readonly ToolStripMenuItem _themeMenuItem;
    private readonly ToolStripMenuItem _forestGreenMenuItem;
    private readonly ToolStripMenuItem _mistBlueMenuItem;
    private readonly ToolStripMenuItem _rockGrayMenuItem;
    private readonly ToolStripMenuItem _pauseMenuItem;
    private readonly ToolStripMenuItem _restNowMenuItem;
    private readonly ToolStripMenuItem _aboutMenuItem;
    private readonly ToolStripMenuItem _exitMenuItem;
    private AppConfig _config;
    private RestReminderForm? _restForm;
    private bool _syncingStartupMenu;
    private bool _syncingThemeMenu;

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

        _themeMenuItem = new ToolStripMenuItem("界面主题");
        _forestGreenMenuItem = CreateThemeMenuItem(AppTheme.ForestGreen);
        _mistBlueMenuItem = CreateThemeMenuItem(AppTheme.MistBlue);
        _rockGrayMenuItem = CreateThemeMenuItem(AppTheme.RockGray);
        _themeMenuItem.DropDownItems.AddRange(
        [
            _forestGreenMenuItem,
            _mistBlueMenuItem,
            _rockGrayMenuItem
        ]);

        _pauseMenuItem = new ToolStripMenuItem("暂停计时");
        _pauseMenuItem.Click += (_, _) => TogglePause();

        _restNowMenuItem = new ToolStripMenuItem("立即休息");
        _restNowMenuItem.Click += (_, _) => _controller.StartRestNow();

        _aboutMenuItem = new ToolStripMenuItem("关于氢护眼");
        _aboutMenuItem.Click += (_, _) => ShowAbout();

        _exitMenuItem = new ToolStripMenuItem("退出程序");
        _exitMenuItem.Click += (_, _) => ExitThread();

        var menu = new ContextMenuStrip();
        menu.Items.AddRange(
        [
            _startupMenuItem,
            _mutedMenuItem,
            _themeMenuItem,
            new ToolStripSeparator(),
            _pauseMenuItem,
            _restNowMenuItem,
            new ToolStripSeparator(),
            _aboutMenuItem,
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

        SyncThemeMenu();
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

    private ToolStripMenuItem CreateThemeMenuItem(AppTheme theme)
    {
        var item = new ToolStripMenuItem(ThemePalette.FromTheme(theme).DisplayName)
        {
            Tag = theme
        };
        item.Click += (_, _) => SelectThemeFromMenu(item);
        return item;
    }

    private void SelectThemeFromMenu(ToolStripMenuItem item)
    {
        if (_syncingThemeMenu || item.Tag is not AppTheme theme)
        {
            return;
        }

        _config.Theme = theme;
        _configStore.Save(_config);
        SyncThemeMenu();
    }

    private void SyncThemeMenu()
    {
        _syncingThemeMenu = true;
        _forestGreenMenuItem.Checked = _config.Theme == AppTheme.ForestGreen;
        _mistBlueMenuItem.Checked = _config.Theme == AppTheme.MistBlue;
        _rockGrayMenuItem.Checked = _config.Theme == AppTheme.RockGray;
        _syncingThemeMenu = false;
    }

    private ThemePalette CurrentThemePalette => ThemePalette.FromTheme(_config.Theme);

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

        var form = new RestReminderForm(e.Duration, e.CanDelay, e.RemainingDelays, CurrentThemePalette);
        var restSessionId = Guid.NewGuid();
        form.RestCompleted += (_, _) =>
        {
            if (!_config.Muted)
            {
                _soundPlayer.PlayRestEnded();
            }

            _dailyRestStats.MarkRestSessionCompleted(restSessionId);
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
        if (!ShouldResetWorkCycleAfterSystemRest(_controller.State))
        {
            UpdateTrayStatus();
            return;
        }

        _restForm?.Close();
        _controller.ResetWorkCycle();
    }

    internal static bool ShouldResetWorkCycleAfterSystemRest(AppState state)
    {
        return state != AppState.Paused;
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

    private static string GetAppVersion()
    {
        return Assembly.GetExecutingAssembly()
            .GetCustomAttribute<AssemblyInformationalVersionAttribute>()
            ?.InformationalVersion ?? "1.0.0";
    }

    private void ShowAbout()
    {
        using var aboutForm = new AboutForm(
            GetAppVersion(),
            _dailyRestStats.SuccessfulRestsToday,
            CurrentThemePalette);
        aboutForm.ShowDialog();
    }
}
