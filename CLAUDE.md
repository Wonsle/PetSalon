<!-- SPECTRA:START v1.0.2 -->

# Spectra Instructions

This project uses Spectra for Spec-Driven Development(SDD). Specs live in `openspec/specs/`, change proposals in `openspec/changes/`.

## Use `/spectra-*` skills when:

- A discussion needs structure before coding → `/spectra-discuss`
- User wants to plan, propose, or design a change → `/spectra-propose`
- Tasks are ready to implement → `/spectra-apply`
- There's an in-progress change to continue → `/spectra-ingest`
- User asks about specs or how something works → `/spectra-ask`
- Implementation is done → `/spectra-archive`
- Commit only files related to a specific change → `/spectra-commit`

## Workflow

discuss? → propose → apply ⇄ ingest → archive

- `discuss` is optional — skip if requirements are clear
- Requirements change mid-work? Plan mode → `ingest` → resume `apply`

## Parked Changes

Changes can be parked（暫存）— temporarily moved out of `openspec/changes/`. Parked changes won't appear in `spectra list` but can be found with `spectra list --parked`. To restore: `spectra unpark <name>`. The `/spectra-apply` and `/spectra-ingest` skills handle parked changes automatically.

<!-- SPECTRA:END -->

# CLAUDE.md

此檔案提供 Claude Code (claude.ai/code) 在此儲存庫中處理程式碼的指引。

## 專案概觀

PetSalon 是一個 .NET 8 Web API 應用程式，用於管理寵物美容沙龍業務。系統處理寵物、聯絡人、預約、訂閱和付款記錄。它使用 Entity Framework Core 與 SQL Server，並包含 JWT 驗證。

## 架構

這是一個多層 .NET 解決方案，包含以下專案：

- **PetSalon.Web**：包含控制器和 JWT 驗證的 Web API 層
- **PetSalon.Services**：包含服務介面和實作的商業邏輯層
- **PetSalon.Models**：資料模型和 Entity Framework DbContext
- **PetSalon.Tools**：包含資料庫攔截器的工具類別

### 核心實體

主要的商業實體為：

- **Pet**：核心實體，包含品種、性別、定價（一般和訂閱）、相片上傳支援
- **ContactPerson**：寵物主人/聯絡人，包含電話號碼和關係類型
- **PetRelation**：將寵物連結到其聯絡人及關係類型
- **ReserveRecord**：預約登記，整合服務和加購項目
- **Subscription**：寵物的月度方案，包含時間週期驗證
- **PaymentRecord**：財務交易，包含收入/支出分類
- **Service**：美容服務，包含定價和持續時間
- **ServiceAddon**：額外服務（造型、護理），包含定價
- **PetServicePrice**：針對特定服務的每隻寵物自訂定價
- **SystemCode**：設定/查找資料（品種、性別、服務類型、狀態）

### 資料庫

- 使用 Entity Framework Core 8.0 與 SQL Server
- 從 EF Core Power Tools 自動產生的 Context
- 資料庫優先方法，SQL 腳本位於 `/SQL` 資料夾中
- 用於稽核欄位（建立/修改使用者/時間）的實體攔截器

## 開發指令

### 後端 (.NET 8 API)

```bash
# 建構整個解決方案
dotnet build PetSalon.Backend/PetSalon.sln

# 執行 Web API（從 PetSalon.Web 目錄）
cd PetSalon.Backend/PetSalon.Web
dotnet run

# API 將在 http://localhost:5150 提供 Swagger UI
```

### 前端 (Vue.js)

```bash
# 安裝相依套件
cd PetSalon.Frontend
npm install

# 執行開發伺服器
npm run dev
# 前端將在 http://localhost:3000 可用（如果 3000 忙碌則為 3001）

# 建構生產版本
npm run build

# 型別檢查
npm run type-check

# Linting
npm run lint
```


### 使用系統代碼初始化資料庫

```sql
-- 依序執行 SystemCode 初始化腳本：
-- SQL/70-InintialData/SystemCode-Breed.sql
-- SQL/70-InintialData/SystemCode-Gender.sql
-- SQL/70-InintialData/SystemCode-ServiceType.sql
-- SQL/70-InintialData/SystemCode-ReservationStatus.sql
-- SQL/70-InintialData/SystemCode-Relationship.sql
-- SQL/70-InintialData/SystemCode-PaymentType.sql
```

### 資料庫操作

```bash
# 執行 Entity Framework 移轉（如果使用移轉）
dotnet ef database update --project PetSalon.Models --startup-project PetSalon.Web

# 從現有資料庫產生模型（當架構變更時）
# 使用 efpt.config.json 中的 EF Core Power Tools 設定
```

### 測試

```bash
# 執行測試（如果存在測試專案）
dotnet test PetSalon.Backend/PetSalon.sln
```

## 關鍵設定

### 連線字串

- 資料庫連線在 `appsettings.json` 中設定為 "DefaultConnection"
- 使用整合安全性的 SQL Server

### JWT 設定

- 使用可設定的發行者和簽署金鑰實作 JWT 驗證
- Token 過期時間預設為 30 分鐘
- 預設指派 Admin 和 Users 角色

### 啟動設定

**後端：**
- 開發設定檔在 5150 連接埠執行
- Swagger UI 可在 `/swagger` 端點存取
- 亦設定了 IIS Express 設定檔

**前端：**
- 開發伺服器在 3000 連接埠執行（如果忙碌則為 3001）
- 自動代理到 localhost:5150 的後端 API
- 用於即時更新的熱模組替換
- Vue DevTools 整合

## 開發注意事項

### 服務註冊

服務在 `Program.cs` 中註冊：

- `ICommonService` → `CommonService`（SystemCode 管理）
- `IPetService` → `PetService`（寵物 CRUD 和相片上傳）
- `IContactPersonService` → `ContactPersonService`（包含關係的聯絡人管理）
- `IReservationService` → `ReservationService`（包含訂閱整合的預約系統）
- `JwtHelpers` 為 Singleton

### 資料庫 Context

- `PetSalonContext` 是自動產生的，不應手動編輯
- 使用 `EntitySaveChangesInterceptor` 進行稽核追蹤
- 使用連線字串建構器進行設定
- 新資料表：Service, ServiceAddon, PetServicePrice, ReservationService, ReservationAddon
- 複雜關係支援訂閱制定價和服務客製化

### API 架構

- 控制器繼承自 `BaseController`
- 設定 JWT 驗證中介軟體
- 啟用開發用的 Swagger 文件

### 目前系統代碼

基於 SQL 初始化檔案：
- **Breed**：各種狗品種（貴賓、黃金獵犬等）
- **Gender**：公/母
- **ServiceType**：洗澡、美容、剪指甲、特殊造型、包月
- **ReservationStatus**：待確認、已確認、進行中、已完成、已取消、未出席
- **Relationship**：主人、父親、母親、兄弟、姊妹、家人、朋友、照顧者
- **IncomeType**：美容、零售、加購、訂閱收入
- **ExpenseType**：水電費、電話費、租金、耗材、設備、行銷

## API 端點

### 寵物管理
- `GET /api/pet` - 列出所有寵物
- `GET /api/pet/{id}` - 取得寵物詳細資訊
- `POST /api/pet` - 建立新寵物
- `PUT /api/pet/{id}` - 更新寵物
- `DELETE /api/pet/{id}` - 刪除寵物
- `POST /api/pet/{id}/photo` - 上傳寵物相片
- `GET /api/pet/contact/{contactPersonId}` - 依聯絡人取得寵物

### 聯絡人管理
- `GET /api/contactperson` - 列出所有聯絡人
- `GET /api/contactperson/{id}` - 取得聯絡人詳細資訊
- `POST /api/contactperson` - 建立新聯絡人
- `PUT /api/contactperson/{id}` - 更新聯絡人
- `DELETE /api/contactperson/{id}` - 刪除聯絡人
- `POST /api/contactperson/{contactId}/pets/{petId}` - 連結聯絡人至寵物
- `DELETE /api/contactperson/{contactId}/pets/{petId}` - 解除聯絡人與寵物的連結

### 系統代碼管理
- `GET /api/common/systemcodes/{codeType}` - 依類型取得代碼
- `GET /api/common/systemcodes/{codeType}/{code}` - 取得特定代碼
- `GET /api/common/systemcode-types` - 取得所有代碼類型
- `POST /api/common/systemcodes` - 建立新系統代碼
- `PUT /api/common/systemcodes/{id}` - 更新系統代碼
- `DELETE /api/common/systemcodes/{id}` - 刪除系統代碼

### 檔案上傳支援
- 寵物相片上傳至 `/wwwroot/uploads/pets/`
- 支援格式：JPG, PNG, GIF
- 檔案命名：`{petId}_{guid}.{extension}`

### 服務加購項目
- 透過臨時 API 端點管理額外服務：`GET /api/common/service-addons`
- 目前使用硬編碼清單，等待 ServiceAddon 資料表實作
- 支援：造型加價、貴賓腳、除蚤處理、指甲彩繪、香水、SPA護理

## 專案結構

### 後端結構
```
PetSalon.Backend/
├── PetSalon.Web/           # Web API 控制器和啟動
├── PetSalon.Services/      # 商業邏輯層
├── PetSalon.Models/        # 資料模型和 DTOs
└── PetSalon.Tools/         # 工具類別
```

### 前端結構
```
PetSalon.Frontend/
├── src/
│   ├── views/              # 頁面元件
│   ├── stores/             # Pinia 狀態管理
│   ├── api/                # API 服務層
│   ├── types/              # TypeScript 型別定義
│   ├── utils/              # 工具函式
│   └── router/             # Vue Router 設定
├── index.html              # 入口 HTML
└── vite.config.ts          # Vite 設定
```

## 使用程式碼庫

### 後端開發
1. **服務層** (`PetSalon.Services`) 包含具有清晰介面的商業邏輯
2. **控制器** (`PetSalon.Web/Controllers`) 處理 HTTP 請求並進行適當的錯誤處理
3. **模型** 是自動產生的 - 修改資料庫架構以進行變更
4. **DTOs** (`PetSalon.Models/DTOs`) 用於 API 資料傳輸和計算
5. **SystemCodes** 提供可設定的查找資料
6. 使用相依性注入來處理服務相依性
7. 遵循既有的稽核欄位和錯誤處理模式
8. 所有服務支援非同步操作
9. 檔案上傳使用適當的驗證和安全儲存

### 前端開發

1. **Vue 3 Composition API** 搭配 TypeScript 確保型別安全
2. **PrimeVue** 用於一致的 UI 元件
3. **Pinia** 用於集中式狀態管理
4. **Axios** 搭配攔截器進行 API 通訊
5. **Vue Router** 搭配導航守衛進行驗證
6. **Auto-import** 用於 Vue API 和 PrimeVue 元件
7. 遵循 Vue 3 最佳實務和 Composition 模式
- 總是使用「use context7」來尋找 Vue 3 Composition API

# PetSalon AI 規則

PetSalon 專案開發指南和程式碼標準。

## 前端 (FRONTEND)

### VUE 指南

#### VUE_CODING_STANDARDS (VUE 編碼標準)

- 使用 Composition API 代替 Options API 以獲得更好的型別推斷和程式碼重複使用
- 實作 <script setup> 以獲得更簡潔的元件定義
- 使用 Suspense 和 async components 處理程式碼分割期間的載入狀態
- 利用 defineProps 和 defineEmits 巨集獲得型別安全的 props 和 events
- 使用新的 defineOptions 獲得額外的元件選項
- 在深度巢狀元件中實作 provide/inject 進行相依性注入，代替 prop drilling
- 使用 Teleport 元件獲得類似 portal 的功能，將 UI 渲染到 DOM 的其他位置
- 利用 ref 勝過 reactive 處理原始值，以避免意外的 unwrapping
- 在渲染繁重的列表渲染場景中使用 v-memo 進行效能最佳化
- 對於不需要深度響應的大型物件實作 shallow refs

#### NUXT

- 使用 Nuxt 3 搭配 Composition API 和 <script setup> 進行現代應用程式開發
- 利用 Vue 和 Nuxt composables 的 auto-imports 減少樣板程式碼
- 使用 server directory 實作 server routes 以提供 API 功能
- 儘可能使用 Nuxt 模組擴充功能，而非自訂外掛程式
- 利用 useAsyncData 和 useFetch composables 進行支援 SSR 的資料擷取
- 實作 middleware (defineNuxtRouteMiddleware) 用於導航守衛
- 使用 Nuxt layouts 在 routes 之間保持一致的頁面佈局
- 利用 Nitro 進行伺服器端渲染和 API routes
- 實作 Nuxt plugins 進行全域功能註冊
- 對於簡單狀態使用 useState，對於複雜應用程式使用 Pinia 進行狀態管理

#### VUEX

- 在 Vue 3 專案中轉移到 Pinia 而非 Vuex，因為它提供更好的 TypeScript 支援
- 如果使用 Vuex，實作 modules pattern 來組織相關的 state、getters、mutations 和 actions
- 使用 namespaced modules 以避免大型應用程式中的命名衝突
- 利用 plugins 處理橫切關注點，如持久性或分析
- 避免在 mutations 之外直接變更 state，以保持可預測的狀態變更
- 使用 mapState、mapGetters 和 mapActions helper 來簡化元件程式碼
- 使用 composition API 搭配 useStore 來實作 Vuex，以獲得更好的 TypeScript 支援
- 使用 actions 進行非同步操作，使用 mutations 進行同步狀態變更
- 利用 getters 處理計算狀態，以避免多餘的計算
- 在 actions 中使用 try/catch 區塊實作適當的錯誤處理

#### VUE_ROUTER

- 使用路由守衛 (beforeEach, beforeEnter) 進行身分驗證和授權檢查
- 對路由元件實作 lazy loading 和 dynamic imports 以改善效能
- 使用具名路由 (named routes) 而非硬編碼路徑，以提高可維護性
- 利用路由 meta 欄位儲存額外的路由資訊，如權限或佈局資料
- 實作 scroll behavior 選項來控制路由導航之間的滾動
- 使用 navigation duplicates 處理機制來防止對目前路由的多餘導航
- 實作 composition API useRouter 和 useRoute hooks 代替 this.$router
- 對於具有父子關係的複雜 UI 使用嵌套路由 (nested routes)
- 利用 sensitive: true 驗證路由參數，針對不應記錄的參數
- 實作包含路徑參數和 regex 模式的動態路由匹配，以獲得靈活的路由

#### PINIA

- 根據邏輯領域建立多個 store，而非單一大型 store
- 使用 setup 語法 (以 setup 函式定義 store) 定義 store，以獲得更好的 TypeScript 推斷
- 實作 getters 用於衍生狀態，以避免多餘的計算
- 利用 storeToRefs helper 提取響應式屬性，同時保持響應性
- 使用 plugins 處理橫切關注點，如持久性、狀態重置或開發工具
- 實作 actions 進行非同步操作和複雜的狀態變更
- 使用可組合 store (composable stores)，透過在其他 store 中匯入和使用 store
- 視需要利用 $reset() 方法恢復初始狀態
- 實作 $subscribe 進行響應式 store 訂閱
- 使用具有適當回傳型別註解的 TypeScript，以獲得最大的型別安全性


## 後端 (BACKEND)

### DOTNET 指南

#### ENTITY_FRAMEWORK

- 使用 Repository 和 Unit of Work 模式抽象化資料存取邏輯並簡化測試
- 實作 eager loading with Include() 避免 N+1 查詢問題
- 使用 Migrations 進行資料庫架構變更和版本控制，並使用適當的命名慣例
- 應用適當的追蹤行為（唯讀查詢使用 AsNoTracking()）以最佳化效能
- 實作查詢最佳化技術，如針對頻繁執行的資料庫操作使用編譯查詢
- 使用值轉換 (value conversions) 處理複雜屬性轉換

#### ASP_NET

- 在 .NET 6+ 應用程式中使用 minimal APIs 處理簡單端點，減少樣板程式碼
- 實作 Mediator 模式 (MediatR) 以解耦請求處理並簡化橫切關注點
- 使用 API 控制器搭配模型繫結和驗證屬性
- 應用適當的回應快取 (response caching) 搭配快取設定檔和 ETags 以改善效能
- 實作適當的例外處理 (ExceptionFilter 或 middleware) 以提供一致的錯誤回應
- 使用相依性注入，請求特定服務使用 scoped lifetime，無狀態服務使用 singleton

#### AUDIT_FIELDS_IMPLEMENTATION (稽核欄位實作)

**使用 SaveChangesInterceptor 自動處理稽核欄位（推薦 .NET 6+）**

Entity Framework Core SaveChangesInterceptor 提供最現代的方法，在儲存操作期間自動填入稽核欄位（CreatedBy、ModifiedBy、CreatedDate、ModifiedDate）：

```csharp
public class AuditingSaveChangesInterceptor : SaveChangesInterceptor
{
    private readonly IUserContext _userContext;

    public AuditingSaveChangesInterceptor(IUserContext userContext)
    {
        _userContext = userContext;
    }

    public override InterceptionResult<int> SavingChanges(DbContextEventData eventData, InterceptionResult<int> result)
    {
        var dbContext = eventData.Context;
        var currentUser = _userContext.CurrentUserId?.ToString() ?? "System";

        foreach (var entry in dbContext.ChangeTracker.Entries()
            .Where(e => e.State == EntityState.Added || e.State == EntityState.Modified))
        {
            if (entry.Entity is IAuditableEntity auditable)
            {
                if (entry.State == EntityState.Added)
                {
                    auditable.CreatedDate = DateTime.UtcNow;
                    auditable.CreatedBy = currentUser;
                }
                else if (entry.State == EntityState.Modified)
                {
                    auditable.ModifiedDate = DateTime.UtcNow;
                    auditable.ModifiedBy = currentUser;

                    // Prevent CreatedBy and CreatedDate from being overwritten
                    entry.Property("CreatedDate").IsModified = false;
                    entry.Property("CreatedBy").IsModified = false;
                }
            }
        }
        return base.SavingChanges(eventData, result);
    }

    public override async ValueTask<InterceptionResult<int>> SavingChangesAsync(
        DbContextEventData eventData, InterceptionResult<int> result, CancellationToken cancellationToken = default)
    {
        SavingChanges(eventData, result);
        return await base.SavingChangesAsync(eventData, result, cancellationToken);
    }
}
```

**Auditable Entity Pattern (可稽核實體模式)**

為需要稽核追蹤的實體建立基底介面和抽象類別：

```csharp
public interface IAuditableEntity
{
    string? CreatedBy { get; set; }
    DateTime CreatedDate { get; set; }
    string? ModifiedBy { get; set; }
    DateTime? ModifiedDate { get; set; }
}

public abstract class AuditableEntity : IAuditableEntity
{
    public virtual string? CreatedBy { get; set; }
    public virtual DateTime CreatedDate { get; set; }
    public virtual string? ModifiedBy { get; set; }
    public virtual DateTime? ModifiedDate { get; set; }
}
```

**User Context Service for JWT Claims (JWT Claims 的使用者上下文服務)**

實作 IUserContext 以從 JWT Token 提取目前使用者資訊：

```csharp
public interface IUserContext
{
    long? CurrentUserId { get; }
    string? CurrentUserName { get; }
}

public class JwtUserContext : IUserContext
{
    private readonly IHttpContextAccessor _httpContextAccessor;

    public JwtUserContext(IHttpContextAccessor httpContextAccessor)
    {
        _httpContextAccessor = httpContextAccessor;
    }

    public long? CurrentUserId =>
        long.TryParse(_httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId)
            ? userId : null;

    public string? CurrentUserName =>
        _httpContextAccessor.HttpContext?.User.FindFirstValue(ClaimTypes.Name);
}
```


**重要安全注意事項：**
- 限制 IHttpContextAccessor 的使用，以避免在高流量場景中的效能問題
- 注意在請求生命週期之外存取 HttpContext，以防止跨請求資料洩漏
- 使用基於宣告 (claims-based) 的授權進行細粒度存取控制，而不是硬編碼角色
- 實作適當的 Token 驗證，使用強大的密鑰和適當的演算法 (HMAC-SHA256/SHA512)


## 資料庫 (DATABASE)

### SQL 指南

#### SQLSERVER

- 使用參數化查詢以防止 SQL 注入
- 根據查詢模式實作適當的索引策略
- 對於需要資料庫存取的複雜商業邏輯使用預存程序
