using AetherWM.Core.Utils;

namespace AetherWM.Core.Tests;

public class ColorUtilsTests
{
    [Fact]
    public void ToColorRef_RedColor_ConvertsToColorRef()
    {
        // Red: R=FF, G=00, B=00 -> 0x000000FF
        uint color = ColorUtils.ToColorRef("#FF0000");
        Assert.Equal(0x000000FFu, color);
    }

    [Fact]
    public void ToColorRef_BlueColor_ConvertsToColorRef()
    {
        // Blue: R=00, G=00, B=FF -> 0x00FF0000
        uint color = ColorUtils.ToColorRef("#0000FF");
        Assert.Equal(0x00FF0000u, color);
    }

    [Fact]
    public void ToColorRef_CyanColor_ConvertsToColorRef()
    {
        // Cyan #00F0FF: R=00, G=F0, B=FF -> 0x00FFF000
        uint color = ColorUtils.ToColorRef("#00F0FF");
        Assert.Equal(0x00FFF000u, color);
    }

    [Fact]
    public void ToColorRef_Invalid_ReturnsFallback()
    {
        uint color = ColorUtils.ToColorRef("invalid-color", fallback: 12345);
        Assert.Equal(12345u, color);
    }
}
