/**
 * Mock 預約資料
 * 提供預約的 CRUD 操作和查詢功能
 */

import type { Reservation, ReservationCreateRequest, ReservationUpdateRequest, ReservationSearchParams, ReservationListResponse, CalendarEvent, CostCalculationRequest, CostCalculationResponse } from '@/types/reservation'
import type { TodayReservationDto } from '@/api/dashboard'
import { getMockPetById } from './pets'
import { getMockSubscriptionById, reserveSubscription, unreserveSubscription } from './subscriptions'
import { getSystemCodeName } from './systemCodes'

// 今天的日期（2025-10-11）
const TODAY = '2025-10-11'

// 預約資料
const mockReservations: Reservation[] = [
  // 今日預約 (2025-10-11) - 8筆
  {
    id: 1,
    petId: 1,
    petName: '小白',
    ownerId: 1,
    ownerName: '王小明',
    contactPhone: '0912-345-678',
    subscriptionId: 1,
    subscriptionName: '洗澡包月',
    reserveDate: '2025-10-11',
    reserveTime: '09:00',
    serviceType: 'BATH',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '寵物對吹風機敏感',
    createTime: '2025-10-09T14:00:00',
    updateTime: '2025-10-09T14:00:00'
  },
  {
    id: 2,
    petId: 5,
    petName: '妞妞',
    ownerId: 5,
    ownerName: '林志明',
    contactPhone: '0956-789-012',
    reserveDate: '2025-10-11',
    reserveTime: '10:30',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-08T10:00:00',
    updateTime: '2025-10-08T10:00:00'
  },
  {
    id: 3,
    petId: 9,
    petName: '皮皮',
    ownerId: 9,
    ownerName: '鄭美玲',
    contactPhone: '0910-123-456',
    reserveDate: '2025-10-11',
    reserveTime: '11:00',
    serviceType: 'NAIL_TRIM',
    designer: '美容師 A',
    status: 'IN_PROGRESS',
    note: '指甲較硬，需要特別工具',
    createTime: '2025-10-10T09:00:00',
    updateTime: '2025-10-11T11:00:00'
  },
  {
    id: 4,
    petId: 13,
    petName: 'lucky',
    ownerId: 13,
    ownerName: '許雅芳',
    contactPhone: '0954-567-890',
    subscriptionId: 3,
    subscriptionName: '洗澡包月',
    reserveDate: '2025-10-11',
    reserveTime: '13:00',
    serviceType: 'BATH',
    designer: '美容師 C',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T16:00:00',
    updateTime: '2025-10-09T16:00:00'
  },
  {
    id: 5,
    petId: 7,
    petName: '哈利',
    ownerId: 7,
    ownerName: '吳佳蓉',
    contactPhone: '0978-901-234',
    subscriptionId: 4,
    subscriptionName: '美容包月',
    reserveDate: '2025-10-11',
    reserveTime: '14:00',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'PENDING',
    note: '大型犬，需預留較長時間',
    createTime: '2025-10-10T15:00:00',
    updateTime: '2025-10-10T15:00:00'
  },
  {
    id: 6,
    petId: 11,
    petName: '可可',
    ownerId: 11,
    ownerName: '周詩涵',
    contactPhone: '0932-345-678',
    reserveDate: '2025-10-11',
    reserveTime: '15:30',
    serviceType: 'STYLING',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '需要特殊造型',
    createTime: '2025-10-08T11:00:00',
    updateTime: '2025-10-08T11:00:00'
  },
  {
    id: 7,
    petId: 2,
    petName: '球球',
    ownerId: 2,
    ownerName: '李小美',
    contactPhone: '0923-456-789',
    subscriptionId: 2,
    subscriptionName: '美容包月',
    reserveDate: '2025-10-11',
    reserveTime: '16:00',
    serviceType: 'GROOMING',
    designer: '美容師 C',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T09:00:00',
    updateTime: '2025-10-09T09:00:00'
  },
  {
    id: 8,
    petId: 15,
    petName: '虎斑',
    ownerId: 15,
    ownerName: '賴文馨',
    contactPhone: '0976-789-012',
    reserveDate: '2025-10-11',
    reserveTime: '17:00',
    serviceType: 'BATH',
    designer: '美容師 B',
    status: 'PENDING',
    note: '',
    createTime: '2025-10-10T18:00:00',
    updateTime: '2025-10-10T18:00:00'
  },

  // 本週預約（2025-10-12 ~ 2025-10-18）- 15筆
  {
    id: 9,
    petId: 3,
    petName: '柴柴',
    ownerId: 3,
    ownerName: '張大同',
    contactPhone: '0934-567-890',
    subscriptionId: 7,
    subscriptionName: '洗澡包月',
    reserveDate: '2025-10-12',
    reserveTime: '10:00',
    serviceType: 'BATH',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-08T14:00:00',
    updateTime: '2025-10-08T14:00:00'
  },
  {
    id: 10,
    petId: 4,
    petName: '布丁',
    ownerId: 4,
    ownerName: '陳雅婷',
    contactPhone: '0945-678-901',
    reserveDate: '2025-10-12',
    reserveTime: '14:00',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '法鬥需要特別注意呼吸',
    createTime: '2025-10-09T10:00:00',
    updateTime: '2025-10-09T10:00:00'
  },
  {
    id: 11,
    petId: 6,
    petName: '豆豆',
    ownerId: 6,
    ownerName: '黃淑芬',
    contactPhone: '0967-890-123',
    reserveDate: '2025-10-13',
    reserveTime: '09:30',
    serviceType: 'BATH',
    designer: '美容師 C',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-10T08:00:00',
    updateTime: '2025-10-10T08:00:00'
  },
  {
    id: 12,
    petId: 8,
    petName: '雪莉',
    ownerId: 8,
    ownerName: '劉建宏',
    contactPhone: '0989-012-345',
    reserveDate: '2025-10-13',
    reserveTime: '11:00',
    serviceType: 'STYLING',
    designer: '美容師 A',
    status: 'PENDING',
    note: '需要蝴蝶結裝飾',
    createTime: '2025-10-10T10:00:00',
    updateTime: '2025-10-10T10:00:00'
  },
  {
    id: 13,
    petId: 10,
    petName: '麥克',
    ownerId: 10,
    ownerName: '謝家豪',
    contactPhone: '0921-234-567',
    subscriptionId: 8,
    subscriptionName: '美容包月',
    reserveDate: '2025-10-14',
    reserveTime: '10:00',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T15:00:00',
    updateTime: '2025-10-09T15:00:00'
  },
  {
    id: 14,
    petId: 12,
    petName: '旺財',
    ownerId: 12,
    ownerName: '蔡志強',
    contactPhone: '0943-456-789',
    reserveDate: '2025-10-14',
    reserveTime: '15:00',
    serviceType: 'BATH',
    designer: '美容師 C',
    status: 'CONFIRMED',
    note: '大型犬，需兩位美容師',
    createTime: '2025-10-08T12:00:00',
    updateTime: '2025-10-08T12:00:00'
  },
  {
    id: 15,
    petId: 14,
    petName: '甜甜',
    ownerId: 14,
    ownerName: '楊宗憲',
    contactPhone: '0965-678-901',
    reserveDate: '2025-10-15',
    reserveTime: '09:00',
    serviceType: 'GROOMING',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-10T14:00:00',
    updateTime: '2025-10-10T14:00:00'
  },
  {
    id: 16,
    petId: 16,
    petName: '小虎',
    ownerId: 16,
    ownerName: '徐國偉',
    contactPhone: '0987-890-123',
    reserveDate: '2025-10-15',
    reserveTime: '13:00',
    serviceType: 'BATH',
    designer: '美容師 B',
    status: 'PENDING',
    note: '哈士奇掉毛嚴重',
    createTime: '2025-10-10T16:00:00',
    updateTime: '2025-10-10T16:00:00'
  },
  {
    id: 17,
    petId: 17,
    petName: '米糕',
    ownerId: 17,
    ownerName: '潘怡君',
    contactPhone: '0998-901-234',
    reserveDate: '2025-10-16',
    reserveTime: '10:30',
    serviceType: 'GROOMING',
    designer: '美容師 C',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T11:00:00',
    updateTime: '2025-10-09T11:00:00'
  },
  {
    id: 18,
    petId: 18,
    petName: 'Nana',
    ownerId: 1,
    ownerName: '王小明',
    contactPhone: '0912-345-678',
    reserveDate: '2025-10-16',
    reserveTime: '14:00',
    serviceType: 'NAIL_TRIM',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '小型犬，動作要輕',
    createTime: '2025-10-08T15:00:00',
    updateTime: '2025-10-08T15:00:00'
  },
  {
    id: 19,
    petId: 19,
    petName: '威利',
    ownerId: 2,
    ownerName: '李小美',
    contactPhone: '0923-456-789',
    reserveDate: '2025-10-17',
    reserveTime: '09:00',
    serviceType: 'BATH',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-10T09:00:00',
    updateTime: '2025-10-10T09:00:00'
  },
  {
    id: 20,
    petId: 20,
    petName: '小乖',
    ownerId: 3,
    ownerName: '張大同',
    contactPhone: '0934-567-890',
    reserveDate: '2025-10-17',
    reserveTime: '11:00',
    serviceType: 'GROOMING',
    designer: '美容師 C',
    status: 'PENDING',
    note: '',
    createTime: '2025-10-10T11:00:00',
    updateTime: '2025-10-10T11:00:00'
  },
  {
    id: 21,
    petId: 21,
    petName: '樂樂',
    ownerId: 4,
    ownerName: '陳雅婷',
    contactPhone: '0945-678-901',
    subscriptionId: 5,
    subscriptionName: '洗澡包月',
    reserveDate: '2025-10-18',
    reserveTime: '10:00',
    serviceType: 'BATH',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T13:00:00',
    updateTime: '2025-10-09T13:00:00'
  },
  {
    id: 22,
    petId: 22,
    petName: 'Cookie',
    ownerId: 5,
    ownerName: '林志明',
    contactPhone: '0956-789-012',
    reserveDate: '2025-10-18',
    reserveTime: '14:30',
    serviceType: 'STYLING',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '柯基造型',
    createTime: '2025-10-08T17:00:00',
    updateTime: '2025-10-08T17:00:00'
  },
  {
    id: 23,
    petId: 1,
    petName: '小白',
    ownerId: 1,
    ownerName: '王小明',
    contactPhone: '0912-345-678',
    reserveDate: '2025-10-18',
    reserveTime: '16:00',
    serviceType: 'SPA',
    designer: '美容師 C',
    status: 'PENDING',
    note: 'SPA護理療程',
    createTime: '2025-10-10T13:00:00',
    updateTime: '2025-10-10T13:00:00'
  },

  // 本月其他預約 - 7筆
  {
    id: 24,
    petId: 5,
    petName: '妞妞',
    ownerId: 5,
    ownerName: '林志明',
    contactPhone: '0956-789-012',
    reserveDate: '2025-10-20',
    reserveTime: '10:00',
    serviceType: 'GROOMING',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-09T08:00:00',
    updateTime: '2025-10-09T08:00:00'
  },
  {
    id: 25,
    petId: 7,
    petName: '哈利',
    ownerId: 7,
    ownerName: '吳佳蓉',
    contactPhone: '0978-901-234',
    reserveDate: '2025-10-22',
    reserveTime: '09:00',
    serviceType: 'BATH',
    designer: '美容師 B',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-10T07:00:00',
    updateTime: '2025-10-10T07:00:00'
  },
  {
    id: 26,
    petId: 9,
    petName: '皮皮',
    ownerId: 9,
    ownerName: '鄭美玲',
    contactPhone: '0910-123-456',
    reserveDate: '2025-10-25',
    reserveTime: '11:00',
    serviceType: 'NAIL_TRIM',
    designer: '美容師 C',
    status: 'PENDING',
    note: '',
    createTime: '2025-10-08T18:00:00',
    updateTime: '2025-10-08T18:00:00'
  },
  {
    id: 27,
    petId: 11,
    petName: '可可',
    ownerId: 11,
    ownerName: '周詩涵',
    contactPhone: '0932-345-678',
    subscriptionId: 5,
    subscriptionName: '洗澡包月',
    reserveDate: '2025-10-27',
    reserveTime: '14:00',
    serviceType: 'BATH',
    designer: '美容師 A',
    status: 'CONFIRMED',
    note: '',
    createTime: '2025-10-10T12:00:00',
    updateTime: '2025-10-10T12:00:00'
  },
  {
    id: 28,
    petId: 13,
    petName: 'lucky',
    ownerId: 13,
    ownerName: '許雅芳',
    contactPhone: '0954-567-890',
    reserveDate: '2025-10-28',
    reserveTime: '10:30',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'PENDING',
    note: '',
    createTime: '2025-10-09T14:00:00',
    updateTime: '2025-10-09T14:00:00'
  },

  // 已完成和已取消的預約
  {
    id: 29,
    petId: 2,
    petName: '球球',
    ownerId: 2,
    ownerName: '李小美',
    contactPhone: '0923-456-789',
    reserveDate: '2025-10-08',
    reserveTime: '10:00',
    serviceType: 'GROOMING',
    designer: '美容師 A',
    status: 'COMPLETED',
    note: '',
    createTime: '2025-10-06T10:00:00',
    updateTime: '2025-10-08T12:00:00'
  },
  {
    id: 30,
    petId: 4,
    petName: '布丁',
    ownerId: 4,
    ownerName: '陳雅婷',
    contactPhone: '0945-678-901',
    reserveDate: '2025-10-09',
    reserveTime: '14:00',
    serviceType: 'BATH',
    designer: '美容師 B',
    status: 'COMPLETED',
    note: '',
    createTime: '2025-10-07T09:00:00',
    updateTime: '2025-10-09T15:30:00'
  },
  {
    id: 31,
    petId: 6,
    petName: '豆豆',
    ownerId: 6,
    ownerName: '黃淑芬',
    contactPhone: '0967-890-123',
    reserveDate: '2025-10-10',
    reserveTime: '09:00',
    serviceType: 'GROOMING',
    designer: '美容師 C',
    status: 'COMPLETED',
    note: '',
    createTime: '2025-10-08T08:00:00',
    updateTime: '2025-10-10T11:00:00'
  },
  {
    id: 32,
    petId: 8,
    petName: '雪莉',
    ownerId: 8,
    ownerName: '劉建宏',
    contactPhone: '0989-012-345',
    reserveDate: '2025-10-07',
    reserveTime: '11:00',
    serviceType: 'BATH',
    designer: '美容師 A',
    status: 'CANCELLED',
    note: '飼主臨時有事取消',
    createTime: '2025-10-05T10:00:00',
    updateTime: '2025-10-07T09:00:00'
  },
  {
    id: 33,
    petId: 10,
    petName: '麥克',
    ownerId: 10,
    ownerName: '謝家豪',
    contactPhone: '0921-234-567',
    reserveDate: '2025-10-05',
    reserveTime: '15:00',
    serviceType: 'GROOMING',
    designer: '美容師 B',
    status: 'COMPLETED',
    note: '',
    createTime: '2025-10-03T14:00:00',
    updateTime: '2025-10-05T17:00:00'
  }
]

/**
 * 取得預約列表（支援分頁和搜尋）
 */
export function getMockReservations(params?: ReservationSearchParams): ReservationListResponse {
  let filteredReservations = [...mockReservations]

  // 關鍵字搜尋
  if (params?.keyword) {
    const keyword = params.keyword.toLowerCase()
    filteredReservations = filteredReservations.filter(reservation =>
      reservation.petName.toLowerCase().includes(keyword) ||
      reservation.ownerName.toLowerCase().includes(keyword) ||
      reservation.contactPhone.includes(keyword)
    )
  }

  // 狀態搜尋
  if (params?.status) {
    filteredReservations = filteredReservations.filter(r => r.status === params.status)
  }

  // 開始日期搜尋
  if (params?.startDate) {
    filteredReservations = filteredReservations.filter(r => r.reserveDate >= params.startDate!)
  }

  // 結束日期搜尋
  if (params?.endDate) {
    filteredReservations = filteredReservations.filter(r => r.reserveDate <= params.endDate!)
  }

  // 寵物搜尋
  if (params?.petId) {
    filteredReservations = filteredReservations.filter(r => r.petId === params.petId)
  }

  // 飼主搜尋
  if (params?.ownerId) {
    filteredReservations = filteredReservations.filter(r => r.ownerId === params.ownerId)
  }

  // 美容師搜尋
  if (params?.designer) {
    filteredReservations = filteredReservations.filter(r => r.designer === params.designer)
  }

  // 分頁
  const page = params?.page || 1
  const pageSize = params?.pageSize || 10
  const startIndex = (page - 1) * pageSize
  const endIndex = startIndex + pageSize

  return {
    data: filteredReservations.slice(startIndex, endIndex),
    total: filteredReservations.length,
    page,
    pageSize
  }
}

/**
 * 根據 ID 取得單一預約
 */
export function getMockReservationById(id: number): Reservation | undefined {
  return mockReservations.find(reservation => reservation.id === id)
}

/**
 * 取得今日預約列表
 */
export function getTodayMockReservations(): TodayReservationDto[] {
  return mockReservations
    .filter(r => r.reserveDate === TODAY && r.status !== 'CANCELLED')
    .map(r => {
      const pet = getMockPetById(r.petId)
      const timeStr = r.reserveTime.replace(':', '')
      return {
        id: r.id,
        reserverTime: parseInt(timeStr),
        petName: r.petName,
        primaryContactName: pet?.primaryContact?.name || r.ownerName,
        primaryContactPhone: pet?.primaryContact?.phone || r.contactPhone,
        services: [getSystemCodeName('ServiceType', r.serviceType)],
        status: r.status
      }
    })
    .sort((a, b) => a.reserverTime - b.reserverTime)
}

/**
 * 取得日曆事件（指定日期範圍）
 */
export function getMockReservationsForCalendar(start: string, end: string): CalendarEvent[] {
  return mockReservations
    .filter(r => r.reserveDate >= start && r.reserveDate <= end && r.status !== 'CANCELLED')
    .map(r => {
      // 根據狀態設定顏色
      let backgroundColor = '#3b82f6' // 藍色 - 預設
      let borderColor = '#2563eb'

      switch (r.status) {
        case 'PENDING':
          backgroundColor = '#fbbf24' // 黃色
          borderColor = '#f59e0b'
          break
        case 'CONFIRMED':
          backgroundColor = '#3b82f6' // 藍色
          borderColor = '#2563eb'
          break
        case 'IN_PROGRESS':
          backgroundColor = '#8b5cf6' // 紫色
          borderColor = '#7c3aed'
          break
        case 'COMPLETED':
          backgroundColor = '#10b981' // 綠色
          borderColor = '#059669'
          break
        case 'CANCELLED':
          backgroundColor = '#6b7280' // 灰色
          borderColor = '#4b5563'
          break
        case 'NO_SHOW':
          backgroundColor = '#ef4444' // 紅色
          borderColor = '#dc2626'
          break
      }

      return {
        id: r.id,
        title: `${r.reserveTime} - ${r.petName} (${r.ownerName})`,
        start: `${r.reserveDate}T${r.reserveTime}:00`,
        end: `${r.reserveDate}T${r.reserveTime}:00`,
        backgroundColor,
        borderColor,
        textColor: '#ffffff',
        extendedProps: {
          reservation: r
        }
      }
    })
}

/**
 * 新增預約
 */
export function createMockReservation(reservation: ReservationCreateRequest): Reservation {
  const maxId = Math.max(...mockReservations.map(r => r.id), 0)
  const now = new Date().toISOString()
  const pet = getMockPetById(reservation.petId)

  if (!pet) {
    throw new Error('寵物不存在')
  }

  let subscriptionName: string | undefined
  if (reservation.subscriptionId) {
    const subscription = getMockSubscriptionById(reservation.subscriptionId)
    if (subscription) {
      subscriptionName = subscription.subscriptionType
      // 預約時增加包月的預約次數
      reserveSubscription(reservation.subscriptionId)
    }
  }

  const newReservation: Reservation = {
    id: maxId + 1,
    petId: reservation.petId,
    petName: pet.petName,
    ownerId: pet.primaryContact?.contactPersonId || 0,
    ownerName: pet.primaryContact?.name || '',
    contactPhone: pet.primaryContact?.phone || '',
    subscriptionId: reservation.subscriptionId,
    subscriptionName,
    reserveDate: reservation.reserveDate,
    reserveTime: reservation.reserveTime,
    serviceType: reservation.serviceType,
    designer: reservation.designer,
    status: 'PENDING',
    note: reservation.note,
    createTime: now,
    updateTime: now
  }

  mockReservations.push(newReservation)
  return newReservation
}

/**
 * 更新預約
 */
export function updateMockReservation(id: number, reservation: ReservationUpdateRequest): Reservation | null {
  const index = mockReservations.findIndex(r => r.id === id)
  if (index === -1) return null

  const existingReservation = mockReservations[index]
  const now = new Date().toISOString()
  const pet = getMockPetById(reservation.petId)

  if (!pet) {
    throw new Error('寵物不存在')
  }

  // 處理包月變更
  if (existingReservation.subscriptionId !== reservation.subscriptionId) {
    // 取消舊的包月預約
    if (existingReservation.subscriptionId) {
      unreserveSubscription(existingReservation.subscriptionId)
    }
    // 新增新的包月預約
    if (reservation.subscriptionId) {
      reserveSubscription(reservation.subscriptionId)
    }
  }

  let subscriptionName: string | undefined
  if (reservation.subscriptionId) {
    const subscription = getMockSubscriptionById(reservation.subscriptionId)
    subscriptionName = subscription?.subscriptionType
  }

  mockReservations[index] = {
    ...existingReservation,
    petId: reservation.petId,
    petName: pet.petName,
    ownerId: pet.primaryContact?.contactPersonId || existingReservation.ownerId,
    ownerName: pet.primaryContact?.name || existingReservation.ownerName,
    contactPhone: pet.primaryContact?.phone || existingReservation.contactPhone,
    subscriptionId: reservation.subscriptionId,
    subscriptionName,
    reserveDate: reservation.reserveDate,
    reserveTime: reservation.reserveTime,
    serviceType: reservation.serviceType,
    designer: reservation.designer,
    status: reservation.status || existingReservation.status,
    note: reservation.note,
    updateTime: now
  }

  return mockReservations[index]
}

/**
 * 刪除預約
 */
export function deleteMockReservation(id: number): boolean {
  const index = mockReservations.findIndex(r => r.id === id)
  if (index === -1) return false

  const reservation = mockReservations[index]

  // 如果有關聯包月，取消預約次數
  if (reservation.subscriptionId) {
    unreserveSubscription(reservation.subscriptionId)
  }

  mockReservations.splice(index, 1)
  return true
}

/**
 * 計算預約費用
 */
export function calculateMockReservationCost(request: CostCalculationRequest): CostCalculationResponse {
  const pet = getMockPetById(request.petId)
  if (!pet) {
    throw new Error('寵物不存在')
  }

  // 簡化計算：基本價格
  const basePrice = pet.normalPrice || 800
  const subscriptionPrice = pet.subscriptionPrice || 600

  // 如果使用包月，檢查包月是否有效
  let useSubscriptionPrice = false
  if (request.useSubscription && request.subscriptionId) {
    const subscription = getMockSubscriptionById(request.subscriptionId)
    if (subscription && subscription.isActive && subscription.remainingUsage > 0) {
      useSubscriptionPrice = true
    }
  }

  const finalPrice = useSubscriptionPrice ? subscriptionPrice : basePrice
  const discount = useSubscriptionPrice ? basePrice - subscriptionPrice : 0

  return {
    serviceTotal: basePrice,
    addonTotal: 0,
    discount,
    totalAmount: finalPrice,
    subscriptionDiscount: discount,
    breakdown: {
      services: [
        {
          serviceId: 1,
          serviceName: '基本服務',
          originalPrice: basePrice,
          finalPrice,
          discount
        }
      ],
      addons: []
    }
  }
}

/**
 * 取得所有預約（不分頁）
 */
export function getAllMockReservations(): Reservation[] {
  return [...mockReservations]
}
