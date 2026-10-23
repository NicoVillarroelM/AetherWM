namespace AetherWM.Core.Models;

/// <summary>
/// Represents a 2D integer bounding box in virtual screen coordinates.
/// Decoupled from Windows/System.Drawing for testability and portability.
/// </summary>
public readonly record struct ScreenRectangle(int X, int Y, int Width, int Height)
{
    public int Right => X + Width;
    public int Bottom => Y + Height;

    /// <summary>
    /// Returns a new rectangle with inner padding/gaps applied.
    /// </summary>
    public ScreenRectangle ApplyPadding(int padding)
    {
        if (padding <= 0) return this;
        int doublePadding = padding * 2;
        if (Width <= doublePadding || Height <= doublePadding)
            return this;

        return new ScreenRectangle(
            X + padding,
            Y + padding,
            Width - doublePadding,
            Height - doublePadding
        );
    }

    /// <summary>
    /// Splits this rectangle vertically (side-by-side: left and right).
    /// </summary>
    public (ScreenRectangle Left, ScreenRectangle Right) SplitVertical(double ratio = 0.5, int gap = 0)
    {
        ratio = Math.Clamp(ratio, 0.1, 0.9);
        int halfGap = gap / 2;
        int leftWidth = (int)Math.Round(Width * ratio) - halfGap;
        int rightWidth = Width - leftWidth - gap;

        var left = new ScreenRectangle(X, Y, Math.Max(0, leftWidth), Height);
        var right = new ScreenRectangle(X + leftWidth + gap, Y, Math.Max(0, rightWidth), Height);
        return (left, right);
    }

    /// <summary>
    /// Splits this rectangle horizontally (stacked: top and bottom).
    /// </summary>
    public (ScreenRectangle Top, ScreenRectangle Bottom) SplitHorizontal(double ratio = 0.5, int gap = 0)
    {
        ratio = Math.Clamp(ratio, 0.1, 0.9);
        int halfGap = gap / 2;
        int topHeight = (int)Math.Round(Height * ratio) - halfGap;
        int bottomHeight = Height - topHeight - gap;

        var top = new ScreenRectangle(X, Y, Width, Math.Max(0, topHeight));
        var bottom = new ScreenRectangle(X, Y + topHeight + gap, Width, Math.Max(0, bottomHeight));
        return (top, bottom);
    }

    public override string ToString() => $"[X:{X}, Y:{Y}, W:{Width}, H:{Height}]";
}
