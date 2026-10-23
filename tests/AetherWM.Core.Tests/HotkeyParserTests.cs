using AetherWM.Core.Hotkeys;

namespace AetherWM.Core.Tests;

public class HotkeyParserTests
{
    [Theory]
    [InlineData("Alt+Shift+Enter", HotkeyModifier.Alt | HotkeyModifier.Shift, 0x0D)]
    [InlineData("Ctrl+Alt+T", HotkeyModifier.Control | HotkeyModifier.Alt, (int)'T')]
    [InlineData("Super+Shift+Q", HotkeyModifier.Win | HotkeyModifier.Shift, (int)'Q')]
    [InlineData("Alt+Space", HotkeyModifier.Alt, 0x20)]
    [InlineData("Alt+Escape", HotkeyModifier.Alt, 0x1B)]
    public void TryParse_ValidBindings_ParsesCorrectly(string binding, HotkeyModifier expectedMods, int expectedKey)
    {
        // Act
        bool success = HotkeyParser.TryParse(binding, out var mods, out var key);

        // Assert
        Assert.True(success);
        Assert.Equal(expectedMods, mods);
        Assert.Equal(expectedKey, key);
    }

    [Theory]
    [InlineData("")]
    [InlineData("   ")]
    [InlineData("Alt+InvalidKeyName")]
    public void TryParse_InvalidBindings_ReturnsFalse(string binding)
    {
        // Act
        bool success = HotkeyParser.TryParse(binding, out _, out _);

        // Assert
        Assert.False(success);
    }
}
