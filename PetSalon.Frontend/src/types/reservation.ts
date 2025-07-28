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