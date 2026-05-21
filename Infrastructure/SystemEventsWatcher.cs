using Microsoft.Win32;

namespace HydrogenEyeCare;

public sealed class SystemEventsWatcher : IDisposable
{
    public SystemEventsWatcher()
    {
        SystemEvents.SessionSwitch += OnSessionSwitch;
        SystemEvents.PowerModeChanged += OnPowerModeChanged;
    }

    public event EventHandler? UserReturnedOrRested;

    private void OnSessionSwitch(object sender, SessionSwitchEventArgs e)
    {
        if (e.Reason is SessionSwitchReason.SessionLock or SessionSwitchReason.SessionUnlock)
        {
            UserReturnedOrRested?.Invoke(this, EventArgs.Empty);
        }
    }

    private void OnPowerModeChanged(object sender, PowerModeChangedEventArgs e)
    {
        if (e.Mode is PowerModes.Resume or PowerModes.Suspend)
        {
            UserReturnedOrRested?.Invoke(this, EventArgs.Empty);
        }
    }

    public void Dispose()
    {
        SystemEvents.SessionSwitch -= OnSessionSwitch;
        SystemEvents.PowerModeChanged -= OnPowerModeChanged;
    }
}
