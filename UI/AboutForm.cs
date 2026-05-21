using System.Diagnostics;

namespace HydrogenEyeCare;

public sealed class AboutForm : Form
{
    private const string RepositoryUrl = "https://github.com/zhirenc13-lab/HydrogenEyeCare";

    public AboutForm(string version)
    {
        Text = "关于氢护眼";
        FormBorderStyle = FormBorderStyle.FixedDialog;
        MaximizeBox = false;
        MinimizeBox = false;
        ShowInTaskbar = false;
        StartPosition = FormStartPosition.CenterScreen;
        ClientSize = new Size(520, 280);
        Font = new Font("Microsoft YaHei UI", 10F, FontStyle.Regular, GraphicsUnit.Point);
        AutoScaleMode = AutoScaleMode.Dpi;
        Padding = new Padding(28);

        var titleLabel = new Label
        {
            AutoSize = false,
            Text = "氢护眼",
            Font = new Font("Microsoft YaHei UI", 15F, FontStyle.Bold, GraphicsUnit.Point),
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
            Text = RepositoryUrl,
            Dock = DockStyle.Fill,
            TextAlign = ContentAlignment.MiddleLeft,
            LinkBehavior = LinkBehavior.HoverUnderline
        };
        linkLabel.Links.Add(0, RepositoryUrl.Length, RepositoryUrl);
        linkLabel.LinkClicked += (_, e) =>
        {
            if (e.Link?.LinkData is string url)
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        };

        var okButton = new Button
        {
            Text = "确定",
            DialogResult = DialogResult.OK,
            AutoSize = true,
            AutoSizeMode = AutoSizeMode.GrowAndShrink,
            MinimumSize = new Size(92, 36),
            Anchor = AnchorStyles.Right | AnchorStyles.Top
        };

        var layout = new TableLayoutPanel
        {
            Dock = DockStyle.Fill,
            ColumnCount = 1,
            RowCount = 5,
            Margin = Padding.Empty,
            Padding = Padding.Empty
        };
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 48F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 38F));
        layout.RowStyles.Add(new RowStyle(SizeType.Absolute, 42F));
        layout.RowStyles.Add(new RowStyle(SizeType.Percent, 100F));
        layout.Controls.Add(titleLabel, 0, 0);
        layout.Controls.Add(versionLabel, 0, 1);
        layout.Controls.Add(summaryLabel, 0, 2);
        layout.Controls.Add(linkLabel, 0, 3);
        layout.Controls.Add(okButton, 0, 4);

        AcceptButton = okButton;
        Controls.Add(layout);
    }
}
