namespace AetherWM.Core.Hotkeys;

/// <summary>
/// Abstraction for system-wide global hotkeys.
/// </summary>
public interface IHotkeyService : IDisposable
{
    /// <summary>
    /// Registers a global hotkey combination.
    /// </summary>
    bool Register(HotkeyModifier modifiers, int virtualKey, Action callback);

    /// <summary>
    /// Unregisters all active hotkeys.
    /// </summary>
    void UnregisterAll();

    /// <summary>
    /// Starts the hotkey listening loop.
    /// </summary>
    void Start();

    /// <summary>
    /// Stops the hotkey listening loop.
    /// </summary>
    void Stop();
}
