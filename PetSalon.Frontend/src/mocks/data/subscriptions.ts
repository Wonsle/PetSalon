/**
 * Mock 包月資料
 * 提供包月方案的 CRUD 操作和查詢功能
 */

import type { Subscription, SubscriptionCreateRequest, SubscriptionUpdateRequest, SubscriptionSearchParams, SubscriptionListResponse } from '@/types/subscription'
import type { ExpiringSubscriptionDto } from '@/api/dashboard'
import { getMockPetById } from './pets'

// 計算日期差異（天數）
function getDaysDiff(date1: string, date2: string): number {
  const d1 = new Date(date1)
  const d2 = new Date(date2)
  const diffTime = d2.getTime() - d1.getTime()
  return Math.ceil(diffTime / (1000 * 60 * 60 * 24))
}

// 今天的日期（2025-10-11）
const TODAY = '2025-10-11'

// 包月資料
const mockSubscriptions: Subscription[] = [
  // 有效包月（距離到期 > 7天）- 60%
  {
    subscriptionId: 1,
    petId: 1,
    petName: '小白',
    subscriptionDate: '2025-09-15',
    startDate: '2025-09-15',
    endDate: '2025-11-15',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 1,
    reservedCount: 1,
    subscriptionPrice: 2400,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 35,
    remainingUsage: 2,
    createUser: 'admin',
    createTime: '2025-09-15T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-15T10:00:00'
  },
  {
    subscriptionId: 2,
    petId: 2,
    petName: '球球',
    subscriptionDate: '2025-09-20',
    startDate: '2025-09-20',
    endDate: '2025-11-20',
    subscriptionType: '美容包月',
    totalUsageLimit: 4,
    usedCount: 2,
    reservedCount: 0,
    subscriptionPrice: 4000,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 40,
    remainingUsage: 2,
    createUser: 'admin',
    createTime: '2025-09-20T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-20T10:00:00'
  },
  {
    subscriptionId: 3,
    petId: 5,
    petName: '妞妞',
    subscriptionDate: '2025-09-25',
    startDate: '2025-09-25',
    endDate: '2025-10-25',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 1,
    reservedCount: 0,
    subscriptionPrice: 2200,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 14,
    remainingUsage: 3,
    createUser: 'admin',
    createTime: '2025-09-25T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-25T10:00:00'
  },
  {
    subscriptionId: 4,
    petId: 7,
    petName: '哈利',
    subscriptionDate: '2025-09-10',
    startDate: '2025-09-10',
    endDate: '2025-11-10',
    subscriptionType: '美容包月',
    totalUsageLimit: 4,
    usedCount: 0,
    reservedCount: 1,
    subscriptionPrice: 4800,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 30,
    remainingUsage: 3,
    createUser: 'admin',
    createTime: '2025-09-10T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-10T10:00:00'
  },
  {
    subscriptionId: 5,
    petId: 11,
    petName: '可可',
    subscriptionDate: '2025-09-18',
    startDate: '2025-09-18',
    endDate: '2025-11-18',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 2,
    reservedCount: 1,
    subscriptionPrice: 2400,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 38,
    remainingUsage: 1,
    createUser: 'admin',
    createTime: '2025-09-18T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-18T10:00:00'
  },
  {
    subscriptionId: 6,
    petId: 15,
    petName: '虎斑',
    subscriptionDate: '2025-09-22',
    startDate: '2025-09-22',
    endDate: '2025-10-22',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 1,
    reservedCount: 0,
    subscriptionPrice: 2600,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 11,
    remainingUsage: 3,
    createUser: 'admin',
    createTime: '2025-09-22T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-22T10:00:00'
  },

  // 即將到期（7天內）- 20%
  {
    subscriptionId: 7,
    petId: 3,
    petName: '柴柴',
    subscriptionDate: '2025-09-08',
    startDate: '2025-09-08',
    endDate: '2025-10-15',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 3,
    reservedCount: 0,
    subscriptionPrice: 2400,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 4,
    remainingUsage: 1,
    createUser: 'admin',
    createTime: '2025-09-08T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-08T10:00:00'
  },
  {
    subscriptionId: 8,
    petId: 10,
    petName: '麥克',
    subscriptionDate: '2025-09-12',
    startDate: '2025-09-12',
    endDate: '2025-10-17',
    subscriptionType: '美容包月',
    totalUsageLimit: 4,
    usedCount: 2,
    reservedCount: 1,
    subscriptionPrice: 3600,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: 6,
    remainingUsage: 1,
    createUser: 'admin',
    createTime: '2025-09-12T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-09-12T10:00:00'
  },

  // 已過期 - 20%
  {
    subscriptionId: 9,
    petId: 4,
    petName: '布丁',
    subscriptionDate: '2025-08-05',
    startDate: '2025-08-05',
    endDate: '2025-10-05',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 4,
    reservedCount: 0,
    subscriptionPrice: 2800,
    status: 'EXPIRED',
    statusName: '已過期',
    isExpired: true,
    isActive: false,
    daysUntilExpiry: -6,
    remainingUsage: 0,
    createUser: 'admin',
    createTime: '2025-08-05T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-08-05T10:00:00'
  },
  {
    subscriptionId: 10,
    petId: 6,
    petName: '豆豆',
    subscriptionDate: '2025-08-10',
    startDate: '2025-08-10',
    endDate: '2025-10-10',
    subscriptionType: '美容包月',
    totalUsageLimit: 4,
    usedCount: 3,
    reservedCount: 0,
    subscriptionPrice: 3800,
    status: 'EXPIRED',
    statusName: '已過期',
    isExpired: true,
    isActive: false,
    daysUntilExpiry: -1,
    remainingUsage: 1,
    createUser: 'admin',
    createTime: '2025-08-10T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-08-10T10:00:00'
  },
  {
    subscriptionId: 11,
    petId: 8,
    petName: '雪莉',
    subscriptionDate: '2025-07-20',
    startDate: '2025-07-20',
    endDate: '2025-09-20',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 4,
    reservedCount: 0,
    subscriptionPrice: 2400,
    status: 'EXPIRED',
    statusName: '已過期',
    isExpired: true,
    isActive: false,
    daysUntilExpiry: -21,
    remainingUsage: 0,
    createUser: 'admin',
    createTime: '2025-07-20T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-07-20T10:00:00'
  },
  {
    subscriptionId: 12,
    petId: 9,
    petName: '皮皮',
    subscriptionDate: '2025-08-01',
    startDate: '2025-08-01',
    endDate: '2025-10-01',
    subscriptionType: '洗澡包月',
    totalUsageLimit: 4,
    usedCount: 2,
    reservedCount: 0,
    subscriptionPrice: 2000,
    status: 'EXPIRED',
    statusName: '已過期',
    isExpired: true,
    isActive: false,
    daysUntilExpiry: -10,
    remainingUsage: 2,
    createUser: 'admin',
    createTime: '2025-08-01T10:00:00',
    modifyUser: 'admin',
    modifyTime: '2025-08-01T10:00:00'
  }
]

/**
 * 重新計算包月狀態
 */
function recalculateSubscriptionStatus(subscription: Subscription): Subscription {
  const today = new Date(TODAY)
  const endDate = new Date(subscription.endDate)

  const daysUntilExpiry = getDaysDiff(TODAY, subscription.endDate)
  const isExpired = endDate < today
  const remainingUsage = subscription.totalUsageLimit - subscription.usedCount - subscription.reservedCount
  const isActive = !isExpired && remainingUsage > 0

  return {
    ...subscription,
    isExpired,
    isActive,
    daysUntilExpiry,
    remainingUsage,
    status: isExpired ? 'EXPIRED' : 'ACTIVE',
    statusName: isExpired ? '已過期' : '有效'
  }
}

/**
 * 取得包月列表（支援分頁和搜尋）
 */
export function getMockSubscriptions(params?: SubscriptionSearchParams): SubscriptionListResponse {
  let filteredSubscriptions = mockSubscriptions.map(recalculateSubscriptionStatus)

  // 寵物ID搜尋
  if (params?.petId) {
    filteredSubscriptions = filteredSubscriptions.filter(sub => sub.petId === params.petId)
  }

  // 狀態搜尋
  if (params?.status) {
    filteredSubscriptions = filteredSubscriptions.filter(sub => sub.status === params.status)
  }

  // 開始日期搜尋
  if (params?.startDate) {
    filteredSubscriptions = filteredSubscriptions.filter(sub => sub.startDate >= params.startDate!)
  }

  // 結束日期搜尋
  if (params?.endDate) {
    filteredSubscriptions = filteredSubscriptions.filter(sub => sub.endDate <= params.endDate!)
  }

  // 分頁
  const page = params?.page || 1
  const pageSize = params?.pageSize || 10
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize

  return {
    data: filteredSubscriptions.slice(startIndex, endIndex),
    total: filteredSubscriptions.length,
    page,
    pageSize
  }
}

/**
 * 根據 ID 取得單一包月
 */
export function getMockSubscriptionById(id: number): Subscription | undefined {
  const subscription = mockSubscriptions.find(sub => sub.subscriptionId === id)
  return subscription ? recalculateSubscriptionStatus(subscription) : undefined
}

/**
 * 根據寵物 ID 取得包月列表
 */
export function getMockSubscriptionsByPetId(petId: number): Subscription[] {
  return mockSubscriptions
    .filter(sub => sub.petId === petId)
    .map(recalculateSubscriptionStatus)
}

/**
 * 取得有效的包月列表
 */
export function getActiveMockSubscriptions(): Subscription[] {
  return mockSubscriptions
    .map(recalculateSubscriptionStatus)
    .filter(sub => sub.isActive)
}

/**
 * 取得即將到期的包月列表
 */
export function getExpiringMockSubscriptions(days: number = 7): ExpiringSubscriptionDto[] {
  const activeSubscriptions = getActiveMockSubscriptions()

  return activeSubscriptions
    .filter(sub => sub.daysUntilExpiry <= days && sub.daysUntilExpiry > 0)
    .map(sub => {
      const pet = getMockPetById(sub.petId)
      return {
        id: sub.subscriptionId,
        petId: sub.petId,
        petName: sub.petName || '',
        subscriptionType: sub.subscriptionType,
        endDate: sub.endDate,
        daysLeft: sub.daysUntilExpiry,
        remainingUsage: sub.remainingUsage,
        primaryContactName: pet?.primaryContact?.name || '',
        primaryContactPhone: pet?.primaryContact?.phone || ''
      }
    })
    .sort((a, b) => a.daysLeft - b.daysLeft)
}

/**
 * 新增包月
 */
export function createMockSubscription(subscription: SubscriptionCreateRequest): Subscription {
  const maxId = Math.max(...mockSubscriptions.map(s => s.subscriptionId), 0)
  const now = new Date().toISOString()
  const pet = getMockPetById(subscription.petId)

  const totalUsageLimit = subscription.totalUsageLimit || subscription.totalTimes || 4
  const subscriptionPrice = subscription.subscriptionPrice || subscription.totalAmount || 0

  const newSubscription: Subscription = {
    subscriptionId: maxId + 1,
    petId: subscription.petId,
    petName: pet?.petName || '',
    subscriptionDate: subscription.subscriptionDate || subscription.startDate,
    startDate: subscription.startDate,
    endDate: subscription.endDate,
    subscriptionType: subscription.name || '包月方案',
    totalUsageLimit,
    usedCount: 0,
    reservedCount: 0,
    subscriptionPrice,
    status: 'ACTIVE',
    statusName: '有效',
    isExpired: false,
    isActive: true,
    daysUntilExpiry: getDaysDiff(TODAY, subscription.endDate),
    remainingUsage: totalUsageLimit,
    notes: subscription.notes || subscription.note,
    createUser: 'admin',
    createTime: now,
    modifyUser: 'admin',
    modifyTime: now,
    // 新增屬性
    name: subscription.name,
    serviceContent: subscription.serviceContent,
    totalTimes: subscription.totalTimes,
    totalAmount: subscription.totalAmount,
    paidAmount: subscription.paidAmount,
    note: subscription.note
  }

  mockSubscriptions.push(newSubscription)
  return recalculateSubscriptionStatus(newSubscription)
}

/**
 * 更新包月
 */
export function updateMockSubscription(id: number, subscription: SubscriptionUpdateRequest): Subscription | null {
  const index = mockSubscriptions.findIndex(s => s.subscriptionId === id || s.subscriptionId === subscription.id)
  if (index === -1) return null

  const existingSubscription = mockSubscriptions[index]
  const now = new Date().toISOString()

  mockSubscriptions[index] = {
    ...existingSubscription,
    startDate: subscription.startDate || existingSubscription.startDate,
    endDate: subscription.endDate || existingSubscription.endDate,
    subscriptionType: subscription.name || existingSubscription.subscriptionType,
    totalUsageLimit: subscription.totalUsageLimit || subscription.totalTimes || existingSubscription.totalUsageLimit,
    subscriptionPrice: subscription.subscriptionPrice || subscription.totalAmount || existingSubscription.subscriptionPrice,
    status: subscription.status || existingSubscription.status,
    notes: subscription.notes || subscription.note || existingSubscription.notes,
    modifyUser: 'admin',
    modifyTime: now,
    // 更新新增屬性
    name: subscription.name || existingSubscription.name,
    serviceContent: subscription.serviceContent || existingSubscription.serviceContent,
    totalTimes: subscription.totalTimes || existingSubscription.totalTimes,
    totalAmount: subscription.totalAmount || existingSubscription.totalAmount,
    paidAmount: subscription.paidAmount ?? existingSubscription.paidAmount,
    note: subscription.note || existingSubscription.note
  }

  return recalculateSubscriptionStatus(mockSubscriptions[index])
}

/**
 * 刪除包月
 */
export function deleteMockSubscription(id: number): boolean {
  const index = mockSubscriptions.findIndex(s => s.subscriptionId === id)
  if (index === -1) return false

  mockSubscriptions.splice(index, 1)
  return true
}

/**
 * 取得所有包月（不分頁）
 */
export function getAllMockSubscriptions(): Subscription[] {
  return mockSubscriptions.map(recalculateSubscriptionStatus)
}

/**
 * 使用包月次數
 */
export function useSubscription(id: number): boolean {
  const subscription = mockSubscriptions.find(s => s.subscriptionId === id)
  if (!subscription) return false

  const updated = recalculateSubscriptionStatus(subscription)
  if (!updated.isActive || updated.remainingUsage <= 0) return false

  subscription.usedCount += 1
  return true
}

/**
 * 預約包月次數
 */
export function reserveSubscription(id: number): boolean {
  const subscription = mockSubscriptions.find(s => s.subscriptionId === id)
  if (!subscription) return false

  const updated = recalculateSubscriptionStatus(subscription)
  if (!updated.isActive || updated.remainingUsage <= 0) return false

  subscription.reservedCount += 1
  return true
}

/**
 * 取消預約包月次數
 */
export function unreserveSubscription(id: number): boolean {
  const subscription = mockSubscriptions.find(s => s.subscriptionId === id)
  if (!subscription || subscription.reservedCount <= 0) return false

  subscription.reservedCount -= 1
  return true
}
