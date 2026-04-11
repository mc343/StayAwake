# ⏰ StayAwake

> **Never let your status go "Away" again** – Keep your system awake and maintain an "Available" status in Teams, Slack, Zoom, Discord, and more.

[![GitHub release](https://img.shields.io/github/v/release/mc343/StayAwake)](https://github.com/mc343/StayAwake/releases)
[![License](https://img.shields.io/github/license/mc343/StayAwake)](LICENSE)
[![Downloads](https://img.shields.io/github/downloads/mc343/StayAwake/total)](https://github.com/mc343/StayAwake/releases)

---

## ✨ Why StayAwake?

### 🎯 The Problem
- Your computer goes to sleep during long meetings or downloads
- Teams/Slack status changes to "Away" after just a few minutes of inactivity
- You look "unavailable" even when you're working hard
- Screen dimming interrupts your workflow

### 💡 The Solution
**StayAwake** is a smart Windows utility that keeps your system awake and maintains an "Available" status by simulating natural user activity. It's like having a tiny assistant keeping you present, even when you're deep in focus.

---

## 🚀 Key Features

### 🖥️ **System Sleep Prevention**
- Prevents Windows from sleeping or hibernating
- Stops screen from dimming or turning off
- Keeps your applications running uninterrupted

### 🟢 **Status Keeper**
- Maintains "Available" status in:
  - **Microsoft Teams** ✓
  - **Slack** ✓
  - **Zoom** ✓
  - **Discord** ✓
  - **Skype** ✓
  - And most status-aware apps!

### 🎛️ **Modern Dashboard UI**
- Clean, intuitive interface
- Choose from preset durations (30min → Unlimited)
- Set custom time limits
- Real-time status display
- Activity log with timestamps

### ⚡ **Smart Activity Levels**
- **Subtle** – 1 pixel movement every 45s (barely noticeable)
- **Medium** – 2 pixels every 30s (balanced)
- **Noticeable** – 5 pixels every 20s with occasional keypress

### ⏰ **Schedule Mode**
- Set automatic start/stop times
- Perfect for work hours (e.g., 9 AM – 5 PM)
- Runs only when you need it

### 🎹 **Hotkey Support**
- Press `Ctrl+Shift+A` to toggle anywhere
- Quick and discreet control

### 🔔 **System Tray Support**
- Runs silently in the background
- Minimize to tray option
- Quick access via tray icon

---

## 📸 Screenshots

### Main Dashboard
```
┌─────────────────────────────────────┐
│  ⏰ StayAwake                        │
├─────────────────────────────────────┤
│  Duration: [1 hour ▼]                │
│  ┌─────────────────────────────────┐ │
│  │     ▶ START                     │ │
│  └─────────────────────────────────┘ │
│  ● Running                          │
│  Time Remaining: 00:45:23           │
│  Activities: 12                     │
├─────────────────────────────────────┤
│  Settings & Schedule               │
└─────────────────────────────────────┘
```

---

## 🎬 Quick Start

### Option 1: Download & Run (Easiest)
1. **Download** the latest `StayAwake.exe` from [Releases](https://github.com/mc343/StayAwake/releases)
2. **Double-click** to run (no installation needed!)
3. **Select duration** and click **START**
4. **Done!** Your system stays awake and status remains "Available"

### Option 2: Build from Source
```bash
git clone https://github.com/mc343/StayAwake.git
cd StayAwake/src
dotnet publish -c Release -r win-x64 --self-contained
```

---

## 💻 How It Works

StayAwake uses smart, imperceptible activity simulation:

```
┌─────────────────────────────────────┐
│  Your Computer                      │
├─────────────────────────────────────┤
│  ┌───────────────────────────────┐  │
│  │ StayAwake runs in background  │  │
│  │                               │  │
│  │ Every 30-60 seconds:          │  │
│  │ • Moves mouse 1-2 pixels      │  │
│  │ • Optional keypress           │  │
│  │ • Randomized for natural feel │  │
│  └───────────────────────────────┘  │
│           ↓                          │
│  ┌───────────────────────────────┐  │
│  │ Windows thinks you're active  │  │
│  └───────────────────────────────┘  │
│           ↓                          │
│  ✓ No sleep                         │
│  ✓ Status = "Available"             │
│  ✓ Apps keep running                │
└─────────────────────────────────────┘
```

---

## 🎯 Use Cases

| Scenario | How StayAwake Helps |
|----------|---------------------|
| **Long meetings** | Keep status green while presenting |
| **Downloads** | Prevent sleep during large downloads |
| **Remote work** | Show "Available" even when deep in focus |
| **Presentations** | Stop screen from dimming |
| **Monitoring** | Keep dashboard apps running overnight |
| **AFK gaming** | Stay online during idle moments |

---

## 🛠️ Configuration

### Settings Panel
- **Activity Level** – Adjust simulation intensity
- **Schedule** – Set automatic work hours
- **Hotkey** – Enable/disable `Ctrl+Shift+A`
- **Minimize to Tray** – Run silently in background

### Command Line Arguments (Advanced)
```bash
# Start with 2-hour duration
StayAwake.exe --duration 2h

# Start with unlimited time
StayAwake.exe --unlimited

# Minimize to tray on start
StayAwake.exe --minimize
```

---

## 📊 Technical Details

| Spec | Details |
|------|---------|
| **Framework** | .NET 8 Windows Forms |
| **Target** | Windows 10/11 x64 |
| **Size** | ~60-80 MB (self-contained) |
| **Memory** | < 50 MB running |
| **CPU** | < 1% when idle |
| **Permissions** | None required (portable) |
| **Network** | 100% offline (no data transmitted) |

---

## 🔒 Privacy & Security

- ✅ **100% Offline** – No internet connection required
- ✅ **No Data Collection** – Nothing is transmitted or stored
- ✅ **Open Source** – Code is transparent and auditable
- ✅ **No Admin Rights** – Runs in user context
- ✅ **Portable** – No installation, no registry changes

**What it does:** Simulates mouse/keyboard input locally
**What it doesn't do:** Monitor your screen, track activity, send data

---

## 🤝 Contributing

Contributions are welcome! Feel free to:
- Report bugs
- Suggest features
- Submit pull requests
- Improve documentation

---

## 📝 License

This project is open source and available under the [MIT License](LICENSE).

---

## 🙏 Acknowledgments

Built with:
- [.NET 8](https://dotnet.microsoft.com/download/dotnet/8.0)
- [Windows Forms](https://docs.microsoft.com/en-us/dotnet/desktop/winforms/)
- [GitHub](https://github.com) for hosting

---

## ⭐ Show Your Support

If you find StayAwake useful:
- ⭐ **Star this repo** – It helps others discover it
- 🍴 **Fork it** – Customize it for your needs
- 🐛 **Report issues** – Help improve it
- 💬 **Share feedback** – Tell me what you think

---

## 📞 Support

- 📧 **Issues:** [GitHub Issues](https://github.com/mc343/StayAwake/issues)
- 💬 **Discussions:** [GitHub Discussions](https://github.com/mc343/StayAwake/discussions)
- 📖 **Wiki:** [Documentation Wiki](https://github.com/mc343/StayAwake/wiki)

---

## 🎉 What's Next?

**Planned Features:**
- [ ] Cross-platform support (macOS, Linux)
- [ ] Custom activity patterns
- [ ] Integration with more apps
- [ ] Cloud status sync
- [ ] Mobile companion app

---

**Made with ❤️ by [mc343](https://github.com/mc343)**

*Keep your status green, stay connected, and never go "Away" again!*

---

<a href="https://github.com/mc343/StayAwake">
<img src="https://img.shields.io/badge/GitHub-StayAwake-brightgreen?style=for-the-badge&logo=github" alt="GitHub">
</a>
<a href="https://github.com/mc343/StayAwake/releases">
<img src="https://img.shields.io/badge/Download-Now-blue?style=for-the-badge&logo=windows" alt="Download">
</a>
