namespace AetherWM.Core.Models;

/// <summary>
/// Domain model representing a managed window on the desktop.
/// </summary>
public record ManagedWindow(
    IntPtr Handle,
    string Title,
    string ProcessName,
    string ClassName,
    ScreenRectangle Bounds,
    bool IsMinimized,
    bool IsMaximized
)
{
    public override string ToString() => $"[HWND: 0x{Handle.ToInt64():X8}] [{ProcessName}] \"{Title}\" {Bounds}";
}
