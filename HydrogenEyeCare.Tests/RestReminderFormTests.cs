namespace HydrogenEyeCare.Tests;

public sealed class RestReminderFormTests
{
    [Fact]
    public void PrimaryActionIsDelayBeforeCountdownEnds()
    {
        var action = RestReminderForm.GetPrimaryActionState(
            TimeSpan.FromSeconds(5),
            canDelay: true,
            remainingDelays: 2);

        Assert.Equal(RestReminderPrimaryAction.Delay, action.Action);
        Assert.Equal("延迟 5 分钟（剩 2 次）", action.Text);
        Assert.True(action.Enabled);
    }

    [Fact]
    public void PrimaryActionIsDisabledDelayBeforeCountdownEndsWhenNoDelaysRemain()
    {
        var action = RestReminderForm.GetPrimaryActionState(
            TimeSpan.FromSeconds(5),
            canDelay: false,
            remainingDelays: 0);

        Assert.Equal(RestReminderPrimaryAction.Delay, action.Action);
        Assert.Equal("已达到上限", action.Text);
        Assert.False(action.Enabled);
    }

    [Fact]
    public void PrimaryActionIsCompletionConfirmationAfterCountdownEnds()
    {
        var action = RestReminderForm.GetPrimaryActionState(
            TimeSpan.Zero,
            canDelay: true,
            remainingDelays: 2);

        Assert.Equal(RestReminderPrimaryAction.Complete, action.Action);
        Assert.Equal("完成", action.Text);
        Assert.True(action.Enabled);
    }

    [Fact]
    public void ReminderStateWaitsForCompletionDuringConfirmationWindow()
    {
        var state = RestReminderForm.GetReminderState(
            elapsed: TimeSpan.FromSeconds(25),
            restDuration: TimeSpan.FromSeconds(20),
            confirmationDuration: TimeSpan.FromSeconds(10),
            canDelay: true,
            remainingDelays: 2);

        Assert.Equal(RestReminderPrimaryAction.Complete, state.PrimaryAction.Action);
        Assert.Equal(5, state.DisplaySeconds);
        Assert.False(state.ShouldAutoClose);
    }

    [Fact]
    public void ReminderStateStartsConfirmationWindowWhenRestCountdownEnds()
    {
        var state = RestReminderForm.GetReminderState(
            elapsed: TimeSpan.FromSeconds(20),
            restDuration: TimeSpan.FromSeconds(20),
            confirmationDuration: TimeSpan.FromSeconds(10),
            canDelay: true,
            remainingDelays: 2);

        Assert.Equal(RestReminderPrimaryAction.Complete, state.PrimaryAction.Action);
        Assert.Equal(10, state.DisplaySeconds);
        Assert.False(state.ShouldAutoClose);
    }

    [Fact]
    public void ReminderStateAutoClosesAfterConfirmationWindow()
    {
        var state = RestReminderForm.GetReminderState(
            elapsed: TimeSpan.FromSeconds(30),
            restDuration: TimeSpan.FromSeconds(20),
            confirmationDuration: TimeSpan.FromSeconds(10),
            canDelay: true,
            remainingDelays: 2);

        Assert.Equal(RestReminderPrimaryAction.Complete, state.PrimaryAction.Action);
        Assert.Equal(0, state.DisplaySeconds);
        Assert.True(state.ShouldAutoClose);
    }

    [Fact]
    public void PromptTextUsesConfirmationCopyAfterCountdownEnds()
    {
        Assert.Equal(
            "完成远眺后点击确认",
            RestReminderForm.GetPromptText(RestReminderPrimaryAction.Complete, "看一看 6 米外的远方"));
    }

    [Fact]
    public void PromptTextKeepsRestCopyBeforeCountdownEnds()
    {
        Assert.Equal(
            "看一看 6 米外的远方",
            RestReminderForm.GetPromptText(RestReminderPrimaryAction.Delay, "看一看 6 米外的远方"));
    }
}
