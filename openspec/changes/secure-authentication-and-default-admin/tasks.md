## 1. 測試骨架與資料模型

- [x] 1.1 建立 PetSalon.Web.Tests 並讓 AuthenticationTests、AuthorizationTests 可被 dotnet test PetSalon.Backend/PetSalon.sln --list-tests 發現；以清單包含兩組測試驗證。
- [x] 1.2 實作「使用 EF Core 帳號服務與 PasswordHasher 儲存密碼」的 SCUser 非空唯一 UserName、MustChangePassword、IsActive 及 ScuserRole 外鍵/唯一關聯；以 migration script 與模型約束測試驗證。
- [x] 1.3 實作「以唯一索引與啟動初始化器建立預設管理者」及 Idempotent default administrator provisioning，使 fresh、repeated、concurrent startup 建立至多一個 admin/password salted hash 與 Admin 關聯且不覆寫既有帳號；以三種 initializer integration tests 驗證。
- [x] 1.4 實作「使用資料庫角色權限與穩定權限代碼保護 API」的 SCPermission、ScrolePermission、PermissionCode 唯一索引、外鍵及 RoleID/PermissionID 唯一關聯，並讓初始化器在權限矩陣尚未建立時種入六個 baseline permissions 與 Admin/Manager/Designer 初始映射且後續啟動不覆寫；以 migration script、fresh/existing/concurrent initializer tests 驗證。

## 2. Secret 設定安全

- [x] 2.1 實作「以標準設定來源注入 Secret 並啟動失敗」、Secrets outside version control 與 Startup secret validation，使 tracked settings 無可用憑證，缺值、placeholder 或短 SignKey 在監聽前失敗且不輸出值；以 startup tests 與 rg 掃描驗證。
- [x] 2.2 完成 Development and deployment setup documentation 與 Credential rotation boundary，文件明列 User Secrets、ConnectionStrings__DefaultConnection、JwtSettings__SignKey、容器注入與舊憑證輪替；以全新環境 smoke test 與內容審查驗證。

## 3. 資料庫登入與首次改密碼

- [x] 3.1 實作 Database-backed credential verification 與 Login audit state，使 IAuthService 使用 PasswordHasher<Scuser>，無效登入統一 401 且只有成功登入更新 LastLogin；以 valid/missing/inactive/wrong-password tests 驗證。
- [x] 3.2 實作 Database-backed role claims，使一般 JWT 僅含資料庫 user ID、normalized UserName 與全部角色，無角色帳號回 403；以 multiple-role/no-role claims tests 驗證。
- [x] 3.3 實作「首次登入只簽發限用途改密碼 Token」與 Mandatory first-login password change，使 MustChangePassword 帳號只取得 RequiresPasswordChange=true、空角色及短效 token_use=password_change；以 initial-admin 與 claims tests 驗證。
- [x] 3.4 實作 Password change validation and transaction，使 POST /api/Password/change 驗證目前密碼、確認值、12 字元下限及禁用 password/UserName，並原子更新 hash 與狀態後要求重登；以 success/weak/mismatch/rollback tests 驗證。
- [x] 3.5 實作「以 auth.login 權限決定一般登入資格」及 Database-backed permission claims and login eligibility，使 IAuthService 聯集並去重資料庫 permission、缺少 auth.login 回 403、一般 JWT 與 LoginResponse 僅採用資料庫解析的 Permissions；以 multi-role deduplication、missing-login-permission、mapping-change-next-login tests 驗證。

## 4. API 授權與錯誤契約

- [x] 4.1 實作「使用 fallback policy 與具名角色政策保護 API」、Authenticated API by default、Explicit anonymous endpoint allowlist 與 Password-change token isolation，使 login 為唯一匿名 Controller API、業務 API 要一般 JWT、改密碼只收限用途 Token；以 401/403/anonymous-login tests 驗證。
- [x] 4.2 完成 Database-backed permission authorization 端點盤點與 RequirePermission/PermissionAuthorizationHandler，使 Controller 不含 Admin、Manager、Designer，系統設定、帳號管理、權限管理、永久刪除及財務端點只宣告各自 PermissionCode；以 controller reflection scan、缺少 permission 403、具備 permission 200、角色名稱變更不影響授權 tests 驗證。
- [x] 4.3 實作「以 permissions.manage 保護角色權限管理」與 Transactional role permission management，提供權限清單及角色權限整組取代 API，未知、停用、重複 code 或持久化失敗均不改原映射；以 success/invalid/rollback integration tests 驗證。
- [x] 4.4 實作「以一致錯誤契約區分驗證與授權失敗」與 Authentication and authorization error semantics，使驗證失敗回 401、缺少 permission 或用途不足回 403，回應不含敏感值或內部例外；以 status 與 sensitive-string absence assertions 驗證。

## 5. 前端首次登入流程

- [x] 5.1 擴充 LoginResponse、auth API 與 Pinia，使 RequiresPasswordChange=true 時只保存限用途 Token、不建立一般 authenticated session，並導向具名 ChangePassword route；以 store unit tests 驗證狀態與導頁。
- [x] 5.2 建立 ChangePassword 畫面，顯示 current/new/confirm 驗證與 400/401，成功後清 Token 並回 Login；以 ChangePassword.spec.ts 覆蓋弱密碼、確認不符、成功與過期 Token。

## 6. 整合驗證與移交

- [x] 6.1 執行 migration、後端完整測試、前端 type-check/unit tests，並依 Migration Plan 演練 Secret 注入、重複部署、admin/password 限用途登入、改密碼、Admin 以 baseline permissions 重登、移除 auth.login 後拒絕登入、角色權限異動後重登與回滾；以零測試失敗、Controller 無角色名稱、資料庫無明文 password、既有 admin/權限矩陣未被覆寫及回滾 smoke test 驗證 Implementation Contract。
