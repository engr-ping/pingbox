using Avalonia;
using System;
using System.Threading.Tasks;
using PingBox.Services;

namespace PingBox;

class Program
{
    [STAThread]
    public static void Main(string[] args)
    {
        AppDomain.CurrentDomain.UnhandledException += (_, eventArgs) =>
        {
            AppLogger.Error("AppDomain unhandled exception", eventArgs.ExceptionObject as Exception);
        };

        TaskScheduler.UnobservedTaskException += (_, eventArgs) =>
        {
            AppLogger.Error("Unobserved task exception", eventArgs.Exception);
        };

        try
        {
            AppLogger.Info($"Application starting. Log file: {AppLogger.GetCurrentLogFilePath()}");
            BuildAvaloniaApp().StartWithClassicDesktopLifetime(args);
            AppLogger.Info("Application exited normally.");
        }
        catch (Exception ex)
        {
            AppLogger.Error("Fatal exception during application startup/runtime", ex);
            throw;
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}