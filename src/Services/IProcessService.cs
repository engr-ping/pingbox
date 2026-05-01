namespace PingBox.Services;

/// <summary>
/// 进程服务接口
/// </summary>
public interface IProcessService
{
    /// <summary>
    /// 运行程序
    /// </summary>
    /// <param name="path">程序路径</param>
    /// <param name="arguments">命令行参数</param>
    /// <param name="runAsAdmin">是否以管理员权限运行</param>
    void Run(string path, string arguments = "", bool runAsAdmin = false);
    
    /// <summary>
    /// 在资源管理器中显示文件
    /// </summary>
    /// <param name="filePath">文件路径</param>
    void ShowInExplorer(string filePath);
    
    /// <summary>
    /// 打开目录
    /// </summary>
    /// <param name="directoryPath">目录路径</param>
    void OpenDirectory(string directoryPath);
}