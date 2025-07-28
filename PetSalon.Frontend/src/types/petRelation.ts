export interface PetRelation {
  petRelationId: number
  petId: number
  contactPersonId: number
  relationshipType: string
  sort: number
  createUser: string
  createTime: string
  modifyUser: string
  modifyTime: string
  // 關聯資料
  contactPerson?: {
    contactPersonId: number
    name: string
    phone: string
    email: string
    address: string
    relationship?: string
  }
  pet?: {
    petId: number
    petName: string
  }
}

export interface PetRelationCreateRequest {
  petId: number
  contactPersonId: number
  relationshipType: string
  sort?: number
}

export interface PetRelationUpdateRequest extends PetRelationCreateRequest {
  petRelationId: number
}

export interface PetRelationSearchParams {
  petId?: number
  contactPersonId?: number
  page?: number
  pageSize?: number
}

export interface PetRelationListResponse {
  data: PetRelation[]
  total: number
  page: number
  pageSize: number
}
