using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public interface IContactPersonService
    {
        Task<IList<ContactPerson>> GetContactPersonList();
        Task<ContactPerson> GetContactPerson(long id);
        Task<long> CreateContactPerson(ContactPerson contactPerson);
        Task UpdateContactPerson(ContactPerson contactPerson);
        Task DeleteContactPerson(long contactPersonId);
        Task<IList<ContactPerson>> GetContactPersonsByPet(long petId);
        Task LinkContactPersonToPet(long contactPersonId, long petId, string relationshipType);
        Task UnlinkContactPersonFromPet(long contactPersonId, long petId);
    }
}