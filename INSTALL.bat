@echo off
echo ================================================================
echo   StayAwake - Quick Installer
echo ================================================================
echo.
echo This will install .NET 8.0 SDK (if needed) and build StayAwake
echo.
pause

powershell -ExecutionPolicy Bypass -File "%~dp0install-dotnet.ps1"

pause
