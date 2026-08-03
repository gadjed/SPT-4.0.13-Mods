@echo off
setlocal EnableExtensions
cd /d "%~dp0"

rem Thin bootstrap: fetch latest manager from GitHub, then run it against this game root.
rem Only this .cmd needs to stay in the SPT folder permanently.

rem %~dp0 always has a trailing backslash. Quoting "D:\SPT\" escapes the closing
rem quote in cmd, so PowerShell would receive D:\SPT" and Test-Path would fail.
set "GAME_ROOT=%~dp0"
if "%GAME_ROOT:~-1%"=="\" set "GAME_ROOT=%GAME_ROOT:~0,-1%"

set "CACHE_DIR=%GAME_ROOT%\.modpack-cache"
set "REMOTE_SCRIPT=%CACHE_DIR%\manage-modpack.ps1"
set "LOCAL_FALLBACK=%GAME_ROOT%\manage-modpack.ps1"
set "SCRIPT_URL=https://raw.githubusercontent.com/gadjed/SPT-4.0.13-Mods/main/mods_patch/manage-modpack.ps1"

if not exist "%CACHE_DIR%" mkdir "%CACHE_DIR%" >nul 2>&1

echo [modpack] Завантаження актуального manage-modpack.ps1 з GitHub...
powershell -NoProfile -ExecutionPolicy Bypass -Command "try { $ProgressPreference='SilentlyContinue'; Invoke-WebRequest -Uri 'https://raw.githubusercontent.com/gadjed/SPT-4.0.13-Mods/main/mods_patch/manage-modpack.ps1' -OutFile '%REMOTE_SCRIPT%.tmp' -UseBasicParsing -Headers @{ 'Cache-Control'='no-cache'; 'User-Agent'='SPT-ModPack-Bootstrap' }; if ((Get-Item '%REMOTE_SCRIPT%.tmp').Length -lt 200) { throw 'empty download' }; Move-Item -Force '%REMOTE_SCRIPT%.tmp' '%REMOTE_SCRIPT%'; Write-Host '[modpack] Скрипт оновлено.' -ForegroundColor Green; exit 0 } catch { Write-Host ('[modpack] Не вдалося завантажити: ' + $_.Exception.Message) -ForegroundColor Yellow; if (Test-Path '%REMOTE_SCRIPT%.tmp') { Remove-Item '%REMOTE_SCRIPT%.tmp' -Force -EA SilentlyContinue }; exit 1 }"

if errorlevel 1 (
  if exist "%REMOTE_SCRIPT%" (
    echo [modpack] Використовую попередню кешовану копію скрипта.
  ) else if exist "%LOCAL_FALLBACK%" (
    echo [modpack] Використовую локальний manage-modpack.ps1 як fallback.
    copy /Y "%LOCAL_FALLBACK%" "%REMOTE_SCRIPT%" >nul
  ) else (
    echo [modpack] ПОМИЛКА: немає ні мережі, ні локального/кешованого скрипта.
    pause
    exit /b 1
  )
)

powershell -NoProfile -ExecutionPolicy Bypass -File "%REMOTE_SCRIPT%" "%GAME_ROOT%" %*
set "EXITCODE=%ERRORLEVEL%"
endlocal & exit /b %EXITCODE%
