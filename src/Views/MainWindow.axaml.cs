using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using PingBox.ViewModels;

namespace PingBox.Views;

public partial class MainWindow : Window
{
    public MainWindow()
    {
        InitializeComponent();
    }

    public MainWindow(MainViewModel viewModel) : this()
    {
        DataContext = viewModel;
        
        // 设置窗口位置
        if (viewModel.Width > 0 && viewModel.Height > 0)
        {
            Width = viewModel.Width;
            Height = viewModel.Height;
        }
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void ShowSettingsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var settingsWindow = new SettingsWindow();
        settingsWindow.ShowDialog(this);
    }

    private void ShowAboutButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }
}