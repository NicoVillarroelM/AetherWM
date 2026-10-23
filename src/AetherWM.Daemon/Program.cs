using AetherWM.Core.Interfaces;
using AetherWM.Core.Models;
using AetherWM.Core.Tiling;
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

Console.WriteLine("⚡ AetherWM Daemon - Sprint 2: Motor de Mosaico (BSP Engine)\n");

IWindowService windowService = new Win32WindowService();

var workArea = windowService.GetPrimaryMonitorWorkArea();
Console.ForegroundColor = ConsoleColor.Yellow;
Console.WriteLine($"🖥️  Área de trabajo del monitor: {workArea.Width}x{workArea.Height} (Origen: X={workArea.X}, Y={workArea.Y})\n");
Console.ResetColor();

Console.WriteLine("🔍 Escaneando ventanas gestionables del usuario...");
var windows = windowService.GetActiveWindows();

if (windows.Count == 0)
{
    Console.ForegroundColor = ConsoleColor.Red;
    Console.WriteLine("No se encontraron ventanas activas de usuario para ordenar.");
    Console.ResetColor();
    return;
}

Console.ForegroundColor = ConsoleColor.Green;
Console.WriteLine($"\n✅ Se detectaron {windows.Count} ventanas activas:");
Console.ResetColor();

// Limit to top 6 windows for tiling to avoid over-crowding small screens
var windowsToTile = windows.Take(6).ToList();

var tilingEngine = new TilingEngine
{
    InnerGap = 10,
    OuterGap = 12
};

tilingEngine.SyncWindows(windowsToTile.Select(w => w.Handle));
var calculatedLayout = tilingEngine.GenerateLayout(workArea);

Console.WriteLine($"\n📐 Layout BSP Calculado (Márgenes: Int={tilingEngine.InnerGap}px, Ext={tilingEngine.OuterGap}px):");
Console.WriteLine($"{"PROCESO",-20} | {"POSICIÓN ANTERIOR",-24} | {"NUEVO MOSAICO BSP",-24}");
Console.WriteLine(new string('-', 75));

foreach (var win in windowsToTile)
{
    if (calculatedLayout.TryGetValue(win.Handle, out var newBounds))
    {
        string oldGeom = $"{win.Bounds.Width}x{win.Bounds.Height} (+{win.Bounds.X}+{win.Bounds.Y})";
        string newGeom = $"{newBounds.Width}x{newBounds.Height} (+{newBounds.X}+{newBounds.Y})";
        Console.WriteLine($"{win.ProcessName,-20} | {oldGeom,-24} | {newGeom,-24}");
    }
}
Console.WriteLine(new string('-', 75));

bool applyLive = args.Contains("--apply") || args.Contains("-a");

if (applyLive)
{
    Console.ForegroundColor = ConsoleColor.Magenta;
    Console.WriteLine("\n✨ MODO EN VIVO: Aplicando mosaico a las ventanas en pantalla...");
    Console.ResetColor();

    // Guardar posiciones originales para restaurar al salir
    var originalPositions = windowsToTile.ToDictionary(w => w.Handle, w => w.Bounds);

    windowService.ApplyLayout(calculatedLayout);

    Console.ForegroundColor = ConsoleColor.Green;
    Console.WriteLine("✅ Mosaico aplicado correctamente.");
    Console.ResetColor();

    Console.WriteLine("\nPresiona ENTER para restaurar todas las ventanas a su estado original...");
    Console.ReadLine();

    Console.ForegroundColor = ConsoleColor.Yellow;
    Console.WriteLine("🔄 Restaurando posiciones originales...");
    Console.ResetColor();
    windowService.ApplyLayout(originalPositions);
    Console.WriteLine("✅ Ventanas restauradas.");
}
else
{
    Console.ForegroundColor = ConsoleColor.Cyan;
    Console.WriteLine("\n💡 Para aplicar el mosaico en vivo en tus pantallas y ver cómo se acomodan:");
    Console.WriteLine("   dotnet run --project src/AetherWM.Daemon -- --apply");
    Console.ResetColor();
}
