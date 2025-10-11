/**
 * Mock 儀表板資料
 * 提供儀表板統計資料和計算函數
 */

import type { DashboardStatisticsDto, TodayReservationDto, MonthlyRevenueDto, ExpiringSubscriptionDto } from '@/api/dashboard'
import { getTodayMockReservations, getAllMockReservations } from './reservations'
import { getExpiringMockSubscriptions, getActiveMockSubscriptions, getAllMockSubscriptions } from './subscriptions'
import { getMockPets } from './pets'

/**
 * 計算本月收入
 */
function calculateMonthlyRevenue(month?: number, year?: number): number {
  const now = new Date('2025-10-11')
  const targetMonth = month || (now.getMonth() + 1)
  const targetYear = year || now.getFullYear()

  const allReservations = getAllMockReservations()
  const monthlyReservations = allReservations.filter(r => {
    const reserveDate = new Date(r.reserveDate)
    return reserveDate.getFullYear() === targetYear &&
      (reserveDate.getMonth() + 1) === targetMonth &&
      r.status === 'COMPLETED'
  })

  // 假設每筆完成的預約平均收入 1000 元
  const reservationRevenue = monthlyReservations.length * 1000

  // 計算本月新增的包月收入
  const allSubscriptions = getAllMockSubscriptions()
  const monthlySubscriptions = allSubscriptions.filter(s => {
    const subscriptionDate = new Date(s.subscriptionDate)
    return subscriptionDate.getFullYear() === targetYear &&
      (subscriptionDate.getMonth() + 1) === targetMonth
  })

  const subscriptionRevenue = monthlySubscriptions.reduce((sum, s) => sum + s.subscriptionPrice, 0)

  return reservationRevenue + subscriptionRevenue
}

/**
 * 取得儀表板統計資料
 */
export function getDashboardStatistics(): DashboardStatisticsDto {
  const todayReservations = getTodayMockReservations()
  const pets = getMockPets({})
  const activeSubscriptions = getActiveMockSubscriptions()
  const monthlyRevenue = calculateMonthlyRevenue()

  return {
    todayReservations: todayReservations.length,
    totalPets: pets.total,
    monthlyRevenue,
    activeSubscriptions: activeSubscriptions.length
  }
}

/**
 * 取得今日預約列表
 */
export function getTodayReservations(): TodayReservationDto[] {
  return getTodayMockReservations()
}

/**
 * 取得月收入統計
 * @param month 月份（1-12）
 * @param year 年份
 */
export function getMonthlyRevenue(month?: number, year?: number): MonthlyRevenueDto {
  const now = new Date('2025-10-11')
  const targetMonth = month || (now.getMonth() + 1)
  const targetYear = year || now.getFullYear()

  const allReservations = getAllMockReservations()
  const monthlyReservations = allReservations.filter(r => {
    const reserveDate = new Date(r.reserveDate)
    return reserveDate.getFullYear() === targetYear &&
      (reserveDate.getMonth() + 1) === targetMonth &&
      r.status === 'COMPLETED'
  })

  // 假設每筆完成的預約平均收入 1000 元
  const reservationRevenue = monthlyReservations.length * 1000

  // 計算本月新增的包月收入
  const allSubscriptions = getAllMockSubscriptions()
  const monthlySubscriptions = allSubscriptions.filter(s => {
    const subscriptionDate = new Date(s.subscriptionDate)
    return subscriptionDate.getFullYear() === targetYear &&
      (subscriptionDate.getMonth() + 1) === targetMonth
  })

  const subscriptionRevenue = monthlySubscriptions.reduce((sum, s) => sum + s.subscriptionPrice, 0)

  // 其他收入（例如：商品銷售）
  const otherRevenue = Math.floor(Math.random() * 5000) + 2000

  const totalRevenue = reservationRevenue + subscriptionRevenue + otherRevenue

  return {
    year: targetYear,
    month: targetMonth,
    totalRevenue,
    reservationRevenue,
    subscriptionRevenue,
    otherRevenue
  }
}

/**
 * 取得有效包月數量
 */
export function getActiveSubscriptionsCount(): number {
  return getActiveMockSubscriptions().length
}

/**
 * 取得即將到期的包月方案
 * @param days 天數，預設7天
 */
export function getExpiringSubscriptions(days: number = 7): ExpiringSubscriptionDto[] {
  return getExpiringMockSubscriptions(days)
}

/**
 * 取得本週預約數量統計（按日期分組）
 */
export function getWeeklyReservationStats(): { date: string; count: number }[] {
  const today = new Date('2025-10-11')
  const stats: { date: string; count: number }[] = []

  for (let i = 0; i < 7; i++) {
    const date = new Date(today)
    date.setDate(date.getDate() + i)
    const dateStr = date.toISOString().split('T')[0]

    const allReservations = getAllMockReservations()
    const count = allReservations.filter(r =>
      r.reserveDate === dateStr &&
      r.status !== 'CANCELLED'
    ).length

    stats.push({ date: dateStr, count })
  }

  return stats
}

/**
 * 取得熱門服務類型統計
 */
export function getPopularServiceTypes(): { serviceType: string; count: number }[] {
  const allReservations = getAllMockReservations()
  const serviceTypeMap = new Map<string, number>()

  allReservations
    .filter(r => r.status !== 'CANCELLED')
    .forEach(r => {
      const count = serviceTypeMap.get(r.serviceType) || 0
      serviceTypeMap.set(r.serviceType, count + 1)
    })

  return Array.from(serviceTypeMap.entries())
    .map(([serviceType, count]) => ({ serviceType, count }))
    .sort((a, b) => b.count - a.count)
}

/**
 * 取得美容師工作量統計
 */
export function getDesignerWorkload(): { designer: string; count: number }[] {
  const today = new Date('2025-10-11')
  const weekStart = new Date(today)
  weekStart.setDate(today.getDate() - today.getDay())
  const weekEnd = new Date(weekStart)
  weekEnd.setDate(weekStart.getDate() + 6)

  const weekStartStr = weekStart.toISOString().split('T')[0]
  const weekEndStr = weekEnd.toISOString().split('T')[0]

  const allReservations = getAllMockReservations()
  const designerMap = new Map<string, number>()

  allReservations
    .filter(r =>
      r.reserveDate >= weekStartStr &&
      r.reserveDate <= weekEndStr &&
      r.status !== 'CANCELLED'
    )
    .forEach(r => {
      const count = designerMap.get(r.designer) || 0
      designerMap.set(r.designer, count + 1)
    })

  return Array.from(designerMap.entries())
    .map(([designer, count]) => ({ designer, count }))
    .sort((a, b) => b.count - a.count)
}

/**
 * 取得營收趨勢（最近6個月）
 */
export function getRevenueTrend(): MonthlyRevenueDto[] {
  const trend: MonthlyRevenueDto[] = []
  const now = new Date('2025-10-11')

  for (let i = 5; i >= 0; i--) {
    const targetDate = new Date(now)
    targetDate.setMonth(now.getMonth() - i)

    const month = targetDate.getMonth() + 1
    const year = targetDate.getFullYear()

    const revenue = getMonthlyRevenue(month, year)
    trend.push(revenue)
  }

  return trend
}
