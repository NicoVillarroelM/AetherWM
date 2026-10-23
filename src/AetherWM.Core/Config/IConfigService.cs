namespace AetherWM.Core.Config;

/// <summary>
/// Abstraction for loading, creating, and hot-reloading AetherWM configuration dotfiles.
/// </summary>
public interface IConfigService : IDisposable
{
    /// <summary>
    /// Current loaded configuration.
    /// </summary>
    AetherConfig CurrentConfig { get; }

    /// <summary>
    /// Path to the configuration file on disk.
    /// </summary>
    string ConfigFilePath { get; }

    /// <summary>
    /// Event triggered when configuration file changes on disk and is reloaded.
    /// </summary>
    event Action<AetherConfig>? ConfigChanged;

    /// <summary>
    /// Loads existing configuration or initializes default config file.
    /// </summary>
    AetherConfig LoadOrCreateDefault();

    /// <summary>
    /// Persists updated configuration to disk.
    /// </summary>
    void SaveConfig(AetherConfig config);
}
