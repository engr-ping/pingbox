using System;
using Avalonia.Controls;
using Avalonia.Input;
using Avalonia.Interactivity;
using Avalonia.Markup.Xaml;
using PingBox.Services;
using PingBox.ViewModels;

namespace PingBox.Views;

public partial class SettingsWindow : Window
{
    private readonly MainViewModel _viewModel = null!;
    private HotkeyRegistration? _hotkeyRegistration;
    private CheckBox? _chkShowInTaskbar;
    private CheckBox? _chkTopMost;
    private CheckBox? _chkHideOnStart;
    private CheckBox? _chkNoExit;
    private CheckBox? _chkHideOnRun;
    private CheckBox? _chkAutoStart;
    private CheckBox? _chkHotkeyEnabled;
    private TextBox? _txtHotkey;

    public SettingsWindow()
    {
        InitializeComponent();
        ResolveControls();
    }

    public SettingsWindow(MainViewModel viewModel) : this()
    {
        _viewModel = viewModel;
        LoadSettings();
    }

    private void InitializeComponent() => AvaloniaXamlLoader.Load(this);

    private void ResolveControls()
    {
        _chkShowInTaskbar = this.FindControl<CheckBox>("chkShowInTaskbar");
        _chkTopMost = this.FindControl<CheckBox>("chkTopMost");
        _chkHideOnStart = this.FindControl<CheckBox>("chkHideOnStart");
        _chkNoExit = this.FindControl<CheckBox>("chkNoExit");
        _chkHideOnRun = this.FindControl<CheckBox>("chkHideOnRun");
        _chkAutoStart = this.FindControl<CheckBox>("chkAutoStart");
        _chkHotkeyEnabled = this.FindControl<CheckBox>("chkHotkeyEnabled");
        _txtHotkey = this.FindControl<TextBox>("txtHotkey");

        if (_chkShowInTaskbar == null || _chkTopMost == null || _chkHideOnStart == null ||
            _chkNoExit == null || _chkHideOnRun == null || _chkAutoStart == null ||
            _chkHotkeyEnabled == null || _txtHotkey == null)
        {
            throw new InvalidOperationException("SettingsWindow controls not found.");
        }
    }

    private void LoadSettings()
    {
        var config = _viewModel.GetConfig();
        _chkShowInTaskbar!.IsChecked = config.ShowInTaskbar;
        _chkTopMost!.IsChecked = config.TopMost;
        _chkHideOnStart!.IsChecked = config.HideOnStart;
        _chkNoExit!.IsChecked = config.NoExit;
        _chkHideOnRun!.IsChecked = config.HideOnRun;
        _chkAutoStart!.IsChecked = AutoStartService.IsEnabled();
        _chkHotkeyEnabled!.IsChecked = config.HotkeyEnabled;
        if (config.Hotkey != null && config.Hotkey.Length >= 2)
            _txtHotkey!.Text = $"{config.Hotkey[0]} + {config.Hotkey[1]}";
    }

    private void OnCancelClick(object? sender, RoutedEventArgs e) => Close();

    private void OnOkClick(object? sender, RoutedEventArgs e) { SaveSettings(); Close(); }

    private void SaveSettings()
    {
        var config = _viewModel.GetConfig();
        config.ShowInTaskbar = _chkShowInTaskbar!.IsChecked ?? true;
        config.TopMost = _chkTopMost!.IsChecked ?? false;
        config.HideOnStart = _chkHideOnStart!.IsChecked ?? false;
        config.NoExit = _chkNoExit!.IsChecked ?? false;
        config.HideOnRun = _chkHideOnRun!.IsChecked ?? false;
        config.HotkeyEnabled = _chkHotkeyEnabled!.IsChecked ?? false;

        if (_chkAutoStart!.IsChecked ?? false) AutoStartService.Enable();
        else AutoStartService.Disable();

        if (config.HotkeyEnabled) RegisterHotkey(config);
        else UnregisterHotkey();

        _viewModel.SaveConfig();
    }

    private void RegisterHotkey(Models.AppConfig config)
    {
        try
        {
            var text = _txtHotkey!.Text ?? string.Empty;
            var parts = text.Split(new[] { " + " }, StringSplitOptions.RemoveEmptyEntries);
            if (parts.Length < 2) return;

            var keyStr = parts[^1].Trim();
            var modsStr = string.Join(" + ", parts[..^1]);
            var key = HotkeyService.ParseKey(keyStr);
            var modifiers = HotkeyService.ParseModifiers(modsStr);
            if (key == Key.None) return;

            var hotkeyService = App.GetService<IHotkeyService>();
            if (hotkeyService == null) return;

            UnregisterHotkey();
            var id = hotkeyService.Register(key, modifiers, () => _viewModel.ToggleWindowVisibility());
            _hotkeyRegistration = new HotkeyRegistration(() => hotkeyService.Unregister(id));
            config.Hotkey = new[] { keyStr, modsStr };
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"注册热键失败: {ex.Message}");
        }
    }

    private void UnregisterHotkey() { _hotkeyRegistration?.Dispose(); _hotkeyRegistration = null; }

    protected override void OnKeyDown(KeyEventArgs e)
    {
        base.OnKeyDown(e);
        if (_txtHotkey?.IsFocused == true)
        {
            e.Handled = true;
            var modsStr = HotkeyService.ModifiersToString(e.KeyModifiers);
            var keyStr = HotkeyService.KeyToString(e.Key);
            _txtHotkey.Text = string.IsNullOrEmpty(modsStr) ? keyStr : $"{modsStr} + {keyStr}";
        }
    }
}

public class HotkeyRegistration : IDisposable
{
    private readonly Action _unregister;
    public HotkeyRegistration(Action unregister) => _unregister = unregister;
    public void Dispose() => _unregister?.Invoke();
}
