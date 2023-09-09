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
            return await _context.Pet.AsNoTracking().ToListAsync();
        }

        public async Task<long> CreatePet(Pet pet)
        {
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

        public async Task<Pet> GetPet(long petID)
        {
            return await _context.Pet.FindAsync(petID);
        }

        public void UpdatePet(Pet pet)
        {
            throw new NotImplementedException();
        }
    }
}
