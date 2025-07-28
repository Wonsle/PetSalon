import axios from '@/utils/axios'
import type { Pet, PetCreateRequest, PetUpdateRequest, PetSearchParams, PetListResponse } from '@/types/pet'

export const petApi = {
  async getPets(params: PetSearchParams): Promise<PetListResponse> {
    const response = await axios.get('/api/pet', { params })

    // 處理不同的回傳格式
    const data = response.data

    // 如果直接回傳陣列
    if (Array.isArray(data)) {
      return {
        data: data,
        total: data.length,
        page: params.page || 1,
        pageSize: params.pageSize || 12
      }
    }

    // 如果回傳物件包含 data 屬性
    if (data && typeof data === 'object' && data.data) {
      return {
        data: data.data || [],
        total: data.total || data.data.length,
        page: data.page || params.page || 1,
        pageSize: data.pageSize || params.pageSize || 12
      }
    }

    // 其他格式，嘗試找到資料陣列
    const dataArray = data.items || data.list || data.pets || []
    return {
      data: dataArray,
      total: data.totalCount || data.total || dataArray.length,
      page: data.currentPage || data.page || params.page || 1,
      pageSize: data.pageSize || params.pageSize || 12
    }
  },

  async getPet(id: number): Promise<Pet> {
    const response = await axios.get(`/api/pet/${id}`)
    return response.data
  },

  async createPet(data: PetCreateRequest): Promise<number> {
    const response = await axios.post('/api/pet', data)
    return response.data
  },

  async updatePet(data: PetUpdateRequest): Promise<void> {
    await axios.put(`/api/pet/${data.petId}`, data)
  },

  async deletePet(id: number): Promise<void> {
    await axios.delete(`/api/pet/${id}`)
  }
}