using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class ContactPersonController : ControllerBase
    {
        private readonly IContactPersonService _contactPersonService;

        public ContactPersonController(IContactPersonService contactPersonService)
        {
            _contactPersonService = contactPersonService;
        }

        [HttpGet(Name = nameof(GetContactPersonList))]
        public async Task<ActionResult<IList<ContactPerson>>> GetContactPersonList()
        {
            return Ok(await _contactPersonService.GetContactPersonList());
        }

        [HttpGet("{contactPersonId}", Name = nameof(GetContactPerson))]
        public async Task<ActionResult<ContactPerson>> GetContactPerson(long contactPersonId)
        {
            var contactPerson = await _contactPersonService.GetContactPerson(contactPersonId);
            if (contactPerson == null)
                return NotFound();
            return contactPerson;
        }

        [HttpPost(Name = nameof(CreateContactPerson))]
        public async Task<ActionResult<long>> CreateContactPerson(ContactPerson contactPerson)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var contactPersonId = await _contactPersonService.CreateContactPerson(contactPerson);
            return CreatedAtAction(nameof(GetContactPerson), 
                new { contactPersonId = contactPersonId }, contactPersonId);
        }

        [HttpPut("{contactPersonId}", Name = nameof(UpdateContactPerson))]
        public async Task<IActionResult> UpdateContactPerson(long contactPersonId, ContactPerson contactPerson)
        {
            if (contactPersonId != contactPerson.ContactPersonId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _contactPersonService.UpdateContactPerson(contactPerson);
            return NoContent();
        }

        [HttpDelete("{contactPersonId}", Name = nameof(DeleteContactPerson))]
        public async Task<IActionResult> DeleteContactPerson(long contactPersonId)
        {
            var contactPerson = await _contactPersonService.GetContactPerson(contactPersonId);
            if (contactPerson == null)
                return NotFound();

            await _contactPersonService.DeleteContactPerson(contactPersonId);
            return NoContent();
        }

        [HttpGet("pet/{petId}", Name = nameof(GetContactPersonsByPet))]
        public async Task<ActionResult<IList<ContactPerson>>> GetContactPersonsByPet(long petId)
        {
            return Ok(await _contactPersonService.GetContactPersonsByPet(petId));
        }

        [HttpPost("{contactPersonId}/pets/{petId}", Name = nameof(LinkContactPersonToPet))]
        public async Task<IActionResult> LinkContactPersonToPet(long contactPersonId, long petId, [FromBody] string relationshipType)
        {
            await _contactPersonService.LinkContactPersonToPet(contactPersonId, petId, relationshipType);
            return NoContent();
        }

        [HttpDelete("{contactPersonId}/pets/{petId}", Name = nameof(UnlinkContactPersonFromPet))]
        public async Task<IActionResult> UnlinkContactPersonFromPet(long contactPersonId, long petId)
        {
            await _contactPersonService.UnlinkContactPersonFromPet(contactPersonId, petId);
            return NoContent();
        }
    }
}