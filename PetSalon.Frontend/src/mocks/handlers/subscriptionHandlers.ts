/**
 * Subscription API Handlers
 *
 * 處理包月相關的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import type {
  SubscriptionCreateRequest,
  SubscriptionUpdateRequest,
  SubscriptionUsage
} from '@/types/subscription'
import {
  getMockSubscriptions,
  getMockSubscriptionById,
  getMockSubscriptionsByPetId,
  getActiveMockSubscriptions,
  getExpiringMockSubscriptions,
  createMockSubscription,
  updateMockSubscription,
  deleteMockSubscription,
  getAllMockSubscriptions
} from '../data/subscriptions'

export const subscriptionHandlers = [
  // GET /api/subscription - 獲取包月列表
  http.get('/api/subscription', async ({ request }) => {
    const url = new URL(request.url)
    const params = {
      petId: url.searchParams.get('petId') ? Number(url.searchParams.get('petId')) : undefined,
      status: url.searchParams.get('status') || undefined,
      startDate: url.searchParams.get('startDate') || undefined,
      endDate: url.searchParams.get('endDate') || undefined,
      page: Number(url.searchParams.get('page')) || 1,
      pageSize: Number(url.searchParams.get('pageSize')) || 10
    }

    await delay(500)

    // 如果沒有分頁參數，返回所有資料
    if (!url.searchParams.has('page') && !url.searchParams.has('pageSize')) {
      const allSubscriptions = getAllMockSubscriptions()
      return HttpResponse.json(allSubscriptions)
    }

    const result = getMockSubscriptions(params)
    return HttpResponse.json(result.data)
  }),

  // GET /api/subscription/:id - 獲取包月詳情
  http.get('/api/subscription/:id', async ({ params }) => {
    await delay(300)

    const subscription = getMockSubscriptionById(Number(params.id))

    if (!subscription) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Subscription not found'
      })
    }

    return HttpResponse.json(subscription)
  }),

  // POST /api/subscription - 創建包月
  http.post('/api/subscription', async ({ request }) => {
    await delay(800)

    try {
      const body = await request.json() as SubscriptionCreateRequest
      const newSubscription = createMockSubscription(body)

      // 返回新包月的 ID
      return HttpResponse.json(newSubscription.subscriptionId, { status: 201 })
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

  // PUT /api/subscription/:id - 更新包月
  http.put('/api/subscription/:id', async ({ params, request }) => {
    await delay(600)

    try {
      const body = await request.json() as SubscriptionUpdateRequest
      const updatedSubscription = updateMockSubscription(Number(params.id), body)

      if (!updatedSubscription) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Subscription not found'
        })
      }

      return new HttpResponse(null, { status: 204 })
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

  // DELETE /api/subscription/:id - 刪除包月
  http.delete('/api/subscription/:id', async ({ params }) => {
    await delay(400)

    const success = deleteMockSubscription(Number(params.id))

    if (!success) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Subscription not found'
      })
    }

    return new HttpResponse(null, { status: 204 })
  }),

  // GET /api/subscription/pet/:petId - 獲取寵物的包月
  http.get('/api/subscription/pet/:petId', async ({ params }) => {
    await delay(400)

    const subscriptions = getMockSubscriptionsByPetId(Number(params.petId))
    return HttpResponse.json(subscriptions)
  }),

  // GET /api/subscription/pet/:petId/active - 獲取有效包月
  http.get('/api/subscription/pet/:petId/active', async ({ params, request }) => {
    await delay(400)

    const url = new URL(request.url)
    const checkDate = url.searchParams.get('checkDate') || new Date().toISOString().split('T')[0]

    const subscriptions = getMockSubscriptionsByPetId(Number(params.petId))
    const activeSubscription = subscriptions.find(sub => {
      const start = new Date(sub.startDate)
      const end = new Date(sub.endDate)
      const check = new Date(checkDate)

      return check >= start && check <= end && sub.remainingUsage > 0
    })

    if (!activeSubscription) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'No active subscription found'
      })
    }

    return HttpResponse.json(activeSubscription)
  }),

  // GET /api/subscription/:id/usage - 使用情況
  http.get('/api/subscription/:id/usage', async ({ params }) => {
    await delay(300)

    const subscription = getMockSubscriptionById(Number(params.id))

    if (!subscription) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Subscription not found'
      })
    }

    // 計算使用情況
    const usage: SubscriptionUsage = {
      subscriptionId: subscription.subscriptionId,
      petName: subscription.petName || '',
      startDate: subscription.startDate,
      endDate: subscription.endDate,
      totalUsageLimit: subscription.totalUsageLimit,
      usedCount: subscription.usedCount,
      remainingUsage: subscription.remainingUsage,
      hasUnlimitedUsage: subscription.totalUsageLimit === -1,
      averageUsagePerMonth: subscription.usedCount > 0 ? subscription.usedCount / 1 : 0, // 簡化計算
      usageDates: [] // 可以加入實際使用日期
    }

    return HttpResponse.json(usage)
  }),

  // GET /api/subscription/:id/remaining - 剩餘次數
  http.get('/api/subscription/:id/remaining', async ({ params }) => {
    await delay(300)

    const subscription = getMockSubscriptionById(Number(params.id))

    if (!subscription) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Subscription not found'
      })
    }

    return HttpResponse.json(subscription.remainingUsage)
  }),

  // GET /api/subscription/expiring - 即將到期
  http.get('/api/subscription/expiring', async ({ request }) => {
    await delay(400)

    const url = new URL(request.url)
    const days = Number(url.searchParams.get('days')) || 7

    const expiringSubscriptions = getExpiringMockSubscriptions(days)
    return HttpResponse.json(expiringSubscriptions)
  }),

  // POST /api/subscription/:id/status - 更新狀態
  http.post('/api/subscription/:id/status', async ({ params, request }) => {
    await delay(500)

    try {
      const body = await request.json() as { status: string }
      const subscription = getMockSubscriptionById(Number(params.id))

      if (!subscription) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Subscription not found'
        })
      }

      // 更新狀態
      const updatedSubscription = updateMockSubscription(Number(params.id), {
        id: Number(params.id),
        status: body.status
      })

      return HttpResponse.json(updatedSubscription)
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Invalid request body' }),
        {
          status: 400,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  })
]
