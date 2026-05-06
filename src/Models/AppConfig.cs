using System.Collections.ObjectModel;

namespace PingBox.Models;

/// <summary>
/// 应用配置类
/// </summary>
public class AppConfig
{
    public AppConfig()
    {
        Title = "PingBox";
        Width = 1280;
        Height = 820;
        LocationX = -1;
        LocationY = -1;
        StartMaximized = false;
        ShowInTaskbar = true;
        HideOnStart = false;
        HideOnRun = false;
        TopMost = false;
        NoExit = false;
        NoReadLnk = false;
        DoubleClickToRun = true;
        LineSpacing = 75;
        ColumnSpacing = 75;
        TabButtonWidth = 55;
        TabButtonHeight = 23;
        TabButtonDock = 1; // 1=Left, 2=Right, 3=Top, 4=Bottom
        TabButtonBackColor1 = "#FFFFFF";
        TabButtonBackColor2 = "#ADD8E6"; // LightBlue
        TabButtonForeColor1 = "#000000";
        TabButtonForeColor2 = "#000000";
        SelectedTabIndex = 0;
        HotkeyEnabled = false;
        HotkeyModifier = "Ctr";
        HotkeyKey = "F2 键";
        AutoStart = false;
        ViewMode = ViewMode.LargeIcon;
        Pages = new ObservableCollection<PageInfo>();
    }

    /// <summary>
    /// 窗口标题
    /// </summary>
    public string Title { get; set; }
    
    /// <summary>
    /// 窗口宽度
    /// </summary>
    public int Width { get; set; }
    
    /// <summary>
    /// 窗口高度
    /// </summary>
    public int Height { get; set; }
    
    /// <summary>
    /// 窗口X位置
    /// </summary>
    public int LocationX { get; set; }
    
    /// <summary>
    /// 窗口Y位置
    /// </summary>
    public int LocationY { get; set; }

    /// <summary>
    /// 启动时是否最大化（恢复上次关闭状态）
    /// </summary>
    public bool StartMaximized { get; set; }
    
    /// <summary>
    /// 是否显示在任务栏
    /// </summary>
    public bool ShowInTaskbar { get; set; }
    
    /// <summary>
    /// 启动时是否隐藏
    /// </summary>
    public bool HideOnStart { get; set; }
    
    /// <summary>
    /// 运行程序后是否隐藏
    /// </summary>
    public bool HideOnRun { get; set; }
    
    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool TopMost { get; set; }
    
    /// <summary>
    /// 是否禁止退出（点击关闭按钮时隐藏而不是退出）
    /// </summary>
    public bool NoExit { get; set; }
    
    /// <summary>
    /// 是否不读取.lnk快捷方式
    /// </summary>
    public bool NoReadLnk { get; set; }
    
    /// <summary>
    /// 是否双击运行（否则单击运行）
    /// </summary>
    public bool DoubleClickToRun { get; set; }
    
    /// <summary>
    /// 图标行间距
    /// </summary>
    public int LineSpacing { get; set; }
    
    /// <summary>
    /// 图标列间距
    /// </summary>
    public int ColumnSpacing { get; set; }
    
    /// <summary>
    /// 标签按钮宽度
    /// </summary>
    public int TabButtonWidth { get; set; }
    
    /// <summary>
    /// 标签按钮高度
    /// </summary>
    public int TabButtonHeight { get; set; }
    
    /// <summary>
    /// 标签按钮停靠位置 (1=Left, 2=Right, 3=Top, 4=Bottom)
    /// </summary>
    public int TabButtonDock { get; set; }
    
    /// <summary>
    /// 标签按钮背景色1（默认）
    /// </summary>
    public string TabButtonBackColor1 { get; set; }
    
    /// <summary>
    /// 标签按钮背景色2（选中）
    /// </summary>
    public string TabButtonBackColor2 { get; set; }
    
    /// <summary>
    /// 标签按钮前景色1（默认）
    /// </summary>
    public string TabButtonForeColor1 { get; set; }
    
    /// <summary>
    /// 标签按钮前景色2（选中）
    /// </summary>
    public string TabButtonForeColor2 { get; set; }
    
    /// <summary>
    /// 选中的标签页索引
    /// </summary>
    public int SelectedTabIndex { get; set; }
    
    /// <summary>
    /// 热键是否启用
    /// </summary>
    public bool HotkeyEnabled { get; set; }
    
    /// <summary>
    /// 热键修饰键 (Ctr, Shift, Alt, Ctr+Shift, Ctr+Alt, Alt+Shift, Ctr+Shift+Alt)
    /// </summary>
    public string HotkeyModifier { get; set; }
    
    /// <summary>
    /// 热键按键
    /// </summary>
    public string HotkeyKey { get; set; }
    
    /// <summary>
    /// 热键配置数组（用于兼容性）
    /// </summary>
    public string[]? Hotkey { get; set; }

    /// <summary>
    /// 是否开机自启动
    /// </summary>
    public bool AutoStart { get; set; }

    /// <summary>
    /// 视图模式
    /// </summary>
    public ViewMode ViewMode { get; set; }
    
    /// <summary>
    /// 页面列表
    /// </summary>
    public ObservableCollection<PageInfo> Pages { get; set; }
}