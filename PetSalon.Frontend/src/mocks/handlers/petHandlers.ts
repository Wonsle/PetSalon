/**
 * Pet API Handlers
 *
 * 處理寵物相關的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import type { PetCreateRequest, PetUpdateRequest } from '@/types/pet'
import {
  getMockPets,
  getMockPetById,
  createMockPet,
  updateMockPet,
  deleteMockPet
} from '../data/pets'

export const petHandlers = [
  // GET /api/pet - 獲取寵物列表
  http.get('/api/pet', async ({ request }) => {
    const url = new URL(request.url)
    const params = {
      page: Number(url.searchParams.get('page')) || 1,
      pageSize: Number(url.searchParams.get('pageSize')) || 12,
      keyword: url.searchParams.get('keyword') || '',
      breed: url.searchParams.get('breed') || undefined,
      gender: url.searchParams.get('gender') || undefined,
      ownerId: url.searchParams.get('ownerId') ? Number(url.searchParams.get('ownerId')) : undefined
    }

    await delay(500)

    const result = getMockPets(params)
    return HttpResponse.json(result)
  }),

  // GET /api/pet/:id - 獲取寵物詳情
  http.get('/api/pet/:id', async ({ params }) => {
    await delay(300)

    const pet = getMockPetById(Number(params.id))

    if (!pet) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Pet not found'
      })
    }

    return HttpResponse.json(pet)
  }),

  // POST /api/pet - 創建寵物
  http.post('/api/pet', async ({ request }) => {
    await delay(800)

    try {
      const body = await request.json() as PetCreateRequest
      const newPet = createMockPet(body)

      // 返回新寵物的 ID
      return HttpResponse.json(newPet.petId, { status: 201 })
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

  // PUT /api/pet/:id - 更新寵物
  http.put('/api/pet/:id', async ({ params, request }) => {
    await delay(600)

    try {
      const body = await request.json() as PetUpdateRequest
      const updatedPet = updateMockPet(Number(params.id), body)

      if (!updatedPet) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Pet not found'
        })
      }

      // 返回 204 No Content
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

  // DELETE /api/pet/:id - 刪除寵物
  http.delete('/api/pet/:id', async ({ params }) => {
    await delay(400)

    const success = deleteMockPet(Number(params.id))

    if (!success) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Pet not found'
      })
    }

    return new HttpResponse(null, { status: 204 })
  })
]
