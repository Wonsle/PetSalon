export interface Pet {
  id: number
  name: string
  breed: number
  breedName: string
  age: number
  gender: 'M' | 'F'
  color: string
  weight: number
  note: string
  photoUrl?: string
  ownerId: number
  ownerName: string
  contactPhone: string
  createTime: string
  updateTime: string
}

export interface PetCreateRequest {
  name: string
  breed: number
  age: number
  gender: 'M' | 'F'
  color: string
  weight: number
  note: string
  ownerId: number
  photo?: File
}

export interface PetUpdateRequest extends PetCreateRequest {
  id: number
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