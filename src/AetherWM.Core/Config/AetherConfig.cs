namespace AetherWM.Core.Config;

/// <summary>
/// Root configuration schema for AetherWM dotfiles.
/// </summary>
public class AetherConfig
{
    public int InnerGap { get; set; } = 10;
    public int OuterGap { get; set; } = 12;
    public string TerminalPath { get; set; } = "wt.exe";
    public bool AutoTileOnLaunch { get; set; } = true;

    public List<string> IgnoredProcesses { get; set; } =
    [
        "Progman",
        "WorkerW",
        "Shell_TrayWnd",
        "Shell_SecondaryTrayWnd",
        "Taskmgr",
        "SnippingTool"
    ];

    public Dictionary<string, string> Keybindings { get; set; } = new()
    {
        { "Alt+Shift+Enter", "SpawnTerminal" },
        { "Alt+Shift+T", "Retile" },
        { "Alt+Shift+Q", "CloseWindow" },
        { "Alt+Shift+H", "FocusLeft" },
        { "Alt+Shift+L", "FocusRight" },
        { "Alt+Shift+Escape", "Quit" }
    };
}
