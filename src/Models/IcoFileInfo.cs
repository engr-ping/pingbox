using System;
using System.IO;

namespace PingBox.Models;

/// <summary>
/// 文件信息类，用于存储快捷方式的相关信息
/// </summary>
public class IcoFileInfo
{
    public IcoFileInfo(string? filename)
    {
        FullName = filename ?? string.Empty;
        Args = string.Empty;
        RunAsAdmin = false;
        
        // 安全处理文件名提取
        if (string.IsNullOrEmpty(filename))
        {
            NameWithExtension = string.Empty;
            Name = string.Empty;
        }
        else
        {
            try
            {
                NameWithExtension = Path.GetFileName(filename);
                if (string.IsNullOrEmpty(NameWithExtension))
                {
                    Name = string.Empty;
                }
                else
                {
                    // 移除扩展名
                    string nameWithoutExt = NameWithExtension.Trim('.');
                    if (nameWithoutExt.Contains('.'))
                    {
                        int lastDotIndex = nameWithoutExt.LastIndexOf('.');
                        Name = nameWithoutExt.Substring(0, lastDotIndex);
                    }
                    else
                    {
                        Name = nameWithoutExt;
                    }
                }
            }
            catch
            {
                NameWithExtension = filename;
                Name = filename;
            }
        }
    }

    /// <summary>
    /// 显示名称（不含扩展名）
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 带扩展名的文件名
    /// </summary>
    public string NameWithExtension { get; set; }
    
    /// <summary>
    /// 完整路径
    /// </summary>
    public string FullName { get; set; }
    
    /// <summary>
    /// 命令行参数
    /// </summary>
    public string Args { get; set; }
    
    /// <summary>
    /// 是否以管理员权限运行
    /// </summary>
    public bool RunAsAdmin { get; set; }
}