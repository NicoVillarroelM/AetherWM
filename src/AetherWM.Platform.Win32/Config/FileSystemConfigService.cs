using System.Text.Json;
using AetherWM.Core.Config;

namespace AetherWM.Platform.Win32.Config;

public class FileSystemConfigService : IConfigService
{
    private readonly string _configDirectory;
    private readonly string _configFilePath;
    private FileSystemWatcher? _watcher;
    private DateTime _lastReadTime = DateTime.MinValue;
    private readonly object _lock = new();

    private static readonly JsonSerializerOptions JsonOptions = new()
    {
        WriteIndented = true,
        PropertyNameCaseInsensitive = true
    };

    public AetherConfig CurrentConfig { get; private set; } = new();

    public string ConfigFilePath => _configFilePath;

    public event Action<AetherConfig>? ConfigChanged;

    public FileSystemConfigService(string? customDirectory = null)
    {
        _configDirectory = customDirectory ?? Path.Combine(
            Environment.GetFolderPath(Environment.SpecialFolder.UserProfile),
            ".config",
            "aetherwm"
        );
        _configFilePath = Path.Combine(_configDirectory, "config.json");
    }

    public AetherConfig LoadOrCreateDefault()
    {
        lock (_lock)
        {
            try
            {
                if (!Directory.Exists(_configDirectory))
                {
                    Directory.CreateDirectory(_configDirectory);
                }

                if (!File.Exists(_configFilePath))
                {
                    CurrentConfig = new AetherConfig();
                    string json = JsonSerializer.Serialize(CurrentConfig, JsonOptions);
                    File.WriteAllText(_configFilePath, json);
                }
                else
                {
                    string json = File.ReadAllText(_configFilePath);
                    var loaded = JsonSerializer.Deserialize<AetherConfig>(json, JsonOptions);
                    CurrentConfig = loaded ?? new AetherConfig();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Config Warning] Could not load config, using defaults: {ex.Message}");
                CurrentConfig = new AetherConfig();
            }

            SetupWatcher();
            return CurrentConfig;
        }
    }

    public void SaveConfig(AetherConfig config)
    {
        lock (_lock)
        {
            try
            {
                if (!Directory.Exists(_configDirectory))
                {
                    Directory.CreateDirectory(_configDirectory);
                }

                CurrentConfig = config;
                string json = JsonSerializer.Serialize(config, JsonOptions);
                File.WriteAllText(_configFilePath, json);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"[Config Error] Could not save config: {ex.Message}");
            }
        }
    }

    private void SetupWatcher()
    {
        if (_watcher != null) return;

        try
        {
            _watcher = new FileSystemWatcher(_configDirectory, "config.json")
            {
                NotifyFilter = NotifyFilters.LastWrite | NotifyFilters.Size | NotifyFilters.FileName,
                EnableRaisingEvents = true
            };

            _watcher.Changed += OnFileChanged;
            _watcher.Created += OnFileChanged;
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[Watcher Warning] Could not setup file watcher: {ex.Message}");
        }
    }

    private void OnFileChanged(object sender, FileSystemEventArgs e)
    {
        lock (_lock)
        {
            if (DateTime.UtcNow - _lastReadTime < TimeSpan.FromMilliseconds(300))
                return;

            _lastReadTime = DateTime.UtcNow;

            for (int i = 0; i < 3; i++)
            {
                try
                {
                    Thread.Sleep(50);
                    string json = File.ReadAllText(_configFilePath);
                    var reloaded = JsonSerializer.Deserialize<AetherConfig>(json, JsonOptions);
                    if (reloaded != null)
                    {
                        CurrentConfig = reloaded;
                        ConfigChanged?.Invoke(CurrentConfig);
                        break;
                    }
                }
                catch
                {
                    // Retry
                }
            }
        }
    }

    public void Dispose()
    {
        _watcher?.Dispose();
        GC.SuppressFinalize(this);
    }
}
