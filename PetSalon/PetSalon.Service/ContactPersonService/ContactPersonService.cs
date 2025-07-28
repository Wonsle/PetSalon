using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly PetSalonContext _context;

        public ContactPersonService(PetSalonContext context)
        {
            _context = context;
        }

        public async Task<IList<ContactPerson>> GetContactPersonList()
        {
            return await _context.ContactPerson.AsNoTracking().ToListAsync();
        }

        public async Task<ContactPerson> GetContactPerson(long id)
        {
            return await _context.ContactPerson.FindAsync(id);
        }

        public async Task<long> CreateContactPerson(ContactPerson contactPerson)
        {
            _context.ContactPerson.Add(contactPerson);
            await _context.SaveChangesAsync();
            return contactPerson.ContactPersonId;
        }

        public async Task UpdateContactPerson(ContactPerson contactPerson)
        {
            _context.ContactPerson.Update(contactPerson);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteContactPerson(long contactPersonId)
        {
            var contactPerson = new ContactPerson() { ContactPersonId = contactPersonId };
            _context.ContactPerson.Attach(contactPerson);
            _context.ContactPerson.Remove(contactPerson);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<ContactPerson>> GetContactPersonsByPet(long petId)
        {
            return await _context.ContactPerson
                .Include(cp => cp.PetRelation)
                .Where(cp => cp.PetRelation.Any(pr => pr.PetId == petId))
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task LinkContactPersonToPet(long contactPersonId, long petId, string relationshipType)
        {
            var petRelation = new PetRelation
            {
                ContactPersonId = contactPersonId,
                PetId = petId,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.PetRelation.Add(petRelation);
            await _context.SaveChangesAsync();
        }

        public async Task UnlinkContactPersonFromPet(long contactPersonId, long petId)
        {
            var petRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.ContactPersonId == contactPersonId && pr.PetId == petId);

            if (petRelation != null)
            {
                _context.PetRelation.Remove(petRelation);
                await _context.SaveChangesAsync();
            }
        }
    }
}