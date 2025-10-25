const puppeteer = require('playwright');

async function testSystemCodesPage() {
  console.log('🚀 開始測試系統代碼維護頁面...');

  let browser;
  try {
    // 啟動瀏覽器
    browser = await puppeteer.chromium.launch({
      headless: false, // 顯示瀏覽器視窗
      args: ['--no-sandbox', '--disable-setuid-sandbox']
    });

    const page = await browser.newPage();

    // 設定較長的超時時間
    page.setDefaultTimeout(60000);

    console.log('📄 導航到系統代碼頁面...');
    await page.goto('http://localhost:3000/settings/system-codes');

    // 等待頁面載入
    await page.waitForLoadState('networkidle');
    console.log('✅ 頁面載入完成');

    // 測試 1: 檢查頁面標題
    console.log('\n📋 測試 1: 檢查頁面標題');
    const titleElement = await page.$('h2:has-text("🔧 系統代碼維護")');
    if (titleElement) {
      console.log('✅ 頁面標題正確顯示');
    } else {
      console.log('❌ 頁面標題未找到');
    }

    // 測試 2: 檢查新增按鈕
    console.log('\n📋 測試 2: 檢查新增按鈕');
    const addButton = await page.$('button:has-text("新增代碼")');
    if (addButton) {
      console.log('✅ 新增按鈕存在');
    } else {
      console.log('❌ 新增按鈕未找到');
    }

    // 測試 3: 檢查篩選器
    console.log('\n📋 測試 3: 檢查篩選器');
    const typeSelect = await page.$('[id="type-select"]');
    const keywordInput = await page.$('[id="keyword-input"]');
    const statusSelect = await page.$('[id="status-select"]');

    if (typeSelect && keywordInput && statusSelect) {
      console.log('✅ 所有篩選器都存在');
    } else {
      console.log('❌ 部分篩選器缺失');
    }

    // 測試 4: 檢查資料表格
    console.log('\n📋 測試 4: 檢查資料表格');
    const dataTable = await page.$('.p-datatable');
    if (dataTable) {
      console.log('✅ 資料表格存在');

      // 檢查表格標題
      const headers = await page.$$eval('.p-datatable thead th', ths => ths.map(th => th.textContent?.trim()));
      console.log('表格標題:', headers);

      const expectedHeaders = ['類型', '代碼', '名稱', '值', '排序', '狀態', '操作'];
      const hasAllHeaders = expectedHeaders.every(header => headers.includes(header));

      if (hasAllHeaders) {
        console.log('✅ 表格標題完整');
      } else {
        console.log('❌ 表格標題不完整');
      }

      // 檢查是否有資料行
      await page.waitForTimeout(3000); // 等待資料載入
      const dataRows = await page.$$('.p-datatable tbody tr');
      console.log(`📊 表格中有 ${dataRows.length} 行資料`);

      if (dataRows.length > 0) {
        // 檢查第一行的類型欄位是否顯示中文
        const firstRowTypeCell = await page.$eval('.p-datatable tbody tr:first-child td:first-child', td => td.textContent?.trim());
        console.log(`第一行類型欄位內容: "${firstRowTypeCell}"`);

        // 檢查是否包含中文字符
        const chinesePattern = /[\u4e00-\u9fff]/;
        if (chinesePattern.test(firstRowTypeCell || '')) {
          console.log('✅ 類型欄位顯示中文名稱');
        } else {
          console.log('❌ 類型欄位未顯示中文名稱');
        }
      }
    } else {
      console.log('❌ 資料表格未找到');
    }

    // 測試 5: 檢查代碼類型選項
    console.log('\n📋 測試 5: 檢查代碼類型選項');
    await page.waitForTimeout(2000); // 等待選項載入

    const options = await page.$$eval('[id="type-select"] option', opts =>
      opts.map(opt => ({ value: opt.value, text: opt.textContent?.trim() }))
    );

    console.log(`找到 ${options.length} 個代碼類型選項:`);
    options.forEach((opt, index) => {
      console.log(`  ${index + 1}. ${opt.value}: ${opt.text}`);
    });

    if (options.length > 0) {
      console.log('✅ 代碼類型選項載入成功');
    } else {
      console.log('❌ 代碼類型選項載入失敗');
    }

    // 測試 6: 檢查網路請求
    console.log('\n📋 測試 6: 檢查網路請求');

    const requests = [];
    const responses = [];

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

    // 重新載入頁面來捕獲請求
    await page.reload();
    await page.waitForLoadState('networkidle');

    console.log(`發送了 ${requests.length} 個 API 請求:`);
    requests.forEach(req => console.log(`  ${req}`));

    console.log(`收到 ${responses.length} 個 API 回應:`);
    responses.forEach(res => console.log(`  ${res}`));

    // 檢查是否有錯誤回應
    const errorResponses = responses.filter(res => parseInt(res.split(' ')[0]) >= 400);
    if (errorResponses.length > 0) {
      console.log('❌ 發現錯誤回應:');
      errorResponses.forEach(err => console.log(`  ${err}`));
    } else {
      console.log('✅ 所有 API 請求都成功');
    }

    // 測試 7: 檢查是否有錯誤訊息
    console.log('\n📋 測試 7: 檢查頁面錯誤訊息');
    const errorMessages = await page.$$('.p-error, .text-red-500, .text-danger');
    if (errorMessages.length > 0) {
      console.log(`❌ 發現 ${errorMessages.length} 個錯誤訊息`);
      for (let i = 0; i < errorMessages.length; i++) {
        const errorText = await errorMessages[i].evaluate(el => el.textContent?.trim());
        console.log(`  錯誤 ${i + 1}: ${errorText}`);
      }
    } else {
      console.log('✅ 頁面上沒有錯誤訊息');
    }

    console.log('\n🎉 測試完成！');

    // 等待一下讓使用者看到結果
    await page.waitForTimeout(5000);

  } catch (error) {
    console.error('❌ 測試失敗:', error);
  } finally {
    if (browser) {
      await browser.close();
    }
  }
}

// 運行測試
testSystemCodesPage().catch(console.error);