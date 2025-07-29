using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IContactPersonService
    {
        Task<ContactPersonListResponse> GetContactPersonList(ContactPersonSearchRequest request);
        Task<ContactPersonResponse?> GetContactPerson(long id);
        Task<long> CreateContactPerson(CreateContactPersonRequest request);
        Task UpdateContactPerson(UpdateContactPersonRequest request);
        Task DeleteContactPerson(long contactPersonId);
        Task<IList<ContactPersonResponse>> GetContactPersonsByPet(long petId);
        Task LinkContactPersonToPet(long contactPersonId, long petId, LinkContactToPetRequest request);
        Task UnlinkContactPersonFromPet(long contactPersonId, long petId);
        Task<IList<ContactPersonResponse>> SearchContactPersons(string keyword);
        Task<IList<RelationshipTypeResponse>> GetRelationshipTypes();
    }
}