using System.Runtime.InteropServices;

namespace AetherWM.Platform.Win32.Native;

public static class DwmApi
{
    public const int DWMWA_CLOAKED = 9;

    [DllImport("dwmapi.dll")]
    public static extern int DwmGetWindowAttribute(
        IntPtr hWnd,
        int dwAttribute,
        out int pvAttribute,
        int cbAttribute
    );
}
