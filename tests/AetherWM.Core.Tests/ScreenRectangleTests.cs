using AetherWM.Core.Models;

namespace AetherWM.Core.Tests;

public class ScreenRectangleTests
{
    [Fact]
    public void SplitVertical_ShouldDivideWidthEqually_WhenRatioIsHalf()
    {
        // Arrange
        var rect = new ScreenRectangle(0, 0, 1920, 1080);

        // Act
        var (left, right) = rect.SplitVertical(0.5);

        // Assert
        Assert.Equal(0, left.X);
        Assert.Equal(960, left.Width);
        Assert.Equal(1080, left.Height);

        Assert.Equal(960, right.X);
        Assert.Equal(960, right.Width);
        Assert.Equal(1080, right.Height);
    }

    [Fact]
    public void SplitHorizontal_ShouldDivideHeightEqually_WhenRatioIsHalf()
    {
        // Arrange
        var rect = new ScreenRectangle(0, 0, 1920, 1080);

        // Act
        var (top, bottom) = rect.SplitHorizontal(0.5);

        // Assert
        Assert.Equal(0, top.Y);
        Assert.Equal(1920, top.Width);
        Assert.Equal(540, top.Height);

        Assert.Equal(540, bottom.Y);
        Assert.Equal(1920, bottom.Width);
        Assert.Equal(540, bottom.Height);
    }

    [Fact]
    public void ApplyPadding_ShouldReduceBoundsCorrectly()
    {
        // Arrange
        var rect = new ScreenRectangle(100, 100, 800, 600);

        // Act
        var padded = rect.ApplyPadding(10);

        // Assert
        Assert.Equal(110, padded.X);
        Assert.Equal(110, padded.Y);
        Assert.Equal(780, padded.Width);
        Assert.Equal(580, padded.Height);
    }
}
