## 1. SDK 與目標框架基線

- [x] 1.1 實作「使用 global.json 固定 SDK feature band」：在 PetSalon.Backend/global.json 指定 10.0.302 與 latestPatch，使 Deterministic .NET 10 SDK selection 成立；在 PetSalon.Backend 執行 dotnet --version 並確認輸出為 10.0.3xx。
- [x] 1.2 實作「統一採用 net10.0 目標框架」與 Unified .NET 10 target framework：方案內所有第一方後端與測試 csproj 均以 net10.0 建置且專案參考相容；以 rg 搜尋確認 csproj 不含 net8.0，並執行 dotnet restore PetSalon.sln 驗證。

## 2. 平台相依升級

- [x] 2.1 實作「Microsoft 平台套件採用 10.0.x 同版系」與 Compatible Microsoft platform dependencies：將所有直接 Microsoft.EntityFrameworkCore、Microsoft.AspNetCore 與 Microsoft.Extensions 套件更新至相容的 10.0.x 穩定版，EF Core 套件版本完全一致；以 dotnet list PetSalon.sln package 與 dotnet build PetSalon.sln --no-restore 驗證沒有 downgrade、framework compatibility warning 或 error。
- [x] 2.2 修正升級直接造成的最小程式碼相容性問題，同時滿足 Preserve backend contracts during toolchain migration：不得新增 migration、修改 model snapshot、HTTP route、DTO shape、JWT 行為或業務規則；以 git diff 檢查範圍並重新執行 dotnet build PetSalon.sln --no-restore 驗證零 error。

## 3. 升級驗證閘門

- [x] 3.1 執行「以還原、建置及測試作為升級閘門」及 Verify available automated tests：依序執行 dotnet restore PetSalon.sln、dotnet build PetSalon.sln --no-restore、dotnet test PetSalon.sln --no-build，所有可發現測試必須通過；若方案沒有測試專案，記錄 dotnet test 的明確結果。
- [x] 3.2 依 Implementation Contract 完成最終範圍審查：確認所有 csproj 為 net10.0、Microsoft 平台直接套件不含 8.x、global.json 可解析、沒有新增 EF migration 或公開契約變更，並以 rg、git diff --check 與完整 solution build 輸出作為驗證證據。
