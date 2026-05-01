using System;
using Avalonia;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PingBox.Services;
using PingBox.ViewModels;

namespace PingBox.Views;

public partial class SettingsWindow : Window
{
    private readonly MainViewModel _viewModel;
    private HotkeyRegistration? _hotkeyRegistration;

    public SettingsWindow()
    {
        InitializeComponent();
    }

    public SettingsWindow(MainViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        LoadSettings();
    }

    private void InitializeComponent()
    {
        AvaloniaXamlLoader.Load(this);
    }

    private void LoadSettings()
    {
        var config = _viewModel.GetConfig();
        chkShowInTaskbar.IsChecked = config.ShowInTaskbar;
        chkTopMost.IsChecked = config.TopMost;
        chkHideOnStart.IsChecked = config.HideOnStart;
        chkHotkeyEnabled.IsChecked = config.HotkeyEnabled;

        if (config.Hotkey != null && config.Hotkey.Length >= 2)
        {
            txtHotkey.Text = $"{config.Hotkey[0]} + {config.Hotkey[1]}";
        }
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e)
    {
        Close();
    }

    private void OnOkClick(object? sender, RoutedEventArgs e)
    {
        SaveSettings();
        Close();
    }

    private void SaveSettings()
    {
        if (_viewModel == null) return;

        var config = _viewModel.GetConfig();
        config.ShowInTaskbar = chkShowInTaskbar.IsChecked ?? true;
        config.TopMost = chkTopMost.IsChecked ?? false;
        config.HideOnStart = chkHideOnStart.IsChecked ?? false;
        config.HotkeyEnabled = chkHotkeyEnabled.IsChecked ?? false;

        // 注册或取消热键
        if (config.HotkeyEnabled)
        {
            RegisterHotkey();
        }
        else
        {
            UnregisterHotkey();
        }

        _viewModel.SaveConfig();
    }

    private async void RegisterHotkey()
    {
        if (_viewModel == null) return;

        try
        {
            var keys = txtHotkey.Text?.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
            if (keys != null && keys.Length >= 2)
            {
                var key = ParseKey(keys[0]);
                var modifiers = ParseModifiers(keys[1]);

                var hotkeyService = App.GetService<IHotkeyService>();
                var id = hotkeyService.Register(key, modifiers, () =>
                {
                    _viewModel.ToggleWindowVisibility();
                });
                _hotkeyRegistration = new HotkeyRegistration(() => hotkeyService.Unregister(id));

                var config = _viewModel.GetConfig();
                config.Hotkey = new[] { keys[0], keys[1] };
            }
        }
        catch (Exception ex)
        {
            // 可以在这里添加错误处理逻辑
            System.Diagnostics.Debug.WriteLine($"注册热键失败: {ex.Message}");
        }
    }

    private void UnregisterHotkey()
    {
        _hotkeyRegistration?.Dispose();
        _hotkeyRegistration = null;
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

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);

        if (txtHotkey.IsFocused)
        {
            e.Handled = true;
            txtHotkey.Text = $"{e.Key} + {e.KeyModifiers}";
        }
    }
}

public class HotkeyRegistration : IDisposable
{
    private readonly Action _unregister;

    public HotkeyRegistration(Action unregister)
    {
        _unregister = unregister;
    }

    public void Dispose()
    {
        _unregister?.Invoke();
    }
}