using System;
using System.Collections.Concurrent;
using System.Runtime.InteropServices;
using System.Threading;
using Avalonia.Input;

namespace PingBox.Services;

internal sealed class WindowsHotkey : IDisposable
{
    private const int WmHotkey = 0x0312;
    private const int WmApp = 0x8000;
    private const int WmRunCommand = WmApp + 1;

    private const uint ModAlt = 0x0001;
    private const uint ModControl = 0x0002;
    private const uint ModShift = 0x0004;
    private const uint ModWin = 0x0008;

    private readonly Action<int> _onHotkeyPressed;
    private readonly ConcurrentQueue<Action> _commandQueue = new();
    private readonly AutoResetEvent _ready = new(false);
    private Thread? _messageThread;
    private uint _threadId;
    private bool _disposed;
    private int _nextId = 1;

    public WindowsHotkey(Action<int> onHotkeyPressed)
    {
        _onHotkeyPressed = onHotkeyPressed;
        StartMessageLoop();
    }

    public int Register(Key key, KeyModifiers modifiers, Action action)
    {
        ThrowIfDisposed();

        var id = Interlocked.Increment(ref _nextId);
        var vk = ConvertKeyToVirtualKey(key);
        var fsModifiers = ConvertModifiers(modifiers);

        if (vk == 0)
        {
            throw new InvalidOperationException($"不支持的热键: {key}");
        }

        RunOnMessageThread(() =>
        {
            if (!RegisterHotKey(IntPtr.Zero, id, fsModifiers, vk))
            {
                var err = Marshal.GetLastWin32Error();
                throw new InvalidOperationException($"注册热键失败，错误码: {err}");
            }
        });

        return id;
    }

    public void Unregister(int id)
    {
        if (_disposed)
        {
            return;
        }

        RunOnMessageThread(() =>
        {
            UnregisterHotKey(IntPtr.Zero, id);
        });
    }

    public void UnregisterAll()
    {
        if (_disposed)
        {
            return;
        }

        for (var id = 1; id <= _nextId; id++)
        {
            Unregister(id);
        }
    }

    public void Dispose()
    {
        if (_disposed)
        {
            return;
        }

        _disposed = true;

        if (_threadId != 0)
        {
            PostThreadMessage(_threadId, 0x0012, IntPtr.Zero, IntPtr.Zero);
        }

        if (_messageThread is { IsAlive: true })
        {
            _messageThread.Join(500);
        }

        _ready.Dispose();
    }

    private void StartMessageLoop()
    {
        _messageThread = new Thread(() =>
        {
            _threadId = GetCurrentThreadId();
            _ready.Set();

            while (GetMessage(out var msg, IntPtr.Zero, 0, 0) > 0)
            {
                if (msg.message == WmRunCommand)
                {
                    while (_commandQueue.TryDequeue(out var command))
                    {
                        command();
                    }
                    continue;
                }

                if (msg.message == WmHotkey)
                {
                    var id = msg.wParam.ToInt32();
                    _onHotkeyPressed(id);
                }
            }
        })
        {
            IsBackground = true,
            Name = "PingBoxHotkeyThread"
        };

        _messageThread.Start();
        _ready.WaitOne(TimeSpan.FromSeconds(2));
    }

    private void RunOnMessageThread(Action action)
    {
        Exception? capturedException = null;
        using var done = new ManualResetEventSlim(false);

        _commandQueue.Enqueue(() =>
        {
            try
            {
                action();
            }
            catch (Exception ex)
            {
                capturedException = ex;
            }
            finally
            {
                done.Set();
            }
        });

        PostThreadMessage(_threadId, WmRunCommand, IntPtr.Zero, IntPtr.Zero);
        done.Wait(TimeSpan.FromSeconds(2));

        if (capturedException != null)
        {
            throw capturedException;
        }
    }

    private static uint ConvertModifiers(KeyModifiers modifiers)
    {
        var result = 0u;

        if (modifiers.HasFlag(KeyModifiers.Alt)) result |= ModAlt;
        if (modifiers.HasFlag(KeyModifiers.Control)) result |= ModControl;
        if (modifiers.HasFlag(KeyModifiers.Shift)) result |= ModShift;
    if (modifiers.HasFlag(KeyModifiers.Meta)) result |= ModWin;

        return result;
    }

    private static uint ConvertKeyToVirtualKey(Key key)
    {
        if (key is >= Key.A and <= Key.Z)
        {
            return (uint)('A' + (key - Key.A));
        }

        if (key is >= Key.D0 and <= Key.D9)
        {
            return (uint)('0' + (key - Key.D0));
        }

        if (key is >= Key.F1 and <= Key.F24)
        {
            return 0x70u + (uint)(key - Key.F1);
        }

        return key switch
        {
            Key.Space => 0x20,
            Key.Return => 0x0D,
            Key.Escape => 0x1B,
            Key.Tab => 0x09,
            Key.Left => 0x25,
            Key.Up => 0x26,
            Key.Right => 0x27,
            Key.Down => 0x28,
            Key.Insert => 0x2D,
            Key.Delete => 0x2E,
            Key.Home => 0x24,
            Key.End => 0x23,
            Key.PageUp => 0x21,
            Key.PageDown => 0x22,
            _ => 0,
        };
    }

    private void ThrowIfDisposed()
    {
        if (_disposed)
        {
            throw new ObjectDisposedException(nameof(WindowsHotkey));
        }
    }

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

    [DllImport("user32.dll")]
    private static extern sbyte GetMessage(out Msg lpMsg, IntPtr hWnd, uint wMsgFilterMin, uint wMsgFilterMax);

    [DllImport("user32.dll", SetLastError = true)]
    private static extern bool PostThreadMessage(uint idThread, int msg, IntPtr wParam, IntPtr lParam);

    [DllImport("kernel32.dll")]
    private static extern uint GetCurrentThreadId();

    [StructLayout(LayoutKind.Sequential)]
    private struct Msg
    {
        public IntPtr hwnd;
        public uint message;
        public IntPtr wParam;
        public IntPtr lParam;
        public uint time;
        public Point pt;
        public uint lPrivate;
    }

    [StructLayout(LayoutKind.Sequential)]
    private struct Point
    {
        public int x;
        public int y;
    }
}
