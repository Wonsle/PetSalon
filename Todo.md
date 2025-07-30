# Todo.md

## 包月管理系統開發執行順序

### 階段一：資料庫結構調整

- [x] 修改 Subscription 表結構 <!-- 已完成，SQL/10-Table/Subscription.sql 已同步 -->
  - [x] 新增包月類型欄位 (SubscriptionType: BATH/GROOM/MIXED)
  - [x] 新增總次數限制欄位 (TotalUsageLimit)
  - [x] 新增已使用次數欄位 (UsedCount)
  - [x] 新增預留次數欄位 (ReservedCount)
  - [x] 新增包月狀態欄位 (Status: ACTIVE/PAUSED/EXPIRED/CANCELLED)
  - [x] 新增價格欄位 (SubscriptionPrice)
  - [x] 新增備註欄位 (Notes)

- [x] 修改 ReserveRecord 表結構 <!-- 已完成，SQL/10-Table/ReserveRecord.sql 已同步 -->
  - [x] 新增預約狀態欄位 (Status: PENDING/CONFIRMED/IN_PROGRESS/COMPLETED/CANCELLED/NO_SHOW)
  - [x] 修正 PetId 外鍵關聯（目前已存在但需檢查約束）
  - [x] 新增服務總價欄位 (TotalAmount)
  - [x] 新增是否使用包月欄位 (UseSubscription)
  - [x] 確保資料完整性約束

- [x] 新增包月類型配置表 (SubscriptionType) <!-- 已完成，SQL/10-Table/SubscriptionType.sql 已同步 -->
  - [x] 包月類型代碼和名稱
  - [x] 預設次數和價格配置
  - [x] 可用服務類型配置

- [x] 新增通知記錄表 (NotificationLog) <!-- 已完成，SQL/10-Table/NotificationLog.sql 已同步 -->
  - [x] 通知類型（到期提醒、次數不足等）
  - [x] 發送狀態和時間記錄
  - [x] 關聯的訂閱或寵物ID

- [x] 強化 PaymentRecord 與 Subscription 關聯 <!-- 已完成，SQL/10-Table/PaymentRecord.sql 已同步 -->
  - [x] 新增 SubscriptionId 外鍵欄位
  - [x] 新增付款類型標識

### 階段二：後端模型和 DTO 調整

- [x] 更新 Entity Models <!-- 已完成，所有 Entity Model 已同步 Schema 並具備必要屬性 -->
  - [x] 重新生成 Subscription.cs 和 ReserveRecord.cs
  - [x] 確保新欄位的屬性對應正確

- [x] 更新 DTO 類別 <!-- 已完成，DTO 已含包月使用次數、狀態、包月選擇、驗證等資訊 -->
  - [x] 修改 SubscriptionDto.cs 加入使用次數資訊
  - [x] 修改 ReservationDto.cs 加入狀態和包月選擇邏輯
  - [x] 新增包月驗證和計算相關 DTO

### 階段三：核心服務邏輯實作

- [x] 增強 SubscriptionService <!-- 已完成，實作包月次數預留/釋放/確認/檢查/狀態更新 -->
  - [x] 實作包月次數預留/釋放/確認機制
  - [x] 新增包月可用性檢查方法
  - [x] 實作包月狀態自動更新邏輯

- [x] 增強 ReservationService <!-- 已完成，整合包月次數流轉與狀態變更處理 -->
  - [x] 修改建立預約邏輯（預留包月次數）
  - [x] 實作預約狀態變更處理
  - [x] 新增預約完成時的包月次數扣除

- [x] 新增服務類型判斷邏輯 <!-- 已完成，實作ServiceTypeService -->
  - [x] 實作根據 ServiceType 判斷洗澡/美容
  - [x] 處理美容服務的複合扣除邏輯（1美容+3洗澡）

### 階段四：API 控制器調整

- [x] 更新 SubscriptionController <!-- 已完成，新增包月預留/釋放/確認等API -->
  - [x] 新增包月使用情況查詢 API
  - [x] 新增包月驗證 API
  - [x] 更新現有 CRUD 操作以支援新欄位

- [x] 更新 ReservationController <!-- 已完成，支援包月選擇與狀態變更 -->
  - [x] 修改預約建立 API 支援包月選擇
  - [x] 新增預約狀態變更 API
  - [x] 新增預約完成處理 API

### 階段五：前端界面調整 ✅

- [x] 更新包月管理頁面 <!-- 已完成，建立功能完整的包月列表組件 -->
  - [x] 新增包月狀態顯示（彩色標籤）
  - [x] 實作包月歷史記錄查看（時間軸顯示）
  - [x] 包月類型選擇和比較功能
  - [ ] 批量包月購買功能
  - [x] 包月使用情況視覺化圖表

- [x] 更新預約相關頁面 <!-- 已完成，更新ReservationList.vue與ReservationForm.vue -->
  - [x] 預約建立時的包月選擇功能
  - [x] 預約列表顯示包月使用狀態
  - [x] 預約狀態變更操作介面
  - [x] 包月次數即時更新顯示

- [x] 新增儀表板統計 <!-- 已完成，建立SubscriptionDashboard.vue -->
  - [x] 包月使用情況統計圖表
  - [x] 即將到期包月提醒卡片
  - [x] 包月收入統計分析
  - [x] 包月銷售趨勢圖表
  - [x] 包月客戶價值分析

- [x] 前端類型安全優化 <!-- 已完成，修復TypeScript編譯錯誤 -->
  - [x] Vue組件類型定義完善
  - [x] 介面型別規範建立
  - [x] 編譯錯誤修復完成

- [x] API測試工具建立 <!-- 已完成，建立完整的API測試腳本 -->
  - [x] 包月服務API測試
  - [x] 預約服務API測試
  - [x] 併發測試腳本
  - [x] 效能測試工具




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