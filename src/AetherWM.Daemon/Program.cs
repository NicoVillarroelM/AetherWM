using System.Diagnostics;
using AetherWM.Core.Config;
using AetherWM.Core.Hotkeys;
using AetherWM.Core.Interfaces;
using AetherWM.Core.Models;
using AetherWM.Core.Tiling;
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

Console.WriteLine("⚡ AetherWM Daemon - Sprint 3 & 4: Atajos Globales y Dotfiles en Caliente\n");

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
bool isRunning = true;

// Función para retilar ventanas
void RetileWindows()
{
    var workArea = windowService.GetPrimaryMonitorWorkArea();
    var activeWindows = windowService.GetActiveWindows();

    // Guardar posición inicial si no se tenía
    foreach (var win in activeWindows)
    {
        originalPositions.TryAdd(win.Handle, win.Bounds);
    }

    var handles = activeWindows.Select(w => w.Handle).Take(6).ToList();
    tilingEngine.SyncWindows(handles);
    var layout = tilingEngine.GenerateLayout(workArea);
    windowService.ApplyLayout(layout);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine($"[AetherWM] Mosaico actualizado: {layout.Count} ventanas ordenadas (Gaps: Int={tilingEngine.InnerGap}px, Ext={tilingEngine.OuterGap}px)");
    Console.ResetColor();
}

// Escuchar cambios en caliente del archivo config.json (Hot-Reload)
configService.ConfigChanged += newConfig =>
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine($"\n🔄 [Hot-Reload] ¡Cambio detectado en config.json! Actualizando parámetros...");
    Console.ResetColor();

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
        {
            Console.WriteLine($"[Atajo Omitido] Combinación no válida: {binding}");
            continue;
        }

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
                    // Fallback to powershell if wt.exe is not installed
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
            "Quit" => () =>
            {
                isRunning = false;
            },
            _ => () => Console.WriteLine($"Acción desconocida: {actionName}")
        };

        hotkeyService.Register(mods, vk, callback);
    }
}

SetupHotkeys(config);
hotkeyService.Start();

Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine("⌨️  Atajos globales activos en segundo plano:");
foreach (var (binding, action) in config.Keybindings)
{
    Console.WriteLine($"   • {binding,-18} -> {action}");
}
Console.ResetColor();

if (config.AutoTileOnLaunch)
{
    Console.WriteLine("\n🚀 AutoTileOnLaunch activado: organizando ventanas iniciales...");
    RetileWindows();
}

Console.ForegroundColor = ConsoleColor.Cyan;
Console.WriteLine("\n✨ AetherWM Daemon está corriendo.");
Console.WriteLine("💡 Puedes presionar Alt+Shift+T para reorganizar, o Alt+Shift+Enter para abrir una terminal.");
Console.WriteLine("💡 O edita tu archivo config.json para ver la recarga de márgenes en caliente.");
Console.WriteLine("Presiona Ctrl+C o la tecla 'Q' en esta consola para salir y restaurar tus ventanas.\n");
Console.ResetColor();

// Loop interactivo seguro de consola
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
Console.WriteLine("\n🔄 Restaurando ventanas y apagando AetherWM...");
Console.ResetColor();

hotkeyService.Stop();
windowService.ApplyLayout(originalPositions);

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine("✅ Ventanas restauradas a su estado original. ¡Hasta luego!\n");
Console.ResetColor();
