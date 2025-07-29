using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class PetRelationService : IPetRelationService
    {
        private readonly PetSalonContext _context;
        private readonly ICommonService _commonService;

        public PetRelationService(PetSalonContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<PetRelationListResponse> GetPetRelationList(PetRelationSearchRequest request)
        {
            var query = _context.PetRelation
                .Include(pr => pr.ContactPerson)
                .Include(pr => pr.Pet)
                .AsNoTracking()
                .AsQueryable();

            // Apply search filters
            if (request.PetId.HasValue)
            {
                query = query.Where(pr => pr.PetId == request.PetId.Value);
            }

            if (request.ContactPersonId.HasValue)
            {
                query = query.Where(pr => pr.ContactPersonId == request.ContactPersonId.Value);
            }

            var total = await query.CountAsync();

            var petRelations = await query
                .OrderBy(pr => pr.Sort)
                .ThenBy(pr => pr.CreateTime)
                .Skip((request.Page - 1) * request.PageSize)
                .Take(request.PageSize)
                .ToListAsync();

            var responseData = await MapPetRelationsToResponse(petRelations);

            return new PetRelationListResponse
            {
                Data = responseData,
                Total = total,
                Page = request.Page,
                PageSize = request.PageSize
            };
        }

        public async Task<PetRelationResponse?> GetPetRelation(long id)
        {
            var petRelation = await _context.PetRelation
                .Include(pr => pr.ContactPerson)
                .Include(pr => pr.Pet)
                .AsNoTracking()
                .FirstOrDefaultAsync(pr => pr.PetRelationId == id);

            if (petRelation == null)
                return null;

            var responseList = await MapPetRelationsToResponse(new[] { petRelation });
            return responseList.FirstOrDefault();
        }

        public async Task<IList<PetRelationResponse>> GetRelationsByPet(long petId)
        {
            var petRelations = await _context.PetRelation
                .Include(pr => pr.ContactPerson)
                .Include(pr => pr.Pet)
                .Where(pr => pr.PetId == petId)
                .OrderBy(pr => pr.Sort)
                .AsNoTracking()
                .ToListAsync();

            return await MapPetRelationsToResponse(petRelations);
        }

        public async Task<IList<PetRelationResponse>> GetRelationsByContact(long contactPersonId)
        {
            var petRelations = await _context.PetRelation
                .Include(pr => pr.ContactPerson)
                .Include(pr => pr.Pet)
                .Where(pr => pr.ContactPersonId == contactPersonId)
                .OrderBy(pr => pr.Sort)
                .AsNoTracking()
                .ToListAsync();

            return await MapPetRelationsToResponse(petRelations);
        }

        public async Task<long> CreatePetRelation(CreatePetRelationApiRequest request)
        {
            // Check if pet exists
            var petExists = await _context.Pet.AnyAsync(p => p.PetId == request.PetId);
            if (!petExists)
                throw new ArgumentException($"Pet with ID {request.PetId} not found");

            // Check if contact person exists
            var contactExists = await _context.ContactPerson.AnyAsync(cp => cp.ContactPersonId == request.ContactPersonId);
            if (!contactExists)
                throw new ArgumentException($"ContactPerson with ID {request.ContactPersonId} not found");

            // Validate relationship type
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            if (!relationshipTypes.Any(rt => rt.Code == request.RelationshipType))
                throw new ArgumentException($"Invalid relationship type: {request.RelationshipType}");

            // Check if relation already exists
            var existingRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.PetId == request.PetId && pr.ContactPersonId == request.ContactPersonId);
            if (existingRelation != null)
                throw new InvalidOperationException("Relation already exists between this pet and contact person");

            var petRelation = new PetRelation
            {
                PetId = request.PetId,
                ContactPersonId = request.ContactPersonId,
                RelationshipType = request.RelationshipType,
                Sort = request.Sort,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.PetRelation.Add(petRelation);
            await _context.SaveChangesAsync();

            return petRelation.PetRelationId;
        }

        public async Task UpdatePetRelation(UpdatePetRelationApiRequest request)
        {
            var petRelation = await _context.PetRelation.FindAsync(request.PetRelationId);
            if (petRelation == null)
                throw new ArgumentException($"PetRelation with ID {request.PetRelationId} not found");

            // Check if pet exists
            var petExists = await _context.Pet.AnyAsync(p => p.PetId == request.PetId);
            if (!petExists)
                throw new ArgumentException($"Pet with ID {request.PetId} not found");

            // Check if contact person exists
            var contactExists = await _context.ContactPerson.AnyAsync(cp => cp.ContactPersonId == request.ContactPersonId);
            if (!contactExists)
                throw new ArgumentException($"ContactPerson with ID {request.ContactPersonId} not found");

            // Validate relationship type
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            if (!relationshipTypes.Any(rt => rt.Code == request.RelationshipType))
                throw new ArgumentException($"Invalid relationship type: {request.RelationshipType}");

            // Check if the updated relation would create a duplicate (excluding current record)
            var existingRelation = await _context.PetRelation
                .FirstOrDefaultAsync(pr => pr.PetId == request.PetId && 
                                          pr.ContactPersonId == request.ContactPersonId && 
                                          pr.PetRelationId != request.PetRelationId);
            if (existingRelation != null)
                throw new InvalidOperationException("Relation already exists between this pet and contact person");

            petRelation.PetId = request.PetId;
            petRelation.ContactPersonId = request.ContactPersonId;
            petRelation.RelationshipType = request.RelationshipType;
            petRelation.Sort = request.Sort;
            petRelation.ModifyUser = "SYSTEM"; // TODO: Get from current user context

            await _context.SaveChangesAsync();
        }

        public async Task DeletePetRelation(long petRelationId)
        {
            var petRelation = await _context.PetRelation.FindAsync(petRelationId);
            if (petRelation == null)
                throw new ArgumentException($"PetRelation with ID {petRelationId} not found");

            _context.PetRelation.Remove(petRelation);
            await _context.SaveChangesAsync();
        }

        private async Task<List<PetRelationResponse>> MapPetRelationsToResponse(IEnumerable<PetRelation> petRelations)
        {
            var relationshipTypes = await _commonService.GetSystemCodeList("Relationship");
            var relationshipTypeLookup = relationshipTypes.ToDictionary(rt => rt.Code, rt => rt.Name);

            var breedTypes = await _commonService.GetSystemCodeList("Breed");
            var breedTypeLookup = breedTypes.ToDictionary(bt => bt.Code, bt => bt.Name);

            return petRelations.Select(pr => new PetRelationResponse
            {
                PetRelationId = pr.PetRelationId,
                PetId = pr.PetId,
                ContactPersonId = pr.ContactPersonId,
                RelationshipType = pr.RelationshipType ?? "",
                RelationshipTypeName = relationshipTypeLookup.TryGetValue(pr.RelationshipType ?? "", out var relationshipName) ? relationshipName : "",
                Sort = pr.Sort,
                CreateUser = pr.CreateUser ?? "",
                CreateTime = pr.CreateTime,
                ModifyUser = pr.ModifyUser ?? "",
                ModifyTime = pr.ModifyTime,
                ContactPerson = pr.ContactPerson != null ? new ContactPersonInfo
                {
                    ContactPersonId = pr.ContactPerson.ContactPersonId,
                    Name = pr.ContactPerson.Name ?? "",
                    NickName = pr.ContactPerson.NickName,
                    ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                    Email = null,
                    Address = null
                } : null,
                Pet = pr.Pet != null ? new PetInfo
                {
                    PetId = pr.Pet.PetId,
                    PetName = pr.Pet.PetName ?? "",
                    Breed = breedTypeLookup.TryGetValue(pr.Pet.Breed ?? "", out var breedName) ? breedName : (pr.Pet.Breed ?? ""),
                    Gender = pr.Pet.Gender ?? ""
                } : null
            }).ToList();
        }
    }
}