/**
 * Dashboard API Handlers
 *
 * 處理儀表板相關的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import {
  getDashboardStatistics,
  getTodayReservations,
  getMonthlyRevenue,
  getActiveSubscriptionsCount
} from '../data/dashboard'

export const dashboardHandlers = [
  // GET /api/dashboard/statistics - 儀表板統計
  http.get('/api/dashboard/statistics', async () => {
    await delay(500)

    const statistics = getDashboardStatistics()
    return HttpResponse.json(statistics)
  }),

  // GET /api/dashboard/today-reservations - 今日預約
  http.get('/api/dashboard/today-reservations', async () => {
    await delay(400)

    const todayReservations = getTodayReservations()
    return HttpResponse.json(todayReservations)
  }),

  // GET /api/dashboard/monthly-revenue - 月收入
  http.get('/api/dashboard/monthly-revenue', async ({ request }) => {
    const url = new URL(request.url)
    const month = url.searchParams.get('month') ? Number(url.searchParams.get('month')) : undefined
    const year = url.searchParams.get('year') ? Number(url.searchParams.get('year')) : undefined

    await delay(400)

    const monthlyRevenue = getMonthlyRevenue(month, year)
    return HttpResponse.json(monthlyRevenue)
  }),

  // GET /api/dashboard/active-subscriptions-count - 有效包月數
  http.get('/api/dashboard/active-subscriptions-count', async () => {
    await delay(300)

    const count = getActiveSubscriptionsCount()
    return HttpResponse.json(count)
  })
]
