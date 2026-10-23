using System.Runtime.InteropServices;

namespace AetherWM.Platform.Win32.Native;

public static class DwmApi
{
    public const int DWMWA_CLOAKED = 9;
    public const int DWMWA_WINDOW_CORNER_PREFERENCE = 33;
    public const int DWMWA_BORDER_COLOR = 34;

    public const uint DWMWA_COLOR_DEFAULT = 0xFFFFFFFF;
    public const uint DWMWA_COLOR_NONE = 0xFFFFFFFE;

    public const int DWMWCP_DEFAULT = 0;
    public const int DWMWCP_DONOTROUND = 1; // Straight / Square corners
    public const int DWMWCP_ROUND = 2;      // Standard Windows 11 rounded
    public const int DWMWCP_ROUNDSMALL = 3; // Subtle rounded

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(
        IntPtr hWnd,
        int dwAttribute,
        out int pvAttribute,
        int cbAttribute
    );

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(
        IntPtr hWnd,
        int dwAttribute,
        ref uint pvAttribute,
        int cbAttribute
    );

    [DllImport("dwmapi.dll")]
    public static extern int DwmSetWindowAttribute(
        IntPtr hWnd,
        int dwAttribute,
        ref int pvAttribute,
        int cbAttribute
    );
}
