using System.Drawing.Drawing2D;

namespace HydrogenEyeCare;

public static class TrayIconFactory
{
    public static Icon CreateWorkingIcon()
    {
        return CreateIcon(Color.FromArgb(44, 160, 90), drawPause: false);
    }

    public static Icon CreatePausedIcon()
    {
        return CreateIcon(Color.FromArgb(120, 120, 120), drawPause: true);
    }

    private static Icon CreateIcon(Color color, bool drawPause)
    {
        using var bitmap = new Bitmap(32, 32);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var brush = new SolidBrush(color);
        graphics.FillEllipse(brush, 4, 8, 24, 16);

        using var whiteBrush = new SolidBrush(Color.White);
        graphics.FillEllipse(whiteBrush, 11, 11, 10, 10);

        using var pupilBrush = new SolidBrush(Color.FromArgb(45, 45, 45));
        graphics.FillEllipse(pupilBrush, 14, 14, 4, 4);

        if (drawPause)
        {
            using var pauseBrush = new SolidBrush(Color.White);
            graphics.FillRectangle(pauseBrush, 11, 10, 3, 12);
            graphics.FillRectangle(pauseBrush, 18, 10, 3, 12);
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
