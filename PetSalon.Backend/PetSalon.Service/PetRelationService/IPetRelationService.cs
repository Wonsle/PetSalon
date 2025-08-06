using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IPetRelationService
    {
        Task<PetRelationListResponse> GetPetRelationList(PetRelationSearchRequest request);
        Task<PetRelationResponse?> GetPetRelation(long id);
        Task<IList<PetRelationResponse>> GetRelationsByPet(long petId);
        Task<IList<PetRelationResponse>> GetRelationsByContact(long contactPersonId);
        Task<long> CreatePetRelation(CreatePetRelationApiRequest request);
        Task DeletePetRelation(long petRelationId);
    }
}