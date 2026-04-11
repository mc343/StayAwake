@echo off
echo ================================================================
echo   GitHub Authentication Setup
echo ================================================================
echo.
echo This will open a browser to authenticate with GitHub
echo.
pause

"%ProgramFiles%\GitHub CLI\gh.exe" auth login

echo.
echo ================================================================
echo   Authentication Complete!
echo ================================================================
echo.
pause
