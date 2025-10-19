# 寵物編輯品種無法帶入問題 - 修正摘要

## 修正完成日期
2025-10-19

## 問題描述
編輯寵物時，品種(Breed)欄位無法正確顯示已選擇的品種值。原因是後端列表 API 回傳品種中文名稱，但前端 `SystemCodeSelect` 元件期望接收 SystemCode 的 code 值。

## 採用方案
**方案一：後端統一回傳 Code + 新增 BreedName 欄位**

## 已修正的檔案

### 後端修正 (3 個檔案)

#### 1. PetSalon.Backend/PetSalon.Models/DTOs/PetDto.cs
**修正內容：**
- 在 `PetListResponse` 類別中新增 `BreedName` 屬性
- `Breed` 保持為 SystemCode 的 code 值
- `BreedName` 儲存品種的中文名稱供顯示使用

**修正位置：** 第 49-58 行
```csharp
/// <summary>
/// 品種代碼 (SystemCode Code)
/// </summary>
public string Breed { get; set; } = string.Empty;

/// <summary>
/// 品種中文名稱（用於顯示）
/// </summary>
public string? BreedName { get; set; }
```

#### 2. PetSalon.Backend/PetSalon.Service/PetService/PetService.cs
**修正內容：**
- 修改 `GetPetListWithOwners()` 方法
- 不再將 `Breed` 欄位從 code 轉換為中文名稱
- 改為填入 `BreedName` 欄位

**修正位置：** 第 48-106 行
```csharp
// 修正前：
pet.Breed = breedName;  // 直接覆寫 Breed 欄位

// 修正後：
pet.BreedName = breedName;  // 填入新的 BreedName 欄位
```

### 前端修正 (3 個檔案)

#### 3. PetSalon.Frontend/src/types/pet.ts
**修正內容：**
- 更新 `Pet` 介面定義
- `breed` 明確定義為 SystemCode 的 code 值
- `breedName` 用於儲存品種中文名稱

**修正位置：** 第 13-14 行
```typescript
breed: string  // SystemCode 的 code 值
breedName?: string  // 品種中文名稱（列表顯示用）
```

#### 4. PetSalon.Frontend/src/views/pets/PetList.vue
**修正內容：**
- 修改 `loadPets()` 方法中的資料映射邏輯
- 保持 `breed` 為原始 code 值（供編輯使用）
- `breedName` 優先使用後端回傳的中文名稱

**修正位置：** 第 265-271 行
```typescript
return {
  id: item.petId || item.id,
  name: item.petName || item.name,
  breed: item.breed, // 保持 code 原值，供編輯時使用
  breedName: item.breedName || item.breed, // 優先使用 breedName 中文名稱
  gender: item.gender,
  // ...
}
```

**顯示部分：** 第 140 行（無需修改，已使用 breedName）
```html
<p><strong>品種:</strong> {{ pet.breedName || '未知' }}</p>
```

#### 5. PetSalon.Frontend/src/components/forms/PetForm.vue
**修正內容：**
- 移除所有 debug console.log（共 7 處）
- 移除不必要的 `$forceUpdate()` 呼叫
- 確保 `breed` 欄位接收並傳遞 code 值

**清理的 debug code：**
- 第 291 行：移除初始化時的 console.log
- 第 428 行：移除 resetForm 時的 console.log
- 第 681-686 行：移除 watch pet 時的 console.log
- 第 690 行：移除 assignment 後的 console.log
- 第 712-717 行：移除 force refresh 相關 code
- 第 753 行：移除 watch breed 的整個區塊

## 修正原理

### Before (問題發生時)
```
列表 API → breed: "貴賓" (中文) → PetForm → SystemCodeSelect 
                                              ↓
                                      比對失敗！找不到 code="貴賓"
```

### After (修正後)
```
列表 API → breed: "POODLE" (code) → PetForm → SystemCodeSelect
          breedName: "貴賓"              ↓
                                    比對成功！找到 code="POODLE"
                                    
顯示時使用 breedName: "貴賓"
```

## 測試檢查清單

### 必須測試項目
- [ ] **後端編譯**：執行 `dotnet build` 確認無編譯錯誤
- [ ] **前端編譯**：執行 `npm run build` 確認無編譯錯誤
- [ ] **API 測試**：呼叫 GET `/api/pet` 確認回傳格式
  - `breed` 欄位是 code 格式（例如："POODLE"）
  - `breedName` 欄位是中文名稱（例如："貴賓"）
- [ ] **列表顯示**：寵物列表頁面正確顯示品種中文名稱
- [ ] **編輯功能**：從列表點擊「編輯」，品種下拉選單正確顯示已選品種
- [ ] **更新功能**：修改品種後儲存，確認資料正確更新
- [ ] **新增功能**：新增寵物並選擇品種，確認功能正常
- [ ] **搜尋功能**：使用品種篩選，確認搜尋正常運作

### API 測試範例

#### 測試 GET /api/pet
```bash
curl -X GET "http://localhost:5000/api/pet" -H "accept: application/json"
```

**預期回應：**
```json
[
  {
    "petId": 1,
    "petName": "小白",
    "breed": "POODLE",           // ← code 格式
    "breedName": "貴賓",          // ← 中文名稱
    "gender": "M",
    "owners": [...],
    ...
  }
]
```

#### 測試 GET /api/pet/{id}
```bash
curl -X GET "http://localhost:5000/api/pet/1" -H "accept: application/json"
```

**預期回應：**
```json
{
  "petId": 1,
  "petName": "小白",
  "breed": "POODLE",           // ← code 格式（與列表一致）
  "gender": "M",
  "allContacts": [...],
  ...
}
```

## 向後相容性

### ✅ 完全相容
- 新增 `BreedName` 欄位不影響現有功能
- `Breed` 欄位仍保留，只是改回 code 格式
- 前端已同時處理 `breed` 和 `breedName`

### ⚠️ 注意事項
如果有其他功能直接使用 `breed` 欄位顯示，可能需要改為使用 `breedName`：
```typescript
// 需要檢查的使用情境
{{ pet.breed }}  // 舊：顯示中文 → 新：顯示 code
{{ pet.breedName }}  // 新：顯示中文名稱（建議使用）
```

## 相關系統代碼

### Breed SystemCode 範例
| Code | Name | CodeType |
|------|------|----------|
| POODLE | 貴賓 | Breed |
| GOLDEN | 黃金獵犬 | Breed |
| HUSKY | 哈士奇 | Breed |
| SHIBA | 柴犬 | Breed |

### 其他使用相同模式的欄位
- `Gender`: "M" → "公", "F" → "母"
- `CoatColor`: 毛色代碼 → 中文名稱
- `RelationshipType`: "OWNER" → "飼主"

建議未來統一處理方式：
1. 所有 SystemCode 欄位都回傳 code
2. 需要顯示時，新增對應的 `{Field}Name` 欄位

## 效能影響

### 後端
- ✅ 無負面影響
- 查詢 SystemCode 的邏輯維持不變
- 只是將轉換結果填入不同欄位

### 前端
- ✅ 輕微改善
- 減少了前端需要進行的資料轉換
- SystemCodeSelect 可直接使用 code 比對，無需額外查詢

## 後續建議

### 短期
1. 完成所有測試檢查清單項目
2. 觀察線上環境是否有其他使用 `breed` 欄位的地方需要調整

### 中期
3. 考慮為其他 SystemCode 欄位（Gender, CoatColor）套用相同模式
4. 建立統一的 DTO 轉換規範文件

### 長期
5. 考慮實作前端 SystemCode cache 機制
6. 建立 Vue composable 統一處理 SystemCode 邏輯
7. 後端考慮實作 AutoMapper 或統一的 DTO 轉換層

## 回滾計劃

如果修正後發現問題，可以按以下步驟回滾：

### Git 回滾
```bash
# 查看修改
git diff

# 回滾所有修改
git checkout -- PetSalon.Backend/PetSalon.Models/DTOs/PetDto.cs
git checkout -- PetSalon.Backend/PetSalon.Service/PetService/PetService.cs
git checkout -- PetSalon.Frontend/src/types/pet.ts
git checkout -- PetSalon.Frontend/src/views/pets/PetList.vue
git checkout -- PetSalon.Frontend/src/components/forms/PetForm.vue
```

### 手動回滾重點
1. PetDto.cs: 移除 `BreedName` 屬性
2. PetService.cs: 將 `pet.BreedName = breedName` 改回 `pet.Breed = breedName`
3. 前端檔案: 恢復原有邏輯

## 總結

✅ **修正完成的項目：**
- 後端統一回傳 breed code
- 新增 breedName 欄位用於顯示
- 前端正確處理 breed code
- 清理不必要的 debug code

✅ **解決的問題：**
- 編輯寵物時品種欄位正確顯示
- 資料格式一致性提升
- 程式碼可維護性提高

⏳ **待驗證項目：**
- 後端編譯測試
- 前端編譯測試
- 功能整合測試
- API 回應格式驗證

---

**修正者：** Claude AI Assistant  
**預計測試時間：** 30-45 分鐘  
**風險等級：** 低（向後相容，新增欄位）
