namespace HydrogenEyeCare.Tests;

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
}
