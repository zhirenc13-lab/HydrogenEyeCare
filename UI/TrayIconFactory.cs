using System.Drawing.Drawing2D;

namespace HydrogenEyeCare;

public static class TrayIconFactory
{
    public static Icon CreateWorkingIcon()
    {
        return CreateIcon(
            Color.FromArgb(32, 142, 92),
            Color.FromArgb(118, 204, 130),
            drawPause: false);
    }

    public static Icon CreatePausedIcon()
    {
        return CreateIcon(
            Color.FromArgb(118, 126, 122),
            Color.FromArgb(176, 184, 180),
            drawPause: true);
    }

    private static Icon CreateIcon(Color startColor, Color endColor, bool drawPause)
    {
        using var bitmap = new Bitmap(32, 32);
        using var graphics = Graphics.FromImage(bitmap);
        graphics.SmoothingMode = SmoothingMode.AntiAlias;
        graphics.Clear(Color.Transparent);

        using var backgroundBrush = new LinearGradientBrush(
            new Rectangle(2, 2, 28, 28),
            startColor,
            endColor,
            45F);
        graphics.FillEllipse(backgroundBrush, 2, 2, 28, 28);

        using var leafPath = new GraphicsPath();
        leafPath.AddBezier(8, 17, 11, 7, 23, 6, 26, 16);
        leafPath.AddBezier(26, 16, 21, 25, 11, 24, 8, 17);

        using var whiteBrush = new SolidBrush(Color.White);
        graphics.FillPath(whiteBrush, leafPath);

        using var veinPen = new Pen(drawPause ? Color.FromArgb(125, 132, 128) : Color.FromArgb(44, 135, 88), 1.6F);
        graphics.DrawBezier(veinPen, 11, 17, 15, 14, 19, 12, 24, 10);

        using var irisBrush = new SolidBrush(drawPause ? Color.FromArgb(112, 118, 115) : Color.FromArgb(48, 174, 112));
        graphics.FillEllipse(irisBrush, 15, 13, 5, 5);

        if (drawPause)
        {
            using var pauseBrush = new SolidBrush(Color.FromArgb(245, 245, 245));
            graphics.FillRectangle(pauseBrush, 11, 9, 3, 14);
            graphics.FillRectangle(pauseBrush, 18, 9, 3, 14);
        }

        return Icon.FromHandle(bitmap.GetHicon());
    }
}
