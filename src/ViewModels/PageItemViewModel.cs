using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PingBox.Models;
using PingBox.Services;

namespace PingBox.ViewModels;

/// <summary>
/// 页面项ViewModel
/// </summary>
public partial class PageItemViewModel : ObservableObject
{
    private readonly IIconService _iconService;
    private readonly IProcessService _processService;
    private readonly PageViewModel _parentPage;
    private readonly Action? _postRunAction;
    private string _name;
    private string _fullPath;
    private string _arguments;
    private bool _runAsAdmin;
    private PageItemType _type;
    private Bitmap? _icon;

    public PageItemViewModel(PageItem item, IIconService iconService, IProcessService processService, PageViewModel parentPage, Action? postRunAction = null)
    {
        _iconService = iconService;
        _processService = processService;
        _parentPage = parentPage;
        _postRunAction = postRunAction;

        _name = item.Name;
        _fullPath = item.FullPath;
        _arguments = item.Arguments;
        _runAsAdmin = item.RunAsAdmin;
        _type = item.Type;

        // 加载图标
        if (!string.IsNullOrEmpty(_fullPath))
        {
            _icon = _iconService.GetIcon(_fullPath, 48);
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string FullPath
    {
        get => _fullPath;
        set
        {
            if (SetProperty(ref _fullPath, value))
            {
                // 重新加载图标
                _icon?.Dispose();
                _icon = !string.IsNullOrEmpty(value) ? _iconService.GetIcon(value, 48) : null;
                OnPropertyChanged(nameof(Icon));
            }
        }
    }

    public string Arguments
    {
        get => _arguments;
        set => SetProperty(ref _arguments, value);
    }

    public bool RunAsAdmin
    {
        get => _runAsAdmin;
        set => SetProperty(ref _runAsAdmin, value);
    }

    public PageItemType Type
    {
        get => _type;
        set => SetProperty(ref _type, value);
    }

    public string CategoryLabel => Type switch
    {
        PageItemType.Software => "软件",
        PageItemType.Folder => "文件夹",
        PageItemType.File => "文件",
        _ => "文件"
    };

    public Bitmap? Icon => _icon;

    /// <summary>
    /// 运行程序
    /// </summary>
    [RelayCommand]
    private void Run()
    {
        if (!string.IsNullOrEmpty(_fullPath))
        {
            try
            {
                _processService.Run(_fullPath, _arguments, _runAsAdmin);
                _postRunAction?.Invoke();
            }
            catch (Exception ex)
            {
                // 可以在这里添加错误处理逻辑
                System.Diagnostics.Debug.WriteLine($"运行失败: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 在资源管理器中显示
    /// </summary>
    [RelayCommand]
    private void ShowInExplorer()
    {
        if (!string.IsNullOrEmpty(_fullPath))
        {
            try
            {
                _processService.ShowInExplorer(_fullPath);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"显示失败: {ex.Message}");
            }
        }
    }

    /// <summary>
    /// 删除项
    /// </summary>
    [RelayCommand]
    private void Delete()
    {
        _parentPage.RemoveItem(this);
    }

    public PageItem ToPageItem()
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