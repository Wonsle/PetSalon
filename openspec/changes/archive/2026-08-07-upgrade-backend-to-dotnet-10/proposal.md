## Why

後端目前以 net8.0 與 8.x 版 Microsoft 套件建置，但開發環境已安裝 .NET 10 SDK。將整個後端統一升級至 .NET 10，可排除 SDK/目標框架不一致，並讓後續開發使用受支援的 .NET 10 工具鏈。

## What Changes

- 將方案內所有後端與測試專案的目標框架由 net8.0 更新為 net10.0。
- 將 Microsoft.EntityFrameworkCore、ASP.NET Core 驗證、MVC NewtonsoftJson 與 Microsoft.Extensions 套件升級至相容的 10.x 版本。
- 新增 global.json，固定使用已安裝的 .NET 10 SDK 並允許相容的最新修補版本。
- 驗證 NuGet 還原、完整方案建置與既有測試；必要時修正 .NET 10 的編譯或執行階段相容性問題。
- 不變更資料庫結構、公開 API 契約或業務行為。

## Capabilities

### New Capabilities

- `dotnet-10-backend-toolchain`: 規範後端方案必須以 .NET 10 SDK 還原、建置與執行，且所有第一方專案及 Microsoft 平台套件版本一致。

### Modified Capabilities

（無）

## Impact

- Affected specs: dotnet-10-backend-toolchain
- Affected code:
  - Modified: PetSalon.Backend/PetSalon.Models/PetSalon.Models.csproj
  - Modified: PetSalon.Backend/PetSalon.Service/PetSalon.Services.csproj
  - Modified: PetSalon.Backend/PetSalon.Tools/PetSalon.Tools.csproj
  - Modified: PetSalon.Backend/PetSalon.Web/PetSalon.Web.csproj
  - Modified: PetSalon.Backend/PetSalon.Web.Tests/PetSalon.Web.Tests.csproj
  - New: PetSalon.Backend/global.json
- Dependencies: Microsoft .NET 10 SDK、ASP.NET Core 10、Entity Framework Core 10 與 Microsoft.Extensions 10
- Systems: 後端本機開發、CI 建置與部署執行環境必須提供 .NET 10
