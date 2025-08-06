/**
 * API 測試工具 - 用於驗證階段六的前端整合
 * 此文件用於測試新實現的 API 端點是否正常工作
 */

import { serviceApi } from '@/api/service'
import { reservationApi } from '@/api/reservation'

export const apiTester = {
  /**
   * 測試服務相關的 API
   */
  async testServiceApis() {
    console.group('🔧 Testing Service APIs')
    
    try {
      // 測試活躍服務項目
      console.log('Testing serviceApi.getActiveServices()...')
      const activeServices = await serviceApi.getActiveServices()
      console.log('✅ Active services:', activeServices)

      // 測試所有服務項目
      console.log('Testing serviceApi.getAllServices()...')
      const allServices = await serviceApi.getAllServices()
      console.log('✅ All services:', allServices)

      return { activeServices, allServices }
    } catch (error) {
      console.error('❌ Service API test failed:', error)
      throw error
    } finally {
      console.groupEnd()
    }
  },


  /**
   * 測試預約計算相關的 API
   */
  async testReservationCalculationApis(petId: number) {
    console.group('🔧 Testing Reservation Calculation APIs')
    
    try {
      // 測試寵物附加服務價格
      console.log(`Testing reservationApi.getPetAddonPrices(${petId})...`)
      const petAddonPrices = await reservationApi.getPetAddonPrices(petId)
      console.log('✅ Pet addon prices:', petAddonPrices)

      // 測試成本計算（需要有效的 serviceIds 和 addonIds）
      const costRequest = {
        petId,
        serviceIds: [1], // 假設存在 serviceId = 1
        addonIds: [1],   // 假設存在 addonId = 1
        useSubscription: false
      }
      
      console.log('Testing reservationApi.calculateCost()...')
      const costResult = await reservationApi.calculateCost(costRequest)
      console.log('✅ Cost calculation:', costResult)

      // 測試時長計算
      const durationRequest = {
        serviceIds: [1],
        addonIds: [1]
      }
      
      console.log(`Testing reservationApi.calculateDuration(${petId})...`)
      const durationResult = await reservationApi.calculateDuration(petId, durationRequest)
      console.log('✅ Duration calculation:', durationResult)

      return { petAddonPrices, costResult, durationResult }
    } catch (error) {
      console.error('❌ Reservation calculation API test failed:', error)
      throw error
    } finally {
      console.groupEnd()
    }
  },

  /**
   * 測試特定寵物的價格 API
   */
  async testPetPriceApis(petId: number) {
    console.group(`🔧 Testing Pet Price APIs for petId: ${petId}`)
    
    try {
      // 測試寵物服務價格
      console.log(`Testing serviceApi.getPetServicePrices(${petId})...`)
      const petServicePrices = await serviceApi.getPetServicePrices(petId)
      console.log('✅ Pet service prices:', petServicePrices)

      return { petServicePrices }
    } catch (error) {
      console.error('❌ Pet price API test failed:', error)
      throw error
    } finally {
      console.groupEnd()
    }
  },

  /**
   * 執行完整的 API 測試
   */
  async runFullTest(testPetId: number = 1) {
    console.log('🚀 Starting comprehensive API test suite...')
    
    try {
      const results = {
        services: await this.testServiceApis(),
        petPrices: await this.testPetPriceApis(testPetId),
        calculations: await this.testReservationCalculationApis(testPetId)
      }

      console.log('✅ All API tests completed successfully!')
      console.log('📊 Test Results Summary:', results)
      
      return results
    } catch (error) {
      console.error('❌ API test suite failed:', error)
      throw error
    }
  }
}

// 開發環境下自動執行測試
if (import.meta.env.DEV) {
  // 可以在控制台調用 window.testApis() 來運行測試
  ;(window as any).testApis = apiTester
  console.log('🔧 API Tester loaded. Use window.testApis.runFullTest() to run tests.')
}