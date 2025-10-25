# 資料表重複設計重構進度報告

**日期**: 2025-10-25
**最後更新**: 2025-10-25
**狀態**: ✅ 階段 1-6 全部完成 | Backend + Frontend 完整重構 100% 完成
**剩餘工作**: 執行 SQL 初始化腳本 + 整合測試

---

## 📊 重構目標

1. ✅ **刪除 PetServiceDuration 表，統一使用 PetServicePrice**
2. ⏸️ 立即移除 Pet 表的 NormalPrice 和 SubscriptionPrice 欄位（待執行）
3. ✅ **保持 PaymentRecord 和 Income/Expense 分離，明確定義用途**

---

## ✅ 已完成的工作（階段 1-3）

### 階段 1: 建立 PetServicePriceService

#### 新增檔案
- ✅ `/PetSalon.Backend/PetSalon.Service/PetServicePriceService/IPetServicePriceService.cs`
  - 定義完整的服務介面
  - 包含 `GetEffectiveServiceDurationAsync()` 和 `GetEffectiveServicePriceAsync()`

- ✅ `/PetSalon.Backend/PetSalon.Service/PetServicePriceService/PetServicePriceService.cs`
  - 實作所有業務邏輯
  - 整合價格和時長管理
  - 支援批次操作

- ✅ `/PetSalon.Backend/PetSalon.Web/Controllers/PetServicePriceController.cs`
  - 提供完整的 REST API
  - 包含 CRUD、批次操作、統計資料等端點

#### 更新檔案
- ✅ `PetSalon.Backend/PetSalon.Web/Program.cs:169`
  - 註冊 `IPetServicePriceService` 服務

- ✅ `PetSalon.Backend/PetSalon.Models/DTOs/PetServicePricingDto.cs`
  - 新增 `PetServicePriceDto`
  - 新增 `CreatePetServicePriceRequest`
  - 新增 `BatchCreatePetServicePricesRequest`

---

### 階段 2: 遷移 ReservationService 依賴

#### 更新檔案
- ✅ `PetSalon.Backend/PetSalon.Service/ReservationService/ReservationService.cs`
  - Line 12: 依賴從 `IPetServiceDurationService` 改為 `IPetServicePriceService`
  - Line 17: 建構函式參數更新
  - Line 239: `GetEffectiveServiceDurationAsync()` 呼叫更新
  - Line 351: `GetEffectiveServiceDurationAsync()` 呼叫更新
  - Line 383-400: `GetPetPricingOverviewAsync()` 方法更新

---

### 階段 3: 刪除 PetServiceDuration

#### 刪除的檔案
- ✅ `/PetSalon.Backend/PetSalon.Web/Controllers/PetServiceDurationController.cs`
- ✅ `/PetSalon.Backend/PetSalon.Service/PetServiceDurationService/PetServiceDurationService.cs`
- ✅ `/PetSalon.Backend/PetSalon.Service/PetServiceDurationService/IPetServiceDurationService.cs`
- ✅ `/PetSalon.Backend/PetSalon.Models/EntityModels/PetServiceDuration.cs`
- ✅ `/SQL/10-Table/PetServiceDuration.sql`

#### 更新的檔案
- ✅ `PetSalon.Backend/PetSalon.Models/EntityModels/Pet.cs`
  - 移除 `PetServiceDuration` navigation property

- ✅ `PetSalon.Backend/PetSalon.Models/EntityModels/Service.cs`
  - 移除 `PetServiceDuration` navigation property

- ✅ `PetSalon.Backend/PetSalon.Models/EntityModels/PetSalonContext.cs`
  - 移除 `DbSet<PetServiceDuration>`
  - 移除 Entity Configuration

- ✅ `PetSalon.Backend/PetSalon.Web/Program.cs`
  - 移除 `IPetServiceDurationService` 服務註冊

- ✅ `PetSalon.Backend/PetSalon.Models/DTOs/ServiceDto.cs`
  - 移除重複的 `PetServicePriceDto` 定義（已移至 PetServicePricingDto.cs）

---

## 🧪 測試結果

### 編譯測試
```bash
cd /Users/kun/Documents/Projects/PetSalon/PetSalon.Backend
dotnet build PetSalon.sln
```

**結果**: ✅ **Build succeeded**
- **Errors**: 0
- **Warnings**: 61 (主要是 nullable reference types 警告，不影響功能)

---

## 🔍 需要注意的事項

### 1. 資料庫遷移
**目前狀態**: 程式碼已更新，但資料庫 Schema 尚未變更

**需要執行**:
```bash
# 建立新的 Migration 以刪除 PetServiceDuration 表
cd PetSalon.Backend/PetSalon.Models
dotnet ef migrations add RemovePetServiceDuration --startup-project ../PetSalon.Web

# 執行 Migration（測試後）
dotnet ef database update --startup-project ../PetSalon.Web
```

### 2. 現有資料
如果資料庫中已有 PetServiceDuration 的資料，需要考慮：
- 是否需要遷移到 PetServicePrice？
- 還是可以直接刪除？

### 3. API 端點變更
**已移除的 API**:
- `GET /api/petserviceduration`
- `POST /api/petserviceduration`
- 等所有 PetServiceDuration 相關端點

**新增的 API**:
- `GET /api/petserviceprice`
- `POST /api/petserviceprice`
- `GET /api/petserviceprice/effective-price/{petId}/{serviceId}`
- `GET /api/petserviceprice/effective-duration/{petId}/{serviceId}`
- 等完整的 PetServicePrice 端點

---

## ✅ 已完成的工作（階段 4）

### 階段 4: 移除 Pet 表的價格欄位並建立服務價格處理

#### 更新的檔案

**1. SQL Schema**
- ✅ `/SQL/10-Table/Pet.sql`
  - 移除 `NormalPrice` 和 `SubscriptionPrice` 欄位
  - 更新表格說明：價格資訊已移至 PetServicePrice 表

**2. Entity Models**
- ✅ `PetSalon.Backend/PetSalon.Models/EntityModels/Pet.cs`
  - 移除 `NormalPrice` 和 `SubscriptionPrice` 屬性
- ✅ `PetSalon.Backend/PetSalon.Models/EntityModels/PetSalonContext.cs`
  - 移除價格欄位的 Entity Configuration

**3. DTOs**
- ✅ `PetSalon.Backend/PetSalon.Models/DTOs/PetDto.cs`
  - 移除 `PetListResponse` 中的價格欄位
  - 移除 `CreatePetRequest` 和 `UpdatePetRequest` 中的價格欄位
  - 新增 `PetServicePriceSetting` 類別
  - 在 `CreatePetRequest` 和 `UpdatePetRequest` 中新增 `ServicePrices` 欄位

**4. Services**
- ✅ `PetSalon.Backend/PetSalon.Service/ServiceService/IServiceService.cs`
  - 新增 `GetDefaultServicesAsync()` 方法
- ✅ `PetSalon.Backend/PetSalon.Service/ServiceService/ServiceService.cs`
  - 實作 `GetDefaultServicesAsync()` - 取得洗澡(BATH)和美容(GROOM)服務
- ✅ `PetSalon.Backend/PetSalon.Service/PetService/PetService.cs`
  - 移除所有價格欄位映射（5個位置）

**5. Controllers**
- ✅ `PetSalon.Backend/PetSalon.Web/Controllers/ServiceController.cs`
  - 新增 `GET /api/service/default` endpoint
  - 返回預設服務清單（洗澡和美容）
- ✅ `PetSalon.Backend/PetSalon.Web/Controllers/PetController.cs`
  - 注入 `IPetServicePriceService`
  - 更新 `CreatePet` 方法：處理 ServicePrices 並建立 PetServicePrice 記錄
  - 更新 `UpdatePet` 方法：刪除舊記錄並建立新的 PetServicePrice 記錄
  - 移除價格欄位設定

#### 測試結果

**編譯測試**:
```bash
dotnet build PetSalon.sln
```

**結果**: ✅ **Build succeeded**
- **Errors**: 0
- **Warnings**: 61 (主要是 nullable reference types 警告，不影響功能)

---

## ✅ 已完成的工作（階段 5）

### 階段 5: Frontend 更新

**已完成任務**:

**1. 類型定義更新**
- ✅ `types/pet.ts` - 移除 normalPrice/subscriptionPrice，新增 PetServicePriceSetting 介面
- ✅ `types/service.ts` - 更新 Service 和 PetServicePrice 介面以匹配後端 DTOs

**2. API 服務更新**
- ✅ `api/service.ts` - 新增 `getDefaultServices()` 方法取得洗澡和美容服務

**3. Vue 組件更新**
- ✅ `components/forms/PetForm.vue`
  - 移除 normalPrice/subscriptionPrice 輸入欄位
  - 新增服務價格設定區塊（洗澡和美容）
  - 更新 handleSubmit 建立 servicePrices 陣列
  - 新增 loadDefaultServices() 函數
  - 新增 onMounted 生命週期鉤子
  - 移除 watch 和 handlePetSelect 中的價格欄位引用

- ✅ `views/pets/PetCreate.vue`
  - 移除 normalPrice/subscriptionPrice 輸入欄位
  - 新增服務價格設定區塊（與 PetForm 相同結構）
  - 更新表單資料結構
  - 新增服務價格處理邏輯

- ✅ `components/common/PetSelector.vue`
  - 移除 showPrice prop
  - 移除訂閱價格顯示
  - 移除相關 CSS 樣式

**4. Composables 更新**
- ✅ `composables/usePetSelector.ts`
  - 移除統計資訊中的 withSubscriptionPrice/withoutSubscriptionPrice
  - 更新 calculateSubscriptionAmount() 函數（保留但返回 0，避免破壞現有調用）

**5. Mock 資料更新**
- ✅ `mocks/data/pets.ts`
  - 移除所有寵物物件中的 normalPrice 和 subscriptionPrice 欄位
  - 更新 createMockPet() 函數
  - 更新 updateMockPet() 函數

---

## ✅ 已完成的工作（階段 6）

### 階段 6: 訂閱價格整合

**實作決策**: 採用方案 B - 從服務表取得訂閱定價（優先 PetServicePrice，其次 Service）

**已完成任務**:

**1. SQL 資料初始化**
- ✅ `SQL/70-InintialData/SystemCode-ServiceType-Subscription.sql`
  - 新增 SUBSCRIPTION 服務類型到 SystemCode

- ✅ `SQL/70-InintialData/Service-Subscription.sql`
  - 新增包月訂閱服務項目（預設價格 1800 元）
  - 可為每隻寵物透過 PetServicePrice 設定客製化訂閱價格

**2. Backend API**
- ✅ `PetServicePriceService/IPetServicePriceService.cs`
  - 新增 `GetSubscriptionPriceAsync(long petId)` 介面方法

- ✅ `PetServicePriceService/PetServicePriceService.cs`
  - 實作訂閱價格取得邏輯：
    1. 優先從 PetServicePrice 取得該寵物的客製化訂閱價格
    2. 如無客製化價格，從 Service 表取得預設訂閱價格
    3. 如都沒有，返回 null

- ✅ `Controllers/PetServicePriceController.cs`
  - 新增 `GET /api/petserviceprice/subscription-price/{petId}` endpoint

**3. Frontend API 服務**
- ✅ `api/petServicePrice.ts`
  - 新增 `getSubscriptionPrice(petId)` 方法呼叫後端 API

**4. Frontend 組件更新**
- ✅ `composables/usePetSelector.ts`
  - 更新 `calculateSubscriptionAmount()` 為非同步函數
  - 改用 API 取得訂閱價格，不再依賴 Pet 實體的 subscriptionPrice 欄位

- ✅ `components/forms/SubscriptionForm.vue`
  - 移除所有對 `pet.subscriptionPrice` 的直接引用
  - 新增 `subscriptionPrice` ref 和 `loadSubscriptionPrice()` 函數
  - 更新寵物選擇時自動載入訂閱價格
  - 更新包月價格顯示（從 API 取得）
  - 更新驗證邏輯（檢查 subscriptionPrice ref 而非 pet 屬性）
  - 更新 watch 函數使用非同步載入

**測試結果**:
- ✅ Backend 編譯成功（0 錯誤，22 警告）

---

## 📋 後續建議

### 立即測試項目
1. **Backend API 測試**
   - 啟動 Backend: `cd PetSalon.Backend/PetSalon.Web && dotnet run`
   - 訪問 Swagger: `http://localhost:5150/swagger`
   - 測試新的 PetServicePrice API 端點

2. **ReservationService 測試**
   - 測試建立預約時是否正確計算服務時長
   - 測試價格計算邏輯

3. **資料完整性檢查**
   - 確認所有引用 PetServiceDuration 的地方都已更新

### 執行 Migration 前的準備
```bash
# 1. 備份資料庫
# 2. 檢查是否有 PetServiceDuration 資料
SELECT COUNT(*) FROM PetServiceDuration;

# 3. 如需保留資料，先遷移到 PetServicePrice
# 4. 確認無誤後執行 Migration
```

### 下一步建議

**立即可執行**:
1. **Frontend 功能測試**
   - 測試新增寵物時服務價格輸入功能
   - 測試編輯寵物時服務價格更新功能
   - 驗證預設服務 API 正常運作

2. **整合測試**
   - 建立寵物並設定洗澡/美容價格
   - 驗證價格正確儲存到 PetServicePrice 表
   - 測試預約系統是否正確讀取服務價格

**需要決策的項目**:
1. **訂閱功能處理方式**
   - 選擇處理訂閱價格的方案（A/B/C）
   - 根據選擇更新 SubscriptionForm 等組件

2. **資料庫 Migration**
   - 決定是否執行 Migration 移除 Pet 表的價格欄位
   - 考慮資料遷移策略（如有現有資料）

---

## 📞 總結

**✅ 已完成**:
- ✅ Backend 完整重構（PetServicePrice 服務、API、實體配置）
- ✅ Frontend 核心重構（Pet 表單、類型定義、API 服務、Mock 資料）
- ✅ 移除 Pet 實體的價格欄位依賴
- ✅ 訂閱價格整合（SUBSCRIPTION 服務類型、訂閱價格 API、SubscriptionForm 更新）
- ✅ 完整的價格邏輯流程：Pet → PetServicePrice（優先）→ Service（預設）

**⏸️ 待處理**:
- 執行 SQL 初始化腳本：
  1. `SystemCode-ServiceType-Subscription.sql` - 新增訂閱服務類型
  2. `Service-Subscription.sql` - 新增訂閱服務項目
- 完整的端到端測試（建議測試流程）:
  1. 執行 SQL 腳本初始化訂閱服務
  2. 測試新增寵物並設定洗澡/美容價格
  3. 為寵物設定訂閱價格（透過 PetServicePrice）
  4. 建立包月訂閱，驗證自動載入訂閱價格
  5. 測試沒有設定訂閱價格的寵物（應使用預設 1800 元）
- 資料庫 Migration 執行（選擇性，移除 Pet 表的價格欄位）

**目前狀態**: ✅ 100% 程式碼完成，所有功能整合完畢
**下一步**: 執行 SQL 初始化腳本後進行完整測試
