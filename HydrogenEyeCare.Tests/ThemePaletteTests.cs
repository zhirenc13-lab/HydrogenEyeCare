namespace HydrogenEyeCare.Tests;

using System.Drawing;

public sealed class ThemePaletteTests
{
    [Theory]
    [InlineData(AppTheme.ForestGreen, "森林绿")]
    [InlineData(AppTheme.MistBlue, "迷雾蓝")]
    [InlineData(AppTheme.RockGray, "暗岩灰")]
    public void PaletteHasChineseDisplayName(AppTheme theme, string expected)
    {
        var palette = ThemePalette.FromTheme(theme);

        Assert.Equal(expected, palette.DisplayName);
    }

    [Fact]
    public void FromThemeThrowsForInvalidThemeValue()
    {
        var theme = (AppTheme)999;

        var exception = Assert.Throws<ArgumentOutOfRangeException>(() => ThemePalette.FromTheme(theme));

        Assert.Equal("theme", exception.ParamName);
        Assert.Equal(theme, exception.ActualValue);
    }

    [Theory]
    [InlineData(AppTheme.ForestGreen, 0x08, 0x91, 0xB2)]
    [InlineData(AppTheme.MistBlue, 0x8B, 0x5C, 0xF6)]
    [InlineData(AppTheme.RockGray, 0x25, 0x63, 0xEB)]
    public void PaletteHasConfirmationCountdownColor(AppTheme theme, int red, int green, int blue)
    {
        var palette = ThemePalette.FromTheme(theme);

        Assert.Equal(Color.FromArgb(red, green, blue), palette.ConfirmationCountdownColor);
        Assert.NotEqual(palette.CountdownColor, palette.ConfirmationCountdownColor);
    }
}
