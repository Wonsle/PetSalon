import axios from '@/utils/axios'

// Dashboard API interfaces
export interface DashboardStatisticsDto {
  todayReservations: number
  totalPets: number
  monthlyRevenue: number
  activeSubscriptions: number
}

export interface TodayReservationDto {
  id: number
  reserverTime: number
  petName: string
  primaryContactName: string
  primaryContactPhone: string
  services: string[]
  status: string
}

export interface MonthlyRevenueDto {
  year: number
  month: number
  totalRevenue: number
  reservationRevenue: number
  subscriptionRevenue: number
  otherRevenue: number
}

export interface ExpiringSubscriptionDto {
  id: number
  petId: number
  petName: string
  subscriptionType: string
  endDate: string
  daysLeft: number
  remainingUsage: number
  primaryContactName: string
  primaryContactPhone: string
}

export const dashboardApi = {
  /**
   * 取得儀表板統計資料
   */
  async getStatistics(): Promise<DashboardStatisticsDto> {
    const response = await axios.get('/api/dashboard/statistics')
    return response.data
  },

  /**
   * 取得今日預約列表
   */
  async getTodayReservations(): Promise<TodayReservationDto[]> {
    const response = await axios.get('/api/dashboard/today-reservations')
    return response.data
  },

  /**
   * 取得月收入統計
   * @param month 月份（可選）
   * @param year 年份（可選）
   */
  async getMonthlyRevenue(month?: number, year?: number): Promise<MonthlyRevenueDto> {
    const params = new URLSearchParams()
    if (month) params.append('month', month.toString())
    if (year) params.append('year', year.toString())
    
    const query = params.toString()
    const response = await axios.get(`/api/dashboard/monthly-revenue${query ? `?${query}` : ''}`)
    return response.data
  },

  /**
   * 取得有效包月數量
   */
  async getActiveSubscriptionsCount(): Promise<number> {
    const response = await axios.get('/api/dashboard/active-subscriptions-count')
    return response.data
  },

  /**
   * 取得即將到期的包月方案
   * @param days 天數，預設7天
   */
  async getExpiringSubscriptions(days: number = 7): Promise<ExpiringSubscriptionDto[]> {
    const response = await axios.get(`/api/subscription/expiring?days=${days}`)
    return response.data
  }
}