# PetSalon Docker 啟動腳本 (Windows)
# 使用方式：在 PowerShell 中執行 .\start-windows.ps1

Write-Host "🐾 PetSalon Docker 環境啟動" -ForegroundColor Cyan
Write-Host "================================" -ForegroundColor Cyan
Write-Host ""

# 檢查 .env 檔案是否存在
if (-Not (Test-Path ".env")) {
    Write-Host "⚠️  未找到 .env 檔案" -ForegroundColor Yellow
    Write-Host "正在從 .env.example 複製..." -ForegroundColor Yellow

    if (Test-Path ".env.example") {
        Copy-Item ".env.example" ".env"
        Write-Host "✅ 已建立 .env 檔案" -ForegroundColor Green
        Write-Host ""
        Write-Host "⚠️  請編輯 .env 檔案並設定 SA_PASSWORD" -ForegroundColor Yellow
        Write-Host "執行: notepad .env" -ForegroundColor Yellow
        Write-Host ""
        $continue = Read-Host "是否已設定密碼？(y/n)"
        if ($continue -ne 'y') {
            Write-Host "❌ 已取消啟動" -ForegroundColor Red
            exit 1
        }
    } else {
        Write-Host "❌ 找不到 .env.example 檔案" -ForegroundColor Red
        exit 1
    }
}

Write-Host "📋 檢查 Docker 服務..." -ForegroundColor Cyan
# 檢查 Docker 是否運行
$dockerRunning = docker info 2>$null
if (-Not $?) {
    Write-Host "❌ Docker 未運行，請啟動 Docker Desktop" -ForegroundColor Red
    exit 1
}
Write-Host "✅ Docker 服務正常" -ForegroundColor Green
Write-Host ""

Write-Host "🚀 啟動 SQL Server 容器..." -ForegroundColor Cyan
docker-compose -f docker-compose.yml -f docker-compose.windows.yml up -d

if ($?) {
    Write-Host ""
    Write-Host "✅ 容器啟動成功！" -ForegroundColor Green
    Write-Host ""
    Write-Host "📊 服務資訊：" -ForegroundColor Cyan
    Write-Host "  - SQL Server: localhost:1433" -ForegroundColor White
    Write-Host "  - 使用者: sa" -ForegroundColor White
    Write-Host "  - 密碼: (請查看 .env 檔案)" -ForegroundColor White
    Write-Host ""
    Write-Host "📝 常用指令：" -ForegroundColor Cyan
    Write-Host "  查看狀態: docker-compose ps" -ForegroundColor White
    Write-Host "  查看日誌: docker-compose logs -f sqlserver" -ForegroundColor White
    Write-Host "  停止服務: docker-compose down" -ForegroundColor White
    Write-Host ""
} else {
    Write-Host ""
    Write-Host "❌ 啟動失敗，請查看錯誤訊息" -ForegroundColor Red
    Write-Host "執行以下指令查看日誌：" -ForegroundColor Yellow
    Write-Host "  docker-compose logs sqlserver" -ForegroundColor White
    exit 1
}
