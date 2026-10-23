using System.Collections.Concurrent;
using AetherWM.Core.Hotkeys;
using AetherWM.Platform.Win32.Native;

namespace AetherWM.Platform.Win32.Hotkeys;

public class Win32HotkeyService : IHotkeyService
{
    private readonly record struct HotkeyEntry(int Id, uint Modifiers, uint VirtualKey, Action Callback);

    private readonly List<HotkeyEntry> _registered = [];
    private readonly ConcurrentDictionary<int, Action> _callbacks = new();

    private Thread? _messageLoopThread;
    private uint _messageLoopThreadId;
    private readonly AutoResetEvent _startedEvent = new(false);
    private bool _isRunning;
    private int _nextId = 1;

    public bool Register(HotkeyModifier modifiers, int virtualKey, Action callback)
    {
        int id = Interlocked.Increment(ref _nextId);
        uint win32Mods = (uint)modifiers;

        var entry = new HotkeyEntry(id, win32Mods, (uint)virtualKey, callback);
        _registered.Add(entry);
        _callbacks[id] = callback;
        return true;
    }

    public void UnregisterAll()
    {
        _registered.Clear();
        _callbacks.Clear();
    }

    public void Start()
    {
        if (_isRunning) return;
        _isRunning = true;

        _messageLoopThread = new Thread(RunMessagePump)
        {
            IsBackground = true,
            Name = "AetherWM.HotkeyPump"
        };
        _messageLoopThread.SetApartmentState(ApartmentState.STA);
        _messageLoopThread.Start();

        _startedEvent.WaitOne(2000);
    }

    public void Stop()
    {
        if (!_isRunning) return;
        _isRunning = false;

        if (_messageLoopThreadId != 0)
        {
            User32.PostThreadMessage(_messageLoopThreadId, User32.WM_QUIT, UIntPtr.Zero, IntPtr.Zero);
            _messageLoopThread?.Join(1000);
        }
    }

    private void RunMessagePump()
    {
        _messageLoopThreadId = User32.GetCurrentThreadId();

        // Register all hotkeys on this message loop thread
        foreach (var entry in _registered)
        {
            User32.RegisterHotKey(IntPtr.Zero, entry.Id, entry.Modifiers, entry.VirtualKey);
        }

        _startedEvent.Set();

        while (_isRunning)
        {
            int status = User32.GetMessage(out var msg, IntPtr.Zero, 0, 0);
            if (status <= 0) break; // WM_QUIT or error

            if (msg.message == User32.WM_HOTKEY)
            {
                int id = (int)msg.wParam;
                if (_callbacks.TryGetValue(id, out var action))
                {
                    try
                    {
                        action();
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine($"[Hotkey Error] {ex.Message}");
                    }
                }
            }

            User32.TranslateMessage(ref msg);
            User32.DispatchMessage(ref msg);
        }

        // Clean up hotkeys on thread exit
        foreach (var entry in _registered)
        {
            User32.UnregisterHotKey(IntPtr.Zero, entry.Id);
        }
    }

    public void Dispose()
    {
        Stop();
        _startedEvent.Dispose();
        GC.SuppressFinalize(this);
    }
}
