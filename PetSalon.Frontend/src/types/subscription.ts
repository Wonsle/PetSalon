export interface Subscription {
  id: number
  name: string
  petId: number
  petName: string
  petPhotoUrl?: string
  ownerId: number
  ownerName: string
  contactPhone: string
  serviceContent: string
  totalTimes: number
  usedTimes: number
  totalAmount: number
  paidAmount: number
  startDate: string
  endDate: string
  status: string
  remainingDays: number
  note: string
  createTime: string
  updateTime: string
}

export interface SubscriptionCreateRequest {
  name: string
  petId: number
  serviceContent: string
  totalTimes: number
  totalAmount: number
  paidAmount: number
  startDate: string
  endDate: string
  note: string
}

export interface SubscriptionUpdateRequest extends SubscriptionCreateRequest {
  id: number
  status?: string
}

export interface SubscriptionSearchParams {
  keyword?: string
  status?: string
  startDate?: string
  endDate?: string
  petId?: number
  ownerId?: number
  page?: number
  pageSize?: number
}

export interface SubscriptionListResponse {
  data: Subscription[]
  total: number
  page: number
  pageSize: number
}