using System;
using System.Collections.Generic;
using System.Windows.Input;
using Avalonia;
using Avalonia.Input;

namespace PingBox.Services;

/// <summary>
/// 热键服务实现（跨平台）
/// </summary>
public class HotkeyService : IHotkeyService, IDisposable
{
    private static int _nextId = 1;
    private readonly Dictionary<int, (Key key, KeyModifiers modifiers, Action action)> _hotkeys;
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

        // 注意：Avalonia的全局热键支持有限
        // 这里简化实现，不实际注册全局热键
        // 在实际应用中，可能需要使用平台特定的API
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

        var id = _nextId++;
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
    }

    /// <summary>
    /// 注销所有热键
    /// </summary>
    public void UnregisterAll()
    {
        if (_disposed)
            return;

        _hotkeys.Clear();
    }

    private void OnGlobalKeyDown(object? sender, KeyEventArgs e)
    {
        // 简化实现：由于Avalonia全局热键支持有限，此方法暂时不实现
        // 在实际应用中，可以通过窗口的KeyDown事件处理热键
    }

    protected virtual void OnHotKeyPressed(int id)
    {
        HotKeyPressed?.Invoke(this, new HotkeyPressedEventArgs { Id = id });
    }

    public void Dispose()
    {
        if (!_disposed)
        {
            // 不再需要取消订阅Application事件
            UnregisterAll();
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
#if WINDOWS
                    modifiers |= KeyModifiers.Windows;
#endif
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
#if WINDOWS
        if (modifiers.HasFlag(KeyModifiers.Windows))
            parts.Add("Win");
#endif

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

/// <summary>
/// 热键注册辅助类
/// </summary>
public class HotkeyRegistration : IDisposable
{
    private readonly IHotkeyService _hotkeyService;
    private readonly int _id;

    public HotkeyRegistration(IHotkeyService hotkeyService, int id)
    {
        _hotkeyService = hotkeyService;
        _id = id;
    }

    public void Dispose()
    {
        _hotkeyService?.Unregister(_id);
    }
}