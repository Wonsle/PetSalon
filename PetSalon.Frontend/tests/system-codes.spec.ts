import { test, expect } from 'playwright/test';

test.describe('系統代碼維護頁面測試', () => {
  test.beforeEach(async ({ page }) => {
    // 設定較長的超時時間，因為前端可能需要載入
    test.setTimeout(60000);

    // 導航到系統代碼頁面
    await page.goto('http://localhost:3000/settings/system-codes');

    // 等待頁面載入完成
    await page.waitForLoadState('networkidle');
  });

  test('頁面標題應該正確顯示', async ({ page }) => {
    // 檢查頁面標題
    const title = page.locator('h2').filter({ hasText: '🔧 系統代碼維護' });
    await expect(title).toBeVisible();
  });

  test('新增代碼按鈕應該存在', async ({ page }) => {
    // 檢查新增按鈕
    const addButton = page.locator('button').filter({ hasText: '新增代碼' });
    await expect(addButton).toBeVisible();
  });

  test('代碼類型篩選器應該存在', async ({ page }) => {
    // 檢查代碼類型篩選器
    const typeSelect = page.locator('label').filter({ hasText: '代碼類型' });
    await expect(typeSelect).toBeVisible();

    // 檢查下拉選單
    const select = page.locator('[id="type-select"]');
    await expect(select).toBeVisible();
  });

  test('關鍵字搜尋輸入框應該存在', async ({ page }) => {
    // 檢查關鍵字搜尋
    const keywordInput = page.locator('[id="keyword-input"]');
    await expect(keywordInput).toBeVisible();
  });

  test('狀態篩選器應該存在', async ({ page }) => {
    // 檢查狀態篩選器
    const statusSelect = page.locator('[id="status-select"]');
    await expect(statusSelect).toBeVisible();
  });

  test('資料表格應該存在', async ({ page }) => {
    // 檢查資料表格
    const dataTable = page.locator('.p-datatable');
    await expect(dataTable).toBeVisible();
  });

  test('表格欄位應該包含類型欄位', async ({ page }) => {
    // 檢查表格標題是否包含「類型」
    const typeHeader = page.locator('.p-datatable thead th').filter({ hasText: '類型' });
    await expect(typeHeader).toBeVisible();
  });

  test('表格欄位應該包含代碼欄位', async ({ page }) => {
    // 檢查表格標題是否包含「代碼」
    const codeHeader = page.locator('.p-datatable thead th').filter({ hasText: '代碼' });
    await expect(codeHeader).toBeVisible();
  });

  test('表格欄位應該包含名稱欄位', async ({ page }) => {
    // 檢查表格標題是否包含「名稱」
    const nameHeader = page.locator('.p-datatable thead th').filter({ hasText: '名稱' });
    await expect(nameHeader).toBeVisible();
  });

  test('表格欄位應該包含狀態欄位', async ({ page }) => {
    // 檢查表格標題是否包含「狀態」
    const statusHeader = page.locator('.p-datatable thead th').filter({ hasText: '狀態' });
    await expect(statusHeader).toBeVisible();
  });

  test('表格欄位應該包含操作欄位', async ({ page }) => {
    // 檢查表格標題是否包含「操作」
    const actionHeader = page.locator('.p-datatable thead th').filter({ hasText: '操作' });
    await expect(actionHeader).toBeVisible();
  });

  test('應該能夠載入代碼類型選項', async ({ page }) => {
    // 等待一下讓 API 請求完成
    await page.waitForTimeout(2000);

    // 檢查代碼類型下拉選單是否有選項
    const select = page.locator('[id="type-select"]');
    const options = select.locator('option');
    const optionCount = await options.count();

    // 至少應該有一個選項（"全部類型"）
    expect(optionCount).toBeGreaterThan(0);

    console.log(`找到 ${optionCount} 個代碼類型選項`);
  });

  test('應該能夠載入系統代碼資料', async ({ page }) => {
    // 等待資料載入
    await page.waitForTimeout(3000);

    // 檢查表格是否有資料行
    const dataRows = page.locator('.p-datatable tbody tr');
    const rowCount = await dataRows.count();

    console.log(`表格中有 ${rowCount} 行資料`);

    // 如果有資料，檢查第一行的類型欄位是否顯示中文名稱
    if (rowCount > 0) {
      const firstRowTypeCell = dataRows.first().locator('td').first();
      const typeText = await firstRowTypeCell.textContent();

      console.log(`第一行類型欄位內容: ${typeText}`);

      // 檢查是否顯示中文名稱而不是英文代碼
      const chinesePattern = /[\u4e00-\u9fff]/; // 中文字符的 Unicode 範圍
      expect(chinesePattern.test(typeText || '')).toBe(true);
    }
  });

  test('篩選功能應該正常運作', async ({ page }) => {
    // 等待資料載入
    await page.waitForTimeout(3000);

    // 記錄初始行數
    const initialRows = page.locator('.p-datatable tbody tr');
    const initialCount = await initialRows.count();

    console.log(`初始資料行數: ${initialCount}`);

    // 如果有資料，嘗試篩選
    if (initialCount > 0) {
      // 選擇第一個非「全部類型」的選項
      const select = page.locator('[id="type-select"]');
      const options = select.locator('option');

      // 找到第一個有值的選項（跳過空值選項）
      for (let i = 0; i < await options.count(); i++) {
        const option = options.nth(i);
        const value = await option.getAttribute('value');

        if (value && value.trim() !== '') {
          await select.selectOption({ value: value });
          console.log(`選擇代碼類型: ${value}`);
          break;
        }
      }

      // 等待篩選完成
      await page.waitForTimeout(1000);

      // 檢查篩選後的行數
      const filteredRows = page.locator('.p-datatable tbody tr');
      const filteredCount = await filteredRows.count();

      console.log(`篩選後資料行數: ${filteredCount}`);

      // 篩選後的行數應該小於或等於初始行數
      expect(filteredCount).toBeLessThanOrEqual(initialCount);
    }
  });

  test('新增代碼對話框應該能夠開啟', async ({ page }) => {
    // 點擊新增按鈕
    const addButton = page.locator('button').filter({ hasText: '新增代碼' });
    await addButton.click();

    // 檢查對話框是否開啟
    const dialog = page.locator('.p-dialog');
    await expect(dialog).toBeVisible();

    // 檢查對話框標題
    const dialogTitle = dialog.locator('.p-dialog-header').filter({ hasText: '新增系統代碼' });
    await expect(dialogTitle).toBeVisible();

    console.log('新增代碼對話框成功開啟');
  });

  test('檢查是否有錯誤訊息', async ({ page }) => {
    // 檢查頁面上是否有錯誤訊息
    const errorMessages = page.locator('.p-error, .text-red-500, .text-danger');
    const errorCount = await errorMessages.count();

    if (errorCount > 0) {
      console.log(`發現 ${errorCount} 個錯誤訊息:`);
      for (let i = 0; i < errorCount; i++) {
        const errorText = await errorMessages.nth(i).textContent();
        console.log(`錯誤 ${i + 1}: ${errorText}`);
      }
    }

    // 錯誤數量應該為 0
    expect(errorCount).toBe(0);
  });

  test('檢查網路請求', async ({ page }) => {
    // 監聽網路請求
    const requests: string[] = [];
    const responses: string[] = [];

    page.on('request', request => {
      if (request.url().includes('/api/')) {
        requests.push(`${request.method()} ${request.url()}`);
      }
    });

    page.on('response', response => {
      if (response.url().includes('/api/')) {
        responses.push(`${response.status()} ${response.url()}`);
      }
    });

    // 重新載入頁面
    await page.reload();
    await page.waitForLoadState('networkidle');

    console.log('API 請求:');
    requests.forEach(req => console.log(`  ${req}`));

    console.log('API 回應:');
    responses.forEach(res => console.log(`  ${res}`));

    // 應該至少有一個成功的 API 請求
    expect(responses.length).toBeGreaterThan(0);

    // 檢查是否有錯誤的回應（狀態碼 >= 400）
    const errorResponses = responses.filter(res => parseInt(res.split(' ')[0]) >= 400);
    if (errorResponses.length > 0) {
      console.log('發現錯誤回應:');
      errorResponses.forEach(err => console.log(`  ${err}`));
    }

    expect(errorResponses.length).toBe(0);
  });
});
