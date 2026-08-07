## Context

PetSalon.Backend 方案包含互相參考的後端與測試專案，並直接引用 8.x 版 Entity Framework Core、ASP.NET Core 與 Microsoft.Extensions 套件。目前本機只有 .NET 10.0.302 SDK 與 10.0 runtime；升級必須維持所有專案與 Microsoft 平台套件同一主版本，避免資產解析與執行階段不一致。

## Goals / Non-Goals

**Goals:**

- 方案內所有第一方後端與測試專案統一以 net10.0 為目標框架。
- Microsoft 平台套件統一升級至可由 .NET 10 SDK 還原的 10.0.x 穩定版本。
- 方案根目錄固定 .NET 10 SDK 選擇規則，並通過 restore、build 與現有測試。
- 保持既有 API、JWT 驗證、EF Core 模型與資料庫結構的行為。

**Non-Goals:**

- 不新增或重建 EF Core migration。
- 不修改前端、API 路由、DTO、資料表或業務邏輯。
- 不在此 change 處理既有編譯警告，除非警告是升級造成且會阻擋 .NET 10 建置。
- 不移除 .NET 10 以外已安裝的 SDK。

## Decisions

### 統一採用 net10.0 目標框架

方案內所有專案同時改為 net10.0，避免跨目標框架專案參考造成不必要的相容性矩陣。替代方案是僅升級 Web 專案，但這會保留混合目標框架並使套件版本管理更複雜。

### Microsoft 平台套件採用 10.0.x 同版系

所有 Microsoft.EntityFrameworkCore、Microsoft.AspNetCore 與 Microsoft.Extensions 直接相依套件使用 10.0.x，且 EF Core runtime、design 與 tools 套件使用相同版本。Swashbuckle.AspNetCore 僅在 NuGet 相容性或編譯要求下升級，因其不跟隨 .NET 主版本。替代方案是保留 8.x 套件，但無法達成完整工具鏈升級且增加執行期不一致風險。

### 使用 global.json 固定 SDK feature band

在 PetSalon.Backend 建立 global.json，指定 10.0.302 並使用 latestPatch roll-forward，使本機與 CI 選擇 10.0.3xx 的最新安全修補版本，而不靜默跨 feature band。替代方案是不固定 SDK，但不同機器可能使用不同 SDK 而產生不可重現結果。

### 以還原、建置及測試作為升級閘門

依序執行 dotnet restore、dotnet build --no-restore 與方案內可發現的 dotnet test --no-build。若沒有測試專案，必須明確記錄此事，並以 Web 專案啟動前的建置成功作為最低驗證。不得產生資料庫 migration 作為驗證副作用。

## Implementation Contract

- **Observable behavior:** 從 PetSalon.Backend 執行 dotnet --version 必須解析至 10.0.3xx；dotnet restore 與 dotnet build PetSalon.sln 必須成功，所有第一方組件的 TargetFramework 必須為 net10.0。
- **Project and dependency shape:** 方案內所有 csproj 的 TargetFramework 必須是 net10.0。Microsoft.EntityFrameworkCore runtime、design、tools，以及 Microsoft.AspNetCore.Authentication.JwtBearer、Microsoft.AspNetCore.Mvc.NewtonsoftJson、Microsoft.Extensions.Configuration 與 Json 套件必須使用相容的 10.0.x 穩定版本。
- **Failure modes:** 若套件不存在、不支援 net10.0 或程式碼因 breaking change 無法編譯，實作必須修正直接相依版本或最小相容性程式碼；不得以忽略 restore/build 錯誤、降回 net8.0 或新增資料庫 migration 規避失敗。
- **Acceptance criteria:** global.json 可被 SDK 正確解析；所有方案專案均還原成功；PetSalon.sln 以零 error 建置；所有可發現測試通過；搜尋專案檔不再出現 net8.0 或 Microsoft 平台 8.x PackageReference。
- **In scope:** SDK 選擇、目標框架、直接 NuGet 相依、由 .NET 10 breaking changes 所需的最小原始碼調整與建置驗證。
- **Out of scope:** API/DTO/資料庫契約改版、功能重構、前端升級、部署基礎設施重設計與非升級造成的警告清理。

## Risks / Trade-offs

- [Risk] NuGet 套件的 10.0.x 版本可能需要受限網路存取 → 使用已設定的 NuGet feeds 還原，若網路受限則明確回報而不修改來源。
- [Risk] EF Core 10 可能出現查詢或模型 breaking changes → 以零 schema migration 為邊界，只做通過編譯與既有測試所需的最小調整。
- [Risk] CI 或部署主機尚未安裝 .NET 10 → global.json 與 net10.0 會快速失敗並顯示缺少 SDK，部署環境升級另行處理。
- [Trade-off] latestPatch 提升安全修補一致性，但要求環境具備同一 10.0.3xx feature band。
