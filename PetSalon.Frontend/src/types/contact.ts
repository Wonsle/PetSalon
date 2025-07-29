export interface Contact {
  contactPersonId: number
  name: string
  nickName?: string
  contactNumber: string
  relatedPets?: PetRelationInfo[]
}

export interface PetRelationInfo {
  petRelationId: number
  petId: number
  petName: string
  breed: string
  gender: string
  relationshipType: string
  relationshipTypeName: string
  sort: number
}

export interface ContactCreateRequest {
  name: string
  nickName?: string
  contactNumber: string
}

export interface ContactUpdateRequest {
  contactPersonId: number
  name: string
  nickName?: string
  contactNumber: string
}

export interface ContactSearchParams {
  keyword?: string
  name?: string
  contactNumber?: string
  page?: number
  pageSize?: number
}

export interface ContactListResponse {
  data: Contact[]
  total: number
  page: number
  pageSize: number
}

export interface LinkContactToPetRequest {
  relationshipType: string
  sort?: number
}