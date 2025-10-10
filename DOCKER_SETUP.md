# 🐳 Docker 使用指南

## 快速開始

### 1. 初次設定

```bash
# 複製環境變數模板
cp .env.example .env

# 編輯 .env 檔案，修改 SA_PASSWORD
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
- **Apple Silicon (M1/M2/M3)**: 首次執行可能需要安裝 Rosetta 2
  ```bash
  softwareupdate --install-rosetta
  ```
- **Intel Mac**: 原生支援，效能最佳

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

**提示**: 密碼必須符合 SQL Server 要求（至少 8 字元，包含大小寫字母、數字和特殊字元）
