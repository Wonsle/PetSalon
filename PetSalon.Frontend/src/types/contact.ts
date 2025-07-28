export interface Contact {
  id: number
  name: string
  phone: string
  email: string
  address: string
  note: string
  status: string
  petCount?: number
  lastVisit?: string
  createTime: string
  updateTime: string
}

export interface ContactCreateRequest {
  name: string
  phone: string
  email: string
  address: string
  note: string
  status: string
}

export interface ContactUpdateRequest extends ContactCreateRequest {
  id: number
}

export interface ContactSearchParams {
  keyword?: string
  type?: string
  page?: number
  pageSize?: number
}

export interface ContactListResponse {
  data: Contact[]
  total: number
  page: number
  pageSize: number
}