using Avalonia;
using Avalonia.Controls;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
using Avalonia.Threading;
using PingBox.Services;
using PingBox.ViewModels;
using PingBox.Views;

namespace PingBox;

public partial class App : Application
{
    private IConfigService? _configService;
    private IProcessService? _processService;
    private IIconService? _iconService;
    private IHotkeyService? _hotkeyService;
    private MainViewModel? _mainViewModel;

    public override void Initialize()
    {
        Dispatcher.UIThread.UnhandledException += (_, e) =>
        {
            AppLogger.Error("Dispatcher UIThread unhandled exception", e.Exception);
        };

        AppLogger.Info("Initializing Avalonia application.");
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        AppLogger.Info("Framework initialization started.");

        // 初始化服务
        _configService ??= new ConfigService();
        _processService ??= new ProcessService();
        _iconService ??= new IconService();
        _hotkeyService ??= new HotkeyService();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            _mainViewModel = new MainViewModel(_configService, _processService, _iconService);
            desktop.MainWindow = new MainWindow(_mainViewModel);
            AppLogger.Info("Main window created.");

            // 注册全局热键
            RegisterGlobalHotkey(_mainViewModel);

            // 连接托盘菜单事件
            SetupTrayIcon();

            // 应用退出时清理资源
            desktop.Exit += (_, _) =>
            {
                AppLogger.Info("Desktop exit requested. Disposing hotkey service.");
                _hotkeyService?.Dispose();
            };
        }

        base.OnFrameworkInitializationCompleted();
    }

    private void SetupTrayIcon()
    {
        var trayIcons = TrayIcon.GetIcons(this);
        if (trayIcons == null || trayIcons.Count == 0)
        {
            return;
        }

        var trayIcon = trayIcons[0];

        // 双击托盘图标：显示/隐藏主窗口
        trayIcon.Clicked += (_, _) => ToggleMainWindow();

        // 连接菜单项
        if (trayIcon.Menu is NativeMenu menu)
        {
            foreach (var item in menu.Items)
            {
                if (item is NativeMenuItem menuItem)
                {
                    if (menuItem.Header == "显示/隐藏")
                    {
                        menuItem.Click += (_, _) =>
                        {
                            AppLogger.Info("Tray menu clicked: 显示/隐藏");
                            ToggleMainWindow();
                        };
                    }
                    else if (menuItem.Header == "退出")
                    {
                        menuItem.Click += (_, _) =>
                        {
                            AppLogger.Info("Tray menu clicked: 退出");
                            _mainViewModel?.SaveConfig();
                            (ApplicationLifetime as IClassicDesktopStyleApplicationLifetime)?.Shutdown();
                        };
                    }
                }
            }
        }
    }

    private void ToggleMainWindow()
    {
        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop &&
            desktop.MainWindow is MainWindow window)
        {
            AppLogger.Info($"ToggleMainWindow invoked. Visible={window.IsVisible}");
            if (window.IsVisible)
            {
                window.Hide();
            }
            else
            {
                window.Show();
                window.WindowState = WindowState.Normal;
                window.Activate();
            }
        }
    }

    private void RegisterGlobalHotkey(MainViewModel viewModel)
    {
        var config = _configService.Load();
        if (config.HotkeyEnabled && config.Hotkey != null && config.Hotkey.Length >= 2)
        {
            try
            {
                var keyStr = config.Hotkey[0];
                var modifiersStr = config.Hotkey[1];

                var key = ParseKey(keyStr);
                var modifiers = ParseModifiers(modifiersStr);

                if (key != Key.None)
                {
                    AppLogger.Info($"Registering global hotkey: key={key}, modifiers={modifiers}");
                    _hotkeyService.Register(key, modifiers, () =>
                    {
                        AppLogger.Info("Global hotkey triggered.");
                        viewModel.ToggleWindowVisibility();
                    });
                }
            }
            catch (Exception ex)
            {
                AppLogger.Error("Failed to register global hotkey", ex);
                System.Diagnostics.Debug.WriteLine($"注册全局热键失败: {ex.Message}");
            }
        }
    }

    private Key ParseKey(string keyStr)
    {
        if (Enum.TryParse(keyStr, true, out Key key))
        {
            return key;
        }
        return Key.None;
    }

    private KeyModifiers ParseModifiers(string modifiersStr)
    {
        var modifiers = KeyModifiers.None;
        var parts = modifiersStr.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

        foreach (var part in parts)
        {
            switch (part.ToLower())
            {
                case "ctrl":
                case "control":
                    modifiers |= KeyModifiers.Control;
                    break;
                case "alt":
                    modifiers |= KeyModifiers.Alt;
                    break;
                case "shift":
                    modifiers |= KeyModifiers.Shift;
                    break;
                case "win":
                case "windows":
                case "meta":
                case "super":
                    modifiers |= KeyModifiers.Meta;
                    break;
            }
        }

        return modifiers;
    }

    public static T GetService<T>() where T : class
    {
        var app = Current as App;
        return typeof(T) switch
        {
            var t when t == typeof(IConfigService) => app?._configService as T,
            var t when t == typeof(IProcessService) => app?._processService as T,
            var t when t == typeof(IIconService) => app?._iconService as T,
            var t when t == typeof(IHotkeyService) => app?._hotkeyService as T,
            _ => null as T
        };
    }
}