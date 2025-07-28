export interface Subscription {
  subscriptionId: number
  petId: number
  petName?: string
  subscriptionDate: string
  startDate: string
  endDate: string
  totalUsageLimit: number
  usedCount: number
  subscriptionPrice: number
  status: string
  statusName?: string
  notes?: string
  isExpired: boolean
  isActive: boolean
  daysUntilExpiry: number
  remainingUsage: number
  createUser: string
  createTime: string
  modifyUser: string
  modifyTime: string
}

export interface SubscriptionCreateRequest {
  petId: number
  startDate: string
  endDate: string
  subscriptionDate: string
  totalUsageLimit: number
  subscriptionPrice: number
  status?: string
  notes?: string
}

export interface SubscriptionUpdateRequest {
  subscriptionId: number
  startDate?: string
  endDate?: string
  totalUsageLimit?: number
  subscriptionPrice?: number
  status?: string
  notes?: string
}

export interface SubscriptionSearchParams {
  petId?: number
  status?: string
  startDate?: string
  endDate?: string
  page?: number
  pageSize?: number
}

export interface SubscriptionListResponse {
  data: Subscription[]
  total: number
  page: number
  pageSize: number
}

export interface SubscriptionUsage {
  subscriptionId: number
  petName: string
  startDate: string
  endDate: string
  totalUsageLimit: number
  usedCount: number
  remainingUsage: number
  hasUnlimitedUsage: boolean
  averageUsagePerMonth: number
  usageDates: string[]
}