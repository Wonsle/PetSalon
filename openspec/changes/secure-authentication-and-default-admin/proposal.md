## Why

目前系統雖啟用 JWT middleware，但業務 API 沒有強制授權、登入仍使用程式碼內的測試帳密，且資料庫密碼與 JWT SignKey 位於受版控設定檔，無法安全部署。

## What Changes

- 業務 API 預設要求有效 JWT；只有登入與明確白名單可匿名。
- 以資料庫 RolePermission/SCPermission 管理登入資格與端點權限；Controller 只宣告穩定權限代碼，不寫死 Admin、Manager 或 Designer。
- 將資料庫密碼與 JWT SignKey 移出受版控檔案，缺少必要 Secret 時拒絕啟動。
- 登入改查 SCUser、驗證不可逆密碼雜湊，並由 UserRole/SCRole/RolePermission/SCPermission 聯集解析登入資格與權限 Claims。
- 安裝時僅在 admin 不存在時建立 admin/password；只儲存 salted hash，且首次登入強制改密碼。
- 增加限用途改密碼 Token、變更密碼 API、前端導頁及自動化測試。

## Capabilities

### New Capabilities

- `api-authorization`: API 預設授權、匿名白名單、Token 用途、權限代碼政策與資料庫角色權限管理。
- `secure-configuration`: Secret 注入、啟動驗證、文件與憑證輪替要求。
- `database-user-authentication`: 資料庫登入、登入權限與權限 Claims、預設 admin 與首次改密碼。

### Modified Capabilities

（無；目前沒有既有能力規格。）

## Impact

- Affected specs: api-authorization, secure-configuration, database-user-authentication
- Affected code:
  - Modified: PetSalon.Backend/PetSalon.Web/Program.cs, PetSalon.Backend/PetSalon.Web/JWTHelpers.cs, PetSalon.Backend/PetSalon.Web/Controllers/AccountController.cs, PetSalon.Backend/PetSalon.Web/appsettings.json, PetSalon.Backend/PetSalon.Models/DTOs/AccountDto.cs, PetSalon.Backend/PetSalon.Models/EntityModels/Scuser.cs, PetSalon.Backend/PetSalon.Models/EntityModels/Scrole.cs, PetSalon.Backend/PetSalon.Models/EntityModels/PetSalonContext.cs, PetSalon.Frontend/src/api/auth.ts, PetSalon.Frontend/src/stores/auth.ts, PetSalon.Frontend/src/router/index.ts, PetSalon.Frontend/src/views/auth/Login.vue, .env.example, README.md
  - New: PetSalon.Backend/PetSalon.Models/EntityModels/ScuserRole.cs, PetSalon.Backend/PetSalon.Models/EntityModels/Scpermission.cs, PetSalon.Backend/PetSalon.Models/EntityModels/ScrolePermission.cs, PetSalon.Backend/PetSalon.Service/AuthService/IAuthService.cs, PetSalon.Backend/PetSalon.Service/AuthService/AuthService.cs, PetSalon.Backend/PetSalon.Models/Migrations/<timestamp>_SecureAuthenticationAndDefaultAdmin.cs, PetSalon.Backend/PetSalon.Web/Authorization/RequirePermissionAttribute.cs, PetSalon.Backend/PetSalon.Web/Authorization/PermissionAuthorizationHandler.cs, PetSalon.Backend/PetSalon.Web/Controllers/PasswordController.cs, PetSalon.Backend/PetSalon.Web/Controllers/PermissionController.cs, PetSalon.Backend/PetSalon.Web.Tests/PetSalon.Web.Tests.csproj, PetSalon.Frontend/src/views/auth/ChangePassword.vue
  - Removed: none
