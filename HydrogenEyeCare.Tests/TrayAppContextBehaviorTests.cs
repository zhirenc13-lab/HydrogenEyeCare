namespace HydrogenEyeCare.Tests;

public sealed class TrayAppContextBehaviorTests
{
    [Fact]
    public void SystemRestDoesNotResetWorkCycleWhenManuallyPaused()
    {
        Assert.False(TrayAppContext.ShouldResetWorkCycleAfterSystemRest(AppState.Paused));
    }

    [Theory]
    [InlineData(AppState.Working)]
    [InlineData(AppState.Delayed)]
    [InlineData(AppState.Resting)]
    public void SystemRestResetsWorkCycleWhenNotManuallyPaused(AppState state)
    {
        Assert.True(TrayAppContext.ShouldResetWorkCycleAfterSystemRest(state));
    }
}
