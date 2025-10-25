import axios from '@/utils/axios'

export interface Breed {
  id: number
  name: string
  category: string
  description?: string
}

export interface SystemCode {
  id: number
  codeType: string
  CodeTypeName: string // 新增：代碼類型顯示名稱（從後端取得）
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

export interface CodeTypeOption {
  label: string
  value: string
}

export const commonApi = {
  async getBreeds(): Promise<Breed[]> {
    // Use systemcodes instead of dedicated breeds endpoint
    const systemCodes = await this.getSystemCodes('Breed')
    return systemCodes.map(code => ({
      id: code.id,
      name: code.name,
      category: code.codeType,
      description: code.value
    }))
  },

  async getSystemCodes(type: string): Promise<SystemCode[]> {
    const response = await axios.get(`/api/Common/systemcodes/${type}`)
    return response.data
  },

  async getAllSystemCodes(): Promise<SystemCode[]> {
    // 使用新的 API 端點，直接取得所有系統代碼（包含類型名稱）
    const response = await axios.get('/api/Common/systemcodes/list')
    return response.data
  },

  /**
   * 取得所有代碼類型選項（從 CodeType 資料表）
   */
  async getCodeTypeOptions(): Promise<CodeTypeOption[]> {
    const codeTypes = await this.getCodeTypes()
    return codeTypes.map(ct => ({
      label: ct.name,
      value: ct.codeType
    }))
  },

  /**
   * 取得所有代碼類型（從 Common API /codetypes 端點）
   */
  async getCodeTypes(): Promise<Array<{ id: number, codeType: string, name: string }>> {
    const response = await axios.get('/api/Common/codetypes')
    return response.data
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

  async uploadFile(file: File, prefix: string = 'pets'): Promise<{ url: string, filename: string }> {
    const formData = new FormData()
    formData.append('file', file)
    formData.append('prefix', prefix)

    const response = await axios.post('/api/Common/upload-photo', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })

    return {
      url: response.data.photoUrl,
      filename: response.data.fileName
    }
  }
}