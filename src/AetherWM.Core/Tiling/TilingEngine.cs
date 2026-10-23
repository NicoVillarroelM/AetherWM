using AetherWM.Core.Models;

namespace AetherWM.Core.Tiling;

/// <summary>
/// High-level engine managing workspaces and geometric tiling state.
/// </summary>
public class TilingEngine
{
    private readonly BspTree _tree = new();

    public int InnerGap { get; set; } = 8;
    public int OuterGap { get; set; } = 12;

    public int ManagedWindowCount => _tree.Count;

    public IReadOnlyList<IntPtr> Windows => _tree.Windows;

    public void SyncWindows(IEnumerable<IntPtr> activeHandles)
    {
        var targetSet = new HashSet<IntPtr>(activeHandles);

        // Remove closed windows
        foreach (var existing in _tree.Windows.ToList())
        {
            if (!targetSet.Contains(existing))
            {
                _tree.RemoveWindow(existing);
            }
        }

        // Add newly opened windows
        foreach (var handle in targetSet)
        {
            if (!_tree.Contains(handle))
            {
                _tree.AddWindow(handle);
            }
        }
    }

    public IReadOnlyDictionary<IntPtr, ScreenRectangle> GenerateLayout(ScreenRectangle screenArea)
    {
        return _tree.CalculateLayout(screenArea, InnerGap, OuterGap);
    }
}
