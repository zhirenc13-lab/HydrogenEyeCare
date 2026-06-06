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
}
