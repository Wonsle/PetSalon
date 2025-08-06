export interface Service {
  serviceId: number
  serviceName: string
  serviceType: string
  basePrice: number
  isActive: boolean
  description?: string
  estimatedDuration?: number // 預估時長（分鐘）
  createdBy?: string
  createdDate?: string
  modifiedBy?: string
  modifiedDate?: string
}

export interface PetServicePrice {
  petId: number
  serviceId: number
  customPrice: number
  isSubscriptionPrice: boolean
  subscriptionPrice?: number
}