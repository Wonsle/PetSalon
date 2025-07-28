import axios from '@/utils/axios'
import type { 
  Subscription, 
  SubscriptionCreateRequest, 
  SubscriptionUpdateRequest, 
  SubscriptionSearchParams, 
  SubscriptionListResponse,
  SubscriptionUsage
} from '@/types/subscription'

export const subscriptionApi = {
  async getSubscriptions(): Promise<Subscription[]> {
    const response = await axios.get('/api/subscription')
    return response.data
  },

  async getSubscription(subscriptionId: number): Promise<Subscription> {
    const response = await axios.get(`/api/subscription/${subscriptionId}`)
    return response.data
  },

  async getSubscriptionsByPet(petId: number): Promise<Subscription[]> {
    const response = await axios.get(`/api/subscription/pet/${petId}`)
    return response.data
  },

  async createSubscription(data: SubscriptionCreateRequest): Promise<number> {
    const response = await axios.post('/api/subscription', data)
    return response.data
  },

  async updateSubscription(data: SubscriptionUpdateRequest): Promise<void> {
    await axios.put(`/api/subscription/${data.subscriptionId}`, data)
  },

  async deleteSubscription(subscriptionId: number): Promise<void> {
    await axios.delete(`/api/subscription/${subscriptionId}`)
  },

  async getActiveSubscription(petId: number, checkDate?: string): Promise<Subscription> {
    const params = checkDate ? { checkDate } : {}
    const response = await axios.get(`/api/subscription/pet/${petId}/active`, { params })
    return response.data
  },

  async getSubscriptionUsage(subscriptionId: number): Promise<SubscriptionUsage> {
    const response = await axios.get(`/api/subscription/${subscriptionId}/usage`)
    return response.data
  },

  async getRemainingUsage(subscriptionId: number): Promise<number> {
    const response = await axios.get(`/api/subscription/${subscriptionId}/remaining`)
    return response.data
  },

  async getExpiringSubscriptions(days: number = 7): Promise<Subscription[]> {
    const response = await axios.get('/api/subscription/expiring', { params: { days } })
    return response.data
  },

  async updateSubscriptionStatus(subscriptionId: number, status: string): Promise<void> {
    await axios.post(`/api/subscription/${subscriptionId}/status`, status, {
      headers: { 'Content-Type': 'application/json' }
    })
  }
}