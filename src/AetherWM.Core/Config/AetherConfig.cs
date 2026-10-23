namespace AetherWM.Core.Config;

/// <summary>
/// Root configuration schema for AetherWM dotfiles.
/// </summary>
public class AetherConfig
{
    // Layout and Gaps
    public int InnerGap { get; set; } = 10;
    public int OuterGap { get; set; } = 12;
    public bool AutoTileOnLaunch { get; set; } = true;

    // Window Aesthetics (Windows 11 DWM)
    public bool EnableBorderStyling { get; set; } = true;
    public string ActiveBorderColor { get; set; } = "#00F0FF";   // Neon Cyan
    public string InactiveBorderColor { get; set; } = "#2E3440"; // Dark Slate Gray
    public string CornerPreference { get; set; } = "Square";     // "Square", "Rounded", "Default"

    // System
    public string TerminalPath { get; set; } = "wt.exe";

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
