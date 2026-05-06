using System;
using System.Reflection;
using System.Runtime.InteropServices;

namespace PingBox.Services;

/// <summary>
/// 管理 Windows 开机自启动（注册表）
/// </summary>
public static class AutoStartService
{
    private const string RegKeyPath = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";
    private const string AppName = "PingBox";

    public static bool IsEnabled()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return false;
        }

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegKeyPath, false);
            return key?.GetValue(AppName) != null;
        }
        catch
        {
            return false;
        }
    }

    public static void Enable()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        try
        {
            var exePath = Environment.ProcessPath ?? Assembly.GetExecutingAssembly().Location;
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            key?.SetValue(AppName, $"\"{exePath}\"");
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"设置自启动失败: {ex.Message}");
        }
    }

    public static void Disable()
    {
        if (!RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
        {
            return;
        }

        try
        {
            using var key = Microsoft.Win32.Registry.CurrentUser.OpenSubKey(RegKeyPath, true);
            key?.DeleteValue(AppName, throwOnMissingValue: false);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"取消自启动失败: {ex.Message}");
        }
    }
}
