import axios from '@/utils/axios'
import type { Pet, PetCreateRequest, PetUpdateRequest, PetSearchParams, PetListResponse } from '@/types/pet'

export const petApi = {
  async getPets(params: PetSearchParams): Promise<PetListResponse> {
    const response = await axios.get('/api/pet', { params })
    return response.data
  },

  async getPet(id: number): Promise<Pet> {
    const response = await axios.get(`/api/pet/${id}`)
    return response.data
  },

  async createPet(data: PetCreateRequest): Promise<Pet> {
    const formData = new FormData()
    formData.append('name', data.name)
    formData.append('breed', data.breed.toString())
    formData.append('age', data.age.toString())
    formData.append('gender', data.gender)
    formData.append('color', data.color)
    formData.append('weight', data.weight.toString())
    formData.append('note', data.note)
    formData.append('ownerId', data.ownerId.toString())
    
    if (data.photo) {
      formData.append('photo', data.photo)
    }

    const response = await axios.post('/api/pet', formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  },

  async updatePet(data: PetUpdateRequest): Promise<Pet> {
    const formData = new FormData()
    formData.append('id', data.id.toString())
    formData.append('name', data.name)
    formData.append('breed', data.breed.toString())
    formData.append('age', data.age.toString())
    formData.append('gender', data.gender)
    formData.append('color', data.color)
    formData.append('weight', data.weight.toString())
    formData.append('note', data.note)
    formData.append('ownerId', data.ownerId.toString())
    
    if (data.photo) {
      formData.append('photo', data.photo)
    }

    const response = await axios.put(`/api/pet/${data.id}`, formData, {
      headers: {
        'Content-Type': 'multipart/form-data'
      }
    })
    return response.data
  },

  async deletePet(id: number): Promise<void> {
    await axios.delete(`/api/pet/${id}`)
  }
}