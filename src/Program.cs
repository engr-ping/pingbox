using Avalonia;
using System;
using System.Threading;
using System.Threading.Tasks;
using PingBox.Services;

namespace PingBox;

class Program
{
    private const string SingleInstanceMutexName = "Local\\PingBox.SingleInstance.Mutex";
    private const string ActivateSignalName = "Local\\PingBox.SingleInstance.Activate";

    [STAThread]
    public static void Main(string[] args)
    {
        using var singleInstanceMutex = new Mutex(true, SingleInstanceMutexName, out var isFirstInstance);
        using var activateSignal = new EventWaitHandle(false, EventResetMode.AutoReset, ActivateSignalName);

        if (!isFirstInstance)
        {
            // Notify the running instance to show/activate its main window.
            activateSignal.Set();
            return;
        }

        using var signalCts = new CancellationTokenSource();
        _ = Task.Run(() => ListenForActivationSignal(activateSignal, signalCts.Token));

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
        }
        finally
        {
            signalCts.Cancel();
        }
    }

    private static void ListenForActivationSignal(EventWaitHandle activateSignal, CancellationToken token)
    {
        while (!token.IsCancellationRequested)
        {
            try
            {
                if (activateSignal.WaitOne(400))
                {
                    App.ActivateMainWindowFromSignal();
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Single-instance activation signal listener failed", ex);
            }
        }
    }

    // Avalonia configuration, don't remove; also used by visual designer.
    public static AppBuilder BuildAvaloniaApp()
        => AppBuilder.Configure<App>()
            .UsePlatformDetect()
            .WithInterFont()
            .LogToTrace();
}