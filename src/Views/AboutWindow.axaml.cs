using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PingBox;

namespace PingBox.Views;

public partial class AboutWindow : Window
{
    public AboutWindow()
    {
        InitializeComponent();
        LoadVersionInfo();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadVersionInfo()
    {
        var assembly = typeof(App).Assembly;
        var version = assembly.GetName().Version?.ToString() ?? "1.0.0";
        if (txtVersion != null)
        {
            txtVersion.Text = $"版本: {version}";
        }
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }
}