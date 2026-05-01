using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Input;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PingBox.Models;
using PingBox.Services;

namespace PingBox.ViewModels;

/// <summary>
/// 主窗口ViewModel
/// </summary>
public partial class MainViewModel : ViewModelBase
{
    private readonly IConfigService _configService;
    private readonly IProcessService _processService;
    private readonly IIconService _iconService;
    private AppConfig _config;
    private int _selectedIndex;
    private bool _isWindowVisible;

    public MainViewModel(
        IConfigService configService,
        IProcessService processService,
        IIconService iconService)
    {
        _configService = configService;
        _processService = processService;
        _iconService = iconService;

        _config = _configService.Load();
        _selectedIndex = Math.Max(0, Math.Min(_config.SelectedTabIndex, _config.Pages.Count - 1));
        _isWindowVisible = true;

        Pages = new ObservableCollection<PageViewModel>();
        foreach (var page in _config.Pages)
        {
            Pages.Add(new PageViewModel(page, _iconService, _processService, SaveConfig));
        }

        // 如果没有页面，创建一个默认页面
        if (Pages.Count == 0)
        {
            AddPage("默认");
        }

        // 初始化命令
        InitializeCommands();
    }

    private void InitializeCommands()
    {
        ShowSettingsCommand = new RelayCommand(ShowSettings);
        ShowAboutCommand = new RelayCommand(ShowAbout);
        ExitCommand = new RelayCommand(ExitApplication);
        AddPageCommand = new RelayCommand(() => AddPage("新页面"));
        AddProgramCommand = new RelayCommand(AddProgram);
        MinimizeToTrayCommand = new RelayCommand(MinimizeToTray);
    }

    #region Properties

    /// <summary>
    /// 窗口标题
    /// </summary>
    public string Title => _config.Title;

    /// <summary>
    /// 页面列表
    /// </summary>
    public ObservableCollection<PageViewModel> Pages { get; }

    /// <summary>
    /// 当前选中的页面索引
    /// </summary>
    public int SelectedIndex
    {
        get => _selectedIndex;
        set => SetProperty(ref _selectedIndex, value);
    }

    /// <summary>
    /// 窗口宽度
    /// </summary>
    public int Width => _config.Width;

    /// <summary>
    /// 窗口高度
    /// </summary>
    public int Height => _config.Height;

    /// <summary>
    /// 是否显示在任务栏
    /// </summary>
    public bool ShowInTaskbar => _config.ShowInTaskbar;

    /// <summary>
    /// 是否置顶
    /// </summary>
    public bool TopMost => _config.TopMost;

    /// <summary>
    /// 启动时是否隐藏
    /// </summary>
    public bool HideOnStart => _config.HideOnStart;

    /// <summary>
    /// 窗口是否可见
    /// </summary>
    public bool IsWindowVisible
    {
        get => _isWindowVisible;
        set => SetProperty(ref _isWindowVisible, value);
    }

    #endregion

    #region Commands

    public ICommand ShowSettingsCommand { get; private set; }
    public ICommand ShowAboutCommand { get; private set; }
    public ICommand ExitCommand { get; private set; }
    public ICommand AddPageCommand { get; private set; }
    public ICommand AddProgramCommand { get; private set; }
    public ICommand MinimizeToTrayCommand { get; private set; }

    /// <summary>
    /// 添加文件命令
    /// </summary>
    [RelayCommand]
    public void AddFiles(string[] files)
    {
        if (SelectedIndex < 0 || SelectedIndex >= Pages.Count)
            return;

        foreach (var file in files)
        {
            if (File.Exists(file) || Directory.Exists(file))
            {
                var item = new PageItem
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    FullPath = file,
                    Arguments = "",
                    RunAsAdmin = false
                };
                Pages[SelectedIndex].AddItem(item);
            }
        }

        SaveConfig();
    }

    /// <summary>
    /// 添加程序命令
    /// </summary>
    private async void AddProgram()
    {
        var dialog = new OpenFileDialog
        {
            Title = "选择要添加的程序",
            AllowMultiple = true,
            Filters = new List<FileDialogFilter>
            {
                new FileDialogFilter { Name = "可执行文件", Extensions = { "exe", "bat", "cmd", "lnk" } },
                new FileDialogFilter { Name = "所有文件", Extensions = { "*" } }
            }
        };

        // 获取主窗口
        var mainWindow = Avalonia.Application.Current?.ApplicationLifetime is Avalonia.Controls.ApplicationLifetimes.ClassicDesktopStyleApplicationLifetime desktop
            ? desktop.MainWindow
            : null;

        if (mainWindow != null)
        {
            var result = await dialog.ShowAsync(mainWindow);
            if (result != null && result.Length > 0)
            {
                AddFiles(result);
            }
        }
    }

    /// <summary>
    /// 添加页面
    /// </summary>
    public void AddPage(string name)
    {
        if (string.IsNullOrEmpty(name))
            return;

        var page = new PageInfo(name);
        var pageVm = new PageViewModel(page, _iconService, _processService, SaveConfig);
        Pages.Add(pageVm);
        SelectedIndex = Pages.Count - 1;

        SaveConfig();
    }

    /// <summary>
    /// 删除页面命令
    /// </summary>
    [RelayCommand(CanExecute = nameof(CanRemovePage))]
    private void RemovePage()
    {
        if (SelectedIndex >= 0 && SelectedIndex < Pages.Count && Pages.Count > 1)
        {
            Pages.RemoveAt(SelectedIndex);
            if (SelectedIndex >= Pages.Count)
            {
                SelectedIndex = Pages.Count - 1;
            }
            SaveConfig();
        }
    }

    private bool CanRemovePage() => Pages.Count > 1 && SelectedIndex >= 0;

    /// <summary>
    /// 重命名页面命令
    /// </summary>
    [RelayCommand]
    private void RenamePage(string newName)
    {
        if (SelectedIndex >= 0 && SelectedIndex < Pages.Count && !string.IsNullOrEmpty(newName))
        {
            Pages[SelectedIndex].Name = newName;
            SaveConfig();
        }
    }

    /// <summary>
    /// 切换窗口可见性
    /// </summary>
    public void ToggleWindowVisibility()
    {
        IsWindowVisible = !IsWindowVisible;
    }

    /// <summary>
    /// 最小化到托盘
    /// </summary>
    private void MinimizeToTray()
    {
        IsWindowVisible = false;
    }

    /// <summary>
    /// 显示设置窗口
    /// </summary>
    private void ShowSettings()
    {
        // 通过消息或服务打开设置窗口
        // 实际应用中应该使用对话框服务
    }

    /// <summary>
    /// 显示关于窗口
    /// </summary>
    private void ShowAbout()
    {
        // 通过消息或服务打开关于窗口
        // 实际应用中应该使用对话框服务
    }

    /// <summary>
    /// 退出应用程序
    /// </summary>
    private void ExitApplication()
    {
        // 保存配置
        SaveConfig();

        // 发送退出消息
        // 实际应用中应该使用ApplicationLifetime来退出
    }

    /// <summary>
    /// 保存配置
    /// </summary>
    public void SaveConfig()
    {
        // 更新配置
        _config.Pages.Clear();
        foreach (var pageVm in Pages)
        {
            _config.Pages.Add(pageVm.ToPageInfo());
        }
        _config.SelectedTabIndex = SelectedIndex;

        _configService.Save(_config);
    }

    /// <summary>
    /// 获取配置
    /// </summary>
    public AppConfig GetConfig()
    {
        // 同步ViewModel状态到配置
        _config.Pages.Clear();
        foreach (var pageVm in Pages)
        {
            _config.Pages.Add(pageVm.ToPageInfo());
        }
        _config.SelectedTabIndex = SelectedIndex;

        return _config;
    }

    #endregion
}