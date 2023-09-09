using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class PetController : ControllerBase
    {

        private readonly IPetService _petService;
        public PetController(IPetService petService)
        {
            _petService = petService;
        }

        [HttpGet("Index", Name = nameof(Index))]
        public async Task<IList<Pet>> Index()
        {
            return await _petService.GetPetList();

        }

        [HttpPut("GetPet", Name = nameof(GetPet))]
        public async Task<Pet> GetPet(long petID)
        {
            return await _petService.GetPet(petID);

        }

        [HttpPost("CreatePet", Name = nameof(CreatePet))]
        public async Task<long> CreatePet(Pet pet)
        {
            return await _petService.CreatePet(pet);

        }

        [HttpDelete("DeletePet", Name = nameof(DeletePet))]
        public async Task<IActionResult> DeletePet(long petID)
        {
            await _petService.DeletePet(petID);
            return Ok();

        }
    }
}
