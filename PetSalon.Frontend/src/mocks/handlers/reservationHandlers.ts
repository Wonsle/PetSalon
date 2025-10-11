/**
 * Reservation API Handlers
 *
 * 處理預約相關的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import type {
  ReservationCreateRequest,
  ReservationUpdateRequest,
  CostCalculationRequest,
  DurationCalculationRequest
} from '@/types/reservation'
import {
  getMockReservations,
  getMockReservationById,
  getMockReservationsForCalendar,
  getTodayMockReservations,
  createMockReservation,
  updateMockReservation,
  deleteMockReservation,
  calculateMockReservationCost,
  getAllMockReservations
} from '../data/reservations'

export const reservationHandlers = [
  // GET /api/reservation - 獲取預約列表
  http.get('/api/reservation', async ({ request }) => {
    const url = new URL(request.url)
    const params = {
      page: Number(url.searchParams.get('page')) || 1,
      pageSize: Number(url.searchParams.get('pageSize')) || 10,
      keyword: url.searchParams.get('keyword') || '',
      status: url.searchParams.get('status') || undefined,
      startDate: url.searchParams.get('startDate') || undefined,
      endDate: url.searchParams.get('endDate') || undefined,
      petId: url.searchParams.get('petId') ? Number(url.searchParams.get('petId')) : undefined,
      ownerId: url.searchParams.get('ownerId') ? Number(url.searchParams.get('ownerId')) : undefined,
      designer: url.searchParams.get('designer') || undefined
    }

    await delay(500)

    const result = getMockReservations(params)
    return HttpResponse.json(result)
  }),

  // GET /api/reservation/:id - 獲取預約詳情
  http.get('/api/reservation/:id', async ({ params }) => {
    await delay(300)

    const reservation = getMockReservationById(Number(params.id))

    if (!reservation) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Reservation not found'
      })
    }

    return HttpResponse.json(reservation)
  }),

  // POST /api/reservation - 創建預約
  http.post('/api/reservation', async ({ request }) => {
    await delay(800)

    try {
      const body = await request.json() as ReservationCreateRequest
      const newReservation = createMockReservation(body)

      // 返回完整的 Reservation 物件
      return HttpResponse.json(newReservation, { status: 201 })
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  }),

  // PUT /api/reservation/:id - 更新預約
  http.put('/api/reservation/:id', async ({ params, request }) => {
    await delay(600)

    try {
      const body = await request.json() as ReservationUpdateRequest
      const updatedReservation = updateMockReservation(Number(params.id), body)

      if (!updatedReservation) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Reservation not found'
        })
      }

      // 返回更新後的 Reservation
      return HttpResponse.json(updatedReservation)
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  }),

  // PATCH /api/reservation/:id/status - 更新預約狀態
  http.patch('/api/reservation/:id/status', async ({ params, request }) => {
    await delay(400)

    try {
      const body = await request.json() as { status: string }
      const reservation = getMockReservationById(Number(params.id))

      if (!reservation) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Reservation not found'
        })
      }

      // 使用 updateMockReservation 更新狀態
      const updatedReservation = updateMockReservation(Number(params.id), {
        id: Number(params.id),
        petId: reservation.petId,
        subscriptionId: reservation.subscriptionId,
        reserveDate: reservation.reserveDate,
        reserveTime: reservation.reserveTime,
        serviceType: reservation.serviceType,
        designer: reservation.designer,
        note: reservation.note,
        status: body.status
      })

      return HttpResponse.json(updatedReservation)
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  }),

  // DELETE /api/reservation/:id - 刪除預約
  http.delete('/api/reservation/:id', async ({ params }) => {
    await delay(400)

    const success = deleteMockReservation(Number(params.id))

    if (!success) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Reservation not found'
      })
    }

    return new HttpResponse(null, { status: 204 })
  }),

  // GET /api/reservation/calendar - 日曆格式資料
  http.get('/api/reservation/calendar', async ({ request }) => {
    const url = new URL(request.url)
    const startDate = url.searchParams.get('startDate') || ''
    const endDate = url.searchParams.get('endDate') || ''

    await delay(500)

    if (!startDate || !endDate) {
      return new HttpResponse(
        JSON.stringify({ message: 'startDate and endDate are required' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }

    const events = getMockReservationsForCalendar(startDate, endDate)
    return HttpResponse.json(events)
  }),

  // GET /api/reservation/availability - 檢查可用性
  http.get('/api/reservation/availability', async ({ request }) => {
    const url = new URL(request.url)
    const date = url.searchParams.get('date') || ''
    const time = url.searchParams.get('time') || ''

    await delay(400)

    if (!date || !time) {
      return new HttpResponse(
        JSON.stringify({ message: 'date and time are required' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }

    // 檢查該時段是否有衝突
    const allReservations = getAllMockReservations()
    const conflicts = allReservations.filter(
      r => r.reserveDate === date && r.reserveTime === time && r.status !== 'Cancelled'
    )

    const available = conflicts.length === 0

    return HttpResponse.json({
      available,
      conflicts
    })
  }),

  // POST /api/reservation/calculate-cost - 計算費用
  http.post('/api/reservation/calculate-cost', async ({ request }) => {
    await delay(500)

    try {
      const body = await request.json() as CostCalculationRequest
      const costResult = calculateMockReservationCost(body)

      return HttpResponse.json(costResult)
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  }),

  // POST /api/reservation/pet/:petId/calculate-duration - 計算服務時長
  http.post('/api/reservation/pet/:petId/calculate-duration', async ({ params, request }) => {
    await delay(400)

    try {
      const body = await request.json() as DurationCalculationRequest
      const petId = Number(params.petId)

      // 模擬計算時長（每個服務 30-60 分鐘）
      const serviceCount = body.serviceIds.length
      const totalDuration = serviceCount * 45 // 平均 45 分鐘

      // 模擬服務細節
      const breakdown = {
        services: body.serviceIds.map((id, index) => ({
          serviceId: id,
          serviceName: `Service ${id}`,
          duration: 45
        })),
        addons: []
      }

      return HttpResponse.json({
        totalDuration,
        estimatedStartTime: '10:00',
        estimatedEndTime: `${10 + Math.floor(totalDuration / 60)}:${(totalDuration % 60).toString().padStart(2, '0')}`,
        breakdown
      })
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  }),

  // GET /api/reservation/pet/:petId/addon-prices - 獲取附加服務價格
  http.get('/api/reservation/pet/:petId/addon-prices', async ({ params }) => {
    await delay(300)

    const petId = Number(params.petId)

    // 模擬附加服務價格列表
    const addonPrices = [
      { addonId: 1, addonName: '造型加價', price: 200, isCustomPrice: false },
      { addonId: 2, addonName: '貴賓腳', price: 150, isCustomPrice: false },
      { addonId: 3, addonName: '除蚤處理', price: 300, isCustomPrice: false },
      { addonId: 4, addonName: '指甲彩繪', price: 100, isCustomPrice: false },
      { addonId: 5, addonName: '香水', price: 50, isCustomPrice: false },
      { addonId: 6, addonName: 'SPA護理', price: 500, isCustomPrice: false }
    ]

    return HttpResponse.json(addonPrices)
  })
]
