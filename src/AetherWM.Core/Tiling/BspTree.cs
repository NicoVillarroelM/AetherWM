using AetherWM.Core.Models;

namespace AetherWM.Core.Tiling;

/// <summary>
/// Manages a Binary Space Partitioning tree of windows for automatic tiling.
/// </summary>
public class BspTree
{
    private readonly List<IntPtr> _windows = [];

    public BspNode? Root { get; private set; }

    public int Count => _windows.Count;

    public IReadOnlyList<IntPtr> Windows => _windows.AsReadOnly();

    public bool Contains(IntPtr handle) => _windows.Contains(handle);

    /// <summary>
    /// Inserts a new window into the BSP tree.
    /// </summary>
    public void AddWindow(IntPtr handle)
    {
        if (handle == IntPtr.Zero || Contains(handle))
            return;

        var newLeaf = new BspLeafNode(handle);

        if (Root == null)
        {
            Root = newLeaf;
            _windows.Add(handle);
            return;
        }

        if (Root is BspLeafNode existingRootLeaf)
        {
            Root = new BspSplitNode(SplitDirection.Vertical, existingRootLeaf, newLeaf);
            _windows.Add(handle);
            return;
        }

        // Insert into the deepest or most recently added leaf
        var targetLeaf = FindDeepestLeaf(Root);
        if (targetLeaf == null || targetLeaf.Parent == null)
        {
            Root = new BspSplitNode(SplitDirection.Vertical, Root, newLeaf);
            _windows.Add(handle);
            return;
        }

        var parent = targetLeaf.Parent;
        // Alternate split direction: if parent was Vertical, split Horizontal
        var nextDirection = parent.Direction == SplitDirection.Vertical
            ? SplitDirection.Horizontal
            : SplitDirection.Vertical;

        var replacementSplit = new BspSplitNode(nextDirection, targetLeaf, newLeaf);

        if (parent.FirstChild == targetLeaf)
        {
            parent.FirstChild = replacementSplit;
        }
        else
        {
            parent.SecondChild = replacementSplit;
        }

        replacementSplit.Parent = parent;
        _windows.Add(handle);
    }

    /// <summary>
    /// Removes a window from the BSP tree and balances the remaining tree.
    /// </summary>
    public bool RemoveWindow(IntPtr handle)
    {
        if (!Contains(handle))
            return false;

        var leaf = FindLeaf(Root, handle);
        if (leaf == null)
            return false;

        _windows.Remove(handle);

        if (leaf == Root)
        {
            Root = null;
            return true;
        }

        var parent = leaf.Parent;
        if (parent == null)
        {
            Root = null;
            return true;
        }

        var sibling = parent.FirstChild == leaf ? parent.SecondChild : parent.FirstChild;
        var grandParent = parent.Parent;

        if (grandParent == null)
        {
            Root = sibling;
            sibling.Parent = null;
        }
        else
        {
            if (grandParent.FirstChild == parent)
            {
                grandParent.FirstChild = sibling;
            }
            else
            {
                grandParent.SecondChild = sibling;
            }
            sibling.Parent = grandParent;
        }

        return true;
    }

    /// <summary>
    /// Computes the exact screen rectangles for all windows in the tree.
    /// </summary>
    public IReadOnlyDictionary<IntPtr, ScreenRectangle> CalculateLayout(
        ScreenRectangle screenArea,
        int innerGap = 8,
        int outerGap = 10)
    {
        var results = new Dictionary<IntPtr, ScreenRectangle>();
        if (Root == null || Count == 0)
            return results;

        var usableArea = screenArea.ApplyPadding(outerGap);
        Root.CalculateBounds(usableArea, innerGap, results);
        return results;
    }

    private static BspLeafNode? FindLeaf(BspNode? current, IntPtr handle)
    {
        if (current == null) return null;
        if (current is BspLeafNode leaf)
        {
            return leaf.WindowHandle == handle ? leaf : null;
        }

        if (current is BspSplitNode split)
        {
            return FindLeaf(split.FirstChild, handle) ?? FindLeaf(split.SecondChild, handle);
        }

        return null;
    }

    private static BspLeafNode? FindDeepestLeaf(BspNode? current)
    {
        if (current == null) return null;
        if (current is BspLeafNode leaf) return leaf;

        if (current is BspSplitNode split)
        {
            return FindDeepestLeaf(split.SecondChild) ?? FindDeepestLeaf(split.FirstChild);
        }

        return null;
    }
}
