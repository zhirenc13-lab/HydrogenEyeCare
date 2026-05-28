namespace HydrogenEyeCare;

public sealed class RestReminderForm : Form
{
    private const int WindowWidth = 560;
    private const int WindowHeight = 260;

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
    private readonly Button _delayButton;
    private readonly Color _borderColor;

    public RestReminderForm(TimeSpan duration, bool canDelay, int remainingDelays, ThemePalette palette)
    {
        _duration = duration;
        _startedAt = DateTime.UtcNow;
        _borderColor = palette.BorderColor;

        FormBorderStyle = FormBorderStyle.None;
        ShowInTaskbar = false;
        TopMost = true;
        StartPosition = FormStartPosition.Manual;
        BackColor = palette.WindowBackColor;
        Opacity = 0.96;
        ClientSize = new Size(WindowWidth, WindowHeight);
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        AutoScaleMode = AutoScaleMode.Dpi;
        Padding = new Padding(24);

        var titleLabel = new Label
        {
            AutoSize = false,
            Text = "休息一下",
            Font = new Font("Microsoft YaHei UI", 12F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(32, 32, 32),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _countdownLabel = new Label
        {
            AutoSize = false,
            TextAlign = ContentAlignment.MiddleRight,
            Font = new Font("Segoe UI", 30F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(38, 38, 38),
            Dock = DockStyle.Fill
        };

        var promptLabel = new Label
        {
            AutoSize = false,
            Text = Prompts[Random.Shared.Next(Prompts.Length)],
            Font = new Font("Microsoft YaHei UI", 11F, FontStyle.Regular, GraphicsUnit.Point),
            ForeColor = Color.FromArgb(82, 82, 82),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        _delayButton = new Button
        {
            Text = canDelay ? $"延迟 5 分钟（剩 {remainingDelays} 次）" : "已达到上限",
            Enabled = canDelay,
            Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point),
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(240, 48),
            Padding = new Padding(14, 6, 14, 6),
            Anchor = AnchorStyles.Left | AnchorStyles.Top,
            Margin = new Padding(0, 10, 0, 0)
        };
        _delayButton.Click += (_, _) =>
        {
            DelayRequested?.Invoke(this, EventArgs.Empty);
            Close();
        };

        titleLabel.ForeColor = palette.TitleColor;
        _countdownLabel.ForeColor = palette.CountdownColor;
        promptLabel.ForeColor = palette.PromptColor;
        _delayButton.BackColor = palette.ButtonBackColor;
        _delayButton.ForeColor = palette.ButtonTextColor;
        _delayButton.FlatStyle = FlatStyle.Flat;
        _delayButton.FlatAppearance.BorderColor = palette.ButtonBorderColor;

        var headerLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Top,
            Height = 104,
            ColumnCount = 2,
            RowCount = 1,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 65F));
        headerLayout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 35F));
        headerLayout.Controls.Add(titleLabel, 0, 0);
        headerLayout.Controls.Add(_countdownLabel, 1, 0);

        var contentLayout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 2,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 48F));
        contentLayout.RowStyles.Add(new RowStyle(SizeType.Percent, 52F));
        contentLayout.Controls.Add(promptLabel, 0, 0);
        contentLayout.Controls.Add(_delayButton, 0, 1);

        Controls.Add(contentLayout);
        Controls.Add(headerLayout);

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
        var remaining = _duration - (DateTime.UtcNow - _startedAt);
        if (remaining <= TimeSpan.Zero)
        {
            _countdownLabel.Text = "0";
            RestCompleted?.Invoke(this, EventArgs.Empty);
            Close();
            return;
        }

        _countdownLabel.Text = Math.Ceiling(remaining.TotalSeconds).ToString("0");
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
