namespace HydrogenEyeCare;

static class Program
{
    [STAThread]
    static void Main()
    {
        ApplicationConfiguration.Initialize();
        Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

        var logger = new ErrorLogger();
        Application.ThreadException += (_, e) => logger.Log(e.Exception);
        AppDomain.CurrentDomain.UnhandledException += (_, e) =>
        {
            if (e.ExceptionObject is Exception exception)
            {
                logger.Log(exception);
            }
        };

        using var guard = new SingleInstanceGuard("HydrogenEyeCare.SingleInstance");
        if (!guard.TryAcquire())
        {
            SingleInstanceGuard.ShowAlreadyRunningNotice();
            return;
        }

        try
        {
            Application.Run(new TrayAppContext(logger));
        }
        catch (Exception exception)
        {
            logger.Log(exception);
            throw;
        }
    }
}
