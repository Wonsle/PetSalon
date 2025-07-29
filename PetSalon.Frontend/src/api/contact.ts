import axios from '@/utils/axios'
import type { Contact, ContactCreateRequest, ContactUpdateRequest, ContactSearchParams, ContactListResponse, LinkContactToPetRequest } from '@/types/contact'

export const contactApi = {
  async getContacts(params: ContactSearchParams): Promise<ContactListResponse> {
    const response = await axios.get('/api/contactperson', { params })
    return response.data
  },

  async getContact(id: number): Promise<Contact> {
    const response = await axios.get(`/api/contactperson/${id}`)
    return response.data
  },

  async createContact(data: ContactCreateRequest): Promise<number> {
    const response = await axios.post('/api/contactperson', data)
    return response.data
  },

  async updateContact(data: ContactUpdateRequest): Promise<void> {
    await axios.put(`/api/contactperson/${data.contactPersonId}`, data)
  },

  async deleteContact(id: number): Promise<void> {
    await axios.delete(`/api/contactperson/${id}`)
  },

  async searchContacts(keyword: string): Promise<Contact[]> {
    const response = await axios.get('/api/contactperson/search', { params: { keyword } })
    return response.data
  },

  async getContactsByPet(petId: number): Promise<Contact[]> {
    const response = await axios.get(`/api/contactperson/pet/${petId}`)
    return response.data
  },

  async linkContactToPet(contactPersonId: number, petId: number, request: LinkContactToPetRequest): Promise<void> {
    await axios.post(`/api/contactperson/${contactPersonId}/pets/${petId}`, request)
  },

  async unlinkContactFromPet(contactPersonId: number, petId: number): Promise<void> {
    await axios.delete(`/api/contactperson/${contactPersonId}/pets/${petId}`)
  },

  async updatePetRelationSort(contactPersonId: number, petId: number, relationshipType: string, sort: number): Promise<void> {
    // 先解除關聯，再重新建立以更新排序
    await this.unlinkContactFromPet(contactPersonId, petId)
    await this.linkContactToPet(contactPersonId, petId, { relationshipType, sort })
  }
}