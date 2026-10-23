# AetherWM 🪟⚡

> A lightweight, modular tiling window manager and ricing suite for Windows 10 & 11, inspired by the Linux desktop experience (bspwm, Hyprland, Polybar).

---

## 🌟 Vision

**AetherWM** brings the minimalist, keyboard-centric, and highly customizable workflow of Linux tiling window managers to Windows, without bloatware, proprietary black boxes, or kernel-level modifications.

- 🧩 **Binary Space Partitioning (BSP)**: Automatically tiles and organizes your windows without manual dragging.
- ⚡ **Lightning Fast & Ultra-lightweight**: Built with .NET 9 and native Win32/DWM APIs.
- ⌨️ **Vim-inspired Keyboard Navigation**: Control focus, swaps, and workspaces entirely from your keyboard.
- 📜 **Dotfile Driven**: Single readable configuration file (`config.json` / `config.toml`) with hot-reload.
- 📊 **Modular Architecture**: Separate Core, Win32 Platform layer, and Daemon, ready for status bar and launcher extensions.

---

## 🏗️ Architecture

```
AetherWM/
├── src/
│   ├── AetherWM.Core/           # Pure tiling logic, BSP tree math, models & interfaces
│   ├── AetherWM.Platform.Win32/ # Safe wrappers for Win32 API (User32, DwmApi, Shell32)
│   └── AetherWM.Daemon/         # Background service, hotkey engine & event loop
├── tests/
│   └── AetherWM.Core.Tests/     # Unit tests for screen partitioning and tree logic
└── AetherWM.sln
```

---

## 🚀 Building & Running

### Prerequisites
- [.NET 9.0+ SDK](https://dotnet.microsoft.com/download)
- Windows 10 (version 1903+) or Windows 11

### Build from source
```powershell
git clone https://github.com/your-username/AetherWM.git
cd AetherWM
dotnet build
```

### Run tests
```powershell
dotnet test
```

---

## 📄 License
This project is licensed under the [MIT License](LICENSE).
