import { test, expect } from '@playwright/test'

test.describe('Reservation Form Checkbox Test', () => {
  test('should display service checkboxes', async ({ page }) => {
    // 導航到預約頁面
    await page.goto('http://localhost:3001')

    // 等待頁面載入
    await page.waitForLoadState('networkidle')

    // 截圖首頁
    await page.screenshot({ path: 'tests/screenshots/homepage.png', fullPage: true })

    // 嘗試找到預約相關的按鈕或連結
    const reservationLink = page.getByText('預約', { exact: false })
    if (await reservationLink.count() > 0) {
      await reservationLink.first().click()
      await page.waitForLoadState('networkidle')
    }

    // 截圖預約頁面
    await page.screenshot({ path: 'tests/screenshots/reservation-page.png', fullPage: true })

    // 查找新增預約按鈕
    const addButton = page.getByRole('button', { name: /新增|建立|新增預約/ })
    if (await addButton.count() > 0) {
      await addButton.first().click()
      await page.waitForTimeout(1000)
    }

    // 截圖預約表單
    await page.screenshot({ path: 'tests/screenshots/reservation-form.png', fullPage: true })

    // 檢查服務項目區塊
    const serviceSection = page.locator('text=服務項目').first()
    await expect(serviceSection).toBeVisible()

    // 檢查checkbox是否存在
    const checkboxes = page.locator('.service-checkbox-item')
    const checkboxCount = await checkboxes.count()
    console.log(`找到 ${checkboxCount} 個服務項目`)

    if (checkboxCount > 0) {
      // 截圖第一個checkbox
      await checkboxes.first().screenshot({ path: 'tests/screenshots/checkbox-detail.png' })

      // 檢查checkbox組件
      const firstCheckbox = checkboxes.first().locator('.p-checkbox')
      console.log(`Checkbox是否可見: ${await firstCheckbox.isVisible()}`)

      // 打印HTML結構
      const html = await checkboxes.first().innerHTML()
      console.log('Checkbox HTML:', html)
    }

    // 打印頁面console訊息
    page.on('console', msg => console.log('PAGE LOG:', msg.text()))
  })
})
