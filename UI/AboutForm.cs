using System.Diagnostics;

namespace HydrogenEyeCare;

public sealed class AboutForm : Form
{
    private const string RepositoryUrl = "https://github.com/zhirenc13-lab/HydrogenEyeCare";
    private const string RepositoryLinkText = "GitHub 项目主页";

    public AboutForm(string version, int successfulRestsToday, ThemePalette palette)
    {
        Text = "关于 氢护眼";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(640, 460);
        MinimumSize = new Size(560, 420);
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        AutoScaleMode = AutoScaleMode.Dpi;
        Padding = new Padding(30);

        var titleLabel = new Label
        {
            AutoSize = false,
            Text = "氢护眼",
            Font = new Font("Microsoft YaHei UI", 16F, FontStyle.Bold, GraphicsUnit.Point),
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var versionLabel = new Label
        {
            AutoSize = false,
            Text = $"版本：{version}",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var summaryLabel = new Label
        {
            AutoSize = false,
            Text = "20 分钟工作，20 秒远眺提醒。",
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var linkLabel = new LinkLabel
        {
            AutoSize = false,
            Text = RepositoryLinkText,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            LinkBehavior = LinkBehavior.HoverUnderline
        };
        linkLabel.Links.Add(0, RepositoryLinkText.Length, RepositoryUrl);
        linkLabel.LinkClicked += (_, e) =>
        {
            if (e.Link?.LinkData is string url)
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        };

        var statsPanel = new Panel
        {
            Dock = DockStyle.Fill,
            BackColor = palette.StatsBackColor,
            Padding = new Padding(18),
            Margin = new Padding(0, 10, 0, 10)
        };
        statsPanel.Paint += (_, e) =>
        {
            using var pen = new Pen(palette.StatsBorderColor);
            e.Graphics.DrawRectangle(pen, 0, 0, statsPanel.ClientSize.Width - 1, statsPanel.ClientSize.Height - 1);
        };

        var statsText = new Label
        {
            AutoSize = false,
            Text = $"今日成功远眺：{successfulRestsToday} 次",
            Dock = DockStyle.Top,
            Height = 42,
            Font = new Font("Microsoft YaHei UI", 13F, FontStyle.Bold, GraphicsUnit.Point),
            ForeColor = palette.StatsTextColor,
            TextAlign = ContentAlignment.MiddleLeft
        };

        var commentText = new Label
        {
            AutoSize = false,
            Text = GetDailyComment(successfulRestsToday),
            Dock = DockStyle.Fill,
            ForeColor = palette.StatsTextColor,
            TextAlign = ContentAlignment.TopLeft
        };

        statsPanel.Controls.Add(commentText);
        statsPanel.Controls.Add(statsText);

        var okButton = new Button
        {
            Text = "确定",
            DialogResult = DialogResult.OK,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(96, 38),
            Margin = new Padding(0)
        };

        var buttonPanel = new FlowLayoutPanel
        {
            Dock = DockStyle.Fill,
            FlowDirection = FlowDirection.RightToLeft,
            WrapContents = false,
            Margin = Padding.Empty,
            Padding = new Padding(0, 6, 0, 0)
        };
        buttonPanel.Controls.Add(okButton);

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 6,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 58F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 40F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 54F));
        layout.Controls.Add(titleLabel, 0, 0);
        layout.Controls.Add(versionLabel, 0, 1);
        layout.Controls.Add(summaryLabel, 0, 2);
        layout.Controls.Add(linkLabel, 0, 3);
        layout.Controls.Add(statsPanel, 0, 4);
        layout.Controls.Add(buttonPanel, 0, 5);

        AcceptButton = okButton;
        Controls.Add(layout);
    }

    private static string GetDailyComment(int successfulRestsToday)
    {
        return successfulRestsToday switch
        {
            0 => "今天还没有完成远眺，先从下一次提醒开始。",
            <= 3 => "今天已经开始照顾眼睛了。",
            <= 7 => "今天节奏不错，继续保持。",
            _ => "今天护眼执行得很稳定，注意也要离开座位活动一下。"
        };
    }
}
