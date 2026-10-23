using AetherWM.Core.Models;

namespace AetherWM.Core.Interfaces;

/// <summary>
/// Operating system abstraction for window querying, positioning, styling, and event tracking.
/// </summary>
public interface IWindowService
{
    /// <summary>
    /// Event fired when the focused foreground window changes.
    /// </summary>
    event Action<IntPtr>? WindowFocused;

    /// <summary>
    /// Retrieves all manageable desktop windows currently open and visible to the user.
    /// Filters out background overlays, tooltips, and system tray/taskbar windows.
    /// </summary>
    IReadOnlyList<ManagedWindow> GetActiveWindows();

    /// <summary>
    /// Retrieves the working area rectangle of the primary monitor (excluding the taskbar).
    /// </summary>
    ScreenRectangle GetPrimaryMonitorWorkArea();

    /// <summary>
    /// Repositions and resizes a single window safely.
    /// </summary>
    void SetWindowPosition(IntPtr handle, ScreenRectangle bounds);

    /// <summary>
    /// Repositions multiple windows atomically in a single redraw cycle to prevent flicker.
    /// </summary>
    void ApplyLayout(IReadOnlyDictionary<IntPtr, ScreenRectangle> layout);

    /// <summary>
    /// Applies custom accent border color to a window via DWM (Windows 11).
    /// </summary>
    void SetWindowBorderColor(IntPtr handle, uint colorRef);

    /// <summary>
    /// Sets window corner curvature (Square, Rounded, etc.) via DWM.
    /// </summary>
    void SetWindowCornerPreference(IntPtr handle, int cornerPreference);

    /// <summary>
    /// Starts tracking foreground window focus changes via native event hooks.
    /// </summary>
    void StartFocusTracking();

    /// <summary>
    /// Stops tracking window focus.
    /// </summary>
    void StopFocusTracking();
}
