using System.Windows;
using System.Windows.Interop;
using AetherWM.Platform.Win32.Native;

namespace AetherWM.Widgets;

public static class DesktopWidgetHelper
{
    private static readonly IntPtr HWND_BOTTOM = new(1);
    private const int WM_WINDOWPOSCHANGING = 0x0046;

    public static void PinToDesktop(Window window)
    {
        var helper = new WindowInteropHelper(window);
        IntPtr hWnd = helper.Handle;

        if (hWnd == IntPtr.Zero)
        {
            window.Loaded += (s, e) => ApplyDesktopPinning(new WindowInteropHelper(window).Handle);
        }
        else
        {
            ApplyDesktopPinning(hWnd);
        }

        var source = HwndSource.FromHwnd(hWnd);
        source?.AddHook(WndProc);
    }

    private static void ApplyDesktopPinning(IntPtr hWnd)
    {
        nint exStyle = User32.GetWindowLongPtr(hWnd, User32.GWL_EXSTYLE);
        exStyle |= (nint)(User32.WS_EX_TOOLWINDOW | User32.WS_EX_NOACTIVATE);
        // Note: SetWindowLongPtr can be called via User32 if needed, or by pin to bottom:
        User32.SetWindowPos(
            hWnd,
            HWND_BOTTOM,
            0, 0, 0, 0,
            User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE
        );
    }

    private static nint WndProc(IntPtr hwnd, int msg, nint wParam, nint lParam, ref bool handled)
    {
        if (msg == WM_WINDOWPOSCHANGING)
        {
            // Keep window at HWND_BOTTOM
            User32.SetWindowPos(
                hwnd,
                HWND_BOTTOM,
                0, 0, 0, 0,
                User32.SWP_NOMOVE | User32.SWP_NOSIZE | User32.SWP_NOACTIVATE
            );
        }

        return IntPtr.Zero;
    }
}
