import axios from '@/utils/axios'
import type {
  CodeType,
  CreateOrUpdateCodeTypeDto,
  CodeTypeSearchParams,
  CodeTypeListResponse,
  CodeTypeExistsResponse
} from '@/types/codeType'

/**
 * 代碼類型 API 服務
 */
export const codeTypeApi = {
  /**
   * 取得所有代碼類型
   */
  async getAllCodeTypes(): Promise<CodeType[]> {
    const response = await axios.get('/api/CodeType')
    return response.data
  },

  /**
   * 根據ID取得代碼類型
   * @param id 代碼類型ID
   */
  async getCodeTypeById(id: number): Promise<CodeType> {
    const response = await axios.get(`/api/CodeType/${id}`)
    return response.data
  },

  /**
   * 根據代碼取得代碼類型
   * @param codeType 代碼類型代碼
   */
  async getCodeTypeByCode(codeType: string): Promise<CodeType> {
    const response = await axios.get(`/api/CodeType/by-code/${encodeURIComponent(codeType)}`)
    return response.data
  },

  /**
   * 建立新的代碼類型
   * @param data 代碼類型資料
   */
  async createCodeType(data: CreateOrUpdateCodeTypeDto): Promise<CodeType> {
    const response = await axios.post('/api/CodeType', data)
    return response.data
  },

  /**
   * 更新代碼類型
   * @param id 代碼類型ID
   * @param data 更新資料
   */
  async updateCodeType(id: number, data: CreateOrUpdateCodeTypeDto): Promise<CodeType> {
    const response = await axios.put(`/api/CodeType/${id}`, data)
    return response.data
  },

  /**
   * 刪除代碼類型
   * @param id 代碼類型ID
   */
  async deleteCodeType(id: number): Promise<void> {
    await axios.delete(`/api/CodeType/${id}`)
  },

  /**
   * 檢查代碼類型是否存在
   * @param codeType 代碼類型代碼
   */
  async checkCodeTypeExists(codeType: string): Promise<boolean> {
    const response = await axios.get(`/api/CodeType/exists/${encodeURIComponent(codeType)}`)
    return response.data.exists
  },

  /**
   * 搜尋代碼類型（前端過濾實作）
   * @param params 搜尋參數
   */
  async searchCodeTypes(params: CodeTypeSearchParams): Promise<CodeTypeListResponse> {
    // 先取得所有代碼類型
    const allCodeTypes = await this.getAllCodeTypes()

    let filteredCodeTypes = allCodeTypes

    // 如果有關鍵字，進行過濾
    if (params.keyword && params.keyword.trim()) {
      const keyword = params.keyword.trim().toLowerCase()
      filteredCodeTypes = allCodeTypes.filter(item =>
        item.codeType.toLowerCase().includes(keyword) ||
        item.name.toLowerCase().includes(keyword) ||
        (item.description && item.description.toLowerCase().includes(keyword))
      )
    }

    // 分頁處理
    const page = params.page || 1
    const pageSize = params.pageSize || 10
    const total = filteredCodeTypes.length
    const totalPages = Math.ceil(total / pageSize)
    const startIndex = (page - 1) * pageSize
    const endIndex = startIndex + pageSize
    const items = filteredCodeTypes.slice(startIndex, endIndex)

    return {
      items,
      total,
      currentPage: page,
      pageSize,
      totalPages
    }
  }
}
