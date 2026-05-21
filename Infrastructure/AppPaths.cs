namespace HydrogenEyeCare;

public static class AppPaths
{
    public static string AppDataDirectory { get; } = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "HydrogenEyeCare");

    public static string ConfigFilePath => Path.Combine(AppDataDirectory, "config.json");

    public static string ErrorLogPath => Path.Combine(AppDataDirectory, "error.log");

    public static void EnsureAppDataDirectory()
    {
        Directory.CreateDirectory(AppDataDirectory);
    }
}
