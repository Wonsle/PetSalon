# MSW Mock 系統實作計劃

> **分支**: `feature/msw-mock-system`
> **目標**: 建立完整的 MSW (Mock Service Worker) 系統，讓前端可以獨立於後端開發和測試
> **預估時間**: 5.5-6 小時

---

## 📋 專案概況

### 背景
- **專案**: PetSalon 寵物美容院管理系統
- **前端框架**: Vue 3.4+ with TypeScript
- **當前問題**: 前端完全依賴後端 API，後端未就緒時無法開發測試
- **API 規模**: 約 65+ 個端點

### 目標模組
1. 🐕 寵物管理 (Pet Management) - 7 個端點
2. 👥 聯絡人管理 (Contact Management) - 7 個端點
3. 📅 預約管理 (Reservation Management) - 15+ 個端點
4. 💳 包月服務 (Subscription Management) - 14+ 個端點
5. 📊 儀表板統計 (Dashboard) - 5 個端點
6. ⚙️ 系統代碼 (System Codes) - 7 個端點

---

## 🎯 階段一：MSW 基礎設置

**負責 Agent**: `frontend-developer`
**預估時間**: 1 小時
**優先級**: 🔴 最高

### 任務清單

#### 1.1 安裝依賴 (10 分鐘)

```bash
cd /Users/kun/Documents/Projects/PetSalon/PetSalon.Frontend
npm install -D msw@latest
```

**驗證**:
- 檢查 `package.json` 中 `devDependencies` 包含 `msw`
- 版本應為 2.x 以上

---

#### 1.2 創建目錄結構 (10 分鐘)

創建以下目錄和檔案：

```
PetSalon.Frontend/src/
└── mocks/
    ├── data/           # Mock 資料層
    │   ├── .gitkeep
    ├── handlers/       # 路由處理層
    │   ├── .gitkeep
    ├── browser.ts      # MSW 瀏覽器配置
    └── README.md       # Mock 系統使用說明
```

**檔案內容**:

**`src/mocks/README.md`**:
```markdown
# Mock 系統說明

本目錄包含 MSW (Mock Service Worker) 的配置和資料。

## 目錄結構
- `data/` - Mock 資料層，包含各模組的模擬資料
- `handlers/` - MSW 路由處理層，攔截 API 請求並返回 mock 資料
- `browser.ts` - MSW 瀏覽器配置入口

## 使用方式
- 開發模式（真實後端）：`npm run dev`
- Mock 模式（模擬資料）：`npm run dev:mock`
```

---

#### 1.3 環境變數配置 (15 分鐘)

**創建 `.env.mock`**:
```env
# Mock 模式環境變數
VITE_USE_MOCK=true
VITE_API_BASE_URL=
```

**更新 `.env.development`**:
```env
# 開發模式環境變數（連接真實後端）
VITE_USE_MOCK=false
VITE_API_BASE_URL=http://localhost:5150
```

**創建 `.env.example`** (如果不存在):
```env
# 環境變數範例
VITE_USE_MOCK=false
VITE_API_BASE_URL=http://localhost:5150
```

---

#### 1.4 配置 MSW Browser (15 分鐘)

**創建 `src/mocks/browser.ts`**:
```typescript
/**
 * MSW Browser Configuration
 *
 * 此檔案配置 Mock Service Worker 在瀏覽器環境中運行
 * 會在 main.ts 中條件性載入
 */
import { setupWorker } from 'msw/browser'

// 目前還沒有 handlers，先創建空的 worker
// 後續會從各個 handlers 檔案中導入
export const worker = setupWorker()

// 開發環境日誌
if (import.meta.env.DEV) {
  console.log('🔧 MSW Mock Service Worker initialized')
}
```

---

#### 1.5 整合到 main.ts (15 分鐘)

**修改 `src/main.ts`**:

在現有代碼前添加：

```typescript
/**
 * 條件性啟用 MSW Mock
 * 只在 VITE_USE_MOCK=true 時載入
 */
async function enableMocking() {
  // 檢查環境變數
  if (import.meta.env.VITE_USE_MOCK !== 'true') {
    return
  }

  console.log('🚀 Starting MSW in mock mode...')

  // 動態導入 MSW worker
  const { worker } = await import('./mocks/browser')

  // 啟動 Service Worker
  return worker.start({
    onUnhandledRequest: 'bypass', // 未匹配的請求繼續發送到真實服務器
    serviceWorker: {
      url: '/mockServiceWorker.js'
    }
  })
}

// 原有的應用啟動代碼改為：
enableMocking().then(() => {
  const app = createApp(App)

  // ... 原有的配置（router, pinia, primevue 等）

  app.mount('#app')
})
```

---

#### 1.6 更新 package.json 腳本 (5 分鐘)

在 `scripts` 區塊添加：

```json
{
  "scripts": {
    "dev": "vite",
    "dev:mock": "vite --mode mock",
    "build": "run-p type-check \"build-only {@}\" --",
    "build:mock": "vite build --mode mock",
    "preview": "vite preview",
    "preview:mock": "vite preview --mode mock"
  }
}
```

---

#### 1.7 生成 MSW Service Worker 檔案 (5 分鐘)

執行以下命令生成 Service Worker 檔案：

```bash
npx msw init public/ --save
```

這會在 `public/` 目錄下創建 `mockServiceWorker.js`

**驗證**:
- 檢查 `public/mockServiceWorker.js` 是否存在
- 檔案大小約 60KB

---

### 階段一驗證清單

- [ ] `msw` 已安裝在 `devDependencies`
- [ ] 目錄結構已創建（`src/mocks/data/`, `src/mocks/handlers/`）
- [ ] 環境變數檔案已配置（`.env.mock`, `.env.development`）
- [ ] `src/mocks/browser.ts` 已創建
- [ ] `main.ts` 已整合 MSW 啟動邏輯
- [ ] `package.json` 腳本已更新
- [ ] `public/mockServiceWorker.js` 已生成
- [ ] 執行 `npm run dev:mock` 可以啟動（雖然還沒有 mock 資料）

---

## 🗄️ 階段二：創建 Mock 資料層

**負責 Agent**: `frontend-developer`
**預估時間**: 1.5 小時
**優先級**: 🟡 高

### 資料層設計原則

1. **符合 TypeScript 類型** - 參考 `src/types/` 下的類型定義
2. **豐富且真實** - 使用繁體中文、台灣電話格式、新台幣價格
3. **關聯性正確** - 資料之間保持正確的關聯關係
4. **可變動性** - 提供 CRUD 輔助函數，支援資料修改
5. **多樣性** - 包含各種狀態、類型的資料

---

### 2.1 寵物資料 - pets.ts (20 分鐘)

**檔案**: `src/mocks/data/pets.ts`

**資料需求**:
- 至少 20 筆寵物資料
- 涵蓋不同品種：貴賓犬、黃金獵犬、柴犬、法國鬥牛犬、博美犬等
- 涵蓋性別：公、母
- 包含價格資訊：單次價格、包月價格
- 包含主要聯絡人資訊
- 包含生日資訊

**需要的輔助函數**:
```typescript
// 獲取所有寵物（支援分頁、搜尋、篩選）
export function getMockPets(params: PetSearchParams): PetListResponse

// 根據 ID 獲取寵物
export function getMockPetById(id: number): Pet | undefined

// 創建新寵物
export function createMockPet(pet: PetCreateRequest): Pet

// 更新寵物
export function updateMockPet(id: number, pet: PetUpdateRequest): Pet | null

// 刪除寵物
export function deleteMockPet(id: number): boolean

// 根據聯絡人 ID 獲取寵物
export function getMockPetsByContactId(contactId: number): Pet[]
```

**範例資料結構**:
```typescript
{
  petId: 1,
  petName: '小白',
  breed: 'POODLE',
  gender: 'MALE',
  birthDay: '2020-01-15',
  normalPrice: 800,
  subscriptionPrice: 600,
  photoUrl: '/uploads/pets/pet1.jpg',
  createUser: 'admin',
  createTime: '2024-01-01T00:00:00',
  modifyUser: 'admin',
  modifyTime: '2024-01-01T00:00:00',
  primaryContact: {
    contactPersonId: 1,
    name: '王小明',
    phone: '0912-345-678',
    relationship: 'OWNER'
  }
}
```

---

### 2.2 聯絡人資料 - contacts.ts (15 分鐘)

**檔案**: `src/mocks/data/contacts.ts`

**資料需求**:
- 至少 15 筆聯絡人資料
- 涵蓋不同關係類型：飼主、家人、朋友、照護者
- 包含完整聯絡資訊：姓名、暱稱、電話、地址

**需要的輔助函數**:
```typescript
export function getMockContacts(params: any): ContactListResponse
export function getMockContactById(id: number): Contact | undefined
export function createMockContact(contact: any): Contact
export function updateMockContact(id: number, contact: any): Contact | null
export function deleteMockContact(id: number): boolean
```

---

### 2.3 預約資料 - reservations.ts (25 分鐘)

**檔案**: `src/mocks/data/reservations.ts`

**資料需求**:
- 至少 30 筆預約資料
- 時間分布：
  - 今日預約：5-8 筆
  - 本週預約：15-20 筆
  - 本月預約：25-30 筆
- 狀態分布：
  - 待確認 (PENDING): 20%
  - 已確認 (CONFIRMED): 40%
  - 進行中 (IN_PROGRESS): 10%
  - 已完成 (COMPLETED): 25%
  - 已取消 (CANCELLED): 5%
- 包含服務項目、附加服務
- 部分預約關聯包月方案

**需要的輔助函數**:
```typescript
export function getMockReservations(params: any): ReservationListResponse
export function getMockReservationById(id: number): Reservation | undefined
export function getTodayMockReservations(): TodayReservationDto[]
export function getMockReservationsForCalendar(start: string, end: string): CalendarEvent[]
export function createMockReservation(reservation: any): Reservation
export function updateMockReservation(id: number, reservation: any): Reservation | null
export function deleteMockReservation(id: number): boolean
export function calculateMockReservationCost(request: CostCalculationRequest): CostCalculationResponse
```

---

### 2.4 包月資料 - subscriptions.ts (20 分鐘)

**檔案**: `src/mocks/data/subscriptions.ts`

**資料需求**:
- 至少 10 筆包月資料
- 狀態分布：
  - 有效 (60%)
  - 即將到期 (7天內, 20%)
  - 已過期 (20%)
- 包含使用次數資訊
- 關聯到特定寵物

**需要的輔助函數**:
```typescript
export function getMockSubscriptions(params: any): SubscriptionListResponse
export function getMockSubscriptionById(id: number): Subscription | undefined
export function getMockSubscriptionsByPetId(petId: number): Subscription[]
export function getActiveMockSubscriptions(): Subscription[]
export function getExpiringMockSubscriptions(days: number): ExpiringSubscriptionDto[]
export function createMockSubscription(subscription: any): Subscription
export function updateMockSubscription(id: number, subscription: any): Subscription | null
export function deleteMockSubscription(id: number): boolean
```

---

### 2.5 儀表板資料 - dashboard.ts (15 分鐘)

**檔案**: `src/mocks/data/dashboard.ts`

**資料需求**:
- 統計資料：
  - 今日預約數
  - 總寵物數
  - 本月收入
  - 有效包月數
- 今日預約列表（從 reservations.ts 計算）
- 即將到期包月列表（從 subscriptions.ts 計算）

**需要的輔助函數**:
```typescript
export function getDashboardStatistics(): DashboardStatisticsDto
export function getTodayReservations(): TodayReservationDto[]
export function getMonthlyRevenue(month?: number, year?: number): MonthlyRevenueDto
export function getExpiringSubscriptions(days: number): ExpiringSubscriptionDto[]
```

---

### 2.6 系統代碼資料 - systemCodes.ts (15 分鐘)

**檔案**: `src/mocks/data/systemCodes.ts`

**資料需求**:
- 品種代碼 (Breed): 貴賓犬、黃金獵犬、柴犬、法鬥、博美等
- 性別代碼 (Gender): 公、母
- 服務類型 (ServiceType): 洗澡、美容、指甲修剪、造型等
- 預約狀態 (ReservationStatus): 待確認、已確認、進行中、已完成、已取消、未出席
- 關係類型 (Relationship): 飼主、家人、朋友、照護者
- 付款方式 (PaymentType): 現金、信用卡、轉帳

**需要的輔助函數**:
```typescript
export function getSystemCodesByType(codeType: string): SystemCode[]
export function getSystemCode(codeType: string, code: string): SystemCode | undefined
export function getAllSystemCodeTypes(): string[]
```

---

### 階段二驗證清單

- [ ] `pets.ts` 已創建，包含 20+ 筆資料和輔助函數
- [ ] `contacts.ts` 已創建，包含 15+ 筆資料和輔助函數
- [ ] `reservations.ts` 已創建，包含 30+ 筆資料和輔助函數
- [ ] `subscriptions.ts` 已創建，包含 10+ 筆資料和輔助函數
- [ ] `dashboard.ts` 已創建，包含統計計算函數
- [ ] `systemCodes.ts` 已創建，包含所有系統代碼
- [ ] 所有資料符合 TypeScript 類型定義
- [ ] 資料之間的關聯正確（petId, contactId 等）
- [ ] 使用繁體中文命名和台灣格式

---

## 🔧 階段三：創建 Handler 路由處理層

**負責 Agent**: `frontend-developer`
**預估時間**: 2-2.5 小時
**優先級**: 🟡 高

### Handler 設計原則

1. **完整的 RESTful API** - 支援 GET, POST, PUT, DELETE
2. **真實的網路行為** - 使用 `delay()` 模擬延遲
3. **正確的 HTTP 狀態碼** - 200, 201, 204, 404, 500 等
4. **查詢參數支援** - 分頁、搜尋、篩選
5. **錯誤處理** - 模擬各種錯誤情況

---

### 3.1 寵物 Handlers - petHandlers.ts (25 分鐘)

**檔案**: `src/mocks/handlers/petHandlers.ts`

**實作端點**:

1. **GET /api/pet** - 獲取寵物列表
   - 支援查詢參數：`page`, `pageSize`, `keyword`, `breed`, `gender`, `ownerId`
   - 返回分頁資料
   - 延遲：500ms

2. **GET /api/pet/:id** - 獲取寵物詳情
   - 返回單筆資料
   - 404 處理：寵物不存在
   - 延遲：300ms

3. **POST /api/pet** - 創建寵物
   - 接收 `PetCreateRequest`
   - 返回新寵物 ID
   - 狀態碼：201
   - 延遲：800ms

4. **PUT /api/pet/:id** - 更新寵物
   - 接收 `PetUpdateRequest`
   - 404 處理：寵物不存在
   - 狀態碼：204
   - 延遲：600ms

5. **DELETE /api/pet/:id** - 刪除寵物
   - 404 處理：寵物不存在
   - 狀態碼：204
   - 延遲：400ms

6. **POST /api/pet/:id/photo** - 上傳寵物照片
   - 模擬檔案上傳
   - 返回照片 URL
   - 延遲：1000ms

7. **GET /api/pet/contact/:contactId** - 按聯絡人查詢
   - 返回該聯絡人的所有寵物
   - 延遲：400ms

**範例代碼結構**:
```typescript
import { http, HttpResponse, delay } from 'msw'
import {
  getMockPets,
  getMockPetById,
  createMockPet,
  updateMockPet,
  deleteMockPet,
  getMockPetsByContactId
} from '../data/pets'

export const petHandlers = [
  // GET /api/pet
  http.get('/api/pet', async ({ request }) => {
    const url = new URL(request.url)
    const params = {
      page: Number(url.searchParams.get('page')) || 1,
      pageSize: Number(url.searchParams.get('pageSize')) || 12,
      keyword: url.searchParams.get('keyword') || '',
      breed: url.searchParams.get('breed') || undefined,
      gender: url.searchParams.get('gender') || undefined,
      ownerId: url.searchParams.get('ownerId') ? Number(url.searchParams.get('ownerId')) : undefined
    }

    await delay(500)

    return HttpResponse.json(getMockPets(params))
  }),

  // GET /api/pet/:id
  http.get('/api/pet/:id', async ({ params }) => {
    const { id } = params
    const pet = getMockPetById(Number(id))

    await delay(300)

    if (!pet) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Pet not found'
      })
    }

    return HttpResponse.json(pet)
  }),

  // ... 其他端點
]
```

---

### 3.2 聯絡人 Handlers - contactHandlers.ts (20 分鐘)

**檔案**: `src/mocks/handlers/contactHandlers.ts`

**實作端點**:

1. **GET /api/contactperson** - 獲取聯絡人列表
2. **GET /api/contactperson/:id** - 獲取聯絡人詳情
3. **POST /api/contactperson** - 創建聯絡人
4. **PUT /api/contactperson/:id** - 更新聯絡人
5. **DELETE /api/contactperson/:id** - 刪除聯絡人
6. **POST /api/contactperson/:contactId/pets/:petId** - 關聯寵物
7. **DELETE /api/contactperson/:contactId/pets/:petId** - 取消關聯

---

### 3.3 預約 Handlers - reservationHandlers.ts (35 分鐘)

**檔案**: `src/mocks/handlers/reservationHandlers.ts`

**實作端點**:

1. **GET /api/reservation** - 獲取預約列表
2. **GET /api/reservation/:id** - 獲取預約詳情
3. **POST /api/reservation** - 創建預約
4. **PUT /api/reservation/:id** - 更新預約
5. **DELETE /api/reservation/:id** - 取消預約
6. **POST /api/reservation/:id/complete** - 完成預約
7. **GET /api/reservation/calendar** - 日曆格式資料
8. **POST /api/reservation/calculate-cost** - 計算費用
9. **GET /api/reservation/statistics** - 統計資料
10. **GET /api/reservation/today** - 今日預約
11. **GET /api/reservation/availability** - 檢查可用性
12. **POST /api/reservation/:id/status** - 更新狀態
13. **GET /api/reservation/pet/:petId/active-subscription-for-reservation** - 獲取可用包月
14. **POST /api/reservation/pet/:petId/calculate-duration** - 計算服務時長
15. **GET /api/reservation/pet/:petId/addon-prices** - 獲取附加服務價格

---

### 3.4 包月 Handlers - subscriptionHandlers.ts (30 分鐘)

**檔案**: `src/mocks/handlers/subscriptionHandlers.ts`

**實作端點**:

1. **GET /api/subscription** - 獲取包月列表
2. **GET /api/subscription/:id** - 獲取包月詳情
3. **POST /api/subscription** - 創建包月
4. **PUT /api/subscription/:id** - 更新包月
5. **DELETE /api/subscription/:id** - 取消包月
6. **GET /api/subscription/pet/:petId** - 獲取寵物包月
7. **GET /api/subscription/pet/:petId/active** - 獲取有效包月
8. **GET /api/subscription/:id/availability** - 檢查可用性
9. **POST /api/subscription/:id/reserve** - 預留次數
10. **POST /api/subscription/:id/release** - 釋放次數
11. **POST /api/subscription/:id/confirm** - 確認使用
12. **GET /api/subscription/:id/usage** - 使用情況
13. **GET /api/subscription/:id/remaining** - 剩餘次數
14. **GET /api/subscription/statistics** - 統計資料
15. **GET /api/subscription/expiring** - 即將到期
16. **GET /api/subscription/dashboard-statistics** - 儀表板統計

---

### 3.5 儀表板 Handlers - dashboardHandlers.ts (15 分鐘)

**檔案**: `src/mocks/handlers/dashboardHandlers.ts`

**實作端點**:

1. **GET /api/dashboard/statistics** - 儀表板統計
2. **GET /api/dashboard/today-reservations** - 今日預約
3. **GET /api/dashboard/monthly-revenue** - 月收入
4. **GET /api/dashboard/active-subscriptions-count** - 有效包月數
5. **GET /api/subscription/expiring** - 即將到期包月（共用）

---

### 3.6 系統代碼 Handlers - commonHandlers.ts (15 分鐘)

**檔案**: `src/mocks/handlers/commonHandlers.ts`

**實作端點**:

1. **GET /api/common/systemcodes/list** - 所有系統代碼
2. **GET /api/common/systemcodes/:type** - 特定類型系統代碼
3. **GET /api/common/systemcodes/:type/:code** - 特定系統代碼
4. **GET /api/common/systemcode-types** - 所有代碼類型
5. **POST /api/common/systemcodes** - 新增系統代碼
6. **PUT /api/common/systemcodes/:id** - 更新系統代碼
7. **DELETE /api/common/systemcodes/:id** - 刪除系統代碼

---

### 3.7 更新 browser.ts 註冊 Handlers (10 分鐘)

**修改 `src/mocks/browser.ts`**:

```typescript
import { setupWorker } from 'msw/browser'
import { petHandlers } from './handlers/petHandlers'
import { contactHandlers } from './handlers/contactHandlers'
import { reservationHandlers } from './handlers/reservationHandlers'
import { subscriptionHandlers } from './handlers/subscriptionHandlers'
import { dashboardHandlers } from './handlers/dashboardHandlers'
import { commonHandlers } from './handlers/commonHandlers'

export const worker = setupWorker(
  ...petHandlers,
  ...contactHandlers,
  ...reservationHandlers,
  ...subscriptionHandlers,
  ...dashboardHandlers,
  ...commonHandlers
)

if (import.meta.env.DEV) {
  console.log('🔧 MSW Mock Service Worker initialized with',
    petHandlers.length +
    contactHandlers.length +
    reservationHandlers.length +
    subscriptionHandlers.length +
    dashboardHandlers.length +
    commonHandlers.length,
    'handlers'
  )
}
```

---

### 階段三驗證清單

- [ ] `petHandlers.ts` 已創建，包含 7 個端點
- [ ] `contactHandlers.ts` 已創建，包含 7 個端點
- [ ] `reservationHandlers.ts` 已創建，包含 15 個端點
- [ ] `subscriptionHandlers.ts` 已創建，包含 16 個端點
- [ ] `dashboardHandlers.ts` 已創建，包含 5 個端點
- [ ] `commonHandlers.ts` 已創建，包含 7 個端點
- [ ] `browser.ts` 已更新，註冊所有 handlers
- [ ] 所有 handlers 使用 `delay()` 模擬延遲
- [ ] 所有 handlers 返回正確的 HTTP 狀態碼
- [ ] 錯誤情況處理完善（404, 500 等）

---

## 🧪 階段四：測試與優化

**負責 Agent**: `frontend-developer` + `test-writer-fixer`
**預估時間**: 1 小時
**優先級**: 🟢 中

### 4.1 啟動測試 (10 分鐘)

1. **啟動 Mock 模式**:
   ```bash
   npm run dev:mock
   ```

2. **驗證啟動**:
   - 檢查控制台是否出現 MSW 啟動訊息
   - 檢查 Network 面板是否看到 Service Worker

3. **檢查 Service Worker**:
   - 開啟 DevTools > Application > Service Workers
   - 應該看到 `mockServiceWorker.js` 正在運行

---

### 4.2 功能測試 (30 分鐘)

**測試項目**:

#### 4.2.1 寵物管理測試
- [ ] 訪問寵物列表頁面，驗證資料載入
- [ ] 測試搜尋功能
- [ ] 測試分頁功能
- [ ] 測試寵物詳情查看
- [ ] 測試新增寵物
- [ ] 測試編輯寵物
- [ ] 測試刪除寵物

#### 4.2.2 聯絡人管理測試
- [ ] 訪問聯絡人列表頁面
- [ ] 測試 CRUD 操作

#### 4.2.3 預約管理測試
- [ ] 訪問預約列表頁面
- [ ] 測試預約日曆視圖
- [ ] 測試新增預約
- [ ] 測試費用計算
- [ ] 測試包月方案選擇

#### 4.2.4 包月管理測試
- [ ] 訪問包月列表頁面
- [ ] 測試包月 CRUD 操作
- [ ] 測試即將到期提醒

#### 4.2.5 儀表板測試
- [ ] 訪問儀表板
- [ ] 驗證統計資料顯示
- [ ] 驗證今日預約列表
- [ ] 驗證即將到期包月列表

---

### 4.3 錯誤情況測試 (10 分鐘)

- [ ] 測試 404 錯誤（查詢不存在的資源）
- [ ] 測試網路延遲效果
- [ ] 測試分頁邊界情況
- [ ] 測試空資料情況

---

### 4.4 性能優化 (10 分鐘)

**優化項目**:

1. **調整延遲時間**:
   - 根據實際使用體驗調整各端點的延遲
   - 建議：200-800ms

2. **資料量優化**:
   - 確保列表資料不會一次載入過多
   - 分頁資料合理

3. **記憶體管理**:
   - 確保 mock 資料不會無限增長
   - CRUD 操作正確更新資料

---

### 4.5 文檔完善 (10 分鐘)

更新 `src/mocks/README.md`，添加：
- 詳細的使用說明
- 各模組的 mock 資料說明
- 常見問題解答
- 開發注意事項

---

### 階段四驗證清單

- [ ] Mock 模式可以正常啟動
- [ ] 所有頁面可以正常載入
- [ ] CRUD 操作功能正常
- [ ] 錯誤處理正確
- [ ] 網路延遲模擬合理
- [ ] 性能表現良好
- [ ] 文檔完善

---

## 📖 階段五：創建使用文檔

**負責 Agent**: `frontend-developer`
**預估時間**: 30 分鐘
**優先級**: 🟢 中

### 5.1 創建 MOCK_GUIDE.md (30 分鐘)

**檔案**: `/Users/kun/Documents/Projects/PetSalon/MOCK_GUIDE.md`

**內容包含**:

1. **概述**
   - MSW 是什麼
   - 為什麼使用 MSW
   - 系統架構圖

2. **快速開始**
   - 環境需求
   - 啟動步驟
   - 驗證方式

3. **使用指南**
   - Mock 模式 vs 真實後端
   - 如何切換模式
   - 開發工作流程

4. **資料說明**
   - 各模組的 mock 資料結構
   - 資料之間的關聯
   - 如何修改 mock 資料

5. **Handler 說明**
   - Handler 的作用
   - 如何新增 handler
   - 如何調試 handler

6. **常見問題**
   - Service Worker 未啟動
   - 資料未載入
   - 延遲過長/過短
   - 如何清除 Service Worker 快取

7. **開發注意事項**
   - 不要提交 .env.mock 的敏感資訊
   - Handler 修改後需要重新整理頁面
   - Mock 資料修改不會持久化

8. **測試建議**
   - 如何測試各種錯誤情況
   - 如何測試邊界條件
   - 如何模擬網路問題

---

## 📊 進度追蹤

### 任務分配表

| 階段 | 負責 Agent | 預估時間 | 狀態 | 備註 |
|------|-----------|---------|------|------|
| 階段一：MSW 基礎設置 | frontend-developer | 1h | ⏳ Pending | 最高優先級 |
| 階段二：Mock 資料層 | frontend-developer | 1.5h | ⏳ Pending | 依賴階段一 |
| 階段三：Handler 層 | frontend-developer | 2-2.5h | ⏳ Pending | 依賴階段二 |
| 階段四：測試與優化 | frontend-developer + test-writer-fixer | 1h | ⏳ Pending | 依賴階段三 |
| 階段五：使用文檔 | frontend-developer | 0.5h | ⏳ Pending | 可與其他階段並行 |

**總預估時間**: 5.5-6 小時

---

## 🎯 成功標準

### 功能性標準
- [x] 前端可以在 Mock 模式下獨立運行
- [ ] 所有 65+ 個 API 端點都有對應的 mock handler
- [ ] Mock 資料豐富且真實
- [ ] CRUD 操作功能完整
- [ ] 資料關聯正確

### 技術性標準
- [ ] 零業務代碼侵入（API 層、組件層無需修改）
- [ ] TypeScript 類型安全
- [ ] 網路行為模擬真實（延遲、狀態碼）
- [ ] 錯誤處理完善
- [ ] 性能表現良好

### 可維護性標準
- [ ] 代碼結構清晰（資料層、handler 層分離）
- [ ] 文檔完善
- [ ] 易於擴展（新增 handler 簡單）
- [ ] 易於調試（console.log、Network 面板）

### 團隊協作標準
- [ ] 使用說明清晰
- [ ] 環境切換簡單（一個命令）
- [ ] 不影響真實後端開發
- [ ] 後端完成後易於移除 mock

---

## 🚀 啟動方式

### 開發模式（連接真實後端）
```bash
npm run dev
```

### Mock 模式（使用模擬資料）
```bash
npm run dev:mock
```

### 建置
```bash
# 真實後端
npm run build

# Mock 模式
npm run build:mock
```

---

## 📝 開發注意事項

### 應該做的
- ✅ 參考 `src/types/` 確保資料格式正確
- ✅ 使用繁體中文和台灣格式
- ✅ 保持資料之間的關聯性
- ✅ 模擬真實的網路延遲
- ✅ 處理各種錯誤情況
- ✅ 在 handler 中添加 console.log 方便調試

### 不應該做的
- ❌ 修改 `src/api/` 下的業務代碼
- ❌ 修改 `src/components/` 下的組件代碼
- ❌ 提交 `.env.mock` 中的敏感資訊
- ❌ 在 mock 資料中使用真實用戶資料
- ❌ 過度優化（保持簡單）

---

## 🔍 調試技巧

### 檢查 MSW 是否運行
```javascript
// 在瀏覽器 Console 執行
console.log('MSW running:', !!window.msw)
```

### 查看攔截的請求
- 開啟 DevTools > Network
- 被 MSW 攔截的請求會顯示為 `(from ServiceWorker)`

### 調試 Handler
在 handler 中添加：
```typescript
http.get('/api/pet', async ({ request }) => {
  console.log('🔍 MSW intercepted:', request.url)
  // ... handler 邏輯
})
```

### 清除 Service Worker
如果遇到問題：
1. 開啟 DevTools > Application > Service Workers
2. 點擊 "Unregister"
3. 重新整理頁面

---

## 📚 參考資料

- [MSW 官方文檔](https://mswjs.io/)
- [MSW Browser Integration](https://mswjs.io/docs/integrations/browser)
- [TypeScript 類型定義](./PetSalon.Frontend/src/types/)
- [現有 API 呼叫](./PetSalon.Frontend/src/api/)

---

## 📞 問題回報

如果在實作過程中遇到問題，請記錄：
1. 錯誤訊息
2. 瀏覽器 Console 輸出
3. Network 面板截圖
4. 重現步驟

---

**文檔建立時間**: 2025-10-10
**分支**: `feature/msw-mock-system`
**狀態**: 📋 計劃階段
