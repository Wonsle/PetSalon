using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PetSalon.Models.EntityModels;
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

        public PetService(PetSalonContext context)
        {
            _context = context;
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
