/**
 * 包月管理系統 API 測試腳本
 * 測試包月次數預留、釋放、確認等核心功能
 */

// API 基底 URL
const API_BASE_URL = 'https://localhost:7001/api'

/**
 * API 測試工具類
 */
class ApiTester {
  constructor(baseUrl) {
    this.baseUrl = baseUrl
  }

  async request(endpoint, options = {}) {
    const url = `${this.baseUrl}${endpoint}`
    const config = {
      headers: {
        'Content-Type': 'application/json',
        ...options.headers
      },
      ...options
    }

    try {
      const response = await fetch(url, config)
      const data = await response.json()

      if (!response.ok) {
        throw new Error(`API Error: ${response.status} - ${data.message || response.statusText}`)
      }

      return data
    } catch (error) {
      console.error(`Request failed for ${endpoint}:`, error)
      throw error
    }
  }

  // 包月服務 API 測試
  async testSubscriptionApis() {
    console.log('🧪 開始測試包月服務 API...')

    try {
      // 1. 測試取得包月列表
      console.log('📋 測試：取得包月列表')
      const subscriptions = await this.request('/Subscription')
      console.log('✅ 成功取得包月列表:', subscriptions.length, '筆')

      if (subscriptions.length > 0) {
        const testSubscription = subscriptions[0]
        const subscriptionId = testSubscription.subscriptionId

        // 2. 測試檢查包月可用性
        console.log('🔍 測試：檢查包月可用性')
        const availability = await this.request(`/Subscription/${subscriptionId}/availability`)
        console.log('✅ 包月可用性:', availability ? '可用' : '不可用')

        // 3. 測試預留包月次數
        console.log('📌 測試：預留包月次數')
        const reserved = await this.request(`/Subscription/${subscriptionId}/reserve`, {
          method: 'POST',
          body: JSON.stringify(1)
        })
        console.log('✅ 預留結果:', reserved ? '成功' : '失敗')

        // 4. 測試取得包月使用情況
        console.log('📊 測試：取得包月使用情況')
        const usage = await this.request(`/Subscription/${subscriptionId}/usage`)
        console.log('✅ 使用情況:', usage)

        // 5. 測試確認包月次數扣除
        console.log('✔️ 測試：確認包月次數扣除')
        const confirmed = await this.request(`/Subscription/${subscriptionId}/confirm`, {
          method: 'POST',
          body: JSON.stringify(1)
        })
        console.log('✅ 確認結果:', confirmed ? '成功' : '失敗')

        // 6. 測試取得剩餘使用次數
        console.log('🔢 測試：取得剩餘使用次數')
        const remaining = await this.request(`/Subscription/${subscriptionId}/remaining`)
        console.log('✅ 剩餘次數:', remaining)
      }

    } catch (error) {
      console.error('❌ 包月服務 API 測試失敗:', error.message)
    }
  }

  // 預約服務 API 測試
  async testReservationApis() {
    console.log('\n🧪 開始測試預約服務 API...')

    try {
      // 1. 測試取得預約列表
      console.log('📋 測試：取得預約列表')
      const reservations = await this.request('/Reservation')
      console.log('✅ 成功取得預約列表:', reservations.length, '筆')

      // 2. 測試建立預約（使用包月）
      console.log('➕ 測試：建立使用包月的預約')
      const newReservation = {
        petId: 1,
        reservationDate: new Date(Date.now() + 86400000).toISOString(), // 明天
        reservationTime: '10:00:00',
        useSubscription: true,
        serviceIds: [1],
        addonIds: [],
        status: 'PENDING',
        memo: 'API 測試預約'
      }

      const createdReservation = await this.request('/Reservation', {
        method: 'POST',
        body: JSON.stringify(newReservation)
      })
      console.log('✅ 建立預約成功，ID:', createdReservation)

      // 3. 測試更新預約狀態為完成
      if (createdReservation) {
        console.log('🏁 測試：完成預約')
        await this.request(`/Reservation/${createdReservation}/status`, {
          method: 'POST',
          body: JSON.stringify('COMPLETED')
        })
        console.log('✅ 預約完成成功')
      }

    } catch (error) {
      console.error('❌ 預約服務 API 測試失敗:', error.message)
    }
  }

  // 服務類型判斷 API 測試
  async testServiceTypeApis() {
    console.log('\n🧪 開始測試服務類型判斷 API...')

    try {
      // 1. 測試判斷服務類型
      console.log('🔍 測試：判斷服務類型')
      const serviceResult = await this.request('/ServiceType/determine', {
        method: 'POST',
        body: JSON.stringify([1, 2, 3])
      })
      console.log('✅ 服務類型判斷結果:', serviceResult)

      // 2. 測試計算扣除次數
      console.log('🧮 測試：計算扣除次數')
      const deductionResult = await this.request('/ServiceType/calculate-deduction', {
        method: 'POST',
        body: JSON.stringify({
          serviceType: 'GROOM',
          serviceIds: [1, 2]
        })
      })
      console.log('✅ 扣除次數計算結果:', deductionResult)

      // 3. 測試驗證相容性
      console.log('✔️ 測試：驗證服務類型相容性')
      const compatibility = await this.request('/ServiceType/validate-compatibility?subscriptionType=MIXED&serviceType=GROOM')
      console.log('✅ 相容性驗證結果:', compatibility ? '相容' : '不相容')

    } catch (error) {
      console.error('❌ 服務類型判斷 API 測試失敗:', error.message)
    }
  }

  // 自動狀態更新測試
  async testAutoStatusUpdate() {
    console.log('\n🧪 開始測試自動狀態更新...')

    try {
      console.log('🔄 測試：自動更新包月狀態')
      await this.request('/Subscription/auto-update-status', {
        method: 'POST'
      })
      console.log('✅ 自動狀態更新成功')

    } catch (error) {
      console.error('❌ 自動狀態更新測試失敗:', error.message)
    }
  }

  // 執行完整測試套件
  async runAllTests() {
    console.log('🚀 開始執行包月管理系統 API 完整測試套件...')
    console.log('=' .repeat(60))

    await this.testSubscriptionApis()
    await this.testReservationApis()
    await this.testServiceTypeApis()
    await this.testAutoStatusUpdate()

    console.log('\n' + '=' .repeat(60))
    console.log('🎉 測試套件執行完畢！')
  }
}

/**
 * 併發測試 - 模擬多個用戶同時使用包月服務
 */
async function testConcurrency() {
  console.log('\n🧪 開始併發測試...')

  const tester = new ApiTester(API_BASE_URL)
  const subscriptionId = 1 // 假設的包月 ID

  // 模擬 5 個用戶同時預留包月次數
  const concurrentRequests = Array.from({ length: 5 }, (_, i) =>
    tester.request(`/Subscription/${subscriptionId}/reserve`, {
      method: 'POST',
      body: JSON.stringify(1)
    }).then(result => ({ user: i + 1, result }))
    .catch(error => ({ user: i + 1, error: error.message }))
  )

  const results = await Promise.all(concurrentRequests)

  console.log('併發測試結果:')
  results.forEach(({ user, result, error }) => {
    if (error) {
      console.log(`❌ 用戶 ${user}: ${error}`)
    } else {
      console.log(`✅ 用戶 ${user}: ${result ? '預留成功' : '預留失敗'}`)
    }
  })
}

/**
 * 效能測試 - 測試 API 回應時間
 */
async function testPerformance() {
  console.log('\n🧪 開始效能測試...')

  const tester = new ApiTester(API_BASE_URL)
  const testCount = 10
  const times = []

  for (let i = 0; i < testCount; i++) {
    const startTime = Date.now()

    try {
      await tester.request('/Subscription')
      const endTime = Date.now()
      times.push(endTime - startTime)
    } catch (error) {
      console.error(`測試 ${i + 1} 失敗:`, error.message)
    }
  }

  if (times.length > 0) {
    const avgTime = times.reduce((a, b) => a + b, 0) / times.length
    const maxTime = Math.max(...times)
    const minTime = Math.min(...times)

    console.log(`⏱️ 效能測試結果 (${testCount} 次請求):`)
    console.log(`   平均回應時間: ${avgTime.toFixed(2)}ms`)
    console.log(`   最快回應時間: ${minTime}ms`)
    console.log(`   最慢回應時間: ${maxTime}ms`)
  }
}

// 執行測試
async function main() {
  const tester = new ApiTester(API_BASE_URL)

  try {
    // 基本功能測試
    await tester.runAllTests()

    // 併發測試
    await testConcurrency()

    // 效能測試
    await testPerformance()

  } catch (error) {
    console.error('測試執行失敗:', error)
  }
}

// 如果在 Node.js 環境中執行
if (typeof module !== 'undefined' && module.exports) {
  module.exports = { ApiTester, testConcurrency, testPerformance }
}

// 如果在瀏覽器環境中執行
if (typeof window !== 'undefined') {
  window.PetSalonApiTester = { ApiTester, testConcurrency, testPerformance }

  // 自動執行測試（可選）
  // main()
}

// 導出測試函數供外部使用
export { ApiTester, testConcurrency, testPerformance, main }
