/**
 * Common API Handlers
 *
 * 處理系統代碼和共用功能的 API 請求
 */
import { http, HttpResponse, delay } from 'msw'
import type { SystemCode } from '../data/systemCodes'
import {
  getSystemCodesByType,
  getAllSystemCodeTypes,
  addSystemCode,
  updateSystemCode,
  deleteSystemCode
} from '../data/systemCodes'

export const commonHandlers = [
  // GET /api/Common/systemcodes/:type - 獲取特定類型系統代碼
  http.get('/api/Common/systemcodes/:type', async ({ params }) => {
    await delay(400)

    const codeType = params.type as string
    const codes = getSystemCodesByType(codeType)

    return HttpResponse.json(codes)
  }),

  // GET /api/Common/systemcode-types - 獲取所有代碼類型
  http.get('/api/Common/systemcode-types', async () => {
    await delay(300)

    const types = getAllSystemCodeTypes()
    return HttpResponse.json(types)
  }),

  // POST /api/Common/systemcodes - 創建系統代碼
  http.post('/api/Common/systemcodes', async ({ request }) => {
    await delay(800)

    try {
      const body = await request.json() as Omit<SystemCode, 'id'>
      const newCode = addSystemCode(body)

      return HttpResponse.json(newCode, { status: 201 })
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

  // PUT /api/Common/systemcodes/:id - 更新系統代碼
  http.put('/api/Common/systemcodes/:id', async ({ params, request }) => {
    await delay(600)

    try {
      const body = await request.json() as Partial<SystemCode>
      const updatedCode = updateSystemCode(Number(params.id), body)

      if (!updatedCode) {
        return new HttpResponse(null, {
          status: 404,
          statusText: 'System code not found'
        })
      }

      return HttpResponse.json(updatedCode)
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

  // DELETE /api/Common/systemcodes/:id - 刪除系統代碼
  http.delete('/api/Common/systemcodes/:id', async ({ params }) => {
    await delay(400)

    const success = deleteSystemCode(Number(params.id))

    if (!success) {
      return new HttpResponse(null, {
        status: 404,
        statusText: 'System code not found'
      })
    }

    return new HttpResponse(null, { status: 204 })
  }),

  // POST /api/Common/upload-photo - 上傳照片
  http.post('/api/Common/upload-photo', async ({ request }) => {
    await delay(1200)

    try {
      // 模擬處理 FormData
      const formData = await request.formData()
      const file = formData.get('file')

      if (!file) {
        return new HttpResponse(
          JSON.stringify({ message: 'No file provided' }),
          {
            status: 400,
            headers: { 'Content-Type': 'application/json' }
          }
        )
      }

      // 生成假的檔案名稱和 URL
      const timestamp = Date.now()
      const randomId = Math.random().toString(36).substring(7)
      const fileName = `pet_${timestamp}_${randomId}.jpg`
      const photoUrl = `/uploads/pets/${fileName}`

      return HttpResponse.json({
        photoUrl,
        fileName
      }, { status: 200 })
    } catch (error) {
      return new HttpResponse(
        JSON.stringify({ message: 'Upload failed' }),
        {
          status: 500,
          headers: { 'Content-Type': 'application/json' }
        }
      )
    }
  })
]
