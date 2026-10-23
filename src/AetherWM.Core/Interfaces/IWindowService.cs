using AetherWM.Core.Models;

namespace AetherWM.Core.Interfaces;

/// <summary>
/// Operating system abstraction for window querying and placement.
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
    /// Repositions and resizes a window safely.
    /// </summary>
    void SetWindowPosition(IntPtr handle, ScreenRectangle bounds);
}
