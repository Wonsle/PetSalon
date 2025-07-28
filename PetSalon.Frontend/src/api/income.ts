import axios from '@/utils/axios'
import type { 
  Income, 
  IncomeCreateRequest, 
  IncomeUpdateRequest, 
  IncomeSearchParams, 
  IncomeListResponse,
  IncomeStatistics 
} from '@/types/income'

export const incomeApi = {
  async getIncomes(params: IncomeSearchParams): Promise<IncomeListResponse> {
    const response = await axios.get('/api/income', { params })
    return response.data
  },

  async getIncome(id: number): Promise<Income> {
    const response = await axios.get(`/api/income/${id}`)
    return response.data
  },

  async createIncome(data: IncomeCreateRequest): Promise<Income> {
    const response = await axios.post('/api/income', data)
    return response.data
  },

  async updateIncome(data: IncomeUpdateRequest): Promise<Income> {
    const response = await axios.put(`/api/income/${data.id}`, data)
    return response.data
  },

  async deleteIncome(id: number): Promise<void> {
    await axios.delete(`/api/income/${id}`)
  },

  async getStatistics(): Promise<IncomeStatistics> {
    const response = await axios.get('/api/income/statistics')
    return response.data
  },

  async exportIncomes(params: IncomeSearchParams): Promise<void> {
    const response = await axios.get('/api/income/export', { 
      params,
      responseType: 'blob'
    })
    
    // Create download link
    const blob = new Blob([response.data], { 
      type: 'application/vnd.openxmlformats-officedocument.spreadsheetml.sheet' 
    })
    const url = window.URL.createObjectURL(blob)
    const link = document.createElement('a')
    link.href = url
    link.download = `收入記錄_${new Date().toISOString().split('T')[0]}.xlsx`
    document.body.appendChild(link)
    link.click()
    document.body.removeChild(link)
    window.URL.revokeObjectURL(url)
  }
}