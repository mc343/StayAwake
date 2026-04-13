# StayAwake - .NET 8.0 SDK Installation Script
# This script downloads and installs .NET 8.0 SDK if not already installed

$ErrorActionPreference = "Stop"
$DotNetVersion = "8.0"
$DotNetInstallerUrl = "https://dot.net/v1/dotnet-install.ps1"
$DesktopPath = [Environment]::GetFolderPath("Desktop")
$ProjectDir = $PSScriptRoot

Write-Host "================================================" -ForegroundColor Cyan
Write-Host "  StayAwake - .NET SDK Installation Script" -ForegroundColor Cyan
Write-Host "================================================" -ForegroundColor Cyan
Write-Host ""

# Function to check if .NET SDK is installed
function Test-DotNetInstalled {
    try {
        $result = & dotnet --list-sdks 2>&1
        if ($LASTEXITCODE -eq 0) {
            foreach ($sdk in $result) {
                if ($sdk -match "8\.\d+\.\d+") {
                    return $true
                }
            }
        }
    }
    catch {
        return $false
    }
    return $false
}

function Add-DotNetToPath {
    $dotnetPath = "$env:USERPROFILE\.dotnet"
    if (Test-Path (Join-Path $dotnetPath "dotnet.exe")) {
        if ($env:PATH -notlike "*$dotnetPath*") {
            $env:PATH = "$dotnetPath;$env:PATH"
        }

        $userPath = [Environment]::GetEnvironmentVariable("PATH", "User")
        if ($userPath -notlike "*$dotnetPath*") {
            [Environment]::SetEnvironmentVariable("PATH", "$dotnetPath;$userPath", "User")
        }

        return $true
    }

    return $false
}

# Function to install .NET SDK
function Install-DotNetSdk {
    Write-Host "Downloading .NET Installer..." -ForegroundColor Yellow

    $installerPath = "$env:TEMP\dotnet-install.ps1"

    try {
        # Download the installer script
        Invoke-WebRequest -Uri $DotNetInstallerUrl -OutFile $installerPath -UseBasicParsing
        Write-Host "Download completed." -ForegroundColor Green
    }
    catch {
        Write-Host "Failed to download .NET installer: $_" -ForegroundColor Red
        Write-Host ""
        Write-Host "Please download manually from:" -ForegroundColor Yellow
        Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
        return $false
    }

    Write-Host ""
    Write-Host "Installing .NET $DotNetVersion SDK..." -ForegroundColor Yellow

    try {
        # Install .NET SDK
        & $installerPath -Version Latest -Channel $DotNetVersion -InstallDir "$env:USERPROFILE\.dotnet"

        if ((Add-DotNetToPath) -and (Test-DotNetInstalled)) {
            Write-Host ".NET SDK is available." -ForegroundColor Green
            Write-Host "Added to PATH (may require terminal restart)" -ForegroundColor Green
            return $true
        }

        Write-Host "Installation failed to make .NET SDK available." -ForegroundColor Red
        return $false
    }
    catch {
        Write-Host "Installation failed: $_" -ForegroundColor Red
        return $false
    }
    finally {
        # Cleanup
        if (Test-Path $installerPath) {
            Remove-Item $installerPath -Force
        }
    }
}

# Function to build StayAwake
function Build-StayAwake {
    Write-Host ""
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host "  Building StayAwake..." -ForegroundColor Cyan
    Write-Host "================================================" -ForegroundColor Cyan
    Write-Host ""

    $srcDir = Join-Path $ProjectDir "src"
    $binDir = Join-Path $ProjectDir "bin"

    if (-not (Test-Path $srcDir)) {
        Write-Host "Error: Source directory not found: $srcDir" -ForegroundColor Red
        return $false
    }

    # Create bin directory
    if (-not (Test-Path $binDir)) {
        New-Item -ItemType Directory -Path $binDir -Force | Out-Null
    }

    try {
        Push-Location $srcDir

        Write-Host "Running: dotnet publish..." -ForegroundColor Yellow
        Write-Host ""

        & dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o $binDir

        if ($LASTEXITCODE -eq 0) {
            Pop-Location

            $exePath = Join-Path $binDir "StayAwake.exe"

            if (Test-Path $exePath) {
                $fileSize = [math]::Round((Get-Item $exePath).Length / 1MB, 2)
                Write-Host ""
                Write-Host "================================================" -ForegroundColor Green
                Write-Host "  BUILD SUCCESSFUL!" -ForegroundColor Green
                Write-Host "================================================" -ForegroundColor Green
                Write-Host ""
                Write-Host "Executable created:" -ForegroundColor Cyan
                Write-Host "  $exePath" -ForegroundColor White
                Write-Host ""
                Write-Host "File size: $fileSize MB" -ForegroundColor Cyan
                Write-Host ""
                Write-Host "You can now run StayAwake.exe or copy it anywhere!" -ForegroundColor Green
                Write-Host ""
                Invoke-Item $binDir

                return $true
            }
            else {
                Write-Host "Error: Executable not found after build" -ForegroundColor Red
                return $false
            }
        }
        else {
            Pop-Location
            Write-Host "Build failed with exit code: $LASTEXITCODE" -ForegroundColor Red
            return $false
        }
    }
    catch {
        Pop-Location
        Write-Host "Build failed: $_" -ForegroundColor Red
        return $false
    }
}

# Function to create desktop shortcut
function Create-DesktopShortcut {
    Write-Host ""
    Write-Host "Creating desktop shortcut..." -ForegroundColor Yellow

    $WshShell = New-Object -ComObject WScript.Shell
    $Shortcut = $WshShell.CreateShortcut("$DesktopPath\StayAwake.lnk")
    $Shortcut.TargetPath = Join-Path $ProjectDir "bin\StayAwake.exe"
    $Shortcut.Description = "Keep your system awake and status active"
    $Shortcut.Save()

    Write-Host "Desktop shortcut created!" -ForegroundColor Green
}

# Main script execution
try {
    # Check if .NET is already installed
    if (Test-DotNetInstalled) {
        Write-Host ".NET $DotNetVersion SDK is already installed." -ForegroundColor Green
        Write-Host ""
        & dotnet --list-sdks
        Write-Host ""
    }
    else {
        Write-Host ".NET $DotNetVersion SDK not found." -ForegroundColor Yellow
        Write-Host "Installing..." -ForegroundColor Yellow
        Write-Host ""

        if (-not (Install-DotNetSdk)) {
            Write-Host ""
            Write-Host "Installation failed. Please install manually:" -ForegroundColor Red
            Write-Host "https://dotnet.microsoft.com/download/dotnet/8.0" -ForegroundColor Cyan
            pause
            exit 1
        }
    }

    # Build StayAwake
    if (Build-StayAwake) {
        # Ask about desktop shortcut
        Write-Host ""
        $createShortcut = Read-Host "Create desktop shortcut? (Y/N)"
        if ($createShortcut -eq "Y" -or $createShortcut -eq "y") {
            Create-DesktopShortcut
        }

        Write-Host ""
        Write-Host "Installation complete!" -ForegroundColor Green
        Write-Host ""
        pause
    }
    else {
        Write-Host ""
        Write-Host "Build failed. Please check the errors above." -ForegroundColor Red
        pause
        exit 1
    }
}
catch {
    Write-Host ""
    Write-Host "An error occurred: $_" -ForegroundColor Red
    pause
    exit 1
}
