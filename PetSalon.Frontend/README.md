# PetSalon Frontend

Amada Pet Grooming 寵物美容管理系統前端

## 技術架構

- Vue 3 + TypeScript
- Element Plus UI 組件庫
- Pinia 狀態管理
- Vue Router 路由
- Vite 建構工具

## 開發設置

### 安裝依賴

```bash
npm install
```

### 開發模式

```bash
npm run dev
```

前端將運行在 http://localhost:3000

### 建構生產版本

```bash
npm run build
```

### 預覽生產建構

```bash
npm run preview
```

### 程式碼檢查

```bash
npm run lint
```

### 類型檢查

```bash
npm run type-check
```

### 執行測試

```bash
npm run test:unit
```

## 環境配置

建立 `.env.local` 檔案來設定本地環境變數：

```
VITE_API_BASE_URL=http://localhost:5150
```

## 功能特色

- 響應式設計
- 多語言支持 (繁體中文)
- JWT 身份驗證
- 角色權限管理
- 即時資料更新
- 檔案上傳支持

## 主要頁面

- 登入頁面
- 儀表板
- 寵物管理
- 聯絡人管理
- 預約管理
- 包月管理
- 財務管理
- 系統設定