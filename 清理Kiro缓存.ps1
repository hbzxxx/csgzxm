# Kiro 缓存清理脚本 (PowerShell版本)
# 使用方法: 右键 -> 使用PowerShell运行

param(
    [switch]$Force,      # 强制清理，不询问
    [switch]$KeepLogs,   # 保留日志文件
    [switch]$KeepChats   # 保留聊天记录
)

# 设置控制台编码
[Console]::OutputEncoding = [System.Text.Encoding]::UTF8

Write-Host "========================================" -ForegroundColor Cyan
Write-Host "     Kiro/VSCode/Cursor 缓存清理工具" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host ""

# 检查管理员权限
$isAdmin = ([Security.Principal.WindowsPrincipal] [Security.Principal.WindowsIdentity]::GetCurrent()).IsInRole([Security.Principal.WindowsBuiltInRole] "Administrator")
if (-not $isAdmin) {
    Write-Host "[警告] 建议以管理员身份运行以确保完全清理" -ForegroundColor Yellow
    Write-Host ""
}

# 检查 IDE 进程
$ideProcesses = Get-Process | Where-Object { 
    $_.ProcessName -like "*kiro*" -or 
    $_.ProcessName -like "*Kiro*" -or 
    $_.ProcessName -like "*Code*" -or 
    $_.ProcessName -like "*cursor*" -or
    $_.ProcessName -like "*Cursor*"
}
if ($ideProcesses) {
    Write-Host "[警告] 检测到 IDE 进程正在运行:" -ForegroundColor Red
    $ideProcesses | ForEach-Object { Write-Host "  - $($_.ProcessName) (PID: $($_.Id))" -ForegroundColor Red }
    
    if (-not $Force) {
        $response = Read-Host "是否强制结束这些进程？(y/n)"
        if ($response -eq 'y' -or $response -eq 'Y') {
            $ideProcesses | Stop-Process -Force
            Write-Host "[完成] 已结束 IDE 进程" -ForegroundColor Green
        } else {
            Write-Host "[退出] 请手动关闭 IDE 后重新运行脚本" -ForegroundColor Yellow
            exit 1
        }
    } else {
        $ideProcesses | Stop-Process -Force
        Write-Host "[完成] 已强制结束 IDE 进程" -ForegroundColor Green
    }
    Write-Host ""
}

# 定义清理目录
$cleanupPaths = @(
    # ========== Kiro 缓存（大头） ==========
    @{
        Path = "$env:APPDATA\Kiro\User\globalStorage"
        Name = "Kiro globalStorage (聊天记录/会话缓存)"
        Safe = $false
    },
    @{
        Path = "$env:APPDATA\Kiro\User\workspaceStorage"
        Name = "Kiro workspaceStorage"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\User\History"
        Name = "Kiro 历史记录"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\CachedExtensionVSIXs"
        Name = "Kiro 扩展缓存"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\CachedData"
        Name = "Kiro CachedData"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\Crashpad"
        Name = "Kiro 崩溃报告"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\Cache"
        Name = "Kiro Cache"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\GPUCache"
        Name = "Kiro GPU缓存"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\Code Cache"
        Name = "Kiro 代码缓存"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Kiro\logs"
        Name = "Kiro 日志"
        Safe = $true
    },
    # ========== VSCode 缓存 ==========
    @{
        Path = "$env:APPDATA\Code\User\workspaceStorage"
        Name = "VSCode workspaceStorage"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Code\User\globalStorage"
        Name = "VSCode globalStorage"
        Safe = $false
    },
    @{
        Path = "$env:APPDATA\Code\User\History"
        Name = "VSCode 历史记录"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Code\CachedExtensionVSIXs"
        Name = "VSCode 扩展缓存"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Code\CachedData"
        Name = "VSCode CachedData"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Code\Crashpad"
        Name = "VSCode 崩溃报告"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Code\logs"
        Name = "VSCode 日志"
        Safe = $true
    },
    # ========== Cursor 缓存 ==========
    @{
        Path = "$env:APPDATA\Cursor\User\workspaceStorage"
        Name = "Cursor workspaceStorage"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Cursor\User\globalStorage"
        Name = "Cursor globalStorage"
        Safe = $false
    },
    @{
        Path = "$env:APPDATA\Cursor\User\History"
        Name = "Cursor 历史记录"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Cursor\CachedExtensionVSIXs"
        Name = "Cursor 扩展缓存"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Cursor\CachedData"
        Name = "Cursor CachedData"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Cursor\Crashpad"
        Name = "Cursor 崩溃报告"
        Safe = $true
    },
    @{
        Path = "$env:APPDATA\Cursor\logs"
        Name = "Cursor 日志"
        Safe = $true
    },
    @{
        Path = "$env:LOCALAPPDATA\cursor-updater"
        Name = "Cursor 更新缓存"
        Safe = $true
    },
    # ========== 临时文件 ==========
    @{
        Path = "$env:TEMP\Kiro"
        Name = "Kiro 临时文件"
        Safe = $true
    },
    @{
        Path = "$env:TEMP\vscode"
        Name = "VSCode 临时文件"
        Safe = $true
    }
)

# 清理函数
function Remove-DirectoryWithStats {
    param(
        [string]$Path,
        [string]$Name,
        [bool]$Safe
    )
    
    if (Test-Path $Path) {
        try {
            # 计算目录大小
            $size = (Get-ChildItem $Path -Recurse -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
            $sizeStr = if ($size -gt 1GB) { "{0:N2} GB" -f ($size / 1GB) }
                      elseif ($size -gt 1MB) { "{0:N2} MB" -f ($size / 1MB) }
                      elseif ($size -gt 1KB) { "{0:N2} KB" -f ($size / 1KB) }
                      else { "$size 字节" }
            
            Write-Host "[清理] $Name" -ForegroundColor Yellow
            Write-Host "  路径: $Path" -ForegroundColor Gray
            Write-Host "  大小: $sizeStr" -ForegroundColor Gray
            
            if (-not $Safe -and -not $Force) {
                $confirm = Read-Host "  [警告] 此操作可能影响用户数据，确认清理？(y/n)"
                if ($confirm -ne 'y' -and $confirm -ne 'Y') {
                    Write-Host "  [跳过] 用户取消" -ForegroundColor Yellow
                    return @{ Success = $false; Size = 0 }
                }
            }
            
            Remove-Item $Path -Recurse -Force -ErrorAction Stop
            Write-Host "  [成功] 已清理 $sizeStr" -ForegroundColor Green
            return @{ Success = $true; Size = $size }
        }
        catch {
            Write-Host "  [失败] $($_.Exception.Message)" -ForegroundColor Red
            return @{ Success = $false; Size = 0 }
        }
    } else {
        Write-Host "[跳过] $Name - 目录不存在" -ForegroundColor Gray
        return @{ Success = $false; Size = 0 }
    }
}

# 执行清理
$totalCleaned = 0
$totalSize = 0
$successCount = 0

Write-Host "开始清理 IDE 缓存..." -ForegroundColor Green
Write-Host ""

foreach ($item in $cleanupPaths) {
    # 应用参数过滤
    if ($KeepLogs -and $item.Name -eq "日志文件") { continue }
    if ($KeepChats -and $item.Name -like "*聊天记录*") { continue }
    
    $result = Remove-DirectoryWithStats -Path $item.Path -Name $item.Name -Safe $item.Safe
    if ($result.Success) {
        $successCount++
        $totalSize += $result.Size
    }
    Write-Host ""
}

# 清理临时目录中的 IDE 文件
Write-Host "[扫描] 临时目录中的 IDE 相关文件..." -ForegroundColor Yellow
$tempIdeItems = Get-ChildItem $env:TEMP | Where-Object { 
    $_.Name -like "*kiro*" -or 
    $_.Name -like "*Kiro*" -or 
    $_.Name -like "*vscode*" -or
    $_.Name -like "*cursor*"
}
foreach ($item in $tempIdeItems) {
    try {
        $size = if ($item.PSIsContainer) {
            (Get-ChildItem $item.FullName -Recurse -ErrorAction SilentlyContinue | Measure-Object -Property Length -Sum).Sum
        } else {
            $item.Length
        }
        
        Remove-Item $item.FullName -Recurse -Force
        Write-Host "  [清理] $($item.Name) - $([math]::Round($size/1KB, 2)) KB" -ForegroundColor Green
        $totalSize += $size
        $successCount++
    }
    catch {
        Write-Host "  [失败] $($item.Name) - $($_.Exception.Message)" -ForegroundColor Red
    }
}

Write-Host ""
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "              清理完成" -ForegroundColor Cyan
Write-Host "========================================" -ForegroundColor Cyan
Write-Host "成功清理项目: $successCount" -ForegroundColor Green
Write-Host "释放空间: $([math]::Round($totalSize/1MB, 2)) MB" -ForegroundColor Green
Write-Host ""

# 显示剩余文件
Write-Host "[检查] 剩余的 IDE 相关目录:" -ForegroundColor Yellow
$remainingPaths = @(
    "$env:APPDATA\Kiro",
    "$env:APPDATA\Code",
    "$env:APPDATA\Cursor",
    "$env:USERPROFILE\.vscode",
    "$env:USERPROFILE\.cursor",
    "$env:USERPROFILE\.kiro"
)

foreach ($path in $remainingPaths) {
    if (Test-Path $path) {
        $size = 0
        Get-ChildItem $path -Recurse -Force -ErrorAction SilentlyContinue | ForEach-Object { $size += $_.Length }
        $sizeGB = [math]::Round($size/1GB, 2)
        $sizeMB = [math]::Round($size/1MB, 0)
        if ($sizeGB -ge 1) {
            Write-Host "  - $path ($sizeGB GB)" -ForegroundColor Yellow
        } else {
            Write-Host "  - $path ($sizeMB MB)" -ForegroundColor Gray
        }
    }
}

Write-Host ""
Write-Host "[提示] 完全卸载选项:" -ForegroundColor Cyan
Write-Host "  如需删除所有 IDE 数据（包括设置和聊天记录），请运行:" -ForegroundColor Gray
Write-Host "  Remove-Item '$env:APPDATA\Kiro' -Recurse -Force" -ForegroundColor Gray
Write-Host "  Remove-Item '$env:APPDATA\Code' -Recurse -Force" -ForegroundColor Gray
Write-Host "  Remove-Item '$env:APPDATA\Cursor' -Recurse -Force" -ForegroundColor Gray
Write-Host ""

if (-not $Force) {
    Read-Host "按回车键退出"
}