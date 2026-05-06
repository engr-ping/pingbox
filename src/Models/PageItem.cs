namespace PingBox.Models;

/// <summary>
/// 页面项类，表示一个快捷方式
/// </summary>
public class PageItem
{
    public PageItem()
    {
        Name = string.Empty;
        FullPath = string.Empty;
        Arguments = string.Empty;
        RunAsAdmin = false;
        Type = PageItemType.File;
    }

    public PageItem(string name, string fullPath, PageItemType type = PageItemType.File) : this()
    {
        Name = name;
        FullPath = fullPath;
        Type = type;
    }

    /// <summary>
    /// 显示名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 完整路径
    /// </summary>
    public string FullPath { get; set; }
    
    /// <summary>
    /// 命令行参数
    /// </summary>
    public string Arguments { get; set; }
    
    /// <summary>
    /// 是否以管理员权限运行
    /// </summary>
    public bool RunAsAdmin { get; set; }

    /// <summary>
    /// 项目类型
    /// </summary>
    public PageItemType Type { get; set; }
    
    /// <summary>
    /// 创建副本
    /// </summary>
    public PageItem Clone()
    {
        return new PageItem
        {
            Name = this.Name,
            FullPath = this.FullPath,
            Arguments = this.Arguments,
            RunAsAdmin = this.RunAsAdmin,
            Type = this.Type
        };
    }
}