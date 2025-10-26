export interface Reservation {
  id: number
  petId: number
  petName: string
  ownerId: number
  ownerName: string
  contactPhone: string
  subscriptionId?: number
  subscriptionName?: string
  reserveDate: string
  reserveTime: string
  serviceType: string
  designer: string
  status: string
  note: string
  createTime: string
  updateTime: string
}

export interface ReservationCreateRequest {
  petId: number
  subscriptionId?: number
  reserveDate: string
  reserveTime: string
  serviceType: string
  designer: string
  note: string
}

export interface ReservationUpdateRequest extends ReservationCreateRequest {
  id: number
  status?: string
}

// 支援新的預約請求格式
export interface ModernReservationRequest {
  petId: number
  reservationDate: string  // Format: 'YYYY-MM-DD'
  reservationTime: string  // Format: 'HH:mm:ss' (TimeSpan)
  serviceIds: number[]
  useSubscription: boolean
  subscriptionId?: number
  status?: string
  memo?: string
}

export interface ReservationSearchParams {
  keyword?: string
  status?: string
  startDate?: string
  endDate?: string
  petId?: number
  ownerId?: number
  designer?: string
  page?: number
  pageSize?: number
}

export interface ReservationListResponse {
  data: Reservation[]
  total: number
  page: number
  pageSize: number
}

export interface CalendarEvent {
  id: number
  title: string
  start: string
  end: string
  backgroundColor?: string
  borderColor?: string
  textColor?: string
  extendedProps?: {
    reservation: Reservation
  }
}

// 新增的 API 類型定義

export interface CostCalculationRequest {
  petId: number
  serviceIds: number[]
  useSubscription: boolean
  subscriptionId?: number
}

export interface CostCalculationResponse {
  serviceTotal: number
  addonTotal: number
  discount: number
  totalAmount: number
  subscriptionDiscount?: number
  breakdown?: {
    services: ServiceCostDetail[]
    addons: AddonCostDetail[]
  }
}

export interface ServiceCostDetail {
  serviceId: number
  serviceName: string
  originalPrice: number
  finalPrice: number
  discount?: number
}

export interface AddonCostDetail {
  addonId: number
  addonName: string
  originalPrice: number
  finalPrice: number
  isCustomPrice: boolean
}

export interface DurationCalculationRequest {
  serviceIds: number[]
}

export interface DurationCalculationResponse {
  totalDuration: number // 總時長（分鐘）
  estimatedStartTime?: string
  estimatedEndTime?: string
  breakdown?: {
    services: ServiceDurationDetail[]
    addons: AddonDurationDetail[]
  }
}

export interface ServiceDurationDetail {
  serviceId: number
  serviceName: string
  duration: number // 分鐘
}

export interface AddonDurationDetail {
  addonId: number
  addonName: string
  duration: number // 分鐘
}

// 更新現有的預約請求類型以支援新的服務結構
export interface ModernReservationCreateRequest {
  petId: number
  subscriptionId?: number
  reservationDate: string  // Format: 'YYYY-MM-DD'
  reservationTime: string  // Format: 'HH:mm:ss' (TimeSpan)
  serviceIds: number[]
  useSubscription: boolean
  status?: string
  memo?: string
}

export interface ModernReservationUpdateRequest extends ModernReservationCreateRequest {
  id: number
}