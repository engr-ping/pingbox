using System;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using PingBox.Services;
using PingBox.ViewModels;

namespace PingBox.Views;

public partial class EditPageWindow : Window
{
    private TextBox? _txtName;
    private TextBox? _txtBgImage;
    private CheckBox? _chkTiled;

    public bool Confirmed { get; private set; }

    public EditPageWindow()
    {
        AppLogger.Info("EditPageWindow constructor entered.");
        InitializeComponent();
        ResolveControls();
        AppLogger.Info("EditPageWindow initialized.");
    }

    public EditPageWindow(PageViewModel page) : this()
    {
        AppLogger.Info($"Binding EditPageWindow to page '{page.Name}'.");
        _txtName!.Text = page.Name;
        _txtBgImage!.Text = page.BackgroundImagePath;
        _chkTiled!.IsChecked = page.BackgroundImageTiled;
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ResolveControls()
    {
        _txtName = this.FindControl<TextBox>("txtName");
        _txtBgImage = this.FindControl<TextBox>("txtBgImage");
        _chkTiled = this.FindControl<CheckBox>("chkTiled");

        if (_txtName == null || _txtBgImage == null || _chkTiled == null)
        {
            throw new InvalidOperationException(
                $"EditPageWindow controls not found. txtName={_txtName != null}, txtBgImage={_txtBgImage != null}, chkTiled={_chkTiled != null}");
        }
    }

    private async void BrowseImage_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            AppLogger.Info("EditPageWindow BrowseImage clicked.");
            var options = new FilePickerOpenOptions
            {
                Title = "选择背景图片",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("图片文件")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.bmp", "*.gif", "*.webp" }
                    },
                    FilePickerFileTypes.All
                }
            };
            var result = await StorageProvider.OpenFilePickerAsync(options);
            if (result.Count > 0)
            {
                var path = result[0].TryGetLocalPath();
                if (!string.IsNullOrEmpty(path))
                {
                    AppLogger.Info($"EditPageWindow selected background image: {path}");
                    _txtBgImage!.Text = path;
                }
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("EditPageWindow BrowseImage failed", ex);
            System.Diagnostics.Debug.WriteLine($"BrowseImage error: {ex}");
            throw;
        }
    }

    private void Confirm_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppLogger.Info($"EditPageWindow confirm clicked. Name='{_txtName!.Text}', BgImage='{_txtBgImage!.Text}', Tiled={_chkTiled!.IsChecked}");
        Confirmed = true;
        Close();
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppLogger.Info("EditPageWindow canceled.");
        Confirmed = false;
        Close();
    }

    public void ApplyTo(PageViewModel page)
    {
        AppLogger.Info($"Applying EditPageWindow changes to page '{page.Name}'.");
        var name = _txtName!.Text?.Trim();
        if (!string.IsNullOrEmpty(name))
            page.Name = name;
        page.BackgroundImagePath = _txtBgImage!.Text?.Trim() ?? string.Empty;
        page.BackgroundImageTiled = _chkTiled!.IsChecked ?? false;
        AppLogger.Info($"Applied page changes. Name='{page.Name}', BgImage='{page.BackgroundImagePath}', Tiled={page.BackgroundImageTiled}");
    }
}
