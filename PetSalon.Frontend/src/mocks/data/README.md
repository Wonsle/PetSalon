# Mock 資料層使用指南

這個目錄包含了 PetSalon 應用程式的 Mock 資料層，提供豐富、真實且符合 TypeScript 類型的測試資料。

## 📁 檔案結構

```
src/mocks/data/
├── systemCodes.ts      # 系統代碼資料（品種、性別、服務類型等）
├── contacts.ts         # 聯絡人資料
├── pets.ts             # 寵物資料
├── subscriptions.ts    # 包月方案資料
├── reservations.ts     # 預約記錄資料
├── dashboard.ts        # 儀表板統計資料
├── index.ts            # 統一導出檔案
├── validate.ts         # 資料驗證腳本
├── README.md           # 使用指南（本檔案）
└── VALIDATION_REPORT.md # 驗證報告
```

## 🚀 快速開始

### 基本使用

```typescript
// 導入所需的函數
import { getMockPets, getMockContacts, getMockReservations } from '@/mocks/data'

// 取得所有寵物（分頁）
const petsResponse = getMockPets({ page: 1, pageSize: 10 })
console.log(petsResponse.data) // 寵物列表
console.log(petsResponse.total) // 總筆數

// 取得單一寵物
const pet = getMockPetById(1)

// 搜尋寵物
const searchResult = getMockPets({
  keyword: '小白',
  breed: 'POODLE',
  page: 1,
  pageSize: 10
})
```

### CRUD 操作

```typescript
import {
  createMockPet,
  updateMockPet,
  deleteMockPet
} from '@/mocks/data'

// 新增寵物
const newPet = createMockPet({
  petName: '小黑',
  breed: 'POODLE',
  gender: 'MALE',
  normalPrice: 800,
  subscriptionPrice: 600
})

// 更新寵物
const updatedPet = updateMockPet(1, {
  petId: 1,
  petName: '小白白',
  breed: 'POODLE',
  gender: 'MALE',
  normalPrice: 850
})

// 刪除寵物
const success = deleteMockPet(1)
```

## 📊 資料模組說明

### 1. 系統代碼 (systemCodes.ts)

提供各種系統設定代碼。

```typescript
import {
  getSystemCodesByType,
  getSystemCode,
  getSystemCodeName
} from '@/mocks/data'

// 取得所有品種代碼
const breeds = getSystemCodesByType('Breed')

// 取得特定代碼
const poodleCode = getSystemCode('Breed', 'POODLE')

// 取得顯示名稱
const breedName = getSystemCodeName('Breed', 'POODLE') // "貴賓犬"
```

**可用的代碼類型**:
- `Breed` - 品種（10種）
- `Gender` - 性別（2種）
- `ServiceType` - 服務類型（6種）
- `ReservationStatus` - 預約狀態（6種）
- `Relationship` - 關係類型（8種）
- `PaymentType` - 付款方式（4種）

### 2. 聯絡人 (contacts.ts)

管理聯絡人資料，包含 17 筆測試資料。

```typescript
import {
  getMockContacts,
  getMockContactById,
  createMockContact
} from '@/mocks/data'

// 取得聯絡人列表
const contacts = getMockContacts({
  keyword: '王小明',
  page: 1,
  pageSize: 10
})

// 新增聯絡人
const newContact = createMockContact({
  name: '陳大明',
  nickName: '大明',
  contactNumber: '0912-345-678'
})
```

**電話格式**: `09XX-XXX-XXX`

### 3. 寵物 (pets.ts)

管理寵物資料，包含 22 筆測試資料。

```typescript
import {
  getMockPets,
  getMockPetById,
  getMockPetsByContactId
} from '@/mocks/data'

// 取得寵物列表（支援搜尋）
const pets = getMockPets({
  keyword: '小白',
  breed: 'POODLE',
  gender: 'MALE',
  ownerId: 1,
  page: 1,
  pageSize: 10
})

// 根據聯絡人取得寵物
const ownerPets = getMockPetsByContactId(1)
```

**價格範圍**:
- normalPrice: 700-1,500 元
- subscriptionPrice: 550-1,200 元

### 4. 包月方案 (subscriptions.ts)

管理包月資料，包含 12 筆測試資料（有效、即將到期、已過期）。

```typescript
import {
  getMockSubscriptions,
  getActiveMockSubscriptions,
  getExpiringMockSubscriptions,
  useSubscription
} from '@/mocks/data'

// 取得包月列表
const subscriptions = getMockSubscriptions({
  petId: 1,
  status: 'ACTIVE',
  page: 1,
  pageSize: 10
})

// 取得有效的包月
const activeSubscriptions = getActiveMockSubscriptions()

// 取得即將到期的包月（7天內）
const expiringSubscriptions = getExpiringMockSubscriptions(7)

// 使用包月次數
const success = useSubscription(1)
```

**狀態分布**:
- 有效（距離到期 > 7天）: 50%
- 即將到期（7天內）: 17%
- 已過期: 33%

### 5. 預約記錄 (reservations.ts)

管理預約資料，包含 33 筆測試資料。

```typescript
import {
  getMockReservations,
  getTodayMockReservations,
  getMockReservationsForCalendar,
  calculateMockReservationCost
} from '@/mocks/data'

// 取得預約列表
const reservations = getMockReservations({
  status: 'CONFIRMED',
  startDate: '2025-10-11',
  endDate: '2025-10-18',
  petId: 1,
  page: 1,
  pageSize: 10
})

// 取得今日預約
const todayReservations = getTodayMockReservations()

// 取得日曆事件
const calendarEvents = getMockReservationsForCalendar(
  '2025-10-01',
  '2025-10-31'
)

// 計算預約費用
const cost = calculateMockReservationCost({
  petId: 1,
  serviceIds: [1, 2],
  useSubscription: true,
  subscriptionId: 1
})
```

**時間分布**:
- 今日預約: 8 筆
- 本週預約: 15 筆
- 本月預約: 33 筆

**狀態分布**:
- PENDING: 21%
- CONFIRMED: 58%
- IN_PROGRESS: 3%
- COMPLETED: 15%
- CANCELLED: 3%

### 6. 儀表板統計 (dashboard.ts)

提供儀表板統計計算函數。

```typescript
import {
  getDashboardStatistics,
  getMonthlyRevenue,
  getExpiringSubscriptions,
  getWeeklyReservationStats
} from '@/mocks/data'

// 取得儀表板統計
const stats = getDashboardStatistics()
// {
//   todayReservations: 8,
//   totalPets: 22,
//   monthlyRevenue: 45000,
//   activeSubscriptions: 6
// }

// 取得月收入
const revenue = getMonthlyRevenue(10, 2025)

// 取得即將到期的包月
const expiring = getExpiringSubscriptions(7)

// 取得本週預約統計
const weeklyStats = getWeeklyReservationStats()
```

## 🔧 進階功能

### 資料驗證

使用內建的驗證腳本檢查資料一致性：

```typescript
import { validateMockData } from '@/mocks/data/validate'

const result = validateMockData()

if (!result.success) {
  console.error('資料驗證失敗:', result.errors)
}

console.log('統計:', result.stats)
```

### 自訂搜尋參數

所有查詢函數都支援豐富的搜尋參數：

```typescript
// 聯絡人搜尋
getMockContacts({
  keyword: '王',        // 關鍵字（姓名、暱稱、電話）
  name: '王小明',       // 精確姓名
  contactNumber: '0912', // 電話號碼
  page: 1,
  pageSize: 10
})

// 寵物搜尋
getMockPets({
  keyword: '小白',      // 關鍵字（寵物名、飼主名、電話）
  breed: 'POODLE',     // 品種
  gender: 'MALE',      // 性別
  ownerId: 1,          // 飼主ID
  page: 1,
  pageSize: 10
})

// 預約搜尋
getMockReservations({
  keyword: '小白',      // 關鍵字
  status: 'CONFIRMED', // 狀態
  startDate: '2025-10-11', // 開始日期
  endDate: '2025-10-18',   // 結束日期
  petId: 1,            // 寵物ID
  ownerId: 1,          // 飼主ID
  designer: '美容師 A', // 美容師
  page: 1,
  pageSize: 10
})
```

## 💡 最佳實踐

### 1. 使用 TypeScript 類型

所有函數都提供完整的類型定義：

```typescript
import type { Pet, PetSearchParams } from '@/types/pet'
import { getMockPets } from '@/mocks/data'

const searchParams: PetSearchParams = {
  breed: 'POODLE',
  page: 1,
  pageSize: 10
}

const response = getMockPets(searchParams)
const pets: Pet[] = response.data
```

### 2. 使用系統代碼

始終通過系統代碼取得顯示名稱：

```typescript
import { getSystemCodeName } from '@/mocks/data'

const pet = getMockPetById(1)
const breedName = getSystemCodeName('Breed', pet.breed) // "貴賓犬"
const genderName = getSystemCodeName('Gender', pet.gender) // "公"
```

### 3. 處理關聯資料

正確處理資料之間的關聯：

```typescript
// 取得寵物和其飼主資訊
const pet = getMockPetById(1)
if (pet.primaryContact) {
  const owner = getMockContactById(pet.primaryContact.contactPersonId)
  console.log(`${pet.petName} 的飼主是 ${owner?.name}`)
}

// 取得預約和其包月資訊
const reservation = getMockReservationById(1)
if (reservation.subscriptionId) {
  const subscription = getMockSubscriptionById(reservation.subscriptionId)
  console.log(`使用包月: ${subscription?.subscriptionType}`)
}
```

### 4. 記憶體內資料修改

所有 CRUD 函數直接修改記憶體內的資料陣列：

```typescript
// 新增後可以立即查詢到
const newPet = createMockPet({ ... })
const found = getMockPetById(newPet.petId) // 可以找到

// 更新後資料會改變
updateMockPet(1, { petName: '新名字' })
const updated = getMockPetById(1)
console.log(updated.petName) // "新名字"

// 刪除後資料會消失
deleteMockPet(1)
const deleted = getMockPetById(1) // undefined
```

## 📝 注意事項

1. **基準日期**: Mock 資料使用 `2025-10-11` 作為今天的日期
2. **台灣本地化**: 所有資料使用繁體中文和台灣格式
3. **電話格式**: 固定為 `09XX-XXX-XXX` 格式
4. **記憶體資料**: 資料存在記憶體中，重新載入頁面會重置
5. **類型安全**: 所有函數都有完整的 TypeScript 類型定義

## 🐛 常見問題

### Q: 為什麼資料重新載入後會重置？

A: Mock 資料存在記憶體中，不會持久化。如需持久化，可以考慮使用 localStorage 或整合到 MSW 的 sessionStorage。

### Q: 如何新增更多測試資料？

A: 直接修改對應的資料檔案（如 `pets.ts`），在資料陣列中新增項目即可。

### Q: 資料 ID 會重複嗎？

A: 不會。所有 `create*` 函數都會自動生成新的 ID（當前最大 ID + 1）。

### Q: 如何驗證資料一致性？

A: 執行 `validateMockData()` 函數，會自動檢查所有資料的一致性和完整性。

## 🔗 相關文件

- [VALIDATION_REPORT.md](./VALIDATION_REPORT.md) - 資料驗證報告
- [TypeScript 類型定義](/src/types/) - 介面定義
- [MSW Handlers](/src/mocks/handlers/) - API Mock 處理器

---

**最後更新**: 2025-10-11
**版本**: v1.0
