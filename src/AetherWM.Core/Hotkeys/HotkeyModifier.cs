namespace AetherWM.Core.Hotkeys;

[Flags]
public enum HotkeyModifier
{
    None = 0,
    Alt = 1,
    Control = 2,
    Shift = 4,
    Win = 8,
    NoRepeat = 0x4000
}
