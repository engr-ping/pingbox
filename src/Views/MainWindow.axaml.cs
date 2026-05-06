using System;
using System.Linq;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Platform.Storage;
using Avalonia.VisualTree;
using PingBox.Services;
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
        AppLogger.Info("MainWindow constructor entered.");
        DataContext = viewModel;

        if (viewModel.Width > 0 && viewModel.Height > 0)
        {
            Width = viewModel.Width;
            Height = viewModel.Height;
        }

        if (viewModel.LocationX >= 0 && viewModel.LocationY >= 0)
        {
            WindowStartupLocation = WindowStartupLocation.Manual;
            Position = new PixelPoint(viewModel.LocationX, viewModel.LocationY);
        }

        if (viewModel.StartMaximized)
        {
            WindowState = WindowState.Maximized;
        }

        viewModel.RequestHideWindow += () => Hide();
        viewModel.RequestShowWindow += () =>
        {
            Show();
            WindowState = WindowState.Normal;
            Activate();
        };
        viewModel.RequestExit += () =>
        {
            if (Avalonia.Application.Current?.ApplicationLifetime is
                Avalonia.Controls.ApplicationLifetimes.IClassicDesktopStyleApplicationLifetime desktop)
            {
                desktop.Shutdown();
            }
        };
        Opened += MainWindow_Opened;
        Closing += MainWindow_Closing;
        AppLogger.Info("MainWindow constructor completed.");
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void MainWindow_Opened(object? sender, EventArgs e)
    {
        AppLogger.Info("MainWindow opened.");
        if (DataContext is MainViewModel viewModel)
        {
            ApplyViewModeButtonState(viewModel.ViewMode);

            if (viewModel.HideOnStart)
            {
                AppLogger.Info("MainWindow hidden on start due to configuration.");
                Hide();
            }
        }
    }

    private void MainWindow_Closing(object? sender, WindowClosingEventArgs e)
    {
        AppLogger.Info("MainWindow closing requested.");
        if (DataContext is MainViewModel viewModelForSave)
        {
            if (WindowState == WindowState.Maximized)
            {
                // 最大化时保留上次普通窗口尺寸与位置，仅更新状态。
                viewModelForSave.UpdateWindowPlacement(
                    viewModelForSave.Width,
                    viewModelForSave.Height,
                    viewModelForSave.LocationX,
                    viewModelForSave.LocationY,
                    true);
            }
            else
            {
                var boundsToSave = Bounds;
                viewModelForSave.UpdateWindowPlacement(
                    (int)Math.Round(boundsToSave.Width),
                    (int)Math.Round(boundsToSave.Height),
                    (int)Math.Round(boundsToSave.X),
                    (int)Math.Round(boundsToSave.Y),
                    false);
            }
            viewModelForSave.SaveConfig();
        }

        if (DataContext is MainViewModel viewModel && viewModel.NoExit)
        {
            e.Cancel = true;
            AppLogger.Info("MainWindow close canceled because NoExit is enabled.");
            Hide();
        }
    }

    private void ShowSettingsButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            AppLogger.Info("ShowSettingsButton clicked.");
            if (DataContext is MainViewModel viewModel)
            {
                var settingsWindow = new SettingsWindow(viewModel);
                settingsWindow.ShowDialog(this);
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed while opening SettingsWindow", ex);
        }
    }

    private void ShowAboutButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            AppLogger.Info("ShowAboutButton clicked.");
            var aboutWindow = new AboutWindow();
            aboutWindow.ShowDialog(this);
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed while opening AboutWindow", ex);
        }
    }

    private async void AddPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            AppLogger.Info("AddPageButton clicked.");
            var dialog = new InputDialog("新页面");
            await dialog.ShowDialog(this);
            AppLogger.Info($"AddPage dialog closed. Confirmed={dialog.Confirmed}, Text='{dialog.InputText}'");
            if (dialog.Confirmed && !string.IsNullOrWhiteSpace(dialog.InputText))
            {
                if (DataContext is MainViewModel viewModel)
                    viewModel.AddPage(dialog.InputText.Trim());
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed while adding page", ex);
        }
    }

    private async void EditPageButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            AppLogger.Info("EditPageButton clicked.");
            if (DataContext is not MainViewModel viewModel)
            {
                AppLogger.Warn("EditPageButton ignored because DataContext is not MainViewModel.");
                return;
            }

            if (viewModel.SelectedIndex < 0 || viewModel.SelectedIndex >= viewModel.Pages.Count)
            {
                AppLogger.Warn($"EditPageButton ignored because SelectedIndex is invalid: {viewModel.SelectedIndex}");
                return;
            }

            var page = viewModel.Pages[viewModel.SelectedIndex];
            AppLogger.Info($"Opening EditPageWindow for page '{page.Name}' at index {viewModel.SelectedIndex}.");
            var dialog = new EditPageWindow(page);
            await dialog.ShowDialog(this);
            AppLogger.Info($"EditPageWindow closed. Confirmed={dialog.Confirmed}, Page='{page.Name}'");
            if (dialog.Confirmed)
            {
                dialog.ApplyTo(page);
                viewModel.SaveConfig();
                AppLogger.Info($"EditPage applied and config saved. New page name='{page.Name}'");
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed while editing page", ex);
        }
    }

    private async void EditItem_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        try
        {
            if (sender is MenuItem { DataContext: PageItemViewModel itemVm } &&
                DataContext is MainViewModel mainVm)
            {
                AppLogger.Info($"Opening EditItemWindow for item '{itemVm.Name}'.");
                var dialog = new EditItemWindow(itemVm);
                await dialog.ShowDialog(this);
                AppLogger.Info($"EditItemWindow closed. Confirmed={dialog.Confirmed}, Item='{itemVm.Name}'");
                if (dialog.Confirmed)
                {
                    dialog.ApplyTo(itemVm);
                    mainVm.SaveConfig();
                    AppLogger.Info($"EditItem applied and config saved. Item='{itemVm.Name}', Path='{itemVm.FullPath}'");
                }
            }
        }
        catch (Exception ex)
        {
            AppLogger.Error("Failed while editing item", ex);
        }
    }

    private void ViewModeButton_Click(object? sender, Avalonia.Interactivity.RoutedEventArgs e)
    {
        if (sender is not Button clickedBtn) return;
        AppLogger.Info($"ViewModeButton clicked. Tag='{clickedBtn.Tag}'");
        var parent = clickedBtn.Parent as Avalonia.Controls.Panel;
        if (parent != null)
        {
            foreach (var child in parent.Children)
            {
                if (child is Button btn && (string?)btn.Tag is "LargeIcon" or "SmallIcon" or "List")
                    btn.Classes.Remove("active");
            }
        }
        clickedBtn.Classes.Add("active");
    }

    private void ApplyViewModeButtonState(Models.ViewMode viewMode)
    {
        if (this.GetVisualDescendants().OfType<Button>().FirstOrDefault(btn => (string?)btn.Tag == viewMode.ToString()) is { } target)
        {
            ViewModeButton_Click(target, new Avalonia.Interactivity.RoutedEventArgs());
        }
    }

    private void MainWindow_DragOver(object? sender, DragEventArgs e)
    {
        var hasFiles = e.Data.Contains(DataFormats.Files);
        var hasFileNames = e.Data.Contains(DataFormats.FileNames);
        if (hasFiles || hasFileNames)
        {
            AppLogger.Info($"DragOver detected. hasFiles={hasFiles}, hasFileNames={hasFileNames}");
            e.DragEffects = DragDropEffects.Copy;
            e.Handled = true;
        }
    }

    private void MainWindow_Drop(object? sender, DragEventArgs e)
    {
        if (DataContext is MainViewModel viewModel)
        {
            string[] files = Array.Empty<string>();

            if (e.Data.Contains(DataFormats.Files))
            {
                files = e.Data.GetFiles()?
                    .Select(file => file.TryGetLocalPath())
                    .Where(path => !string.IsNullOrWhiteSpace(path))
                    .Cast<string>()
                    .ToArray() ?? Array.Empty<string>();
            }

            if (files.Length == 0 && e.Data.Contains(DataFormats.FileNames))
            {
                files = e.Data.GetFileNames()?.ToArray() ?? Array.Empty<string>();
            }

            AppLogger.Info($"Drop received. Count={files.Length}; Files={string.Join(" | ", files)}");
            if (files.Length > 0)
            {
                viewModel.AddFiles(files);
            }

            e.Handled = files.Length > 0;
        }
    }

    private void PageItem_PointerReleased(object? sender, PointerReleasedEventArgs e)
    {
        if (e.InitialPressMouseButton == MouseButton.Right &&
            sender is Control control && control.ContextMenu is ContextMenu menu)
        {
            AppLogger.Info("Opening page item context menu via right click.");
            menu.Open(control);
            e.Handled = true;
        }
    }
}
