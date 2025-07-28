using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly PetSalonContext _context;

        public ContactPersonService(PetSalonContext context)
        {
            _context = context;
        }

        public async Task<ContactPersonListResponse> GetContactPersonList(ContactPersonSearchRequest request)
        {
            var query = _context.ContactPerson
                .Include(cp => cp.PetRelation)
                    .ThenInclude(pr => pr.Pet)
                .AsNoTracking()
                .AsQueryable();

            // Apply search filters
            if (!string.IsNullOrEmpty(request.Keyword))
            {
                query = query.Where(cp => cp.Name.Contains(request.Keyword) ||
                                         cp.NickName.Contains(request.Keyword) ||
                                         cp.ContactNumber.Contains(request.Keyword));
            }

            if (!string.IsNullOrEmpty(request.Name))
            {
                query = query.Where(cp => cp.Name.Contains(request.Name));
            }

            if (!string.IsNullOrEmpty(request.ContactNumber))
            {
                query = query.Where(cp => cp.ContactNumber.Contains(request.ContactNumber));
            }

            var total = await query.CountAsync();

            var contactPersons = await query
                .OrderBy(cp => cp.Name)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var responseData = contactPersons.Select(cp => new ContactPersonResponse
            {
                ContactPersonId = cp.ContactPersonId,
                Name = cp.Name,
                NickName = cp.NickName,
                ContactNumber = cp.ContactNumber,
                CreateUser = cp.CreateUser,
                CreateTime = cp.CreateTime,
                ModifyUser = cp.ModifyUser,
                ModifyTime = cp.ModifyTime,
                RelatedPets = cp.PetRelation?.Select(pr => new PetRelationInfo
                {
                    PetRelationId = pr.PetRelationId,
                    PetId = pr.PetId,
                    PetName = pr.Pet?.PetName ?? "",
                    Breed = pr.Pet?.Breed ?? "",
                    Gender = pr.Pet?.Gender ?? "",
                    Sort = pr.Sort
                }).ToList()
            }).ToList();

            return new ContactPersonListResponse
            {
                Data = responseData,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<ContactPersonResponse?> GetContactPerson(long id)
        {
            var contactPerson = await _context.ContactPerson
                .Include(cp => cp.PetRelation)
                    .ThenInclude(pr => pr.Pet)
                .AsNoTracking()
                .FirstOrDefaultAsync(cp => cp.ContactPersonId == id);

            if (contactPerson == null)
                return null;

            return new ContactPersonResponse
            {
                ContactPersonId = contactPerson.ContactPersonId,
                Name = contactPerson.Name,
                NickName = contactPerson.NickName,
                ContactNumber = contactPerson.ContactNumber,
                CreateUser = contactPerson.CreateUser,
                CreateTime = contactPerson.CreateTime,
                ModifyUser = contactPerson.ModifyUser,
                ModifyTime = contactPerson.ModifyTime,
                RelatedPets = contactPerson.PetRelation?.Select(pr => new PetRelationInfo
                {
                    PetRelationId = pr.PetRelationId,
                    PetId = pr.PetId,
                    PetName = pr.Pet?.PetName ?? "",
                    Breed = pr.Pet?.Breed ?? "",
                    Gender = pr.Pet?.Gender ?? "",
                    Sort = pr.Sort
                }).ToList()
            };
        }

        public async Task<long> CreateContactPerson(CreateContactPersonRequest request)
        {
            var contactPerson = new ContactPerson
            {
                Name = request.Name,
                NickName = request.NickName,
                ContactNumber = request.ContactNumber,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.ContactPerson.Add(contactPerson);
            await _context.SaveChangesAsync();
            return contactPerson.ContactPersonId;
        }

        public async Task UpdateContactPerson(UpdateContactPersonRequest request)
        {
            var contactPerson = await _context.ContactPerson.FindAsync(request.ContactPersonId);
            if (contactPerson == null)
                throw new ArgumentException($"ContactPerson with ID {request.ContactPersonId} not found");

            contactPerson.Name = request.Name;
            contactPerson.NickName = request.NickName;
            contactPerson.ContactNumber = request.ContactNumber;
            contactPerson.ModifyUser = "SYSTEM"; // TODO: Get from current user context

            await _context.SaveChangesAsync();
        }

        public async Task DeleteContactPerson(long contactPersonId)
        {
            var contactPerson = await _context.ContactPerson.FindAsync(contactPersonId);
            if (contactPerson == null)
                throw new ArgumentException($"ContactPerson with ID {contactPersonId} not found");

            // Check if contact person has any pet relations
            var hasRelations = await _context.PetRelation.AnyAsync(pr => pr.ContactPersonId == contactPersonId);
            if (hasRelations)
                throw new InvalidOperationException("Cannot delete contact person with existing pet relations");

            _context.ContactPerson.Remove(contactPerson);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<ContactPersonResponse>> GetContactPersonsByPet(long petId)
        {
            var contactPersons = await _context.ContactPerson
                .Include(cp => cp.PetRelation.Where(pr => pr.PetId == petId))
                .Where(cp => cp.PetRelation.Any(pr => pr.PetId == petId))
                .AsNoTracking()
                .ToListAsync();

            return contactPersons.Select(cp => new ContactPersonResponse
            {
                ContactPersonId = cp.ContactPersonId,
                Name = cp.Name,
                NickName = cp.NickName,
                ContactNumber = cp.ContactNumber,
                CreateUser = cp.CreateUser,
                CreateTime = cp.CreateTime,
                ModifyUser = cp.ModifyUser,
                ModifyTime = cp.ModifyTime
            }).ToList();
        }

        public async Task LinkContactPersonToPet(long contactPersonId, long petId, LinkContactToPetRequest request)
        {
            // Check if contact person exists
            var contactExists = await _context.ContactPerson.AnyAsync(cp => cp.ContactPersonId == contactPersonId);
            if (!contactExists)
                throw new ArgumentException($"ContactPerson with ID {contactPersonId} not found");

            // Check if pet exists
            var petExists = await _context.Pet.AnyAsync(p => p.PetId == petId);
            if (!petExists)
                throw new ArgumentException($"Pet with ID {petId} not found");

            // Check if relation already exists
            var existingRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.ContactPersonId == contactPersonId && pr.PetId == petId);
            if (existingRelation != null)
                throw new InvalidOperationException("Relation already exists between this contact person and pet");

            var petRelation = new PetRelation
            {
                ContactPersonId = contactPersonId,
                PetId = petId,
                Sort = request.Sort,
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

            if (petRelation == null)
                throw new ArgumentException("Relation not found between this contact person and pet");

            _context.PetRelation.Remove(petRelation);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<ContactPersonResponse>> SearchContactPersons(string keyword)
        {
            if (string.IsNullOrEmpty(keyword))
                return new List<ContactPersonResponse>();

            var contactPersons = await _context.ContactPerson
                .Where(cp => cp.Name.Contains(keyword) ||
                            cp.NickName.Contains(keyword) ||
                            cp.ContactNumber.Contains(keyword))
                .AsNoTracking()
                .OrderBy(cp => cp.Name)
                .Take(10) // Limit results for search suggestions
                .ToListAsync();

            return contactPersons.Select(cp => new ContactPersonResponse
            {
                ContactPersonId = cp.ContactPersonId,
                Name = cp.Name,
                NickName = cp.NickName,
                ContactNumber = cp.ContactNumber,
                CreateUser = cp.CreateUser,
                CreateTime = cp.CreateTime,
                ModifyUser = cp.ModifyUser,
                ModifyTime = cp.ModifyTime
            }).ToList();
        }
    }
}