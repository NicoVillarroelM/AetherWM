using System.Globalization;

namespace AetherWM.Core.Utils;

public static class ColorUtils
{
    public const uint ColorDefault = 0xFFFFFFFF;
    public const uint ColorNone = 0xFFFFFFFE;

    /// <summary>
    /// Parses a hex color string (e.g. "#00F0FF" or "00F0FF") to a Win32 COLORREF (0x00BBGGRR).
    /// </summary>
    public static uint ToColorRef(string? hex, uint fallback = ColorDefault)
    {
        if (string.IsNullOrWhiteSpace(hex))
            return fallback;

        string cleanHex = hex.Trim().TrimStart('#');

        if (cleanHex.Equals("default", StringComparison.OrdinalIgnoreCase))
            return ColorDefault;

        if (cleanHex.Equals("none", StringComparison.OrdinalIgnoreCase))
            return ColorNone;

        if (cleanHex.Length == 3)
        {
            cleanHex = $"{cleanHex[0]}{cleanHex[0]}{cleanHex[1]}{cleanHex[1]}{cleanHex[2]}{cleanHex[2]}";
        }

        if (cleanHex.Length == 6 && uint.TryParse(cleanHex, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint rgb))
        {
            uint r = (rgb >> 16) & 0xFF;
            uint g = (rgb >> 8) & 0xFF;
            uint b = rgb & 0xFF;

            // COLORREF format is 0x00BBGGRR
            return r | (g << 8) | (b << 16);
        }

        return fallback;
    }
}
