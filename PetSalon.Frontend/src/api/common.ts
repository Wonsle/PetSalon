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
  createUser?: string
  createTime?: string
  updateUser?: string
  updateTime?: string
}

export const commonApi = {
  async getBreeds(): Promise<Breed[]> {
    // Use systemcodes instead of dedicated breeds endpoint
    const systemCodes = await this.getSystemCodes('Breed')
    return systemCodes.map(code => ({
      id: code.id,
      name: code.name,
      category: code.type,
      description: code.value
    }))
  },

  async getSystemCodes(type: string): Promise<SystemCode[]> {
    const response = await axios.get(`/api/Common/systemcodes/${type}`)
    return response.data
  },

  async getAllSystemCodes(): Promise<SystemCode[]> {
    // Get all types first
    const types = await this.getSystemCodeTypes()
    
    // Fetch codes for each type in parallel
    const allCodeArrays = await Promise.all(
      types.map(type => this.getSystemCodes(type))
    )
    
    // Flatten the arrays
    return allCodeArrays.flat()
  },

  async createSystemCode(data: Omit<SystemCode, 'id'>): Promise<SystemCode> {
    const response = await axios.post('/api/Common/systemcodes', data)
    return response.data
  },

  async updateSystemCode(data: SystemCode): Promise<void> {
    await axios.put(`/api/Common/systemcodes/${data.id}`, data)
  },

  async deleteSystemCode(id: number): Promise<void> {
    await axios.delete(`/api/Common/systemcodes/${id}`)
  },

  async getSystemCodeTypes(): Promise<string[]> {
    const response = await axios.get('/api/Common/systemcode-types')
    return response.data
  },

  async uploadFile(file: File, type: string = 'pet'): Promise<{ url: string, filename: string }> {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('type', type)

    const response = await axios.post('/api/Common/upload', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  }
}