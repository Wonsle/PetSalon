using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
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

        [HttpGet(Name = nameof(GetPets))]
        public async Task<IList<Pet>> GetPets()
        {
            return await _petService.GetPetList();
        }

        [HttpGet("{petID}", Name = nameof(GetPet))]
        public async Task<ActionResult<Pet>> GetPet(long petID)
        {
            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();
            return pet;
        }

        [HttpPost(Name = nameof(CreatePet))]
        public async Task<ActionResult<long>> CreatePet([FromBody] CreatePetRequest request)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);
            
            var pet = new Pet
            {
                PetName = request.PetName,
                Gender = request.Gender,
                Breed = request.Breed,
                BirthDay = request.BirthDay,
                NormalPrice = request.NormalPrice,
                SubscriptionPrice = request.SubscriptionPrice
            };
            
            var petId = await _petService.CreatePet(pet);
            return CreatedAtAction(nameof(GetPet), new { petID = petId }, petId);
        }

        [HttpPut("{petID}", Name = nameof(UpdatePet))]
        public async Task<IActionResult> UpdatePet(long petID, [FromBody] UpdatePetRequest request)
        {
            if (petID != request.PetId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();

            pet.PetName = request.PetName;
            pet.Gender = request.Gender;
            pet.Breed = request.Breed;
            pet.BirthDay = request.BirthDay;
            pet.NormalPrice = request.NormalPrice;
            pet.SubscriptionPrice = request.SubscriptionPrice;

            await _petService.UpdatePet(pet);
            return NoContent();
        }

        [HttpDelete("{petID}", Name = nameof(DeletePet))]
        public async Task<IActionResult> DeletePet(long petID)
        {
            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();
                
            await _petService.DeletePet(petID);
            return NoContent();
        }

        [HttpGet("contact/{contactPersonId}", Name = nameof(GetPetsByContact))]
        public async Task<ActionResult<IList<Pet>>> GetPetsByContact(long contactPersonId)
        {
            return Ok(await _petService.GetPetsByContactPerson(contactPersonId));
        }

        [HttpPost("{petID}/photo", Name = nameof(UploadPetPhoto))]
        public async Task<IActionResult> UploadPetPhoto(long petID, IFormFile photo)
        {
            if (photo == null || photo.Length == 0)
                return BadRequest("No photo provided");

            var pet = await _petService.GetPet(petID);
            if (pet == null)
                return NotFound();

            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif" };
            var extension = Path.GetExtension(photo.FileName).ToLowerInvariant();
            
            if (!allowedExtensions.Contains(extension))
                return BadRequest("Invalid file type. Only JPG, PNG, and GIF are allowed.");

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", "pets");
            Directory.CreateDirectory(uploadsFolder);

            var fileName = $"{petID}_{Guid.NewGuid()}{extension}";
            var filePath = Path.Combine(uploadsFolder, fileName);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await photo.CopyToAsync(stream);
            }

            return Ok(new { PhotoUrl = $"/uploads/pets/{fileName}" });
        }
    }
}
