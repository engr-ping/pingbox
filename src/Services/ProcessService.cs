using System;
using System.Diagnostics;

namespace PingBox.Services;

/// <summary>
/// 进程服务实现
/// </summary>
public class ProcessService : IProcessService
{
    /// <summary>
    /// 运行程序
    /// </summary>
    public void Run(string path, string arguments = "", bool runAsAdmin = false)
    {
        if (string.IsNullOrEmpty(path))
            throw new ArgumentNullException(nameof(path));
        
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = path,
                Arguments = arguments,
                UseShellExecute = true,
                WorkingDirectory = GetDirectory(path)
            };
            
            if (runAsAdmin)
            {
                startInfo.Verb = "runas";
            }
            
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"无法运行程序: {path}", ex);
        }
    }
    
    /// <summary>
    /// 在资源管理器中显示文件
    /// </summary>
    public void ShowInExplorer(string filePath)
    {
        if (string.IsNullOrEmpty(filePath))
            throw new ArgumentNullException(nameof(filePath));
        
        try
        {
            var startInfo = new ProcessStartInfo
            {
                FileName = "Explorer.exe",
                Arguments = $"/e,/select,\"{filePath}\""
            };
            Process.Start(startInfo);
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"无法在资源管理器中显示: {filePath}", ex);
        }
    }
    
    /// <summary>
    /// 打开目录
    /// </summary>
    public void OpenDirectory(string directoryPath)
    {
        if (string.IsNullOrEmpty(directoryPath))
            throw new ArgumentNullException(nameof(directoryPath));
        
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = directoryPath,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            throw new InvalidOperationException($"无法打开目录: {directoryPath}", ex);
        }
    }
    
    private string GetDirectory(string path)
    {
        try
        {
            return System.IO.Path.GetDirectoryName(path) ?? string.Empty;
        }
        catch
        {
            return string.Empty;
        }
    }
}