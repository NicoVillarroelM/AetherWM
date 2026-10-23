using System.Diagnostics;
using System.Text;
using AetherWM.Core.Interfaces;
using AetherWM.Core.Models;
using AetherWM.Platform.Win32.Native;

namespace AetherWM.Platform.Win32.Services;

public class Win32WindowService : IWindowService
{
    private IntPtr _focusHook;
    private User32.WinEventDelegate? _focusDelegate;

    public event Action<IntPtr>? WindowFocused;

    private static readonly HashSet<string> ExcludedTitles = new(StringComparer.OrdinalIgnoreCase)
    {
        "Program Manager",
        "Experiencia de entrada de Windows",
        "Windows Input Experience"
    };

    public void StartFocusTracking()
    {
        if (_focusHook != IntPtr.Zero) return;

        _focusDelegate = (hWinEventHook, eventType, hWnd, idObject, idChild, dwEventThread, dwmsEventTime) =>
        {
            if (eventType == User32.EVENT_SYSTEM_FOREGROUND && hWnd != IntPtr.Zero)
            {
                WindowFocused?.Invoke(hWnd);
            }
        };

        _focusHook = User32.SetWinEventHook(
            User32.EVENT_SYSTEM_FOREGROUND,
            User32.EVENT_SYSTEM_FOREGROUND,
            IntPtr.Zero,
            _focusDelegate,
            0,
            0,
            User32.WINEVENT_OUTOFCONTEXT | User32.WINEVENT_SKIPOWNPROCESS
        );
    }

    public void StopFocusTracking()
    {
        if (_focusHook != IntPtr.Zero)
        {
            User32.UnhookWinEvent(_focusHook);
            _focusHook = IntPtr.Zero;
            _focusDelegate = null;
        }
    }

    public void SetWindowBorderColor(IntPtr handle, uint colorRef)
    {
        DwmApi.DwmSetWindowAttribute(handle, DwmApi.DWMWA_BORDER_COLOR, ref colorRef, sizeof(uint));
    }

    public void SetWindowCornerPreference(IntPtr handle, int cornerPreference)
    {
        DwmApi.DwmSetWindowAttribute(handle, DwmApi.DWMWA_WINDOW_CORNER_PREFERENCE, ref cornerPreference, sizeof(int));
    }

    public IReadOnlyList<ManagedWindow> GetActiveWindows()
    {
        var windows = new List<ManagedWindow>();

        IntPtr hDesktop = User32.OpenDesktop("default", 0, false, User32.DESKTOP_ALL_ACCESS);
        if (hDesktop == IntPtr.Zero)
        {
            hDesktop = User32.OpenInputDesktop(0, false, User32.DESKTOP_ALL_ACCESS);
        }

        if (hDesktop != IntPtr.Zero)
        {
            try
            {
                User32.EnumDesktopWindows(hDesktop, (hWnd, _) =>
                {
                    if (IsManageableWindow(hWnd, out var window))
                    {
                        windows.Add(window);
                    }
                    return true;
                }, IntPtr.Zero);
            }
            finally
            {
                User32.CloseDesktop(hDesktop);
            }
        }
        else
        {
            User32.EnumWindows((hWnd, _) =>
            {
                if (IsManageableWindow(hWnd, out var window))
                {
                    windows.Add(window);
                }
                return true;
            }, IntPtr.Zero);
        }

        return windows;
    }

    public ScreenRectangle GetPrimaryMonitorWorkArea()
    {
        if (User32.SystemParametersInfo(User32.SPI_GETWORKAREA, 0, out var rect, 0))
        {
            return new ScreenRectangle(rect.Left, rect.Top, rect.Width, rect.Height);
        }

        return new ScreenRectangle(0, 0, 1920, 1080);
    }

    public void SetWindowPosition(IntPtr handle, ScreenRectangle bounds)
    {
        User32.SetWindowPos(
            handle,
            IntPtr.Zero,
            bounds.X,
            bounds.Y,
            bounds.Width,
            bounds.Height,
            User32.SWP_NOZORDER | User32.SWP_NOACTIVATE | User32.SWP_FRAMECHANGED
        );
    }

    public void ApplyLayout(IReadOnlyDictionary<IntPtr, ScreenRectangle> layout)
    {
        if (layout.Count == 0) return;

        IntPtr hdwp = User32.BeginDeferWindowPos(layout.Count);
        if (hdwp == IntPtr.Zero)
        {
            foreach (var (handle, bounds) in layout)
            {
                SetWindowPosition(handle, bounds);
            }
            return;
        }

        foreach (var (handle, bounds) in layout)
        {
            hdwp = User32.DeferWindowPos(
                hdwp,
                handle,
                IntPtr.Zero,
                bounds.X,
                bounds.Y,
                bounds.Width,
                bounds.Height,
                User32.SWP_NOZORDER | User32.SWP_NOACTIVATE | User32.SWP_FRAMECHANGED
            );

            if (hdwp == IntPtr.Zero)
            {
                break;
            }
        }

        if (hdwp != IntPtr.Zero)
        {
            User32.EndDeferWindowPos(hdwp);
        }
    }

    private static bool IsManageableWindow(IntPtr hWnd, out ManagedWindow window)
    {
        window = null!;

        // 1. Must be visible
        if (!User32.IsWindowVisible(hWnd))
            return false;

        // 2. Must not be cloaked by DWM (Virtual desktops or suspended apps)
        if (DwmApi.DwmGetWindowAttribute(hWnd, DwmApi.DWMWA_CLOAKED, out int cloaked, sizeof(int)) == 0 && cloaked != 0)
            return false;

        // 3. Must have title
        var titleBuilder = new StringBuilder(512);
        int textLength = User32.GetWindowText(hWnd, titleBuilder, titleBuilder.Capacity);
        if (textLength == 0)
            return false;

        string title = titleBuilder.ToString().Trim();
        if (string.IsNullOrEmpty(title) || ExcludedTitles.Contains(title))
            return false;

        // 4. Class name check
        var classBuilder = new StringBuilder(256);
        User32.GetClassName(hWnd, classBuilder, classBuilder.Capacity);
        string className = classBuilder.ToString();

        if (className.Equals("Progman", StringComparison.OrdinalIgnoreCase) ||
            className.Equals("WorkerW", StringComparison.OrdinalIgnoreCase) ||
            className.Equals("Shell_TrayWnd", StringComparison.OrdinalIgnoreCase) ||
            className.Equals("Shell_SecondaryTrayWnd", StringComparison.OrdinalIgnoreCase))
        {
            return false;
        }

        // 5. Styles check
        nint style = User32.GetWindowLongPtr(hWnd, User32.GWL_STYLE);
        nint exStyle = User32.GetWindowLongPtr(hWnd, User32.GWL_EXSTYLE);

        bool isToolWindow = (exStyle & User32.WS_EX_TOOLWINDOW) != 0;
        bool isAppWindow = (exStyle & User32.WS_EX_APPWINDOW) != 0;

        if (isToolWindow && !isAppWindow)
            return false;

        // 6. Geometry check
        if (!User32.GetWindowRect(hWnd, out var rect))
            return false;

        if (rect.Width < 100 || rect.Height < 100)
            return false;

        // 7. Process name resolution
        string processName = "Unknown";
        User32.GetWindowThreadProcessId(hWnd, out uint processId);
        if (processId > 0)
        {
            try
            {
                using var proc = Process.GetProcessById((int)processId);
                processName = proc.ProcessName;
            }
            catch
            {
                // Process may have exited or access denied
            }
        }

        bool isMinimized = (style & User32.WS_MINIMIZE) != 0;
        bool isMaximized = (style & User32.WS_MAXIMIZE) != 0;

        var bounds = new ScreenRectangle(rect.Left, rect.Top, rect.Width, rect.Height);

        window = new ManagedWindow(
            hWnd,
            title,
            processName,
            className,
            bounds,
            isMinimized,
            isMaximized
        );

        return true;
    }
}
