import axios from '@/utils/axios'
import type { Contact, ContactCreateRequest, ContactUpdateRequest, ContactSearchParams, ContactListResponse } from '@/types/contact'

export const contactApi = {
  async getContacts(params: ContactSearchParams): Promise<ContactListResponse> {
    const response = await axios.get('/api/contactperson', { params })
    return response.data
  },

  async getContact(id: number): Promise<Contact> {
    const response = await axios.get(`/api/contactperson/${id}`)
    return response.data
  },

  async createContact(data: ContactCreateRequest): Promise<Contact> {
    const response = await axios.post('/api/contactperson', data)
    return response.data
  },

  async updateContact(data: ContactUpdateRequest): Promise<Contact> {
    const response = await axios.put(`/api/contactperson/${data.id}`, data)
    return response.data
  },

  async deleteContact(id: number): Promise<void> {
    await axios.delete(`/api/contactperson/${id}`)
  },

  async searchContacts(params: { keyword: string, limit?: number }): Promise<ContactListResponse> {
    const response = await axios.get('/api/contactperson/search', { params })
    return response.data
  }
}