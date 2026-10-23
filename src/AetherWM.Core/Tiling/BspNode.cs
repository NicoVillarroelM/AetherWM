using AetherWM.Core.Models;

namespace AetherWM.Core.Tiling;

/// <summary>
/// Base class representing a node in the Binary Space Partitioning tree.
/// </summary>
public abstract class BspNode
{
    public BspSplitNode? Parent { get; set; }

    public abstract bool IsLeaf { get; }

    public abstract void CalculateBounds(ScreenRectangle targetArea, int innerGap, Dictionary<IntPtr, ScreenRectangle> results);
}

/// <summary>
/// A leaf node holding a single managed window.
/// </summary>
public sealed class BspLeafNode : BspNode
{
    public IntPtr WindowHandle { get; }

    public override bool IsLeaf => true;

    public BspLeafNode(IntPtr windowHandle)
    {
        WindowHandle = windowHandle;
    }

    public override void CalculateBounds(ScreenRectangle targetArea, int innerGap, Dictionary<IntPtr, ScreenRectangle> results)
    {
        results[WindowHandle] = targetArea;
    }

    public override string ToString() => $"Leaf [0x{WindowHandle.ToInt64():X8}]";
}

/// <summary>
/// An internal container node dividing its space between two child nodes.
/// </summary>
public sealed class BspSplitNode : BspNode
{
    public SplitDirection Direction { get; set; }
    public double Ratio { get; set; } = 0.5;

    public BspNode FirstChild { get; set; }
    public BspNode SecondChild { get; set; }

    public override bool IsLeaf => false;

    public BspSplitNode(SplitDirection direction, BspNode firstChild, BspNode secondChild, double ratio = 0.5)
    {
        Direction = direction;
        Ratio = Math.Clamp(ratio, 0.1, 0.9);
        FirstChild = firstChild;
        SecondChild = secondChild;

        FirstChild.Parent = this;
        SecondChild.Parent = this;
    }

    public override void CalculateBounds(ScreenRectangle targetArea, int innerGap, Dictionary<IntPtr, ScreenRectangle> results)
    {
        if (Direction == SplitDirection.Vertical)
        {
            var (left, right) = targetArea.SplitVertical(Ratio, innerGap);
            FirstChild.CalculateBounds(left, innerGap, results);
            SecondChild.CalculateBounds(right, innerGap, results);
        }
        else
        {
            var (top, bottom) = targetArea.SplitHorizontal(Ratio, innerGap);
            FirstChild.CalculateBounds(top, innerGap, results);
            SecondChild.CalculateBounds(bottom, innerGap, results);
        }
    }

    public override string ToString() => $"Split [{Direction}, Ratio: {Ratio:F2}]";
}
