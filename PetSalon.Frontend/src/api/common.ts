import axios from '@/utils/axios'

export interface Breed {
  id: number
  name: string
  category: string
  description?: string
}

export interface SystemCode {
  id: number
  type: string
  code: string
  name: string
  value: string
  sort: number
  isActive: boolean
}

export const commonApi = {
  async getBreeds(): Promise<Breed[]> {
    const response = await axios.get('/api/common/breeds')
    return response.data
  },

  async getSystemCodes(type?: string): Promise<SystemCode[]> {
    const response = await axios.get('/api/common/systemcodes', {
      params: { type }
    })
    return response.data
  },

  async uploadFile(file: File, type: string = 'pet'): Promise<{ url: string, filename: string }> {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('type', type)

    const response = await axios.post('/api/common/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  }
}