using System;
using System.IO;
using System.Text;

namespace PingBox.Services;

public static class AppLogger
{
    private static readonly object SyncRoot = new();
    private static readonly string LogDirectory = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
        "PingBox",
        "logs");

    private static string LogFilePath => Path.Combine(LogDirectory, $"pingbox-{DateTime.Now:yyyyMMdd}.log");

    public static void Info(string message) => Write("INFO", message, null);

    public static void Warn(string message) => Write("WARN", message, null);

    public static void Error(string message, Exception? exception = null) => Write("ERROR", message, exception);

    private static void Write(string level, string message, Exception? exception)
    {
        try
        {
            Directory.CreateDirectory(LogDirectory);

            var builder = new StringBuilder();
            builder.Append('[')
                .Append(DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss.fff"))
                .Append("] [")
                .Append(level)
                .Append("] ")
                .AppendLine(message);

            if (exception != null)
            {
                builder.AppendLine(exception.ToString());
            }

            lock (SyncRoot)
            {
                File.AppendAllText(LogFilePath, builder.ToString(), Encoding.UTF8);
            }
        }
        catch
        {
            // Logging must never crash the app.
        }
    }

    public static string GetCurrentLogFilePath() => LogFilePath;
}