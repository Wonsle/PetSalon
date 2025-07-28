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

  // 新增屬性以支持表單功能
  name?: string  // 方案名稱
  serviceContent?: string  // 服務內容
  totalTimes?: number  // 服務次數
  totalAmount?: number  // 方案總額
  paidAmount?: number  // 已付金額
  note?: string  // 備註（與notes兼容）
}

export interface SubscriptionCreateRequest {
  name: string  // 新增：方案名稱
  petId: number
  serviceContent: string  // 新增：服務內容
  totalTimes: number  // 新增：服務次數
  totalAmount: number  // 新增：方案總額
  paidAmount: number  // 新增：已付金額
  startDate: string
  endDate: string
  subscriptionDate?: string  // 改為可選
  totalUsageLimit?: number  // 改為可選，與totalTimes功能重疊
  subscriptionPrice?: number  // 改為可選，與totalAmount功能重疊
  status?: string
  notes?: string
  note?: string  // 新增：備註（與notes互相兼容）
}

export interface SubscriptionUpdateRequest {
  id: number  // 新增：訂閱ID
  subscriptionId?: number  // 保持向後兼容
  name?: string  // 新增：方案名稱
  serviceContent?: string  // 新增：服務內容
  totalTimes?: number  // 新增：服務次數
  totalAmount?: number  // 新增：方案總額
  paidAmount?: number  // 新增：已付金額
  startDate?: string
  endDate?: string
  totalUsageLimit?: number
  subscriptionPrice?: number
  status?: string
  notes?: string
  note?: string  // 新增：備註
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