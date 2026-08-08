## Context

ASP.NET Core 已註冊 JWT 驗證，但 Controllers 原先沒有 fallback policy；登入以明文測試字典比對，JWT 固定加入 Admin。實作途中已完成資料庫帳密、首次改密碼與 authenticated fallback policy。後續授權不得在 Controller 寫死 Admin、Manager 或 Designer，而要由 SCUser、UserRole、SCRole、RolePermission 與 SCPermission 決定登入資格和端點權限。appsettings.json 原先另含資料庫密碼與 JWT SignKey。本變更橫跨資料、API、前端與部署設定。

## Goals / Non-Goals

**Goals:**

- 除明確白名單外，所有 API 預設驗證 JWT。
- 由 SCUser 與角色權限關聯驗證身分、登入資格並產生 permission Claims。
- Controller 只宣告穩定權限代碼，角色與權限對應完全由資料庫管理。
- 首次安裝冪等建立 admin/password，只存雜湊並強制首次改密碼。
- Secret 移出版本控制，啟動時驗證。
- 用自動化測試驗證登入、授權、初始化與改密碼。

**Non-Goals:**

- 不導入 OAuth、OIDC、MFA 或完整 ASP.NET Core Identity UI。
- 不實作忘記密碼、帳號鎖定或使用者管理 CRUD。
- 不在本變更建立角色權限管理前端；後端僅提供列出權限及以整組取代方式更新角色權限的 API。
- 不自動重寫 Git 歷史；舊憑證由維運者輪替。

## Decisions

### 使用 EF Core 帳號服務與 PasswordHasher 儲存密碼

IAuthService 封裝 SCUser 與角色查詢，使用 Microsoft.AspNetCore.Identity.PasswordHasher<Scuser> 的版本化 PBKDF2 salted hash。採 shared framework 而不導入完整 Identity schema；排除自行 SHA 或固定 salt。

### 以唯一索引與啟動初始化器建立預設管理者

Migration 將 SCUser.UserName 設為非空唯一、加入 MustChangePassword 與 IsActive，並映射 ScuserRole。啟動初始化器在 migration 後以交易檢查 admin：不存在才建立 Admin 角色、admin 與關聯；已存在時不改任何帳號狀態。啟動階段才能用 PasswordHasher 產生不同 salt，故不在 migration 寫固定 hash。

### 首次登入只簽發限用途改密碼 Token

MustChangePassword=true 時回傳 RequiresPasswordChange=true 與短效 token_use=password_change JWT，不加入業務角色；該 Token 只能呼叫改密碼端點。成功後清除狀態、清除前端 Token並要求用新密碼重登。一般 Admin Token 不會在預設密碼仍有效時簽發。

### 使用資料庫角色權限與穩定權限代碼保護 API

AddAuthorization 設 authenticated fallback policy，login 明確 AllowAnonymous。新增 SCPermission 與 RolePermission；PermissionCode 非空唯一，RolePermission 對 RoleID/PermissionID 有外鍵與唯一複合索引。Controller 使用 RequirePermission 宣告 `system.settings.manage`、`accounts.manage`、`permissions.manage`、`files.delete.permanent` 或 `finance.read`，不得出現角色名稱。自訂 authorization handler 比對 JWT 的 `permission` claims；PasswordChangeOnly 仍只驗證限用途 Claim。前端權限只改善 UI，不取代後端政策。

預設權限種子包含 `auth.login`、`system.settings.manage`、`accounts.manage`、`permissions.manage`、`files.delete.permanent` 與 `finance.read`。全新安裝的 Admin 角色取得全部預設權限；既有環境第一次建立權限結構時，Admin 取得全部、Manager 取得 `auth.login` 與 `finance.read`、Designer 取得 `auth.login`，之後啟動不得覆寫管理者調整。角色名稱只允許出現在資料種子，不得出現在 Controller 授權條件。

### 以 auth.login 權限決定一般登入資格

IAuthService 在帳密驗證成功後聯集使用者所有啟用角色的啟用權限。MustChangePassword=true 時仍可取得限用途改密碼 Token；一般 Token 只有在權限聯集包含 `auth.login` 時才簽發，否則回 403。一般 JWT 包含去重後的 `permission` claims；角色 claims 可保留作顯示資訊，但 authorization handler 不得使用角色 claims。角色權限修改在下一次登入簽發新 Token 時生效。

### 以 permissions.manage 保護角色權限管理

Permission API 可列出權限與角色目前對應，並以單一交易整組取代指定角色的 PermissionCode 集合。請求包含未知、停用或重複 PermissionCode 時回 400 且不改資料；呼叫者必須具有 `permissions.manage`。此 API 不以 Admin 角色名稱判斷，避免新增角色後還需改 Controller。

### 以標準設定來源注入 Secret 並啟動失敗

appsettings 只保留非敏感值；ConnectionStrings__DefaultConnection 與 JwtSettings__SignKey 由 User Secrets、環境變數或平台 Secret 提供。空值、文件 placeholder 或少於 32 bytes 的 SignKey 使應用在接受請求前終止，錯誤只列設定鍵。

### 以一致錯誤契約區分驗證與授權失敗

不存在帳號、停用與錯誤密碼一律回 401；缺少/無效 Token 回 401；缺少 `auth.login`、端點權限或 Token 用途不足回 403；密碼規則錯誤回 400。回應與 log 不含密碼、hash、Token、Secret 或內部例外。

## Implementation Contract

**Behavior:** Fresh database 啟動後唯一存在啟用的 admin、Admin 關聯與預設權限映射，password 僅能驗證 salted hash。既有 admin 在重啟時不得被覆寫。預設 admin 只取得改密碼 Token，改密碼並重新登入後才取得包含 `auth.login` 與管理權限的 JWT。停用帳號或缺少 `auth.login` 權限的帳號不得取得一般 JWT。Controller 不含角色名稱，角色權限異動後使用者重新登入即可採用新權限。

**Interfaces:** LoginRequest 維持 UserName/Password；LoginResponse 新增 RequiresPasswordChange，UserInfo 新增 Permissions 字串陣列。ChangePasswordRequest 包含 CurrentPassword/NewPassword/ConfirmPassword。新密碼至少 12 字元且不得等於 password 或 UserName。SCUser 新增 MustChangePassword bit default false、IsActive bit default true；UserName 非空唯一；UserRole 與 RolePermission 都有外鍵與唯一複合索引；SCPermission.PermissionCode 非空唯一。RequirePermission 接受一個 PermissionCode，不接受角色名稱。角色權限更新 API 接受 RoleID 與完整 PermissionCode 陣列。

**Failures:** 初始化競爭由唯一索引與重讀收斂；不完整或重複的帳號、角色、權限資料使 migration 失敗。密碼 hash 更新與 MustChangePassword 清除同一交易，角色權限整組取代也在單一交易，任一失敗全數 rollback。Secret 錯誤不得記錄值。

**Acceptance:** integration tests 覆蓋匿名 401、缺少權限 403、具備權限成功、缺少 `auth.login` 登入 403、Controller 無角色字串、角色權限更新後重新登入取得新 claims、錯誤登入 401、初始 admin 限用途 Token、改密碼後重登、初始化冪等與 Secret 缺失。前端測試確認 RequiresPasswordChange 導頁且不建立一般登入狀態。

**Scope:** In scope 為認證授權、資料庫權限結構、角色權限查詢與更新 API、Secret、migration、預設 admin、首次改密碼與測試文件；out of scope 為第三方登入、MFA、忘記密碼、鎖定、使用者 CRUD、角色 CRUD、角色權限管理前端與 Git 歷史清理。

## Risks / Trade-offs

- [Risk] admin/password 公開且弱 → 只允許取得限用途 Token並強制改密碼。
- [Risk] fallback policy 或權限代碼漏盤使端點突然 401/403 → 建立端點權限清單、反射測試與 integration tests。
- [Risk] permission claims 在 Token 有效期間不會即時反映資料庫異動 → 權限管理完成後要求受影響使用者重新登入，後續可另案導入 security stamp 或短效 Token。
- [Risk] 多 instance 初始化競爭 → 唯一索引、交易及競爭後重讀。
- [Risk] 移除 appsettings Secret 後舊環境無法啟動 → 合併前提供注入與輪替說明並驗證 staging。
- [Trade-off] 不使用完整 Identity 可降低改動，但重設、鎖定與 security stamp 留待後續。

## Migration Plan

1. 先輪替已提交的資料庫密碼與 JWT SignKey，配置新 Secret。
2. 部署 migration 前處理重複 UserName、PermissionCode、角色與權限關聯，再加入欄位與約束。
3. 部署後端，由初始化器視需要建立 admin、預設權限及首次角色權限映射。
4. 部署前端，以 admin/password 完成首次改密碼。
5. 執行登入、授權與核心業務 smoke tests。
6. 回滾保留新增欄位與帳號；不得回復舊 Secret。

## Open Questions

無；預設角色只用於首次資料種子，Controller 與 authorization handler 均不依賴角色名稱。
