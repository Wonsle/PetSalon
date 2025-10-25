import axios from '@/utils/axios'

/**
 * 寵物服務價格回應資料
 */
export interface PetServicePriceResponse {
  petServicePriceId: number
  petId: number
  serviceId: number
  customPrice?: number
  duration?: number
  notes?: string
  isActive: boolean
  service?: {
    serviceId: number
    serviceName: string
    basePrice: number
    duration: number
  }
}

export const petServicePriceApi = {
  /**
   * 取得寵物的訂閱價格（優先使用 PetServicePrice，其次使用 Service 預設值）
   */
  async getSubscriptionPrice(petId: number): Promise<number | null> {
    const response = await axios.get(`/api/petserviceprice/subscription-price/${petId}`)
    return response.data.subscriptionPrice
  },

  /**
   * 取得寵物的所有服務價格設定
   */
  async getPetServicePrices(petId: number): Promise<PetServicePriceResponse[]> {
    const response = await axios.get(`/api/petserviceprice/pet/${petId}`)
    return response.data
  }
}
