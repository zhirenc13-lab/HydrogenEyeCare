using Microsoft.Win32;

namespace HydrogenEyeCare;

public sealed class StartupRegistry
{
    private const string RunKeyPath = @"Software\Microsoft\Windows\CurrentVersion\Run";
    private const string ValueName = "氢护眼";
    private const string LegacyValueName = "HydrogenEyeCare";

    public bool IsEnabled()
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, false);
        return HasValue(key, ValueName) || HasValue(key, LegacyValueName);
    }

    public void SetEnabled(bool enabled)
    {
        using var key = Registry.CurrentUser.OpenSubKey(RunKeyPath, true)
            ?? Registry.CurrentUser.CreateSubKey(RunKeyPath, true);

        key.DeleteValue(LegacyValueName, false);

        if (enabled)
        {
            key.SetValue(ValueName, $"\"{Application.ExecutablePath}\"");
        }
        else
        {
            key.DeleteValue(ValueName, false);
        }
    }

    private static bool HasValue(RegistryKey? key, string valueName)
    {
        return key?.GetValue(valueName) is string value && !string.IsNullOrWhiteSpace(value);
    }
}
