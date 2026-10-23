using AetherWM.Core.Models;

namespace AetherWM.Core.Interfaces;

/// <summary>
/// Operating system abstraction for window querying, positioning, and tiling.
/// </summary>
public interface IWindowService
{
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
}
