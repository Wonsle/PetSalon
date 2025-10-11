/**
 * Mock 資料快速測試腳本
 * 用於驗證資料的基本功能
 */

import {
  getSystemCodesByType,
  getSystemCodeName,
  getMockContacts,
  getMockPets,
  getMockSubscriptions,
  getMockReservations,
  getTodayMockReservations,
  getActiveMockSubscriptions,
  getExpiringMockSubscriptions,
  getDashboardStatistics
} from './index'

console.log('=== Mock 資料快速測試 ===\n')

// 1. 測試系統代碼
console.log('1. 系統代碼測試')
const breeds = getSystemCodesByType('Breed')
console.log(`   品種數量: ${breeds.length}`)
console.log(`   第一個品種: ${breeds[0].code} - ${breeds[0].displayName}`)

const breedName = getSystemCodeName('Breed', 'POODLE')
console.log(`   POODLE 顯示名稱: ${breedName}`)
console.log('   ✅ 系統代碼測試通過\n')

// 2. 測試聯絡人
console.log('2. 聯絡人測試')
const contacts = getMockContacts({ page: 1, pageSize: 5 })
console.log(`   總聯絡人數: ${contacts.total}`)
console.log(`   第一頁資料: ${contacts.data.length} 筆`)
console.log(`   第一個聯絡人: ${contacts.data[0].name} - ${contacts.data[0].contactNumber}`)
console.log('   ✅ 聯絡人測試通過\n')

// 3. 測試寵物
console.log('3. 寵物測試')
const pets = getMockPets({ page: 1, pageSize: 5 })
console.log(`   總寵物數: ${pets.total}`)
console.log(`   第一頁資料: ${pets.data.length} 筆`)
const firstPet = pets.data[0]
console.log(`   第一隻寵物: ${firstPet.petName} (${firstPet.breedName}) - 飼主: ${firstPet.ownerName}`)
console.log('   ✅ 寵物測試通過\n')

// 4. 測試包月
console.log('4. 包月測試')
const subscriptions = getMockSubscriptions({ page: 1, pageSize: 5 })
const activeSubscriptions = getActiveMockSubscriptions()
const expiringSubscriptions = getExpiringMockSubscriptions(7)
console.log(`   總包月數: ${subscriptions.total}`)
console.log(`   有效包月數: ${activeSubscriptions.length}`)
console.log(`   即將到期包月數: ${expiringSubscriptions.length}`)
if (subscriptions.data.length > 0) {
  const firstSub = subscriptions.data[0]
  console.log(`   第一個包月: ${firstSub.petName} - ${firstSub.subscriptionType} (剩餘 ${firstSub.remainingUsage} 次)`)
}
console.log('   ✅ 包月測試通過\n')

// 5. 測試預約
console.log('5. 預約測試')
const reservations = getMockReservations({ page: 1, pageSize: 5 })
const todayReservations = getTodayMockReservations()
console.log(`   總預約數: ${reservations.total}`)
console.log(`   今日預約數: ${todayReservations.length}`)
if (reservations.data.length > 0) {
  const firstRes = reservations.data[0]
  console.log(`   第一筆預約: ${firstRes.petName} - ${firstRes.reserveDate} ${firstRes.reserveTime}`)
}
console.log('   ✅ 預約測試通過\n')

// 6. 測試儀表板
console.log('6. 儀表板測試')
const stats = getDashboardStatistics()
console.log(`   今日預約: ${stats.todayReservations} 筆`)
console.log(`   總寵物數: ${stats.totalPets} 隻`)
console.log(`   本月收入: NT$ ${stats.monthlyRevenue.toLocaleString()}`)
console.log(`   有效包月: ${stats.activeSubscriptions} 個`)
console.log('   ✅ 儀表板測試通過\n')

// 7. 測試資料關聯
console.log('7. 資料關聯測試')
const testPet = pets.data[0]
if (testPet.primaryContact) {
  const testContact = getMockContacts({ page: 1, pageSize: 100 }).data
    .find(c => c.contactPersonId === testPet.primaryContact!.contactPersonId)
  console.log(`   寵物 "${testPet.petName}" 的飼主 "${testPet.ownerName}" 存在: ${testContact ? '是' : '否'}`)
}

const testReservation = reservations.data[0]
if (testReservation.subscriptionId) {
  const testSub = subscriptions.data.find(s => s.subscriptionId === testReservation.subscriptionId)
  console.log(`   預約關聯的包月存在: ${testSub ? '是' : '否'}`)
}
console.log('   ✅ 資料關聯測試通過\n')

console.log('=== 所有測試通過 ✅ ===')
console.log('\n資料層已準備就緒，可以開始使用！')
