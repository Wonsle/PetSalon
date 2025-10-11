/**
 * Contact Person API Handlers
 *
 * 處理聯絡人相關的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import type { ContactCreateRequest, ContactUpdateRequest, PetRelationInfo } from '@/types/contact'
import {
  getMockContacts,
  getMockContactById,
  createMockContact,
  updateMockContact,
  deleteMockContact,
  updateContactRelatedPets,
  getAllMockContacts
} from '../data/contacts'
import { getMockPetsByContactId, getMockPetById } from '../data/pets'
import { getSystemCodeName } from '../data/systemCodes'

export const contactHandlers = [
  // GET /api/contactperson - 獲取聯絡人列表
  http.get('/api/contactperson', async ({ request }) => {
    const url = new URL(request.url)
    const params = {
      page: Number(url.searchParams.get('page')) || 1,
      pageSize: Number(url.searchParams.get('pageSize')) || 10,
      keyword: url.searchParams.get('keyword') || ''
    }

    await delay(500)

    const result = getMockContacts(params)
    return HttpResponse.json(result)
  }),

  // GET /api/contactperson/:id - 獲取聯絡人詳情
  http.get('/api/contactperson/:id', async ({ params }) => {
    await delay(300)

    const contact = getMockContactById(Number(params.id))

    if (!contact) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Contact not found'
      })
    }

    return HttpResponse.json(contact)
  }),

  // POST /api/contactperson - 創建聯絡人
  http.post('/api/contactperson', async ({ request }) => {
    await delay(800)

    try {
      const body = await request.json() as ContactCreateRequest
      const newContact = createMockContact(body)

      // 返回新聯絡人的 ID
      return HttpResponse.json(newContact.contactPersonId, { status: 201 })
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

  // PUT /api/contactperson/:id - 更新聯絡人
  http.put('/api/contactperson/:id', async ({ params, request }) => {
    await delay(600)

    try {
      const body = await request.json() as ContactUpdateRequest
      const updatedContact = updateMockContact(Number(params.id), body)

      if (!updatedContact) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'Contact not found'
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

  // DELETE /api/contactperson/:id - 刪除聯絡人
  http.delete('/api/contactperson/:id', async ({ params }) => {
    await delay(400)

    const success = deleteMockContact(Number(params.id))

    if (!success) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Contact not found'
      })
    }

    return new HttpResponse(null, { status: 204 })
  }),

  // GET /api/contactperson/search - 搜尋聯絡人
  http.get('/api/contactperson/search', async ({ request }) => {
    const url = new URL(request.url)
    const keyword = url.searchParams.get('keyword') || ''

    await delay(400)

    // 使用現有的 getMockContacts 函數進行搜尋
    const result = getMockContacts({ keyword, page: 1, pageSize: 100 })

    // 只返回陣列，不是分頁格式
    return HttpResponse.json(result.data)
  }),

  // GET /api/contactperson/pet/:petId - 獲取寵物的聯絡人
  http.get('/api/contactperson/pet/:petId', async ({ params }) => {
    await delay(400)

    const petId = Number(params.petId)
    const pet = getMockPetById(petId)

    if (!pet) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Pet not found'
      })
    }

    // 從所有聯絡人中找出與此寵物相關的
    const allContacts = getAllMockContacts()
    const relatedContacts = allContacts.filter(contact =>
      contact.relatedPets?.some(rp => rp.petId === petId)
    )

    return HttpResponse.json(relatedContacts)
  }),

  // POST /api/contactperson/:contactId/pets/:petId - 關聯寵物
  http.post('/api/contactperson/:contactId/pets/:petId', async ({ params, request }) => {
    await delay(600)

    const contactId = Number(params.contactId)
    const petId = Number(params.petId)

    const contact = getMockContactById(contactId)
    const pet = getMockPetById(petId)

    if (!contact) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Contact not found'
      })
    }

    if (!pet) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Pet not found'
      })
    }

    try {
      const body = await request.json() as { relationshipType: string; sort?: number }

      // 更新聯絡人的 relatedPets
      const currentRelatedPets = contact.relatedPets || []
      const existingIndex = currentRelatedPets.findIndex(rp => rp.petId === petId)

      let updatedRelatedPets: PetRelationInfo[]
      if (existingIndex >= 0) {
        // 更新現有關係
        updatedRelatedPets = [...currentRelatedPets]
        updatedRelatedPets[existingIndex] = {
          ...updatedRelatedPets[existingIndex],
          relationshipType: body.relationshipType,
          relationshipTypeName: getSystemCodeName('Relationship', body.relationshipType),
          sort: body.sort ?? updatedRelatedPets[existingIndex].sort
        }
      } else {
        // 新增關係 - 需要完整的 PetRelationInfo 結構
        const newRelation: PetRelationInfo = {
          petRelationId: Date.now(), // 生成臨時 ID
          petId,
          petName: pet.petName,
          breed: pet.breed,
          gender: pet.gender,
          relationshipType: body.relationshipType,
          relationshipTypeName: getSystemCodeName('Relationship', body.relationshipType),
          sort: body.sort ?? currentRelatedPets.length + 1
        }
        updatedRelatedPets = [...currentRelatedPets, newRelation]
      }

      updateContactRelatedPets(contactId, updatedRelatedPets)

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

  // DELETE /api/contactperson/:contactId/pets/:petId - 取消關聯
  http.delete('/api/contactperson/:contactId/pets/:petId', async ({ params }) => {
    await delay(400)

    const contactId = Number(params.contactId)
    const petId = Number(params.petId)

    const contact = getMockContactById(contactId)

    if (!contact) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'Contact not found'
      })
    }

    // 移除關係
    const updatedRelatedPets = (contact.relatedPets || []).filter(rp => rp.petId !== petId)
    updateContactRelatedPets(contactId, updatedRelatedPets)

    return new HttpResponse(null, { status: 204 })
  })
]
