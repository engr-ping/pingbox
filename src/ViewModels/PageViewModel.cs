using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using Avalonia.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using PingBox.Models;
using PingBox.Services;

namespace PingBox.ViewModels;

/// <summary>
/// 页面ViewModel
/// </summary>
public partial class PageViewModel : ObservableObject
{
    private readonly IIconService _iconService;
    private readonly IProcessService _processService;
    private readonly Action _saveConfigCallback;
    private readonly Action? _postRunAction;
    private string _name;
    private string _backgroundImagePath;
    private bool _backgroundImageTiled;
    private string _foreColor;
    private string _backColor;

    public PageViewModel(PageInfo pageInfo, IIconService iconService, IProcessService processService, Action saveConfigCallback, Action? postRunAction = null)
    {
        _iconService = iconService;
        _processService = processService;
        _saveConfigCallback = saveConfigCallback;
        _postRunAction = postRunAction;

        _name = pageInfo.Name;
        _backgroundImagePath = pageInfo.BackgroundImagePath;
        _backgroundImageTiled = pageInfo.BackgroundImageTiled;
        _foreColor = pageInfo.ForeColor;
        _backColor = pageInfo.BackColor;

        Items = new ObservableCollection<PageItemViewModel>();
        foreach (var item in pageInfo.Items)
        {
            Items.Add(new PageItemViewModel(item, _iconService, _processService, this, _postRunAction));
        }
    }

    public string Name
    {
        get => _name;
        set => SetProperty(ref _name, value);
    }

    public string BackgroundImagePath
    {
        get => _backgroundImagePath;
        set => SetProperty(ref _backgroundImagePath, value);
    }

    public bool BackgroundImageTiled
    {
        get => _backgroundImageTiled;
        set => SetProperty(ref _backgroundImageTiled, value);
    }

    public string ForeColor
    {
        get => _foreColor;
        set => SetProperty(ref _foreColor, value);
    }

    public string BackColor
    {
        get => _backColor;
        set => SetProperty(ref _backColor, value);
    }

    public ObservableCollection<PageItemViewModel> Items { get; }

    public IEnumerable<PageItemViewModel> SoftwareItems => Items.Where(item => item.Type == PageItemType.Software);
    public IEnumerable<PageItemViewModel> FolderItems => Items.Where(item => item.Type == PageItemType.Folder);
    public IEnumerable<PageItemViewModel> FileItems => Items.Where(item => item.Type == PageItemType.File);

    public void AddItem(PageItem item)
    {
        var vm = new PageItemViewModel(item, _iconService, _processService, this, _postRunAction);
        Items.Add(vm);
    }

    public void RemoveItem(PageItemViewModel item)
    {
        Items.Remove(item);
        _saveConfigCallback?.Invoke();
    }

    public PageInfo ToPageInfo()
    {
        var page = new PageInfo
        {
            Name = this.Name,
            BackgroundImagePath = this.BackgroundImagePath,
            BackgroundImageTiled = this.BackgroundImageTiled,
            ForeColor = this.ForeColor,
            BackColor = this.BackColor
        };

        foreach (var itemVm in Items)
        {
            page.Items.Add(itemVm.ToPageItem());
        }

        return page;
    }
}