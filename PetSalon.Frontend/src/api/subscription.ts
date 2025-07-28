import axios from '@/utils/axios'
import type { 
  Subscription, 
  SubscriptionCreateRequest, 
  SubscriptionUpdateRequest, 
  SubscriptionSearchParams, 
  SubscriptionListResponse 
} from '@/types/subscription'

export const subscriptionApi = {
  async getSubscriptions(params: SubscriptionSearchParams): Promise<SubscriptionListResponse> {
    const response = await axios.get('/api/subscription', { params })
    return response.data
  },

  async getSubscription(id: number): Promise<Subscription> {
    const response = await axios.get(`/api/subscription/${id}`)
    return response.data
  },

  async createSubscription(data: SubscriptionCreateRequest): Promise<Subscription> {
    const response = await axios.post('/api/subscription', data)
    return response.data
  },

  async updateSubscription(data: SubscriptionUpdateRequest): Promise<Subscription> {
    const response = await axios.put(`/api/subscription/${data.id}`, data)
    return response.data
  },

  async updateSubscriptionStatus(id: number, status: string): Promise<void> {
    await axios.patch(`/api/subscription/${id}/status`, { status })
  },

  async deleteSubscription(id: number): Promise<void> {
    await axios.delete(`/api/subscription/${id}`)
  },

  async getAvailableSubscriptions(petId: number): Promise<Subscription[]> {
    const response = await axios.get(`/api/subscription/available/${petId}`)
    return response.data
  },

  async useSubscription(id: number, times: number = 1): Promise<void> {
    await axios.post(`/api/subscription/${id}/use`, { times })
  }
}