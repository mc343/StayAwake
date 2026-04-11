@echo off
echo Building StayAwake...
cd src
dotnet publish -c Release -r win-x64 --self-contained -p:PublishSingleFile=true -p:IncludeNativeLibrariesForSelfExtract=true -o ../bin
echo.
echo Build complete! Executable is in: bin\StayAwake.exe
pause
