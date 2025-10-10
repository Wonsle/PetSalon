#!/bin/bash
# PetSalon Docker 啟動腳本 (macOS/Linux)
# 使用方式：chmod +x start-mac.sh && ./start-mac.sh

echo "🐾 PetSalon Docker 環境啟動"
echo "================================"
echo ""

# 檢查 .env 檔案是否存在
if [ ! -f ".env" ]; then
    echo "⚠️  未找到 .env 檔案"
    echo "正在從 .env.example 複製..."

    if [ -f ".env.example" ]; then
        cp .env.example .env
        echo "✅ 已建立 .env 檔案"
        echo ""
        echo "⚠️  請編輯 .env 檔案並設定 SA_PASSWORD"
        echo "執行: nano .env 或 vim .env"
        echo ""
        read -p "是否已設定密碼？(y/n) " -n 1 -r
        echo
        if [[ ! $REPLY =~ ^[Yy]$ ]]; then
            echo "❌ 已取消啟動"
            exit 1
        fi
    else
        echo "❌ 找不到 .env.example 檔案"
        exit 1
    fi
fi

echo "📋 檢查 Docker 服務..."
# 檢查 Docker 是否運行
if ! docker info &> /dev/null; then
    echo "❌ Docker 未運行，請啟動 Docker Desktop"
    exit 1
fi
echo "✅ Docker 服務正常"
echo ""

echo "🚀 啟動 SQL Server 容器..."
docker-compose -f docker-compose.yml -f docker-compose.mac.yml up -d

if [ $? -eq 0 ]; then
    echo ""
    echo "✅ 容器啟動成功！"
    echo ""
    echo "📊 服務資訊："
    echo "  - SQL Server: localhost:1433"
    echo "  - 使用者: sa"
    echo "  - 密碼: (請查看 .env 檔案)"
    echo ""
    echo "📝 常用指令："
    echo "  查看狀態: docker-compose ps"
    echo "  查看日誌: docker-compose logs -f sqlserver"
    echo "  停止服務: docker-compose down"
    echo ""
else
    echo ""
    echo "❌ 啟動失敗，請查看錯誤訊息"
    echo "執行以下指令查看日誌："
    echo "  docker-compose logs sqlserver"
    exit 1
fi
