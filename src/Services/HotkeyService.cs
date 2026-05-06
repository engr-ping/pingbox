using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using System.Windows.Input;
using Avalonia.Input;
using Avalonia.Threading;

namespace PingBox.Services;

/// <summary>
/// 热键服务实现（跨平台，Windows 上支持全局热键）
/// </summary>
public class HotkeyService : IHotkeyService, IDisposable
{
    private readonly Dictionary<int, (Key key, KeyModifiers modifiers, Action action)> _hotkeys;
    private WindowsHotkey? _windowsHotkey;
    private bool _disposed;
    private bool _isInitialized;

    public event EventHandler<HotkeyPressedEventArgs>? HotKeyPressed;

    public HotkeyService()
    {
        _hotkeys = new Dictionary<int, (Key, KeyModifiers, Action)>();
    }

    /// <summary>
    /// 初始化热键服务
    /// </summary>
    public void Initialize()
    {
        if (_isInitialized) return;

        try
        {
            // 在 Windows 上初始化全局热键支持
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
            {
                _windowsHotkey = new WindowsHotkey(OnWindowsHotkeyPressed);
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"热键服务初始化失败: {ex.Message}");
        }

        _isInitialized = true;
    }

    /// <summary>
    /// 注册热键
    /// </summary>
    public int Register(Key key, KeyModifiers modifiers, Action action)
    {
        if (_disposed)
            throw new ObjectDisposedException(nameof(HotkeyService));

        if (!_isInitialized)
            Initialize();

        var id = _hotkeys.Count + 1;

        // 尝试在 Windows 上注册全局热键
        if (_windowsHotkey != null)
        {
            try
            {
                id = _windowsHotkey.Register(key, modifiers, action);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"注册全局热键失败: {ex.Message}");
            }
        }

        _hotkeys[id] = (key, modifiers, action);

        return id;
    }

    /// <summary>
    /// 注销热键
    /// </summary>
    public void Unregister(int id)
    {
        if (_disposed)
            return;

        _hotkeys.Remove(id);
        _windowsHotkey?.Unregister(id);
    }

    /// <summary>
    /// 注销所有热键
    /// </summary>
    public void UnregisterAll()
    {
        if (_disposed)
            return;

        _hotkeys.Clear();
        _windowsHotkey?.UnregisterAll();
    }

    protected virtual void OnHotKeyPressed(int id)
    {
        HotKeyPressed?.Invoke(this, new HotkeyPressedEventArgs { Id = id });
    }

    private void OnWindowsHotkeyPressed(int id)
    {
        if (!_hotkeys.TryGetValue(id, out var registered))
        {
            return;
        }

        Dispatcher.UIThread.Post(() =>
        {
            try
            {
                registered.action();
                OnHotKeyPressed(id);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"执行热键回调失败: {ex.Message}");
            }
        });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            UnregisterAll();
            _windowsHotkey?.Dispose();
            _disposed = true;
        }
    }

    /// <summary>
    /// 解析键字符串
    /// </summary>
    public static Key ParseKey(string keyStr)
    {
        if (Enum.TryParse(keyStr, true, out Key key))
        {
            return key;
        }
        return Key.None;
    }

    /// <summary>
    /// 解析修饰键字符串
    /// </summary>
    public static KeyModifiers ParseModifiers(string modifiersStr)
    {
        var modifiers = KeyModifiers.None;
        var parts = modifiersStr.Split(new[] { '+', ' ' }, StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);

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

    /// <summary>
    /// 将修饰键转换为字符串
    /// </summary>
    public static string ModifiersToString(KeyModifiers modifiers)
    {
        var parts = new List<string>();

        if (modifiers.HasFlag(KeyModifiers.Control))
            parts.Add("Ctrl");
        if (modifiers.HasFlag(KeyModifiers.Alt))
            parts.Add("Alt");
        if (modifiers.HasFlag(KeyModifiers.Shift))
            parts.Add("Shift");
        if (modifiers.HasFlag(KeyModifiers.Meta))
            parts.Add("Win");

        return string.Join(" + ", parts);
    }

    /// <summary>
    /// 将按键转换为字符串
    /// </summary>
    public static string KeyToString(Key key)
    {
        return key.ToString();
    }

    /// <summary>
    /// 从配置字符串解析热键
    /// </summary>
    public static (Key key, KeyModifiers modifiers) ParseHotkey(string? keyStr, string? modifiersStr)
    {
        var key = Key.F2; // 默认
        var modifiers = KeyModifiers.Control | KeyModifiers.Alt; // 默认

        if (!string.IsNullOrEmpty(keyStr))
        {
            key = ParseKey(keyStr);
        }

        if (!string.IsNullOrEmpty(modifiersStr))
        {
            modifiers = ParseModifiers(modifiersStr);
        }

        return (key, modifiers);
    }
}
