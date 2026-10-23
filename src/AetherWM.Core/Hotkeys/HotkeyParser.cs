namespace AetherWM.Core.Hotkeys;

public static class HotkeyParser
{
    public static bool TryParse(string binding, out HotkeyModifier modifiers, out int virtualKey)
    {
        modifiers = HotkeyModifier.None;
        virtualKey = 0;

        if (string.IsNullOrWhiteSpace(binding))
            return false;

        var parts = binding.Split('+', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries);
        if (parts.Length == 0)
            return false;

        for (int i = 0; i < parts.Length - 1; i++)
        {
            var part = parts[i];
            if (part.Equals("Alt", StringComparison.OrdinalIgnoreCase))
                modifiers |= HotkeyModifier.Alt;
            else if (part.Equals("Ctrl", StringComparison.OrdinalIgnoreCase) || part.Equals("Control", StringComparison.OrdinalIgnoreCase))
                modifiers |= HotkeyModifier.Control;
            else if (part.Equals("Shift", StringComparison.OrdinalIgnoreCase))
                modifiers |= HotkeyModifier.Shift;
            else if (part.Equals("Win", StringComparison.OrdinalIgnoreCase) || part.Equals("Super", StringComparison.OrdinalIgnoreCase))
                modifiers |= HotkeyModifier.Win;
        }

        string keyPart = parts[^1];
        virtualKey = ParseKey(keyPart);
        return virtualKey != 0;
    }

    private static int ParseKey(string key)
    {
        if (key.Length == 1 && char.IsLetterOrDigit(key[0]))
        {
            return char.ToUpperInvariant(key[0]);
        }

        return key.ToLowerInvariant() switch
        {
            "enter" or "return" => 0x0D,
            "space" => 0x20,
            "escape" or "esc" => 0x1B,
            "tab" => 0x09,
            "backspace" or "back" => 0x08,
            "delete" or "del" => 0x2E,
            "left" => 0x25,
            "up" => 0x26,
            "right" => 0x27,
            "down" => 0x28,
            "f1" => 0x70,
            "f2" => 0x71,
            "f3" => 0x72,
            "f4" => 0x73,
            "f5" => 0x74,
            "f6" => 0x75,
            "f7" => 0x76,
            "f8" => 0x77,
            "f9" => 0x78,
            "f10" => 0x79,
            "f11" => 0x7A,
            "f12" => 0x7B,
            _ => 0
        };
    }
}
