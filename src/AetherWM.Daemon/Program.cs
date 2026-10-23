using AetherWM.Core.Interfaces;
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

Console.WriteLine("⚡ AetherWM Daemon - Sprint 1: Detección y Filtrado de Ventanas\n");

IWindowService windowService = new Win32WindowService();

var workArea = windowService.GetPrimaryMonitorWorkArea();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"🖥️  Área de trabajo del monitor: {workArea.Width}x{workArea.Height} (Origen: X={workArea.X}, Y={workArea.Y})\n");
Console.ResetColor();

Console.WriteLine("🔍 Escaneando ventanas gestionables del usuario...");
var windows = windowService.GetActiveWindows();

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n✅ Se detectaron {windows.Count} ventanas activas de usuario:\n");
Console.ResetColor();

Console.WriteLine($"{"HWND",-12} | {"PROCESO",-18} | {"GEOMETRÍA",-24} | {"TÍTULO"}");
Console.WriteLine(new string('-', 95));

foreach (var win in windows)
{
    string hexHwnd = $"0x{win.Handle.ToInt64():X8}";
    string geom = $"{win.Bounds.Width}x{win.Bounds.Height} (+{win.Bounds.X}+{win.Bounds.Y})";
    string truncatedTitle = win.Title.Length > 35 ? win.Title[..32] + "..." : win.Title;

    Console.WriteLine($"{hexHwnd,-12} | {win.ProcessName,-18} | {geom,-24} | {truncatedTitle}");
}

Console.WriteLine(new string('-', 95));
