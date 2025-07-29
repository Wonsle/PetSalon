import axios from '@/utils/axios'
import type {
  PetRelation,
  PetRelationCreateRequest,
  PetRelationSearchParams,
  PetRelationListResponse
} from '@/types/petRelation'

export const petRelationApi = {
  async getPetRelations(params: PetRelationSearchParams): Promise<PetRelationListResponse> {
    const response = await axios.get('/api/petrelation', { params })
    return response.data
  },

  async getPetRelation(id: number): Promise<PetRelation> {
    const response = await axios.get(`/api/petrelation/${id}`)
    return response.data
  },

  async getRelationsByPet(petId: number): Promise<PetRelation[]> {
    const response = await axios.get(`/api/petrelation/bypet/${petId}`)
    return response.data
  },

  async getRelationsByContact(contactPersonId: number): Promise<PetRelation[]> {
    const response = await axios.get(`/api/petrelation/bycontact/${contactPersonId}`)
    return response.data
  },

  async createPetRelation(data: PetRelationCreateRequest): Promise<number> {
    const response = await axios.post('/api/petrelation', data)
    return response.data
  },


  async deletePetRelation(id: number): Promise<void> {
    await axios.delete(`/api/petrelation/${id}`)
  }
}
