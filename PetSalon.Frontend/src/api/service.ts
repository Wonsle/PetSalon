import axios from '@/utils/axios'
import type { Service, PetServicePrice } from '@/types/service'

export const serviceApi = {
  /**
   * 取得所有活躍的服務項目
   */
  async getActiveServices(): Promise<Service[]> {
    const response = await axios.get('/api/service/active')
    return response.data
  },

  /**
   * 取得所有服務項目
   */
  async getAllServices(): Promise<Service[]> {
    const response = await axios.get('/api/service')
    return response.data
  },

  /**
   * 取得指定服務詳細資訊
   */
  async getService(id: number): Promise<Service> {
    const response = await axios.get(`/api/service/${id}`)
    return response.data
  },

  /**
   * 取得預設服務清單（洗澡和美容）
   */
  async getDefaultServices(): Promise<Service[]> {
    const response = await axios.get('/api/service/default')
    return response.data
  },

  /**
   * 取得指定寵物的服務價格 (已移除功能，保留API以避免破壞現有調用)
   */
  async getPetServicePrices(petId: number): Promise<PetServicePrice[]> {
    // 功能已移除，返回空陣列
    return []
  },

  /**
   * 創建新的服務項目
   */
  async createService(data: Omit<Service, 'serviceId'>): Promise<Service> {
    const response = await axios.post('/api/service', data)
    return response.data
  },

  /**
   * 更新服務項目
   */
  async updateService(data: Service): Promise<Service> {
    const response = await axios.put(`/api/service/${data.serviceId}`, data)
    return response.data
  },

  /**
   * 刪除服務項目
   */
  async deleteService(id: number): Promise<void> {
    await axios.delete(`/api/service/${id}`)
  }
}