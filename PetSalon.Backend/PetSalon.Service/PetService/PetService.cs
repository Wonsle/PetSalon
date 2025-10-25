using Microsoft.EntityFrameworkCore;
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
                    CoatColor = p.CoatColor,
                    BodyWeight = p.BodyWeight,
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
            // 取得品種和關係SystemCode列表，建立查找字典
            var breedCodes = await _commonService.GetSystemCodeList("Breed");
            var relationshipCodes = await _commonService.GetSystemCodeList("Relationship");
            var breedDict = breedCodes.ToDictionary(sc => sc.Code, sc => sc.Name);
            var relationshipDict = relationshipCodes.ToDictionary(sc => sc.Code, sc => sc.Name);

            var pets = await _context.Pet
                .Include(p => p.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                .AsNoTracking()
                .ToListAsync();

            // 查詢所有寵物的照片
            var petIds = pets.Select(p => p.PetId).ToList();
            var photos = await _context.FileAttachment
                .Where(f => f.EntityType == "Pet" && petIds.Contains(f.EntityId) && f.IsActive && f.AttachmentType == "Photo")
                .OrderBy(f => f.EntityId)
                .ThenBy(f => f.DisplayOrder)
                .Select(f => new { f.EntityId, f.FilePath })
                .ToListAsync();

            var photoDict = photos
                .GroupBy(p => p.EntityId)
                .ToDictionary(g => g.Key, g => g.Select(p => p.FilePath).ToList());

            var result = pets.Select(p => new PetListResponse
            {
                PetId = p.PetId,
                PetName = p.PetName,
                Breed = p.Breed, // 保持原始 code 值
                Gender = p.Gender,
                BirthDay = p.BirthDay,
                CoatColor = p.CoatColor,
                BodyWeight = p.BodyWeight,
                CreateUser = p.CreateUser,
                CreateTime = p.CreateTime,
                ModifyUser = p.ModifyUser,
                ModifyTime = p.ModifyTime,
                PhotoUrl = photoDict.TryGetValue(p.PetId, out var petPhotos) && petPhotos.Any()
                    ? petPhotos.First() : null,
                Photos = photoDict.TryGetValue(p.PetId, out var allPhotos)
                    ? allPhotos : null,
                Owners = p.PetRelation
                    .OrderBy(pr => pr.Sort) // 按 Sort 排序
                    .Take(1) // 只取第一個（Sort 最小的）
                    .Select(pr => new PetOwnerInfo
                    {
                        ContactPersonId = pr.ContactPersonId,
                        Name = pr.ContactPerson.Name,
                        NickName = pr.ContactPerson.NickName,
                        ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                        RelationshipType = pr.RelationshipType,
                        RelationshipTypeName = relationshipDict.TryGetValue(pr.RelationshipType, out var relationshipName)
                            ? relationshipName : pr.RelationshipType,
                        Sort = pr.Sort
                    })
                    .ToList()
            }).ToList();

            // 填入品種中文名稱到 BreedName 欄位
            foreach (var pet in result)
            {
                if (breedDict.TryGetValue(pet.Breed, out var breedName))
                {
                    pet.BreedName = breedName;
                }
                // 如果找不到對應的品種名稱，BreedName 保持 null
            }

            return result;
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
                    CoatColor = p.CoatColor,
                    BodyWeight = p.BodyWeight,
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
            // 取得關係SystemCode列表，建立查找字典
            var relationshipCodes = await _commonService.GetSystemCodeList("Relationship");
            var relationshipDict = relationshipCodes.ToDictionary(sc => sc.Code, sc => sc.Name);

            var pet = await _context.Pet
                .Include(p => p.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                .AsNoTracking()
                .Where(p => p.PetId == petID)
                .FirstOrDefaultAsync();

            if (pet == null)
                return null;

            // 查詢寵物照片
            var photos = await _context.FileAttachment
                .Where(f => f.EntityType == "Pet" && f.EntityId == petID && f.IsActive && f.AttachmentType == "Photo")
                .OrderBy(f => f.DisplayOrder)
                .Select(f => f.FilePath)
                .ToListAsync();

            return new PetDetailResponse
            {
                PetId = pet.PetId,
                PetName = pet.PetName,
                Breed = pet.Breed, // 保留原始代碼供前端 SystemCodeSelect 使用
                Gender = pet.Gender,
                BirthDay = pet.BirthDay,
                CoatColor = pet.CoatColor,
                BodyWeight = pet.BodyWeight,
                CreateUser = pet.CreateUser,
                CreateTime = pet.CreateTime,
                ModifyUser = pet.ModifyUser,
                ModifyTime = pet.ModifyTime,
                PhotoUrl = photos.FirstOrDefault(),
                Photos = photos.Any() ? photos : null,
                Owners = pet.PetRelation
                    .OrderBy(pr => pr.Sort) // 按 Sort 排序
                    .Take(1) // 只取第一個（Sort 最小的）
                    .Select(pr => new PetOwnerInfo
                    {
                        ContactPersonId = pr.ContactPersonId,
                        Name = pr.ContactPerson.Name,
                        NickName = pr.ContactPerson.NickName,
                        ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                        RelationshipType = pr.RelationshipType,
                        RelationshipTypeName = relationshipDict.TryGetValue(pr.RelationshipType, out var relationshipName)
                            ? relationshipName : pr.RelationshipType,
                        Sort = pr.Sort
                    })
                    .ToList(),
                AllContacts = pet.PetRelation
                    .OrderBy(pr => pr.Sort) // 按 Sort 排序
                    .Select(pr => new PetOwnerInfo
                    {
                        ContactPersonId = pr.ContactPersonId,
                        Name = pr.ContactPerson.Name,
                        NickName = pr.ContactPerson.NickName,
                        ContactNumber = pr.ContactPerson.ContactNumber ?? "",
                        RelationshipType = pr.RelationshipType,
                        RelationshipTypeName = relationshipDict.TryGetValue(pr.RelationshipType, out var relationshipName)
                            ? relationshipName : pr.RelationshipType,
                        Sort = pr.Sort
                    })
                    .ToList()
            };
        }

        public async Task UpdatePet(Pet pet)
        {
            pet.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
            _context.Pet.Update(pet);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<Pet>> GetPetsByContactPerson(long contactPersonId)
        {
            // 取得品種SystemCode列表，建立查找字典
            var breedCodes = await _commonService.GetSystemCodeList("Breed");
            var breedDict = breedCodes.ToDictionary(sc => sc.Code, sc => sc.Name);

            var pets = await _context.Pet
                .Include(p => p.PetRelation)
                .Where(p => p.PetRelation.Any(pr => pr.ContactPersonId == contactPersonId))
                .AsNoTracking()
                .ToListAsync();

            return pets.Select(p => new Pet
            {
                PetId = p.PetId,
                PetName = p.PetName,
                Breed = breedDict.TryGetValue(p.Breed, out var breedName) ? breedName : p.Breed,
                Gender = p.Gender,
                BirthDay = p.BirthDay,
                CoatColor = p.CoatColor,
                BodyWeight = p.BodyWeight,
                CreateUser = p.CreateUser,
                CreateTime = p.CreateTime,
                ModifyUser = p.ModifyUser,
                ModifyTime = p.ModifyTime,
                PetRelation = p.PetRelation
            }).ToList();
        }
    }
}
