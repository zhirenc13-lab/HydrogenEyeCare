namespace HydrogenEyeCare;

public sealed class RestReminderForm : Form
{
    private const int WindowWidth = 560;
    private const int WindowHeight = 300;
    private static readonly TimeSpan CompletionConfirmationDuration = TimeSpan.FromSeconds(10);
    private const string ConfirmationPrompt = "完成远眺后点击确认";

    private static readonly string[] Prompts =
    [
        "看一看 6 米外的远方",
        "放松肩颈，眨眨眼",
        "让眼睛离开屏幕 20 秒",
        "望向窗外或房间远处",
        "慢慢呼吸，放松眼周"
    ];

    private readonly System.Windows.Forms.Timer _timer;
    private readonly TimeSpan _duration;
    private readonly DateTime _startedAt;
    private readonly Label _countdownLabel;
    private readonly Label _promptLabel;
    private readonly Button _primaryActionButton;
    private readonly bool _canDelay;
    private readonly int _remainingDelays;
    private readonly string _restPrompt;
    private readonly ThemePalette _palette;
    private readonly Color _borderColor;
    private RestReminderPrimaryAction _primaryAction;
    private bool _completionRaised;

    public RestReminderForm(TimeSpan duration, bool canDelay, int remainingDelays, ThemePalette palette)
    {
        _duration = duration;
        _startedAt = DateTime.UtcNow;
        _canDelay = canDelay;
        _remainingDelays = remainingDelays;
        _restPrompt = Prompts[Random.Shared.Next(Prompts.Length)];
        _palette = palette;
        _borderColor = palette.BorderColor;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        BackColor = palette.WindowBackColor;
        Opacity = 0.96;
        ClientSize = new Size(WindowWidth, WindowHeight);
        MinimumSize = new Size(WindowWidth, WindowHeight);
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        AutoScaleMode = AutoScaleMode.Dpi;
        Padding = new Padding(26);

        var titleLabel = new Label
        {
            AutoSize = false,
            Text = "休息一下",
            Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = palette.TitleColor,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _countdownLabel = new Label
        {
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 34F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = palette.CountdownColor,
            Dock = DockStyle.Fill
        };

        _promptLabel = new Label
        {
            AutoSize = false,
            Text = _restPrompt,
            Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = palette.PromptColor,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _primaryActionButton = new Button
        {
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(280, 52),
            Padding = new Padding(16, 8, 16, 8),
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Margin = new Padding(0, 4, 0, 0),
            BackColor = palette.ButtonBackColor,
            ForeColor = palette.ButtonTextColor,
            FlatStyle = FlatStyle.Flat
        };
        _primaryActionButton.FlatAppearance.BorderColor = palette.ButtonBorderColor;
        _primaryActionButton.Click += (_, _) => HandlePrimaryActionClick();

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 62F));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 38F));
        headerLayout.Controls.Add(titleLabel, 0, 0);
        headerLayout.Controls.Add(_countdownLabel, 1, 0);

        var buttonPanel = new Panel
        {
            Dock = DockStyle.Fill,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        buttonPanel.Controls.Add(_primaryActionButton);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 3,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 118F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(headerLayout, 0, 0);
        layout.Controls.Add(_promptLabel, 0, 1);
        layout.Controls.Add(buttonPanel, 0, 2);

        Controls.Add(layout);

        _timer = new System.Windows.Forms.Timer
        {
            Interval = 250
        };
        _timer.Tick += (_, _) => UpdateCountdown();

        PositionNearTray();
        UpdateCountdown();
    }

    public event EventHandler? RestCompleted;

    public event EventHandler? DelayRequested;

    public event EventHandler? CompletionExpired;

    protected override bool ShowWithoutActivation => true;

    protected override CreateParams CreateParams
    {
        get
        {
            const int wsExNoActivate = 0x08000000;
            var createParams = base.CreateParams;
            createParams.ExStyle |= wsExNoActivate;
            return createParams;
        }
    }

    internal static RestReminderPrimaryActionState GetPrimaryActionState(
        TimeSpan remaining,
        bool canDelay,
        int remainingDelays)
    {
        if (remaining <= TimeSpan.Zero)
        {
            return new RestReminderPrimaryActionState(RestReminderPrimaryAction.Complete, "完成", Enabled: true);
        }

        return canDelay
            ? new RestReminderPrimaryActionState(
                RestReminderPrimaryAction.Delay,
                $"延迟 5 分钟（剩 {remainingDelays} 次）",
                Enabled: true)
            : new RestReminderPrimaryActionState(RestReminderPrimaryAction.Delay, "已达到上限", Enabled: false);
    }

    internal static RestReminderState GetReminderState(
        TimeSpan elapsed,
        TimeSpan restDuration,
        TimeSpan confirmationDuration,
        bool canDelay,
        int remainingDelays)
    {
        var restRemaining = restDuration - elapsed;
        if (restRemaining > TimeSpan.Zero)
        {
            return new RestReminderState(
                DisplaySeconds: (int)Math.Ceiling(restRemaining.TotalSeconds),
                PrimaryAction: GetPrimaryActionState(restRemaining, canDelay, remainingDelays),
                ShouldAutoClose: false);
        }

        var confirmationRemaining = confirmationDuration - (elapsed - restDuration);
        return new RestReminderState(
            DisplaySeconds: Math.Max(0, (int)Math.Ceiling(confirmationRemaining.TotalSeconds)),
            PrimaryAction: GetPrimaryActionState(TimeSpan.Zero, canDelay, remainingDelays),
            ShouldAutoClose: confirmationRemaining <= TimeSpan.Zero);
    }

    internal static string GetPromptText(RestReminderPrimaryAction action, string restPrompt)
    {
        return action == RestReminderPrimaryAction.Complete ? ConfirmationPrompt : restPrompt;
    }

    protected override void OnShown(EventArgs e)
    {
        base.OnShown(e);
        _timer.Start();
    }

    protected override void OnFormClosed(FormClosedEventArgs e)
    {
        _timer.Dispose();
        base.OnFormClosed(e);
    }

    protected override void OnPaint(PaintEventArgs e)
    {
        base.OnPaint(e);
        using var borderPen = new Pen(_borderColor);
        e.Graphics.DrawRectangle(borderPen, 0, 0, ClientSize.Width - 1, ClientSize.Height - 1);
    }

    private void UpdateCountdown()
    {
        var state = GetReminderState(
            DateTime.UtcNow - _startedAt,
            _duration,
            CompletionConfirmationDuration,
            _canDelay,
            _remainingDelays);

        if (state.ShouldAutoClose)
        {
            _timer.Stop();
            CompletionExpired?.Invoke(this, EventArgs.Empty);
            Close();
            return;
        }

        _countdownLabel.Text = state.DisplaySeconds.ToString("0");
        ApplyPrimaryActionState(state.PrimaryAction);
    }

    private void ApplyPrimaryActionState(RestReminderPrimaryActionState state)
    {
        _primaryAction = state.Action;
        _countdownLabel.ForeColor = state.Action == RestReminderPrimaryAction.Complete
            ? _palette.ConfirmationCountdownColor
            : _palette.CountdownColor;
        _promptLabel.Text = GetPromptText(state.Action, _restPrompt);
        _primaryActionButton.Text = state.Text;
        _primaryActionButton.Enabled = state.Enabled;
    }

    private void HandlePrimaryActionClick()
    {
        if (_primaryAction == RestReminderPrimaryAction.Complete)
        {
            if (_completionRaised)
            {
                return;
            }

            _completionRaised = true;
            RestCompleted?.Invoke(this, EventArgs.Empty);
            Close();
            return;
        }

        DelayRequested?.Invoke(this, EventArgs.Empty);
        Close();
    }

    private void PositionNearTray()
    {
        var screen = Screen.FromPoint(Cursor.Position)
            ?? Screen.PrimaryScreen
            ?? Screen.AllScreens[0];
        var area = screen.WorkingArea;
        Location = new Point(area.Right - Width - 16, area.Bottom - Height - 16);
    }
}

internal enum RestReminderPrimaryAction
{
    Delay,
    Complete
}

internal sealed record RestReminderPrimaryActionState(
    RestReminderPrimaryAction Action,
    string Text,
    bool Enabled);

internal sealed record RestReminderState(
    int DisplaySeconds,
    RestReminderPrimaryActionState PrimaryAction,
    bool ShouldAutoClose);
