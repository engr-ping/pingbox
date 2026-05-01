using Avalonia;
using Avalonia.Controls.ApplicationLifetimes;
using Avalonia.Input;
using Avalonia.Markup.Xaml;
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

    public override void Initialize()
    {
        AvaloniaXamlLoader.Load(this);
    }

    public override void OnFrameworkInitializationCompleted()
    {
        // 初始化服务
        _configService ??= new ConfigService();
        _processService ??= new ProcessService();
        _iconService ??= new IconService();
        _hotkeyService ??= new HotkeyService();

        if (ApplicationLifetime is IClassicDesktopStyleApplicationLifetime desktop)
        {
            var mainViewModel = new MainViewModel(_configService, _processService, _iconService);
            desktop.MainWindow = new MainWindow(mainViewModel);

            // 注册全局热键
            RegisterGlobalHotkey(mainViewModel);
        }

        base.OnFrameworkInitializationCompleted();
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
                    _hotkeyService.Register(key, modifiers, () =>
                    {
                        viewModel.ToggleWindowVisibility();
                    });
                }
            }
            catch (Exception ex)
            {
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
#if WINDOWS
                    modifiers |= KeyModifiers.Windows;
#endif
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