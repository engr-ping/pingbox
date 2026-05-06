using System;
using System.Linq;
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

        if (viewModel.Width > 0 && viewModel.Height > 0)
        {
            Width = viewModel.Width;
            Height = viewModel.Height;
        }

        viewModel.RequestHideWindow += () => Hide();
        Opened += MainWindow_Opened;
        Closing += MainWindow_Closing;
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MainWindow_Opened(object? sender, EventArgs e)
    {
        if (DataContext is MainViewModel viewModel && viewModel.HideOnStart)
        {
            Hide();
        }
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        if (DataContext is MainViewModel viewModel && viewModel.NoExit)
        {
            e.Cancel = true;
            Hide();
        }
    }

    private void ShowSettingsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            var settingsWindow = new SettingsWindow(viewModel);
            settingsWindow.ShowDialog(this);
        }
    }

    private void ShowAboutButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        var aboutWindow = new AboutWindow();
        aboutWindow.ShowDialog(this);
    }

    private void MainWindow_DragOver(object? sender, DragEventArgs e)
    {
        if (e.Data.Contains(DataFormats.FileNames))
        {
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private void MainWindow_Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is MainViewModel viewModel && e.Data.Contains(DataFormats.FileNames))
        {
            var files = e.Data.GetFileNames()?.ToArray();
            if (files != null && files.Length > 0)
            {
                viewModel.AddFiles(files);
            }
            e.Handled = true;
        }
    }

    private void PageItem_PointerPressed(object? sender, PointerPressedEventArgs e)
    {
        if (sender is Control control && control.ContextMenu is ContextMenu menu)
        {
            var point = e.GetCurrentPoint(control);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonPressed)
            {
                menu.PlacementTarget = control;
                menu.Open(control);
                e.Handled = true;
            }
        }
    }

    private void PageItem_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (sender is Control control && control.ContextMenu is ContextMenu menu)
        {
            var point = e.GetCurrentPoint(control);
            if (point.Properties.PointerUpdateKind == PointerUpdateKind.RightButtonReleased)
            {
                menu.PlacementTarget = control;
                menu.Open(control);
                e.Handled = true;
            }
        }
    }
}
