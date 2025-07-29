using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Services
{
    public class PetService : IPetService
    {
        PetSalonContext _context { get; set; }
        ICommonService _commonService { get; set; }

        public PetService(PetSalonContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }
        public async Task<IList<Pet>> GetPetList()
        {
            var pets = await _context.Pet
                .AsNoTracking()
                .Select(p => new Pet
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Breed = p.Breed, // 保持原始的 code 值
                    Gender = p.Gender,
                    BirthDay = p.BirthDay,
                    NormalPrice = p.NormalPrice,
                    SubscriptionPrice = p.SubscriptionPrice,
                    CreateUser = p.CreateUser,
                    CreateTime = p.CreateTime,
                    ModifyUser = p.ModifyUser,
                    ModifyTime = p.ModifyTime
                })
                .ToListAsync();

            return pets;
        }

        public async Task<IList<PetListResponse>> GetPetListWithOwners()
        {
            // 取得品種SystemCode列表，建立查找字典
            var breedCodes = await _commonService.GetSystemCodeList("Breed");
            var breedDict = breedCodes.ToDictionary(sc => sc.Code, sc => sc.Name);

            var pets = await _context.Pet
                .Include(p => p.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                .AsNoTracking()
                .Select(p => new PetListResponse
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Breed = p.Breed, // 暫時保留原始代碼，後續將轉換為名稱
                    Gender = p.Gender,
                    BirthDay = p.BirthDay,
                    NormalPrice = p.NormalPrice,
                    SubscriptionPrice = p.SubscriptionPrice,
                    CreateUser = p.CreateUser,
                    CreateTime = p.CreateTime,
                    ModifyUser = p.ModifyUser,
                    ModifyTime = p.ModifyTime,
                    Owners = p.PetRelation
                        .Where(pr => pr.RelationshipType == "OWNER") // 只取飼主關係
                        .Select(pr => new PetOwnerInfo
                        {
                            ContactPersonId = pr.ContactPersonId,
                            Name = pr.ContactPerson.Name,
                            ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                            RelationshipType = pr.RelationshipType,
                            RelationshipTypeName = _context.SystemCode
                                .Where(sc => sc.CodeType == "Relationship" && sc.Code == pr.RelationshipType)
                                .Select(sc => sc.Name)
                                .FirstOrDefault() ?? pr.RelationshipType
                        })
                        .OrderBy(o => o.Name)
                        .ToList()
                })
                .ToListAsync();

            // 轉換品種代碼為品種名稱
            foreach (var pet in pets)
            {
                if (breedDict.TryGetValue(pet.Breed, out var breedName))
                {
                    pet.Breed = breedName;
                }
                // 如果找不到對應的品種名稱，保留原始代碼
            }

            return pets;
        }

        public async Task<long> CreatePet(Pet pet)
        {
            pet.CreateUser = "System"; // TODO: 從認證中取得實際使用者
            pet.ModifyUser = "System";
            _context.Pet.Add(pet);
            await _context.SaveChangesAsync();

            return pet.PetId;

        }

        public async Task DeletePet(long petID)
        {
            var pet = new Pet() { PetId = petID};
            _context.Pet.Attach(pet);
            _context.Pet.Remove(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<Pet?> GetPet(long petID)
        {
            var pet = await _context.Pet
                .Where(p => p.PetId == petID)
                .Select(p => new Pet
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Breed = p.Breed, // 保持原始的 code 值
                    Gender = p.Gender,
                    BirthDay = p.BirthDay,
                    NormalPrice = p.NormalPrice,
                    SubscriptionPrice = p.SubscriptionPrice,
                    CreateUser = p.CreateUser,
                    CreateTime = p.CreateTime,
                    ModifyUser = p.ModifyUser,
                    ModifyTime = p.ModifyTime
                })
                .FirstOrDefaultAsync();

            return pet;
        }

        public async Task<PetDetailResponse?> GetPetDetailWithContacts(long petID)
        {
            // 取得品種SystemCode列表，建立查找字典
            var breedCodes = await _commonService.GetSystemCodeList("Breed");
            var breedDict = breedCodes.ToDictionary(sc => sc.Code, sc => sc.Name);

            var pet = await _context.Pet
                .Include(p => p.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                .AsNoTracking()
                .Where(p => p.PetId == petID)
                .Select(p => new PetDetailResponse
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Breed = p.Breed, // 暫時保留原始代碼，後續將轉換為名稱
                    Gender = p.Gender,
                    BirthDay = p.BirthDay,
                    NormalPrice = p.NormalPrice,
                    SubscriptionPrice = p.SubscriptionPrice,
                    CreateUser = p.CreateUser,
                    CreateTime = p.CreateTime,
                    ModifyUser = p.ModifyUser,
                    ModifyTime = p.ModifyTime,
                    Owners = p.PetRelation
                        .Where(pr => pr.RelationshipType == "OWNER") // 只取飼主關係
                        .Select(pr => new PetOwnerInfo
                        {
                            ContactPersonId = pr.ContactPersonId,
                            Name = pr.ContactPerson.Name,
                            ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                            RelationshipType = pr.RelationshipType,
                            RelationshipTypeName = _context.SystemCode
                                .Where(sc => sc.CodeType == "Relationship" && sc.Code == pr.RelationshipType)
                                .Select(sc => sc.Name)
                                .FirstOrDefault() ?? pr.RelationshipType
                        })
                        .OrderBy(o => o.Name)
                        .ToList(),
                    AllContacts = p.PetRelation
                        .Select(pr => new PetOwnerInfo
                        {
                            ContactPersonId = pr.ContactPersonId,
                            Name = pr.ContactPerson.Name,
                            ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                            RelationshipType = pr.RelationshipType,
                            RelationshipTypeName = _context.SystemCode
                                .Where(sc => sc.CodeType == "Relationship" && sc.Code == pr.RelationshipType)
                                .Select(sc => sc.Name)
                                .FirstOrDefault() ?? pr.RelationshipType
                        })
                        .OrderBy(c => c.RelationshipType == "OWNER" ? 0 : 1) // 飼主排在前面
                        .ThenBy(c => c.Name)
                        .ToList()
                })
                .FirstOrDefaultAsync();

            // 轉換品種代碼為品種名稱
            if (pet != null && breedDict.TryGetValue(pet.Breed, out var breedName))
            {
                pet.Breed = breedName;
            }

            return pet;
        }

        public async Task UpdatePet(Pet pet)
        {
            pet.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
            _context.Pet.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<Pet>> GetPetsByContactPerson(long contactPersonId)
        {
            return await _context.Pet
                .Include(p => p.PetRelation)
                .Where(p => p.PetRelation.Any(pr => pr.ContactPersonId == contactPersonId))
                .Select(p => new Pet
                {
                    PetId = p.PetId,
                    PetName = p.PetName,
                    Breed = _context.SystemCode
                        .Where(sc => sc.CodeType == "Breed" && sc.Code == p.Breed)
                        .Select(sc => sc.Name)
                        .FirstOrDefault() ?? p.Breed,
                    Gender = p.Gender,
                    BirthDay = p.BirthDay,
                    NormalPrice = p.NormalPrice,
                    SubscriptionPrice = p.SubscriptionPrice,
                    CreateUser = p.CreateUser,
                    CreateTime = p.CreateTime,
                    ModifyUser = p.ModifyUser,
                    ModifyTime = p.ModifyTime,
                    PetRelation = p.PetRelation
                })
                .AsNoTracking()
                .ToListAsync();
        }
    }
}
