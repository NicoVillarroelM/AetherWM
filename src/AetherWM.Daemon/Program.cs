using System.Diagnostics;
using AetherWM.Core.Config;
using AetherWM.Core.Hotkeys;
using AetherWM.Core.Interfaces;
using AetherWM.Core.Models;
using AetherWM.Core.Tiling;
using AetherWM.Core.Utils;
using AetherWM.Platform.Win32.Config;
using AetherWM.Platform.Win32.Hotkeys;
using AetherWM.Platform.Win32.Native;
using AetherWM.Platform.Win32.Services;

Console.OutputEncoding = System.Text.Encoding.UTF8;

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine(@"
    ___       __  __            _       ____  ___
   /   | ___ / /_/ /_  ___  ___| |     / /  |/  /
  / /| |/ _ \ __/ __ \/ _ \/ ___/ | /| / / /|_/ / 
 / ___ /  __/ /_/ / / /  __/ /   | |/ |/ / /  / /  
/_/  |_|\___/\__/_/ /_/\___/_/    |__/|__/_/  /_/   
");
Console.ResetColor();

Console.WriteLine("⚡ AetherWM Daemon - Bordes Dinámicos, Estilo DWM y Atajos\n");

// 1. Cargar Configuración (Dotfile)
using IConfigService configService = new FileSystemConfigService();
var config = configService.LoadOrCreateDefault();

Console.ForegroundColor = ConsoleColor.DarkGray;
Console.WriteLine($"📄 Archivo de configuración: {configService.ConfigFilePath}");
Console.ResetColor();

// 2. Servicios del sistema
IWindowService windowService = new Win32WindowService();
using IHotkeyService hotkeyService = new Win32HotkeyService();
var tilingEngine = new TilingEngine
{
    InnerGap = config.InnerGap,
    OuterGap = config.OuterGap
};

Dictionary<IntPtr, ScreenRectangle> originalPositions = [];
IntPtr lastFocusedHandle = IntPtr.Zero;
bool isRunning = true;

// Mapear opciones de esquinas
int GetCornerPreferenceCode(string pref) => pref.ToLowerInvariant() switch
{
    "square" or "straight" => DwmApi.DWMWCP_DONOTROUND,
    "rounded" => DwmApi.DWMWCP_ROUND,
    "small" => DwmApi.DWMWCP_ROUNDSMALL,
    _ => DwmApi.DWMWCP_DEFAULT
};

// Aplicar estilos visuales (bordes de colores y esquinas)
void ApplyVisualStyles(IEnumerable<IntPtr> handles, IntPtr focusedHandle)
{
    if (!config.EnableBorderStyling) return;

    uint activeColor = ColorUtils.ToColorRef(config.ActiveBorderColor);
    uint inactiveColor = ColorUtils.ToColorRef(config.InactiveBorderColor);
    int cornerCode = GetCornerPreferenceCode(config.CornerPreference);

    foreach (var h in handles)
    {
        windowService.SetWindowCornerPreference(h, cornerCode);
        windowService.SetWindowBorderColor(h, h == focusedHandle ? activeColor : inactiveColor);
    }
}

// Función para retilar ventanas
void RetileWindows()
{
    var workArea = windowService.GetPrimaryMonitorWorkArea();
    var activeWindows = windowService.GetActiveWindows();

    foreach (var win in activeWindows)
    {
        originalPositions.TryAdd(win.Handle, win.Bounds);
    }

    var handles = activeWindows.Select(w => w.Handle).Take(6).ToList();
    tilingEngine.SyncWindows(handles);
    var layout = tilingEngine.GenerateLayout(workArea);
    windowService.ApplyLayout(layout);

    IntPtr currentForeground = User32.GetForegroundWindow();
    ApplyVisualStyles(handles, currentForeground);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[AetherWM] Mosaico y estilos actualizados: {layout.Count} ventanas (Gaps: {tilingEngine.InnerGap}px | Borde activo: {config.ActiveBorderColor})");
    Console.ResetColor();
}

// Escuchar cambios de foco de ventanas en tiempo real (Resaltar ventana activa con DWM)
windowService.WindowFocused += (newFocusHandle) =>
{
    if (!config.EnableBorderStyling) return;

    uint activeColor = ColorUtils.ToColorRef(config.ActiveBorderColor);
    uint inactiveColor = ColorUtils.ToColorRef(config.InactiveBorderColor);

    if (lastFocusedHandle != IntPtr.Zero && lastFocusedHandle != newFocusHandle)
    {
        windowService.SetWindowBorderColor(lastFocusedHandle, inactiveColor);
    }

    windowService.SetWindowBorderColor(newFocusHandle, activeColor);
    lastFocusedHandle = newFocusHandle;
};
windowService.StartFocusTracking();

// Escuchar cambios en caliente del archivo config.json (Hot-Reload)
configService.ConfigChanged += newConfig =>
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"\n🔄 [Hot-Reload] ¡Cambio detectado en config.json! Actualizando parámetros y estilos...");
    Console.ResetColor();

    config = newConfig;
    tilingEngine.InnerGap = newConfig.InnerGap;
    tilingEngine.OuterGap = newConfig.OuterGap;
    RetileWindows();
};

// 3. Mapear Atajos Globales
void SetupHotkeys(AetherConfig currentConfig)
{
    hotkeyService.UnregisterAll();

    foreach (var (binding, actionName) in currentConfig.Keybindings)
    {
        if (!HotkeyParser.TryParse(binding, out var mods, out int vk))
            continue;

        Action callback = actionName switch
        {
            "SpawnTerminal" => () =>
            {
                try
                {
                    Process.Start(new ProcessStartInfo(currentConfig.TerminalPath) { UseShellExecute = true });
                }
                catch
                {
                    Process.Start(new ProcessStartInfo("powershell.exe") { UseShellExecute = true });
                }
            },
            "Retile" => () => RetileWindows(),
            "CloseWindow" => () =>
            {
                IntPtr focused = User32.GetForegroundWindow();
                if (focused != IntPtr.Zero)
                {
                    User32.PostMessage(focused, User32.WM_CLOSE, 0, 0);
                }
            },
            "FocusLeft" => () =>
            {
                var wins = windowService.GetActiveWindows();
                if (wins.Count > 1)
                {
                    User32.SetForegroundWindow(wins[^1].Handle);
                }
            },
            "FocusRight" => () =>
            {
                var wins = windowService.GetActiveWindows();
                if (wins.Count > 1)
                {
                    User32.SetForegroundWindow(wins[0].Handle);
                }
            },
            "Quit" => () => isRunning = false,
            _ => () => { }
        };

        hotkeyService.Register(mods, vk, callback);
    }
}

SetupHotkeys(config);
hotkeyService.Start();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("🎨 Estilos visuales activos:");
Console.WriteLine($"   • Borde Activo:   {config.ActiveBorderColor}");
Console.WriteLine($"   • Borde Inactivo: {config.InactiveBorderColor}");
Console.WriteLine($"   • Esquinas:       {config.CornerPreference}");
Console.WriteLine("\n⌨️  Atajos globales activos en segundo plano:");
foreach (var (binding, action) in config.Keybindings)
{
    Console.WriteLine($"   • {binding,-18} -> {action}");
}
Console.ResetColor();

if (config.AutoTileOnLaunch)
{
    Console.WriteLine("\n🚀 AutoTileOnLaunch activado: organizando ventanas y aplicando bordes...");
    RetileWindows();
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n✨ AetherWM Daemon está corriendo.");
Console.WriteLine("💡 Cambia de ventana con el ratón o Alt+Tab para ver cómo el borde neón salta a la ventana activa.");
Console.WriteLine("💡 Presiona 'Q' o Ctrl+C en esta consola para salir y restaurar ventanas y bordes originales.\n");
Console.ResetColor();

while (isRunning)
{
    if (Console.KeyAvailable)
    {
        var key = Console.ReadKey(intercept: true);
        if (key.Key == ConsoleKey.Q || (key.Modifiers.HasFlag(ConsoleModifiers.Control) && key.Key == ConsoleKey.C))
        {
            break;
        }
    }
    Thread.Sleep(100);
}

// Restauración limpia al salir
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("\n🔄 Restaurando ventanas y bordes originales de Windows...");
Console.ResetColor();

windowService.StopFocusTracking();
hotkeyService.Stop();

// Restaurar bordes y esquinas de Windows a su valor original por defecto
foreach (var (handle, _) in originalPositions)
{
    windowService.SetWindowBorderColor(handle, DwmApi.DWMWA_COLOR_DEFAULT);
    windowService.SetWindowCornerPreference(handle, DwmApi.DWMWCP_DEFAULT);
}

windowService.ApplyLayout(originalPositions);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Sistema restaurado al estado original. ¡Hasta luego!\n");
Console.ResetColor();
