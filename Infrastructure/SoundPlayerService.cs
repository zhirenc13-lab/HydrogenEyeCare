using System.Media;

namespace HydrogenEyeCare;

public sealed class SoundPlayerService
{
    public void PlayRestStarted()
    {
        SystemSounds.Asterisk.Play();
    }

    public void PlayRestEnded()
    {
        SystemSounds.Exclamation.Play();
    }
}
