import axios from '@/utils/axios'
import type { Pet, PetCreateRequest, PetUpdateRequest, PetSearchParams, PetListResponse } from '@/types/pet'

export const petApi = {
  async getPets(params: PetSearchParams): Promise<Pet[]> {
    const response = await axios.get('/api/pet/Index', { params })
    return response.data
  },

  async getPet(id: number): Promise<Pet> {
    const response = await axios.get(`/api/pet/${id}`)
    return response.data
  },

  async createPet(data: PetCreateRequest): Promise<number> {
    const response = await axios.post('/api/pet', data)
    return response.data
  },

  async updatePet(data: PetUpdateRequest): Promise<void> {
    await axios.put(`/api/pet/${data.petId}`, data)
  },

  async deletePet(id: number): Promise<void> {
    await axios.delete(`/api/pet/${id}`)
  }
}