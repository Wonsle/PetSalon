export interface PetOwnerInfo {
  contactPersonId: number
  name: string
  nickName?: string
  contactNumber: string
  relationshipType: string
  relationshipTypeName: string
  sort?: number
  displayText: string
}

export interface Pet {
  petId: number
  petName: string
  breed: string  // SystemCode 的 code 值
  breedName?: string  // 品種中文名稱（列表顯示用）
  gender: string
  birthDay?: string
  coatColor?: string
  bodyWeight?: number
  photoUrl?: string
  createUser: string
  createTime: string
  modifyUser: string
  modifyTime: string
  // 聯絡人關聯資訊
  relations?: PetRelation[]
  primaryContact?: {
    contactPersonId: number
    name: string
    phone: string
    relationship: string
  }

  // 為了向後兼容和表單使用而新增的別名屬性
  id?: number  // petId的別名
  name?: string  // petName的別名
  ownerName?: string  // 主人姓名，從primaryContact或relations中獲取
  contactPhone?: string  // 聯絡電話，從primaryContact中獲取
}

interface PetRelation {
  petRelationId: number
  contactPersonId: number
  sort: number
  contactPerson: {
    contactPersonId: number
    name: string
    phone: string
    address: string
    relationship: string
  }
}

/**
 * 寵物服務價格設定
 */
export interface PetServicePriceSetting {
  serviceId: number
  customPrice?: number
  duration?: number
}

export interface PetCreateRequest {
  petName: string
  breed: string
  gender: string
  birthDay?: Date
  coatColor?: string
  bodyWeight?: number
  servicePrices?: PetServicePriceSetting[]
}

export interface PetUpdateRequest {
  petId: number
  petName: string
  breed: string
  gender: string
  birthDay?: Date
  coatColor?: string
  bodyWeight?: number
  servicePrices?: PetServicePriceSetting[]
}

export interface PetSearchParams {
  keyword?: string
  breed?: string
  gender?: string
  ownerId?: number
  page?: number
  pageSize?: number
}

export interface PetWithOwners extends Pet {
  owners: PetOwnerInfo[]
  ownersDisplay: string
}

export interface PetDetailWithContacts extends PetWithOwners {
  allContacts: PetOwnerInfo[]
}

export interface PetListResponse {
  data: Pet[]
  total: number
  page: number
  pageSize: number
}