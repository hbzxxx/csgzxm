@echo off
chcp 65001 >nul
echo ========================================
echo   Kiro/VSCode/Cursor 缓存清理工具
echo ========================================
echo.

:: 检查 IDE 是否正在运行
tasklist /FI "IMAGENAME eq Kiro.exe" 2>NUL | find /I /N "Kiro.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [警告] Kiro 正在运行，请先关闭
    pause
    exit /b 1
)
tasklist /FI "IMAGENAME eq Code.exe" 2>NUL | find /I /N "Code.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [警告] VSCode 正在运行，请先关闭
    pause
    exit /b 1
)
tasklist /FI "IMAGENAME eq Cursor.exe" 2>NUL | find /I /N "Cursor.exe">NUL
if "%ERRORLEVEL%"=="0" (
    echo [警告] Cursor 正在运行，请先关闭
    pause
    exit /b 1
)

echo [信息] 开始清理 IDE 缓存...
echo.

:: ========== Kiro 缓存（大头） ==========
echo === Kiro 缓存 ===

if exist "%APPDATA%\Kiro\User\globalStorage" (
    echo [清理] Kiro globalStorage (聊天记录/会话缓存)
    rd /s /q "%APPDATA%\Kiro\User\globalStorage" 2>nul
    if not exist "%APPDATA%\Kiro\User\globalStorage" echo [成功]
)

if exist "%APPDATA%\Kiro\User\workspaceStorage" (
    echo [清理] Kiro workspaceStorage
    rd /s /q "%APPDATA%\Kiro\User\workspaceStorage" 2>nul
)

if exist "%APPDATA%\Kiro\User\History" (
    echo [清理] Kiro History
    rd /s /q "%APPDATA%\Kiro\User\History" 2>nul
)

if exist "%APPDATA%\Kiro\CachedExtensionVSIXs" (
    echo [清理] Kiro CachedExtensionVSIXs
    rd /s /q "%APPDATA%\Kiro\CachedExtensionVSIXs" 2>nul
)

if exist "%APPDATA%\Kiro\CachedData" (
    echo [清理] Kiro CachedData
    rd /s /q "%APPDATA%\Kiro\CachedData" 2>nul
)

if exist "%APPDATA%\Kiro\Crashpad" (
    echo [清理] Kiro Crashpad
    rd /s /q "%APPDATA%\Kiro\Crashpad" 2>nul
)

if exist "%APPDATA%\Kiro\Cache" (
    echo [清理] Kiro Cache
    rd /s /q "%APPDATA%\Kiro\Cache" 2>nul
)

if exist "%APPDATA%\Kiro\GPUCache" (
    echo [清理] Kiro GPUCache
    rd /s /q "%APPDATA%\Kiro\GPUCache" 2>nul
)

if exist "%APPDATA%\Kiro\Code Cache" (
    echo [清理] Kiro Code Cache
    rd /s /q "%APPDATA%\Kiro\Code Cache" 2>nul
)

if exist "%APPDATA%\Kiro\logs" (
    echo [清理] Kiro logs
    rd /s /q "%APPDATA%\Kiro\logs" 2>nul
)

echo.

:: ========== VSCode 缓存 ==========
echo === VSCode 缓存 ===

if exist "%APPDATA%\Code\User\workspaceStorage" (
    echo [清理] VSCode workspaceStorage
    rd /s /q "%APPDATA%\Code\User\workspaceStorage" 2>nul
)

if exist "%APPDATA%\Code\User\globalStorage" (
    echo [清理] VSCode globalStorage
    rd /s /q "%APPDATA%\Code\User\globalStorage" 2>nul
)

if exist "%APPDATA%\Code\User\History" (
    echo [清理] VSCode History
    rd /s /q "%APPDATA%\Code\User\History" 2>nul
)

if exist "%APPDATA%\Code\CachedExtensionVSIXs" (
    echo [清理] VSCode CachedExtensionVSIXs
    rd /s /q "%APPDATA%\Code\CachedExtensionVSIXs" 2>nul
)

if exist "%APPDATA%\Code\CachedData" (
    echo [清理] VSCode CachedData
    rd /s /q "%APPDATA%\Code\CachedData" 2>nul
)

if exist "%APPDATA%\Code\Crashpad" (
    echo [清理] VSCode Crashpad
    rd /s /q "%APPDATA%\Code\Crashpad" 2>nul
)

if exist "%APPDATA%\Code\logs" (
    echo [清理] VSCode logs
    rd /s /q "%APPDATA%\Code\logs" 2>nul
)

echo.

:: ========== Cursor 缓存 ==========
echo === Cursor 缓存 ===

if exist "%APPDATA%\Cursor\User\workspaceStorage" (
    echo [清理] Cursor workspaceStorage
    rd /s /q "%APPDATA%\Cursor\User\workspaceStorage" 2>nul
)

if exist "%APPDATA%\Cursor\User\globalStorage" (
    echo [清理] Cursor globalStorage
    rd /s /q "%APPDATA%\Cursor\User\globalStorage" 2>nul
)

if exist "%APPDATA%\Cursor\User\History" (
    echo [清理] Cursor History
    rd /s /q "%APPDATA%\Cursor\User\History" 2>nul
)

if exist "%APPDATA%\Cursor\CachedExtensionVSIXs" (
    echo [清理] Cursor CachedExtensionVSIXs
    rd /s /q "%APPDATA%\Cursor\CachedExtensionVSIXs" 2>nul
)

if exist "%APPDATA%\Cursor\CachedData" (
    echo [清理] Cursor CachedData
    rd /s /q "%APPDATA%\Cursor\CachedData" 2>nul
)

if exist "%APPDATA%\Cursor\Crashpad" (
    echo [清理] Cursor Crashpad
    rd /s /q "%APPDATA%\Cursor\Crashpad" 2>nul
)

if exist "%APPDATA%\Cursor\logs" (
    echo [清理] Cursor logs
    rd /s /q "%APPDATA%\Cursor\logs" 2>nul
)

if exist "%LOCALAPPDATA%\cursor-updater" (
    echo [清理] Cursor updater cache
    rd /s /q "%LOCALAPPDATA%\cursor-updater" 2>nul
)

echo.

:: ========== 临时文件 ==========
echo === 临时文件 ===

if exist "%TEMP%\Kiro" (
    echo [清理] Kiro 临时文件
    rd /s /q "%TEMP%\Kiro" 2>nul
)

if exist "%TEMP%\vscode" (
    echo [清理] VSCode 临时文件
    rd /s /q "%TEMP%\vscode" 2>nul
)

for /d %%i in ("%TEMP%\*kiro*") do (
    echo [清理] %%i
    rd /s /q "%%i" 2>nul
)

for /d %%i in ("%TEMP%\*vscode*") do (
    echo [清理] %%i
    rd /s /q "%%i" 2>nul
)

for /d %%i in ("%TEMP%\*cursor*") do (
    echo [清理] %%i
    rd /s /q "%%i" 2>nul
)

echo.
echo ========================================
echo              清理完成
echo ========================================
echo.
echo [提示] 如需完全清除所有数据，手动删除:
echo   %APPDATA%\Kiro
echo   %APPDATA%\Code
echo   %APPDATA%\Cursor
echo.
pause
