export interface Income {
  id: number
  type: string
  customerName: string
  petName?: string
  description: string
  amount: number
  paymentMethod: string
  designer: string
  incomeDate: string
  reservationId?: number
  subscriptionId?: number
  note: string
  createTime: string
  updateTime: string
}

export interface IncomeCreateRequest {
  type: string
  customerName: string
  petName?: string
  description: string
  amount: number
  paymentMethod: string
  designer: string
  incomeDate: string
  reservationId?: number
  subscriptionId?: number
  note: string
}

export interface IncomeUpdateRequest extends IncomeCreateRequest {
  id: number
}

export interface IncomeSearchParams {
  keyword?: string
  type?: string
  paymentMethod?: string
  startDate?: string
  endDate?: string
  designer?: string
  page?: number
  pageSize?: number
  export?: boolean
}

export interface IncomeListResponse {
  data: Income[]
  total: number
  page: number
  pageSize: number
}

export interface IncomeStatistics {
  todayIncome: number
  monthIncome: number
  todayCount: number
  avgAmount: number
  yearIncome: number
  topServices: Array<{
    name: string
    amount: number
    count: number
  }>
  monthlyTrend: Array<{
    month: string
    income: number
    count: number
  }>
}