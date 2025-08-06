import axios from '@/utils/axios'
import type { 
  Reservation, 
  ReservationCreateRequest, 
  ReservationUpdateRequest, 
  ReservationSearchParams, 
  ReservationListResponse,
  CalendarEvent,
  CostCalculationRequest,
  CostCalculationResponse,
  DurationCalculationRequest,
  DurationCalculationResponse,
  ModernReservationRequest 
} from '@/types/reservation'

export const reservationApi = {
  async getReservations(params: ReservationSearchParams): Promise<ReservationListResponse> {
    const response = await axios.get('/api/reservation', { params })
    return response.data
  },

  async getReservation(id: number): Promise<Reservation> {
    const response = await axios.get(`/api/reservation/${id}`)
    return response.data
  },

  async createReservation(data: ReservationCreateRequest | ModernReservationRequest): Promise<Reservation> {
    const response = await axios.post('/api/reservation', data)
    return response.data
  },

  async updateReservation(data: ReservationUpdateRequest | (ModernReservationRequest & { id: number })): Promise<Reservation> {
    const response = await axios.put(`/api/reservation/${data.id}`, data)
    return response.data
  },

  async updateReservationStatus(id: number, status: string): Promise<void> {
    await axios.patch(`/api/reservation/${id}/status`, { status })
  },

  async deleteReservation(id: number): Promise<void> {
    await axios.delete(`/api/reservation/${id}`)
  },

  async getCalendarEvents(startDate: string, endDate: string): Promise<CalendarEvent[]> {
    const response = await axios.get('/api/reservation/calendar', {
      params: { startDate, endDate }
    })
    return response.data
  },

  async checkAvailability(date: string, time: string): Promise<{ available: boolean, conflicts: Reservation[] }> {
    const response = await axios.get('/api/reservation/availability', {
      params: { date, time }
    })
    return response.data
  },

  /**
   * 計算預約總成本
   */
  async calculateCost(data: CostCalculationRequest): Promise<CostCalculationResponse> {
    const response = await axios.post('/api/reservation/calculate-cost', data)
    return response.data
  },

  /**
   * 計算服務總時長
   */
  async calculateDuration(petId: number, data: DurationCalculationRequest): Promise<DurationCalculationResponse> {
    const response = await axios.post(`/api/reservation/pet/${petId}/calculate-duration`, data)
    return response.data
  },

  /**
   * 取得指定寵物的附加服務價格
   */
  async getPetAddonPrices(petId: number): Promise<any[]> {
    const response = await axios.get(`/api/reservation/pet/${petId}/addon-prices`)
    return response.data
  }
}