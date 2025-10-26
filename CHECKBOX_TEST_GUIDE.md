# Checkbox顯示問題測試指南

## 問題診斷步驟

### 1. 啟動應用程序
```bash
cd PetSalon.Frontend
npm run dev
```
應用程序應該在 http://localhost:3001 啟動

### 2. 打開瀏覽器開發者工具
1. 訪問 http://localhost:3001
2. 按 F12 打開開發者工具
3. 切換到 Console 標籤，檢查是否有錯誤

### 3. 導航到預約表單
1. 在應用程序中找到「預約」或「Reservations」菜單
2. 點擊「新增預約」按鈕
3. 預約表單應該打開

### 4. 檢查Checkbox元素

#### 使用開發者工具 Elements 標籤:
1. 打開 Elements (元素) 標籤
2. 找到 `.service-checkbox-item` 類別的元素
3. 展開查看內部的 `.checkbox-container`
4. 檢查是否有 `<div class="p-checkbox">` 元素

#### 檢查項目:
- [ ] checkbox-container 元素存在
- [ ] p-checkbox 元素存在
- [ ] p-checkbox-box 元素存在
- [ ] checkbox有寬度和高度 (應該是 1.25rem)
- [ ] checkbox沒有 display: none 或 visibility: hidden

### 5. 檢查CSS樣式

在開發者工具的 Computed 標籤檢查checkbox的計算樣式：

**期望的樣式值：**
- `display`: inline-flex 或 flex
- `width`: 20px (1.25rem)
- `height`: 20px (1.25rem)
- `border`: 2px solid (某個顏色)
- `visibility`: visible

### 6. 如果Checkbox仍然不可見

可能的原因和解決方案：

#### 原因 1: Checkbox組件未正確導入
檢查 ReservationForm.vue 的 script 部分是否有 import Checkbox:
```typescript
import Checkbox from 'primevue/checkbox' // 應該有這行（如果使用手動導入）
```

**解決方案**: 確認 Checkbox 組件已在 main.ts 或 vite.config.ts 中全局註冊

#### 原因 2: CSS變數未定義
檢查 console 中是否有 CSS 變數錯誤

**解決方案**: 確認 PrimeVue 主題已正確載入

#### 原因 3: Flex佈局問題
檢查 `.service-checkbox-item` 的 flex 設置

**解決方案**:
```css
.service-checkbox-item {
  display: flex;
  gap: 1rem; /* 確保有間距 */
}

.checkbox-container {
  flex-shrink: 0; /* 防止被壓縮 */
  min-width: 1.5rem; /* 最小寬度 */
}
```

#### 原因 4: z-index問題
Checkbox可能被其他元素覆蓋

**解決方案**: 添加 z-index
```css
.checkbox-container {
  z-index: 1;
  position: relative;
}
```

### 7. 手動測試Checkbox功能

如果Checkbox可見：
1. 嘗試點擊checkbox
2. 檢查是否有視覺反饋（勾選標記出現）
3. 檢查 Vue DevTools 中 serviceIds 陣列是否更新
4. 檢查價格輸入框是否相應顯示

### 8. 截圖分享

如果問題仍然存在，請提供以下截圖：
1. 完整的預約表單頁面
2. 開發者工具中的 Elements 標籤（顯示checkbox HTML結構）
3. 開發者工具中的 Computed 標籤（顯示checkbox的計算樣式）
4. Console 標籤（顯示任何錯誤訊息）

## 快速診斷命令

檢查Checkbox相關的DOM元素：

```javascript
// 在瀏覽器 Console 中執行
// 檢查是否有checkbox元素
document.querySelectorAll('.service-checkbox-item').length

// 檢查checkbox-container
document.querySelectorAll('.checkbox-container').length

// 檢查PrimeVue checkbox組件
document.querySelectorAll('.p-checkbox').length

// 檢查第一個checkbox的樣式
const firstCheckbox = document.querySelector('.p-checkbox');
if (firstCheckbox) {
  console.log('Width:', window.getComputedStyle(firstCheckbox).width);
  console.log('Height:', window.getComputedStyle(firstCheckbox).height);
  console.log('Display:', window.getComputedStyle(firstCheckbox).display);
  console.log('Visibility:', window.getComputedStyle(firstCheckbox).visibility);
}
```

## 已實作的修復

1. ✅ 添加 `!important` 確保樣式優先級
2. ✅ 設置 `display: inline-flex` 明確顯示方式
3. ✅ 添加 `min-width` 防止flex收縮
4. ✅ 明確設置寬度和高度為 1.25rem
5. ✅ 設置 flex 對齊方式確保居中

## 測試環境

- Node.js 版本: (請填寫)
- 瀏覽器: (請填寫 Chrome/Firefox/Safari 及版本)
- 操作系統: macOS (Darwin 24.6.0)
- Vite 版本: 5.4.19

## 聯絡支援

如果以上步驟都無法解決問題，請提供：
1. 完整的錯誤日誌
2. package.json 的依賴版本
3. 瀏覽器開發者工具截圖
