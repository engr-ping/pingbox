using Avalonia.Input;

namespace PingBox.Services;

/// <summary>
/// 热键服务接口
/// </summary>
public interface IHotkeyService : IDisposable
{
    /// <summary>
    /// 注册热键
    /// </summary>
    int Register(Key key, KeyModifiers modifiers, Action action);

    /// <summary>
    /// 注销热键
    /// </summary>
    void Unregister(int id);

    /// <summary>
    /// 注销所有热键
    /// </summary>
    void UnregisterAll();

    /// <summary>
    /// 热键按下事件
    /// </summary>
    event EventHandler<HotkeyPressedEventArgs>? HotKeyPressed;

    /// <summary>
    /// 初始化热键服务
    /// </summary>
    void Initialize();
}

/// <summary>
/// 热键按下事件参数
/// </summary>
public class HotkeyPressedEventArgs : EventArgs
{
    public int Id { get; set; }
}