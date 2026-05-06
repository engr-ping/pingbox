using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using PingBox.Services;
using PingBox.ViewModels;

namespace PingBox.Views;

public partial class EditItemWindow : Window
{
    private TextBox? _txtName;
    private TextBox? _txtPath;
    private TextBox? _txtArgs;
    private CheckBox? _chkRunAsAdmin;

    public bool Confirmed { get; private set; }

    public EditItemWindow()
    {
        AppLogger.Info("EditItemWindow constructor entered.");
        InitializeComponent();
        ResolveControls();
        AppLogger.Info("EditItemWindow initialized.");
    }

    public EditItemWindow(PageItemViewModel item) : this()
    {
        AppLogger.Info($"Binding EditItemWindow to item '{item.Name}'.");
        _txtName!.Text = item.Name;
        _txtPath!.Text = item.FullPath;
        _txtArgs!.Text = item.Arguments;
        _chkRunAsAdmin!.IsChecked = item.RunAsAdmin;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ResolveControls()
    {
        _txtName = this.FindControl<TextBox>("txtName");
        _txtPath = this.FindControl<TextBox>("txtPath");
        _txtArgs = this.FindControl<TextBox>("txtArgs");
        _chkRunAsAdmin = this.FindControl<CheckBox>("chkRunAsAdmin");

        if (_txtName == null || _txtPath == null || _txtArgs == null || _chkRunAsAdmin == null)
        {
            throw new InvalidOperationException(
                $"EditItemWindow controls not found. txtName={_txtName != null}, txtPath={_txtPath != null}, txtArgs={_txtArgs != null}, chkRunAsAdmin={_chkRunAsAdmin != null}");
        }
    }

    private async void BrowsePath_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppLogger.Info("EditItemWindow BrowsePath clicked.");
        var options = new FilePickerOpenOptions { Title = "选择文件", AllowMultiple = false };
        var result = await StorageProvider.OpenFilePickerAsync(options);
        if (result.Count > 0)
        {
            var path = result[0].TryGetLocalPath();
            if (!string.IsNullOrEmpty(path))
            {
                AppLogger.Info($"EditItemWindow selected path: {path}");
                _txtPath!.Text = path;
                if (string.IsNullOrEmpty(_txtName!.Text))
                    _txtName.Text = Path.GetFileNameWithoutExtension(path);
            }
        }
    }

    private void Confirm_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppLogger.Info($"EditItemWindow confirm clicked. Name='{_txtName!.Text}', Path='{_txtPath!.Text}', RunAsAdmin={_chkRunAsAdmin!.IsChecked}");
        Confirmed = true;
        Close();
    }

    private void Cancel_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        AppLogger.Info("EditItemWindow canceled.");
        Confirmed = false;
        Close();
    }

    public void ApplyTo(PageItemViewModel item)
    {
        AppLogger.Info($"Applying EditItemWindow changes to item '{item.Name}'.");
        item.Name = _txtName!.Text ?? item.Name;
        item.FullPath = _txtPath!.Text ?? item.FullPath;
        item.Arguments = _txtArgs!.Text ?? string.Empty;
        item.RunAsAdmin = _chkRunAsAdmin!.IsChecked ?? false;
        AppLogger.Info($"Applied item changes. Name='{item.Name}', Path='{item.FullPath}', RunAsAdmin={item.RunAsAdmin}");
    }
}
