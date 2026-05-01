using System.Collections.ObjectModel;

namespace PingBox.Models;

/// <summary>
/// 页面信息类，表示一个标签页
/// </summary>
public class PageInfo
{
    public PageInfo()
    {
        Name = string.Empty;
        BackgroundImagePath = string.Empty;
        BackgroundImageTiled = false;
        Items = new ObservableCollection<PageItem>();
        ForeColor = "#000000";
        BackColor = "#FFFFFF";
    }

    public PageInfo(string name) : this()
    {
        Name = name;
    }

    /// <summary>
    /// 页面名称
    /// </summary>
    public string Name { get; set; }
    
    /// <summary>
    /// 背景图片路径
    /// </summary>
    public string BackgroundImagePath { get; set; }
    
    /// <summary>
    /// 背景图片是否平铺
    /// </summary>
    public bool BackgroundImageTiled { get; set; }
    
    /// <summary>
    /// 前景色（文字颜色）
    /// </summary>
    public string ForeColor { get; set; }
    
    /// <summary>
    /// 背景色
    /// </summary>
    public string BackColor { get; set; }
    
    /// <summary>
    /// 页面中的快捷方式列表
    /// </summary>
    public ObservableCollection<PageItem> Items { get; set; }
}