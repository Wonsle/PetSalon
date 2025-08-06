# Todo.md

## 包月管理系統開發執行順序

## 預約定價系統優化開發

### 階段一：資料表架構建立

#### 新增資料表
- [ ] 建立 PetServiceAddonPrice 表
  - [ ] 建立表結構 (SQL/10-Table/PetServiceAddonPrice.sql)
  - [ ] 設定外鍵約束 (Pet, ServiceAddon)
  - [ ] 建立唯一約束 (PetID, ServiceAddonID)
  - [ ] 加入審計欄位

- [ ] 建立 PetServiceDuration 表
  - [ ] 建立表結構 (SQL/10-Table/PetServiceDuration.sql)
  - [ ] 設定外鍵約束 (Pet, Service)
  - [ ] 建立唯一約束 (PetID, ServiceID)
  - [ ] 加入審計欄位

#### SystemCode 擴充
- [ ] 新增 AddonType 代碼類型
  - [ ] 建立 SystemCode-AddonType.sql
  - [ ] 插入 STYLING (造型服務)
  - [ ] 插入 TREATMENT (護理服務)
  - [ ] 插入 BEAUTY (美容服務)
  - [ ] 插入 HEALTH (健康服務)

#### 索引優化
- [ ] PetServiceAddonPrice 表索引
  - [ ] 唯一約束索引：IX_PetServiceAddonPrice_Pet_Addon
  - [ ] 查詢優化索引：IX_PetServiceAddonPrice_ServiceAddon
  - [ ] 外鍵索引：IX_PetServiceAddonPrice_Pet

- [ ] PetServiceDuration 表索引
  - [ ] 唯一約束索引：IX_PetServiceDuration_Pet_Service
  - [ ] 查詢優化索引：IX_PetServiceDuration_Service

### 階段二：初始化數據建立

#### ServiceAddon 初始化
- [ ] 建立 ServiceAddon-InitialData.sql
  - [ ] 造型加價 (STYLING, $200)
  - [ ] 貴賓腳 (STYLING, $100)
  - [ ] 除蚤處理 (TREATMENT, $150)
  - [ ] 指甲彩繪 (BEAUTY, $80)
  - [ ] 香水 (BEAUTY, $50)
  - [ ] SPA護理 (TREATMENT, $300)

#### Service 初始化
- [ ] 建立 Service-InitialData.sql
  - [ ] 基礎洗澡 (BATH, $300, 60分鐘)
  - [ ] 精緻美容 (GROOM, $800, 120分鐘)
  - [ ] 指甲修剪 (NAIL, $100, 15分鐘)
  - [ ] 耳朵清潔 (EAR, $80, 10分鐘)

### 階段三：Entity Framework 整合

#### Entity Models 更新
- [ ] 生成 ServiceAddon Entity Model
- [ ] 生成 PetServiceAddonPrice Entity Model
- [ ] 生成 PetServiceDuration Entity Model
- [ ] 更新 DbContext 設定

#### Repository 層建立
- [ ] 建立 ServiceAddonRepository
- [ ] 建立 PetServiceAddonPriceRepository
- [ ] 建立 PetServiceDurationRepository

### 階段四：業務邏輯層實作 ✅ 完成

#### ServiceAddonService 實作
- [x] 建立 IServiceAddonService 介面
- [x] 實作 ServiceAddonService
  - [x] GetAllAddons() / GetActiveServiceAddonListAsync()
  - [x] GetAddonsByType() / GetServiceAddonsByTypeAsync()
  - [x] GetAddonPrice() / GetEffectiveAddonPriceAsync()
  - [x] CRUD 操作 (Create/Update/Delete/Toggle)

#### PetServiceAddonPriceService 實作
- [x] 建立 IPetServiceAddonPriceService 介面
- [x] 實作 PetServiceAddonPriceService
  - [x] GetEffectiveAddonPriceAsync() - 客製化價格優先邏輯
  - [x] GetActivePetServiceAddonPricesAsync()
  - [x] 完整 CRUD 操作支援

#### PetServiceDurationService 實作
- [x] 建立 IPetServiceDurationService 介面
- [x] 實作 PetServiceDurationService
  - [x] GetEffectiveServiceDurationAsync() - 客製化時間優先邏輯
  - [x] GetActivePetServiceDurationsAsync()
  - [x] 完整 CRUD 操作支援

#### 定價計算服務優化
- [x] 更新 ReservationService 定價邏輯
  - [x] 整合 PetServiceAddonPrice 查詢
  - [x] 實作定價優先級邏輯（客製化優先於預設）
  - [x] 支援附加服務價格為 $0 的情況
  - [x] 移除硬編碼價格邏輯
  - [x] 實作包月/非包月不同定價邏輯

- [x] 時間計算服務實作
  - [x] 整合 PetServiceDuration 查詢
  - [x] 實作時間優先級邏輯（客製化優先於預設）
  - [x] 支援個別化服務時間設定
  - [x] 新增 CalculateTotalServiceDurationAsync 方法

#### 新增輔助功能
- [x] GetPetAddonPricesAsync() - 取得寵物所有附加服務價格
- [x] GetPetPricingOverviewAsync() - 寵物完整定價設定概覽
- [x] 更新服務註冊到 Program.cs
- [x] 完整的業務需求實作：
  - 需求1: 包月不帶入洗澡美容金額 ✅
  - 需求2: 非包月帶入寵物單次金額 ✅ 
  - 需求3: 附加服務帶入預設金額 ✅
  - 需求4: 附加服務可設定金額 ✅
  - 需求5: 未設定金額預設為0 ✅

### 階段五：API 控制器更新 ✅ 完成

#### ServiceAddonController 改進
- [x] 移除硬編碼 service-addons 端點
- [x] 實作完整 ServiceAddon CRUD API
- [x] 新增價格查詢 API
- [x] 新增類型篩選 API

#### ReservationController 整合
- [x] 更新預約建立邏輯
- [x] 整合真實價格計算
- [x] 整合真實時間計算
- [x] 支援包月定價邏輯

#### 新增管理功能API
- [x] 建立 PetServiceAddonPriceController - 寵物客製化附加服務價格管理
- [x] 建立 PetServiceDurationController - 寵物客製化服務時間管理
- [x] 完整的 CRUD 操作支援
- [x] 批次操作功能
- [x] 統計查詢功能

### 階段六：前端整合更新

#### API 服務層更新
- [ ] 更新 serviceAddonApi 使用真實端點
- [ ] 新增價格查詢 API 呼叫
- [ ] 更新錯誤處理邏輯

#### 預約表單優化
- [ ] 更新 ReservationForm.vue
  - [x] 使用真實 ServiceAddon API
  - [ ] 支援附加服務價格為 $0 的顯示
  - [ ] 即時價格計算和更新
  - [ ] 顯示預估服務時間

#### 管理功能新增
- [ ] 建立 ServiceAddon 管理頁面
  - [ ] 附加服務列表顯示
  - [ ] 價格編輯功能
  - [ ] 新增/刪除附加服務
  - [ ] 批量價格更新

- [ ] 建立寵物客製化定價管理
  - [ ] PetServicePrice 管理介面
  - [ ] PetServiceAddonPrice 管理介面
  - [ ] PetServiceDuration 管理介面

### 階段七：測試與驗證

#### 單元測試
- [ ] ServiceAddonService 測試
- [ ] 定價計算邏輯測試
- [ ] 時間計算邏輯測試

#### 整合測試
- [ ] API 端點測試
- [ ] 資料庫整合測試
- [ ] 前後端整合測試

#### 業務邏輯驗證
- [ ] 驗證需求1：包月不帶入洗澡美容金額
- [ ] 驗證需求2：非包月帶入寵物單次金額
- [ ] 驗證需求3：附加服務帶入預設金額
- [ ] 驗證需求4：附加服務可設定金額
- [ ] 驗證需求5：未設定金額預設為0

### 🎯 開發重點提醒

**核心階段（必須完成）：**
- 階段1-3：基礎設施，必須優先完成
- 階段4-6：核心邏輯和測試，是系統功能的關鍵

**增強階段（建議完成）：**
- 階段7-8：業務邏輯增強和效能優化
- 階段9-10：運營管理和系統整合

**執行策略：**
1. 先完成階段1-6的核心功能
2. 根據業務需求決定階段7-10的優先級
3. 每個階段完成後進行測試驗證
4. 建議採用敏捷開發方式，分批交付功能

**風險控制：**
- 資料庫結構變更需要謹慎規劃
- 併發控制是關鍵技術難點
- 效能優化需要持續監控和調整