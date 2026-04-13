@echo off
setlocal
powershell -ExecutionPolicy Bypass -File "%~dp0install-dotnet.ps1"
endlocal
