# AetherWM 🪟⚡

> Un gestor de ventanas en mosaico (Tiling Window Manager) y suite de personalización modular, ultraligero y de código abierto para Windows 10 y 11, inspirado en la experiencia y filosofía de Linux (bspwm, Hyprland, Polybar, Conky/eww).

---

## 🌟 Visión del Proyecto

**AetherWM** traslada el flujo de trabajo minimalista, modular y controlado por teclado propio de Linux al entorno de Windows, sin necesidad de software propietario pesado, sin telemetría invasiva y sin modificar archivos protegidos del sistema operativo.

- 🧩 **Partición de Espacio Binario (BSP)**: Organiza y ajusta automáticamente tus ventanas en mosaico dinámico sin tener que arrastrarlas manualmente.
- 🎨 **Bordes Neón Dinámicos (Efecto Hyprland)**: Resalta la ventana en foco con bordes de color personalizados y esquinas rectas o redondeadas con DWM nativo.
- 🕒 **Widgets de Escritorio Nativos (Estilo Conky/eww)**: Paneles translúcidos integrados al fondo de pantalla con reloj digital y monitores de CPU y Memoria RAM en tiempo real.
- ⚙️ **Panel de Control Gráfico (AetherControlCenter)**: Una aplicación moderna para configurar márgenes, elegir colores de bordes y consultar la guía interactiva de atajos sin memorizar comandos.
- ⚡ **Ultraligero y Rendimiento Nativo**: Desarrollado en .NET interactuando de forma directa y segura con las APIs de bajo nivel de Windows (Win32 y DWM).
- 📜 **Configuración mediante Dotfiles**: Archivo de configuración único, limpio y legible (`config.json`) en `%USERPROFILE%\.config\aetherwm\` con recarga en caliente (*hot-reload*) instantánea.

---

## 🏗️ Arquitectura Modular

```
AetherWM/
├── config/
│   └── config.example.json      # Plantilla base de dotfiles para el usuario
├── src/
│   ├── AetherWM.Core/           # Lógica pura de mosaicos, árbol BSP, parser de atajos, modelos
│   ├── AetherWM.Platform.Win32/ # Capa aislada de Win32 (Hooks, DWM, Hotkeys, Hardware Monitor)
│   ├── AetherWM.Daemon/         # Servicio en segundo plano, atajos de teclado y bucle de eventos
│   ├── AetherWM.Widgets/        # Widgets translúcidos anclados al fondo de pantalla (Reloj, CPU, RAM)
│   └── AetherWM.Settings/       # Panel de control gráfico moderno (Mosaico, Colores, Cheatsheet)
├── tests/
│   └── AetherWM.Core.Tests/     # Pruebas unitarias automatizadas (Geometría, BSP, Atajos, Colores)
└── AetherWM.slnx
```

---

## ⌨️ Atajos de Teclado Predeterminados

| Atajo | Acción | Descripción |
| :--- | :--- | :--- |
| `Alt + Shift + Enter` | **SpawnTerminal** | Abre la terminal configurada (`wt.exe` o `powershell.exe`) |
| `Alt + Shift + T` | **Retile** | Reorganiza instantáneamente las ventanas activas en mosaico BSP |
| `Alt + Shift + Q` | **CloseWindow** | Cierra la ventana activa enfrente de forma segura (`WM_CLOSE`) |
| `Alt + Shift + H` | **FocusLeft** | Mueve el foco a la ventana anterior |
| `Alt + Shift + L` | **FocusRight** | Mueve el foco a la siguiente ventana |
| `Alt + Shift + Escape`| **Quit** | Restaura todas las ventanas y bordes a su estado original |

---

## 🚀 Compilación y Ejecución Local

### Requisitos
- [SDK de .NET 9.0 o superior](https://dotnet.microsoft.com/download)
- Windows 10 (versión 1903+) o Windows 11

### Compilar desde el código fuente
```powershell
git clone https://github.com/NicoVillarroelM/AetherWM.git
cd AetherWM
dotnet build
```

### 1. Abrir el Panel de Control Gráfico
```powershell
dotnet run --project src/AetherWM.Settings
```

### 2. Ejecutar el Widget de Escritorio
```powershell
dotnet run --project src/AetherWM.Widgets
```

### 3. Ejecutar el Gestor de Ventanas (Daemon)
```powershell
dotnet run --project src/AetherWM.Daemon
```

---

## 🗺️ Hoja de Ruta (Roadmap)

- [x] **Sprint 0**: Estructura modular, repositorio Git y configuración base.
- [x] **Sprint 1**: Detección nativa de ventanas de usuario y filtrado inteligente del sistema.
- [x] **Sprint 2**: Motor de partición en mosaico (Binary Space Partitioning Tree).
- [x] **Sprint 3**: Gestor de atajos globales (`RegisterHotKey` en bucle de mensajes STA).
- [x] **Sprint 4**: Archivo de configuración (`dotfiles`) con recarga en caliente (*hot-reload*).
- [x] **Sprint 5 (Visual 1)**: Bordes de colores dinámicos y personalización DWM (Efecto Hyprland).
- [x] **Sprint 6 (Visual 2)**: Widgets nativos de escritorio (Reloj minimalista, monitor de CPU y RAM).
- [x] **Sprint 7 (Visual 3)**: App gráfica de configuración y guía de atajos (AetherControlCenter).
- [ ] **Sprint 8**: Barra de estado superior translúcida (AetherBar con AppBar nativa).
- [ ] **Sprint 9**: Empaquetado nativo (Native AOT) y publicación en Winget.

---

## 📄 Licencia

Este proyecto está bajo la [Licencia MIT](LICENSE). Siéntete libre de usarlo, modificarlo y contribuir.
