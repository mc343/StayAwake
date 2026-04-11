# StayAwake

A modern Windows utility that keeps your computer awake and maintains your "available" status in apps like Microsoft Teams, Slack, Zoom, and more.

## Features

### Core Functionality
- **Prevents Sleep Mode**: Keeps your computer awake and prevents screen dimming
- **Status Keeper**: Simulates subtle mouse/keyboard activity to maintain "Available" status
- **Portable**: Single executable, no installation required
- **System Tray**: Runs quietly in the background with easy controls

### Dashboard Features
- **Duration Selection**: Choose from presets (30min, 1hr, 2hrs, 4hrs, 8hrs, Unlimited) or set custom time
- **Activity Levels**: Three sensitivity modes
  - **Subtle**: 1 pixel movement every 45 seconds (barely noticeable)
  - **Medium**: 2 pixels every 30 seconds (balanced)
  - **Noticeable**: 5 pixels every 20 seconds with occasional keypress
- **Schedule Mode**: Set automatic start/stop times (e.g., work hours 9 AM - 5 PM)
- **Activity Log**: View recent activity history with timestamps
- **Hotkey Support**: Press `Ctrl+Shift+A` to quickly toggle on/off
- **Minimize to Tray**: Option to run silently in background

## How It Works

1. Uses Windows API (`SetThreadExecutionState`) to prevent system/display sleep
2. Simulates mouse movement (1-5 pixels) at configurable intervals
3. Optional simulated keypress (Shift key) for more noticeable activity
4. Randomized intervals make activity appear natural and undetectable

## Screenshots

The dashboard includes:
- **Duration selector** with presets and custom time input
- **Start/Stop button** with visual feedback
- **Status display** showing time remaining and activity count
- **Settings panel** for activity sensitivity and preferences
- **Schedule panel** for automatic operation
- **Activity log** with timestamped entries

## Building

### Prerequisites
- .NET 8.0 SDK ([Download here](https://dotnet.microsoft.com/download))

### Build Steps

**Option 1: Using the build script**
```batch
build.bat
```

**Option 2: Manual build**
```batch
cd src
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true
```

The executable will be created in `bin\StayAwake.exe`

## Usage

### Basic Usage
1. Run `StayAwake.exe`
2. Select how long you want to stay awake
3. Choose your preferred activity level
4. Click **START**
5. The app will minimize to tray (optional) and keep you active

### System Tray
- **Double-click** the tray icon to show/hide the dashboard
- **Right-click** for options:
  - Show/Hide
  - Start/Stop
  - Exit

### Hotkey
- Press `Ctrl+Shift+A` to toggle the app on/off from anywhere

### Schedule Mode
1. Check "Enable automatic schedule"
2. Set your start and end times
3. The app will automatically start/stop based on the schedule
4. Perfect for work hours!

## Tips

- **Subtle mode** is best for avoiding detection
- Add `StayAwake.exe` to Windows Startup folder for automatic launch
- The executable is completely portable - copy it anywhere
- Works with Teams, Slack, Zoom, Discord, and most status-aware apps

## Project Structure

```
StayAwake/
├── src/
│   ├── StayAwake.csproj    # Project configuration
│   ├── Program.cs          # Application entry point
│   └── MainForm.cs         # Main dashboard UI
├── bin/                    # Built executable (created after build)
├── build.bat               # Quick build script
└── README.md               # This file
```

## Technical Details

- **Framework**: .NET 8 Windows Forms
- **Target**: Windows x64 (self-contained)
- **Single File**: Yes (no dependencies required)
- **Size**: ~60-80 MB (due to .NET runtime)

## Privacy Note

This app:
- Does not collect or transmit any data
- Does not monitor your screen or activities
- Only simulates mouse/keyboard input locally
- Is completely offline and private
