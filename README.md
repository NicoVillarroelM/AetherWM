# AetherWM 🪟⚡

> Un gestor de ventanas en mosaico (Tiling Window Manager) y suite de personalización modular, ultraligero y de código abierto para Windows 10 y 11, inspirado en la experiencia y filosofía de Linux (bspwm, Hyprland, Polybar).

---

## 🌟 Visión del Proyecto

**AetherWM** traslada el flujo de trabajo minimalista, modular y controlado por teclado propio de Linux al entorno de Windows, sin necesidad de software propietario pesado, sin telemetría invasiva y sin modificar archivos protegidos del sistema operativo.

- 🧩 **Partición de Espacio Binario (BSP)**: Organiza y ajusta automáticamente tus ventanas en mosaico dinámico sin tener que arrastrarlas manualmente.
- ⚡ **Ultraligero y Rendimiento Nativo**: Desarrollado en .NET interactuando de forma directa y segura con las APIs de bajo nivel de Windows (Win32 y DWM).
- ⌨️ **Navegación Eficiente por Teclado**: Control de foco, intercambio de ventanas y acciones rápidas mediante atajos inspirados en Vim.
- 📜 **Configuración mediante Dotfiles**: Archivo de configuración único, limpio y legible (`config.json`) alojado en `%USERPROFILE%\.config\aetherwm\` con recarga en caliente (*hot-reload*) al guardar cambios.
- 🧱 **Arquitectura Totalmente Modular**: Lógica matemática pura desacoplada de las llamadas a Windows, lista para acoplar barra superior (*status bar*) y lanzador rápido (*launcher*).

---

## 🏗️ Arquitectura Modular

```
AetherWM/
├── config/
│   └── config.example.json      # Plantilla base de dotfiles para el usuario
├── src/
│   ├── AetherWM.Core/           # Lógica pura de mosaicos, árbol BSP, parser de atajos, modelos
│   ├── AetherWM.Platform.Win32/ # Capa aislada y segura de Win32 (Hooks, DWM, Hotkeys, FileWatcher)
│   └── AetherWM.Daemon/         # Servicio en segundo plano, atajos de teclado y bucle de eventos
├── tests/
│   └── AetherWM.Core.Tests/     # Pruebas unitarias automatizadas (Geometría, BSP y Atajos)
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
| `Alt + Shift + Escape`| **Quit** | Restaura todas las ventanas a su tamaño original y cierra el daemon |

> [!TIP]
> Todos los atajos y márgenes se pueden personalizar libremente editando tu archivo `%USERPROFILE%\.config\aetherwm\config.json`. Al guardar el archivo en VS Code / Notepad, los cambios se aplican automáticamente sin reiniciar el programa.

---

## 🚀 Compilación y Pruebas Locales

### Requisitos
- [SDK de .NET 9.0 o superior](https://dotnet.microsoft.com/download)
- Windows 10 (versión 1903+) o Windows 11

### Compilar desde el código fuente
```powershell
git clone https://github.com/NicoVillarroelM/AetherWM.git
cd AetherWM
dotnet build
```

### Ejecutar las pruebas unitarias
```powershell
dotnet test
```

### Ejecutar el Daemon
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
- [ ] **Sprint 5**: Barra de estado superior translúcida (AetherBar con AppBar nativa).
- [ ] **Sprint 6**: Lanzador modal de aplicaciones (AetherRun).
- [ ] **Sprint 7**: Empaquetado nativo (Native AOT) y publicación en Winget.

---

## 📄 Licencia

Este proyecto está bajo la [Licencia MIT](LICENSE). Siéntete libre de usarlo, modificarlo y contribuir.
