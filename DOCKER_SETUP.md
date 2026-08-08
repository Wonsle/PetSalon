# 🐳 Docker 使用指南

## 快速開始

### 1. 初次設定

```bash
# 複製環境變數模板
cp .env.example .env

# 編輯 .env 檔案，替換所有 CHANGE_ME_BEFORE_USE
# Windows: notepad .env
# macOS/Linux: nano .env
```

### 2. 啟動服務

#### 方法 1: 使用腳本（推薦）

**Windows:**
```powershell
.\start-windows.ps1
```

**macOS/Linux:**
```bash
chmod +x start-mac.sh
./start-mac.sh
```

#### 方法 2: 使用 Docker Compose

**Windows:**
```bash
docker-compose -f docker-compose.yml -f docker-compose.windows.yml up -d
```

**macOS:**
```bash
docker-compose -f docker-compose.yml -f docker-compose.mac.yml up -d
```

**通用（使用預設配置）:**
```bash
docker-compose up -d
```

---

## 常用指令

```bash
# 查看服務狀態
docker-compose ps

# 查看日誌
docker-compose logs -f sqlserver

# 停止服務
docker-compose down

# 重啟服務
docker-compose restart

# 連線到 SQL Server
docker exec -it petsalon-sqlserver /opt/mssql-tools/bin/sqlcmd -S localhost -U sa -P 'YourPassword'
```

---

## 配置檔案說明

```
docker-compose.yml          # 基礎配置（所有平台共享）
docker-compose.windows.yml  # Windows 特定配置
docker-compose.mac.yml      # macOS 特定配置
.env.example                # 環境變數模板
.env                        # 實際環境變數（不提交到 Git）
```

---

## 平台注意事項

### Windows
- 推薦啟用 WSL 2（Docker Desktop > Settings > Use WSL 2 based engine）
- 路徑格式：`C:/Docker/Data` 或 `C:\\Docker\\Data`

### macOS
- **Apple Silicon**：Compose 不再強制 `linux/amd64`；Docker 會依可用映像選擇架構。
- 若 SQL Server 映像不支援目前主機架構，請改用 Azure SQL Edge 或遠端 SQL Server，避免安裝額外的 x86 模擬層。
- 後端的 `ConnectionStrings__DefaultConnection` 與 `JwtSettings__SignKey` 必須由未追蹤的 `.env` 或部署平台 secret manager 注入。

### Linux
- 確保目前使用者在 docker 群組中：
  ```bash
  sudo usermod -aG docker $USER
  newgrp docker
  ```

---

## 資料備份

```bash
# 備份資料卷
docker run --rm \
  -v petsalon_sqlserver_data:/source \
  -v $(pwd):/backup \
  alpine tar czf /backup/sqlserver-backup-$(date +%Y%m%d).tar.gz -C /source .

# 還原資料卷
docker run --rm \
  -v petsalon_sqlserver_data:/target \
  -v $(pwd):/backup \
  alpine tar xzf /backup/sqlserver-backup-YYYYMMDD.tar.gz -C /target
```

---

## 環境需求

- **Docker Desktop**: 4.0+
- **記憶體**: 至少 8GB RAM
- **磁碟空間**: 至少 10GB

---

**提示**: `SA_PASSWORD` 必須符合 SQL Server 要求（至少 8 字元，包含大小寫字母、數字和特殊字元）；所有 `CHANGE_ME_BEFORE_USE` 都必須在啟動前替換。
