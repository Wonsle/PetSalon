using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/contactperson")]
    public class ContactPersonController : BaseController
    {
        private readonly IContactPersonService _contactPersonService;

        public ContactPersonController(IContactPersonService contactPersonService)
        {
            _contactPersonService = contactPersonService;
        }

        /// <summary>
        /// 取得聯絡人列表 (支援搜尋和分頁)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<ContactPersonListResponse>> GetContactPersonList([FromQuery] ContactPersonSearchRequest request)
        {
            try
            {
                var result = await _contactPersonService.GetContactPersonList(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<ContactPersonListResponse>(ex);
            }
        }

        /// <summary>
        /// 根據ID取得聯絡人詳細資訊
        /// </summary>
        [HttpGet("{contactPersonId}")]
        public async Task<ActionResult<ContactPersonResponse>> GetContactPerson(long contactPersonId)
        {
            try
            {
                var contactPerson = await _contactPersonService.GetContactPerson(contactPersonId);
                if (contactPerson == null)
                    return NotFound($"找不到ID為 {contactPersonId} 的聯絡人");
                
                return Ok(contactPerson);
            }
            catch (Exception ex)
            {
                return HandleException<ContactPersonResponse>(ex);
            }
        }

        /// <summary>
        /// 新增聯絡人
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> CreateContactPerson([FromBody] CreateContactPersonRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var contactPersonId = await _contactPersonService.CreateContactPerson(request);
                return CreatedAtAction(nameof(GetContactPerson), 
                    new { contactPersonId = contactPersonId }, contactPersonId);
            }
            catch (Exception ex)
            {
                return HandleException<long>(ex);
            }
        }

        /// <summary>
        /// 更新聯絡人資訊
        /// </summary>
        [HttpPut("{contactPersonId}")]
        public async Task<IActionResult> UpdateContactPerson(long contactPersonId, [FromBody] UpdateContactPersonRequest request)
        {
            try
            {
                if (contactPersonId != request.ContactPersonId)
                    return BadRequest("路徑中的ID與請求資料中的ID不一致");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _contactPersonService.UpdateContactPerson(request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 刪除聯絡人
        /// </summary>
        [HttpDelete("{contactPersonId}")]
        public async Task<IActionResult> DeleteContactPerson(long contactPersonId)
        {
            try
            {
                await _contactPersonService.DeleteContactPerson(contactPersonId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 根據寵物ID取得相關聯絡人
        /// </summary>
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IList<ContactPersonResponse>>> GetContactPersonsByPet(long petId)
        {
            try
            {
                var result = await _contactPersonService.GetContactPersonsByPet(petId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<IList<ContactPersonResponse>>(ex);
            }
        }

        /// <summary>
        /// 建立聯絡人與寵物的關聯
        /// </summary>
        [HttpPost("{contactPersonId}/pets/{petId}")]
        public async Task<IActionResult> LinkContactPersonToPet(long contactPersonId, long petId, [FromBody] LinkContactToPetRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _contactPersonService.LinkContactPersonToPet(contactPersonId, petId, request);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 解除聯絡人與寵物的關聯
        /// </summary>
        [HttpDelete("{contactPersonId}/pets/{petId}")]
        public async Task<IActionResult> UnlinkContactPersonFromPet(long contactPersonId, long petId)
        {
            try
            {
                await _contactPersonService.UnlinkContactPersonFromPet(contactPersonId, petId);
                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return NotFound(ex.Message);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 搜尋聯絡人 (用於下拉選單等場景)
        /// </summary>
        [HttpGet("search")]
        public async Task<ActionResult<IList<ContactPersonResponse>>> SearchContactPersons([FromQuery] string keyword)
        {
            try
            {
                var result = await _contactPersonService.SearchContactPersons(keyword);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<IList<ContactPersonResponse>>(ex);
            }
        }
    }
}