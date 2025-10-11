/**
 * Mock 資料驗證腳本
 * 檢查資料的一致性和完整性
 */

import { getAllSystemCodes, getSystemCodesByType } from './systemCodes'
import { getAllMockContacts } from './contacts'
import { getAllMockPets } from './pets'
import { getAllMockSubscriptions } from './subscriptions'
import { getAllMockReservations } from './reservations'

/**
 * 驗證資料一致性
 */
export function validateMockData(): {
  success: boolean
  errors: string[]
  warnings: string[]
  stats: {
    systemCodes: number
    contacts: number
    pets: number
    subscriptions: number
    reservations: number
  }
} {
  const errors: string[] = []
  const warnings: string[] = []

  // 1. 取得所有資料
  const systemCodes = getAllSystemCodes()
  const contacts = getAllMockContacts()
  const pets = getAllMockPets()
  const subscriptions = getAllMockSubscriptions()
  const reservations = getAllMockReservations()

  console.log('=== Mock 資料驗證開始 ===')
  console.log(`系統代碼: ${systemCodes.length} 筆`)
  console.log(`聯絡人: ${contacts.length} 筆`)
  console.log(`寵物: ${pets.length} 筆`)
  console.log(`包月: ${subscriptions.length} 筆`)
  console.log(`預約: ${reservations.length} 筆`)

  // 2. 驗證系統代碼
  const breeds = getSystemCodesByType('Breed')
  const genders = getSystemCodesByType('Gender')
  const serviceTypes = getSystemCodesByType('ServiceType')
  const relationships = getSystemCodesByType('Relationship')

  if (breeds.length < 10) {
    warnings.push(`品種代碼數量不足: ${breeds.length} (建議至少 10 個)`)
  }

  if (genders.length !== 2) {
    errors.push(`性別代碼數量錯誤: ${genders.length} (應為 2 個)`)
  }

  // 3. 驗證聯絡人
  if (contacts.length < 15) {
    warnings.push(`聯絡人數量不足: ${contacts.length} (要求至少 15 筆)`)
  }

  contacts.forEach(contact => {
    if (!contact.contactNumber.match(/^09\d{2}-\d{3}-\d{3}$/)) {
      errors.push(`聯絡人 ${contact.name} 的電話格式錯誤: ${contact.contactNumber}`)
    }
  })

  // 4. 驗證寵物
  if (pets.length < 20) {
    warnings.push(`寵物數量不足: ${pets.length} (要求至少 20 筆)`)
  }

  const petContactIds = new Set<number>()
  pets.forEach(pet => {
    // 檢查品種代碼是否有效
    const breedCode = breeds.find(b => b.code === pet.breed)
    if (!breedCode) {
      errors.push(`寵物 ${pet.petName} 的品種代碼無效: ${pet.breed}`)
    }

    // 檢查性別代碼是否有效
    const genderCode = genders.find(g => g.code === pet.gender)
    if (!genderCode) {
      errors.push(`寵物 ${pet.petName} 的性別代碼無效: ${pet.gender}`)
    }

    // 檢查聯絡人是否存在
    if (pet.primaryContact) {
      const contact = contacts.find(c => c.contactPersonId === pet.primaryContact!.contactPersonId)
      if (!contact) {
        errors.push(`寵物 ${pet.petName} 的主要聯絡人不存在: ID ${pet.primaryContact.contactPersonId}`)
      } else {
        petContactIds.add(pet.primaryContact.contactPersonId)
      }
    } else {
      warnings.push(`寵物 ${pet.petName} 沒有主要聯絡人`)
    }

    // 檢查價格
    if (pet.normalPrice && pet.normalPrice < 0) {
      errors.push(`寵物 ${pet.petName} 的一般價格無效: ${pet.normalPrice}`)
    }

    if (pet.subscriptionPrice && pet.subscriptionPrice < 0) {
      errors.push(`寵物 ${pet.petName} 的包月價格無效: ${pet.subscriptionPrice}`)
    }

    if (pet.normalPrice && pet.subscriptionPrice && pet.subscriptionPrice >= pet.normalPrice) {
      warnings.push(`寵物 ${pet.petName} 的包月價格應低於一般價格`)
    }
  })

  // 5. 驗證包月
  if (subscriptions.length < 10) {
    warnings.push(`包月數量不足: ${subscriptions.length} (要求至少 10 筆)`)
  }

  const activeCount = subscriptions.filter(s => s.isActive).length
  const expiringCount = subscriptions.filter(s => !s.isExpired && s.daysUntilExpiry <= 7 && s.daysUntilExpiry > 0).length
  const expiredCount = subscriptions.filter(s => s.isExpired).length

  console.log(`包月狀態分布: 有效 ${activeCount}, 即將到期 ${expiringCount}, 已過期 ${expiredCount}`)

  subscriptions.forEach(sub => {
    // 檢查寵物是否存在
    const pet = pets.find(p => p.petId === sub.petId)
    if (!pet) {
      errors.push(`包月 ${sub.subscriptionId} 的寵物不存在: ID ${sub.petId}`)
    }

    // 檢查日期邏輯
    if (sub.startDate > sub.endDate) {
      errors.push(`包月 ${sub.subscriptionId} 的開始日期晚於結束日期`)
    }

    // 檢查使用次數邏輯
    if (sub.usedCount + sub.reservedCount > sub.totalUsageLimit) {
      errors.push(`包月 ${sub.subscriptionId} 的已使用+預約次數超過總次數`)
    }

    if (sub.usedCount < 0 || sub.reservedCount < 0) {
      errors.push(`包月 ${sub.subscriptionId} 的使用次數不能為負數`)
    }
  })

  // 6. 驗證預約
  if (reservations.length < 30) {
    warnings.push(`預約數量不足: ${reservations.length} (要求至少 30 筆)`)
  }

  const todayReservations = reservations.filter(r => r.reserveDate === '2025-10-11')
  console.log(`今日預約: ${todayReservations.length} 筆`)

  if (todayReservations.length < 5) {
    warnings.push(`今日預約數量不足: ${todayReservations.length} (建議 5-8 筆)`)
  }

  const statusCounts = new Map<string, number>()
  reservations.forEach(r => {
    statusCounts.set(r.status, (statusCounts.get(r.status) || 0) + 1)

    // 檢查寵物是否存在
    const pet = pets.find(p => p.petId === r.petId)
    if (!pet) {
      errors.push(`預約 ${r.id} 的寵物不存在: ID ${r.petId}`)
    }

    // 檢查飼主是否存在
    const owner = contacts.find(c => c.contactPersonId === r.ownerId)
    if (!owner) {
      errors.push(`預約 ${r.id} 的飼主不存在: ID ${r.ownerId}`)
    }

    // 檢查包月是否存在（如果有關聯）
    if (r.subscriptionId) {
      const subscription = subscriptions.find(s => s.subscriptionId === r.subscriptionId)
      if (!subscription) {
        errors.push(`預約 ${r.id} 的包月不存在: ID ${r.subscriptionId}`)
      }
    }

    // 檢查服務類型是否有效
    const serviceType = serviceTypes.find(st => st.code === r.serviceType)
    if (!serviceType) {
      errors.push(`預約 ${r.id} 的服務類型無效: ${r.serviceType}`)
    }

    // 檢查時間格式
    if (!r.reserveTime.match(/^\d{2}:\d{2}$/)) {
      errors.push(`預約 ${r.id} 的時間格式錯誤: ${r.reserveTime}`)
    }
  })

  console.log('預約狀態分布:')
  statusCounts.forEach((count, status) => {
    console.log(`  ${status}: ${count} 筆`)
  })

  // 7. 檢查資料關聯性
  const unusedContacts = contacts.filter(c => !petContactIds.has(c.contactPersonId))
  if (unusedContacts.length > 5) {
    warnings.push(`有 ${unusedContacts.length} 個聯絡人沒有關聯任何寵物`)
  }

  console.log('=== Mock 資料驗證完成 ===')

  return {
    success: errors.length === 0,
    errors,
    warnings,
    stats: {
      systemCodes: systemCodes.length,
      contacts: contacts.length,
      pets: pets.length,
      subscriptions: subscriptions.length,
      reservations: reservations.length
    }
  }
}

/**
 * 執行驗證（在開發環境中可以調用）
 */
if (import.meta.env.DEV) {
  const result = validateMockData()

  console.log('\n=== 驗證結果 ===')
  console.log(`狀態: ${result.success ? '✅ 成功' : '❌ 失敗'}`)

  if (result.errors.length > 0) {
    console.log('\n❌ 錯誤:')
    result.errors.forEach(error => console.log(`  - ${error}`))
  }

  if (result.warnings.length > 0) {
    console.log('\n⚠️  警告:')
    result.warnings.forEach(warning => console.log(`  - ${warning}`))
  }

  console.log('\n📊 統計:')
  console.log(`  系統代碼: ${result.stats.systemCodes} 筆`)
  console.log(`  聯絡人: ${result.stats.contacts} 筆`)
  console.log(`  寵物: ${result.stats.pets} 筆`)
  console.log(`  包月: ${result.stats.subscriptions} 筆`)
  console.log(`  預約: ${result.stats.reservations} 筆`)
}
