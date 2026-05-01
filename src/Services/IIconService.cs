using Avalonia.Media.Imaging;

namespace PingBox.Services;

/// <summary>
/// 图标服务接口
/// </summary>
public interface IIconService
{
    /// <summary>
    /// 获取文件/文件夹图标
    /// </summary>
    /// <param name="path">文件/文件夹路径</param>
    /// <param name="size">图标大小（16, 32, 48等）</param>
    /// <returns>图标位图，如果获取失败则返回null</returns>
    Bitmap? GetIcon(string path, int size = 48);
    
    /// <summary>
    /// 获取默认文件图标
    /// </summary>
    /// <param name="size">图标大小</param>
    /// <returns>默认文件图标</returns>
    Bitmap GetDefaultFileIcon(int size = 48);
    
    /// <summary>
    /// 获取默认文件夹图标
    /// </summary>
    /// <param name="size">图标大小</param>
    /// <returns>默认文件夹图标</returns>
    Bitmap GetDefaultFolderIcon(int size = 48);
}