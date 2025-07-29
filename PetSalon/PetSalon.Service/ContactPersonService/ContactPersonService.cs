using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class ContactPersonService : IContactPersonService
    {
        private readonly PetSalonContext _context;
        private readonly ICommonService _commonService;

        public ContactPersonService(PetSalonContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
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

            var responseData = await MapContactPersonsToResponse(contactPersons);

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

            var responseList = await MapContactPersonsToResponse(new[] { contactPerson });
            return responseList.FirstOrDefault();
        }

        public async Task<long> CreateContactPerson(CreateContactPersonRequest request)
        {
            using var transaction = await _context.Database.BeginTransactionAsync();
            try
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

                // Create pet relations if specified
                if (request.PetRelations != null && request.PetRelations.Any())
                {
                    foreach (var petRelation in request.PetRelations)
                    {
                        // Validate pet exists
                        var petExists = await _context.Pet.AnyAsync(p => p.PetId == petRelation.PetId);
                        if (!petExists)
                            throw new ArgumentException($"Pet with ID {petRelation.PetId} not found");

                        // Validate relationship type
                        var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
                        if (!relationshipTypes.Any(rt => rt.Code == petRelation.RelationshipType))
                            throw new ArgumentException($"Invalid relationship type: {petRelation.RelationshipType}");

                        // Check if relation already exists
                        var existingRelation = await _context.PetRelation
                            .FirstOrDefaultAsync(pr => pr.ContactPersonId == contactPerson.ContactPersonId && pr.PetId == petRelation.PetId);
                        if (existingRelation != null)
                            continue; // Skip duplicate relation

                        var relation = new PetRelation
                        {
                            ContactPersonId = contactPerson.ContactPersonId,
                            PetId = petRelation.PetId,
                            RelationshipType = petRelation.RelationshipType,
                            Sort = petRelation.Sort,
                            CreateUser = "SYSTEM",
                            ModifyUser = "SYSTEM"
                        };

                        _context.PetRelation.Add(relation);
                    }
                    await _context.SaveChangesAsync();
                }

                await transaction.CommitAsync();
                return contactPerson.ContactPersonId;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
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
                ContactNumber = cp.ContactNumber
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

            // Validate relationship type
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            if (!relationshipTypes.Any(rt => rt.Code == request.RelationshipType))
                throw new ArgumentException($"Invalid relationship type: {request.RelationshipType}");

            // Check if relation already exists
            var existingRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.ContactPersonId == contactPersonId && pr.PetId == petId);
            if (existingRelation != null)
                throw new InvalidOperationException("Relation already exists between this contact person and pet");

            var petRelation = new PetRelation
            {
                ContactPersonId = contactPersonId,
                PetId = petId,
                RelationshipType = request.RelationshipType,
                Sort = request.Sort,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.PetRelation.Add(petRelation);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateContactPersonToPetRelation(long contactPersonId, long petId, UpdateContactToPetRequest request)
        {
            // Check if contact person exists
            var contactExists = await _context.ContactPerson.AnyAsync(cp => cp.ContactPersonId == contactPersonId);
            if (!contactExists)
                throw new ArgumentException($"ContactPerson with ID {contactPersonId} not found");

            // Check if pet exists
            var petExists = await _context.Pet.AnyAsync(p => p.PetId == petId);
            if (!petExists)
                throw new ArgumentException($"Pet with ID {petId} not found");

            // Validate relationship type
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            if (!relationshipTypes.Any(rt => rt.Code == request.RelationshipType))
                throw new ArgumentException($"Invalid relationship type: {request.RelationshipType}");

            // Find existing relation
            var petRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.ContactPersonId == contactPersonId && pr.PetId == petId);
            
            if (petRelation == null)
                throw new ArgumentException("Relation not found between this contact person and pet");

            // Update the relation
            petRelation.RelationshipType = request.RelationshipType;
            petRelation.Sort = request.Sort;
            petRelation.ModifyUser = "SYSTEM"; // TODO: Get from current user context

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
                ContactNumber = cp.ContactNumber
            }).ToList();
        }

        public async Task<IList<RelationshipTypeResponse>> GetRelationshipTypes()
        {
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            return relationshipTypes.Select(rt => new RelationshipTypeResponse
            {
                Code = rt.Code,
                Name = rt.Name,
                Description = rt.Description,
                Sort = rt.Sort ?? 0
            }).OrderBy(rt => rt.Sort).ToList();
        }

        private async Task<List<ContactPersonResponse>> MapContactPersonsToResponse(IEnumerable<ContactPerson> contactPersons)
        {
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            var relationshipTypeLookup = relationshipTypes.ToDictionary(rt => rt.Code, rt => rt.Name);
            
            var breedTypes = await _commonService.GetSystemCodeList("Breed");
            var breedTypeLookup = breedTypes.ToDictionary(bt => bt.Code, bt => bt.Name);

            return contactPersons.Select(cp => new ContactPersonResponse
            {
                ContactPersonId = cp.ContactPersonId,
                Name = cp.Name,
                NickName = cp.NickName,
                ContactNumber = cp.ContactNumber,
                RelatedPets = cp.PetRelation?.Select(pr => new PetRelationInfo
                {
                    PetRelationId = pr.PetRelationId,
                    PetId = pr.PetId,
                    PetName = pr.Pet?.PetName ?? "",
                    Breed = breedTypeLookup.TryGetValue(pr.Pet?.Breed ?? "", out var breedName) ? breedName : (pr.Pet?.Breed ?? ""),
                    Gender = pr.Pet?.Gender ?? "",
                    RelationshipType = pr.RelationshipType ?? "",
                    RelationshipTypeName = relationshipTypeLookup.TryGetValue(pr.RelationshipType ?? "", out var name) ? name : "",
                    Sort = pr.Sort
                }).OrderBy(pr => pr.Sort).ToList()
            }).ToList();
        }
    }
}