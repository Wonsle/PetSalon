export interface Service {
  serviceId: number
  serviceName: string
  serviceType: string
  serviceTypeName?: string // From SystemCode
  basePrice: number
  duration: number // 服務時長（分鐘）
  description?: string
  isActive: boolean
  sort: number
}

export interface PetServicePrice {
  petServicePriceId: number
  petId: number
  serviceId: number
  customPrice?: number
  duration?: number
  isActive: boolean
  createUser?: string
  createTime?: string
  modifyUser?: string
  modifyTime?: string
  // 關聯資料
  pet?: any
  service?: Service
}