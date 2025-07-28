export interface Pet {
  petId: number
  petName: string
  breed: string
  gender: string
  birthDay?: string
  normalPrice?: number
  subscriptionPrice?: number
  createUser: string
  createTime: string
  modifyUser: string
  modifyTime: string
}

export interface PetCreateRequest {
  petName: string
  breed: string
  gender: string
  birthDay?: Date
  normalPrice?: number
  subscriptionPrice?: number
}

export interface PetUpdateRequest {
  petId: number
  petName: string
  breed: string
  gender: string
  birthDay?: Date
  normalPrice?: number
  subscriptionPrice?: number
}

export interface PetSearchParams {
  keyword?: string
  breed?: number
  gender?: 'M' | 'F'
  ownerId?: number
  page?: number
  pageSize?: number
}

export interface PetListResponse {
  data: Pet[]
  total: number
  page: number
  pageSize: number
}