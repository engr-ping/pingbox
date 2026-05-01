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
    private string _name;
    private string _fullPath;
    private string _arguments;
    private bool _runAsAdmin;
    private Bitmap? _icon;

    public PageItemViewModel(PageItem item, IIconService iconService, IProcessService processService, PageViewModel parentPage)
    {
        _iconService = iconService;
        _processService = processService;
        _parentPage = parentPage;

        _name = item.Name;
        _fullPath = item.FullPath;
        _arguments = item.Arguments;
        _runAsAdmin = item.RunAsAdmin;

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
            RunAsAdmin = this.RunAsAdmin
        };
    }
}