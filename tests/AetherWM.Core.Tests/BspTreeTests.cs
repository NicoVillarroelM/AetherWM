using AetherWM.Core.Models;
using AetherWM.Core.Tiling;

namespace AetherWM.Core.Tests;

public class BspTreeTests
{
    [Fact]
    public void CalculateLayout_SingleWindow_OccupiesWholeUsableArea()
    {
        // Arrange
        var tree = new BspTree();
        var handle = new IntPtr(0x1000);
        tree.AddWindow(handle);

        var screen = new ScreenRectangle(0, 0, 1920, 1080);
        int outerGap = 10;
        int innerGap = 8;

        // Act
        var layout = tree.CalculateLayout(screen, innerGap, outerGap);

        // Assert
        Assert.Single(layout);
        var expected = screen.ApplyPadding(outerGap);
        Assert.Equal(expected, layout[handle]);
    }

    [Fact]
    public void CalculateLayout_TwoWindows_SplitSideBySide()
    {
        // Arrange
        var tree = new BspTree();
        var h1 = new IntPtr(0x1000);
        var h2 = new IntPtr(0x2000);
        tree.AddWindow(h1);
        tree.AddWindow(h2);

        var screen = new ScreenRectangle(0, 0, 1920, 1080);
        int outerGap = 0;
        int innerGap = 10;

        // Act
        var layout = tree.CalculateLayout(screen, innerGap, outerGap);

        // Assert
        Assert.Equal(2, layout.Count);
        var left = layout[h1];
        var right = layout[h2];

        // Left window starts at 0, right window starts after gap
        Assert.Equal(0, left.X);
        Assert.Equal(left.Right + innerGap, right.X);
        Assert.Equal(screen.Width, right.Right);
        Assert.Equal(1080, left.Height);
        Assert.Equal(1080, right.Height);
    }

    [Fact]
    public void RemoveWindow_WhenTwoWindows_RemainingWindowBecomesFullScreen()
    {
        // Arrange
        var tree = new BspTree();
        var h1 = new IntPtr(0x1000);
        var h2 = new IntPtr(0x2000);
        tree.AddWindow(h1);
        tree.AddWindow(h2);

        // Act
        bool removed = tree.RemoveWindow(h2);
        var screen = new ScreenRectangle(0, 0, 1920, 1080);
        var layout = tree.CalculateLayout(screen, innerGap: 0, outerGap: 0);

        // Assert
        Assert.True(removed);
        Assert.Single(layout);
        Assert.Equal(screen, layout[h1]);
    }

    [Fact]
    public void CalculateLayout_ThreeWindows_CreatesMasterStackLayout()
    {
        // Arrange
        var tree = new BspTree();
        var h1 = new IntPtr(0x1000);
        var h2 = new IntPtr(0x2000);
        var h3 = new IntPtr(0x3000);
        tree.AddWindow(h1);
        tree.AddWindow(h2);
        tree.AddWindow(h3);

        var screen = new ScreenRectangle(0, 0, 1920, 1080);

        // Act
        var layout = tree.CalculateLayout(screen, innerGap: 8, outerGap: 10);

        // Assert
        Assert.Equal(3, layout.Count);
        Assert.True(layout.ContainsKey(h1));
        Assert.True(layout.ContainsKey(h2));
        Assert.True(layout.ContainsKey(h3));

        // Windows do not have negative dimensions or zero area
        foreach (var (_, rect) in layout)
        {
            Assert.True(rect.Width > 0);
            Assert.True(rect.Height > 0);
            Assert.True(rect.Right <= screen.Width);
            Assert.True(rect.Bottom <= screen.Height);
        }
    }
}
