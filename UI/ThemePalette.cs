namespace HydrogenEyeCare;

public sealed record ThemePalette(
    AppTheme Theme,
    string DisplayName,
    Color WindowBackColor,
    Color BorderColor,
    Color TitleColor,
    Color CountdownColor,
    Color ConfirmationCountdownColor,
    Color PromptColor,
    Color ButtonBackColor,
    Color ButtonBorderColor,
    Color ButtonTextColor,
    Color StatsBackColor,
    Color StatsBorderColor,
    Color StatsTextColor)
{
    public static ThemePalette FromTheme(AppTheme theme)
    {
        return theme switch
        {
            AppTheme.ForestGreen => new ThemePalette(
                theme,
                "森林绿",
                Color.FromArgb(244, 250, 244),
                Color.FromArgb(185, 215, 189),
                Color.FromArgb(23, 59, 39),
                Color.FromArgb(31, 138, 76),
                Color.FromArgb(8, 145, 178),
                Color.FromArgb(78, 111, 90),
                Color.FromArgb(228, 243, 230),
                Color.FromArgb(169, 205, 176),
                Color.FromArgb(31, 94, 54),
                Color.FromArgb(238, 246, 240),
                Color.FromArgb(200, 223, 204),
                Color.FromArgb(35, 115, 68)),
            AppTheme.MistBlue => new ThemePalette(
                theme,
                "迷雾蓝",
                Color.FromArgb(244, 248, 251),
                Color.FromArgb(191, 208, 221),
                Color.FromArgb(24, 50, 68),
                Color.FromArgb(46, 117, 163),
                Color.FromArgb(139, 92, 246),
                Color.FromArgb(87, 112, 130),
                Color.FromArgb(228, 238, 246),
                Color.FromArgb(170, 194, 210),
                Color.FromArgb(33, 79, 109),
                Color.FromArgb(232, 241, 247),
                Color.FromArgb(185, 207, 222),
                Color.FromArgb(33, 79, 109)),
            AppTheme.RockGray => new ThemePalette(
                theme,
                "暗岩灰",
                Color.FromArgb(243, 244, 244),
                Color.FromArgb(197, 201, 201),
                Color.FromArgb(36, 41, 43),
                Color.FromArgb(79, 91, 96),
                Color.FromArgb(37, 99, 235),
                Color.FromArgb(99, 107, 110),
                Color.FromArgb(230, 232, 232),
                Color.FromArgb(184, 190, 190),
                Color.FromArgb(51, 59, 62),
                Color.FromArgb(236, 238, 238),
                Color.FromArgb(196, 202, 202),
                Color.FromArgb(51, 59, 62)),
            _ => throw new ArgumentOutOfRangeException(nameof(theme), theme, null)
        };
    }
}
