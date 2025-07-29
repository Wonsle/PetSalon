using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/petrelation")]
    public class PetRelationController : BaseController
    {
        private readonly IPetRelationService _petRelationService;

        public PetRelationController(IPetRelationService petRelationService)
        {
            _petRelationService = petRelationService;
        }

        /// <summary>
        /// 取得寵物關係列表 (支援搜尋和分頁)
        /// </summary>
        [HttpGet]
        public async Task<ActionResult<PetRelationListResponse>> GetPetRelationList([FromQuery] PetRelationSearchRequest request)
        {
            try
            {
                var result = await _petRelationService.GetPetRelationList(request);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<PetRelationListResponse>(ex);
            }
        }

        /// <summary>
        /// 根據ID取得寵物關係詳細資訊
        /// </summary>
        [HttpGet("{petRelationId}")]
        public async Task<ActionResult<PetRelationResponse>> GetPetRelation(long petRelationId)
        {
            try
            {
                var petRelation = await _petRelationService.GetPetRelation(petRelationId);
                if (petRelation == null)
                    return NotFound($"找不到ID為 {petRelationId} 的寵物關係");
                
                return Ok(petRelation);
            }
            catch (Exception ex)
            {
                return HandleException<PetRelationResponse>(ex);
            }
        }

        /// <summary>
        /// 根據寵物ID取得相關聯絡人
        /// </summary>
        [HttpGet("bypet/{petId}")]
        public async Task<ActionResult<IList<PetRelationResponse>>> GetRelationsByPet(long petId)
        {
            try
            {
                var result = await _petRelationService.GetRelationsByPet(petId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<IList<PetRelationResponse>>(ex);
            }
        }

        /// <summary>
        /// 根據聯絡人ID取得相關寵物
        /// </summary>
        [HttpGet("bycontact/{contactPersonId}")]
        public async Task<ActionResult<IList<PetRelationResponse>>> GetRelationsByContact(long contactPersonId)
        {
            try
            {
                var result = await _petRelationService.GetRelationsByContact(contactPersonId);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return HandleException<IList<PetRelationResponse>>(ex);
            }
        }

        /// <summary>
        /// 新增寵物關係
        /// </summary>
        [HttpPost]
        public async Task<ActionResult<long>> CreatePetRelation([FromBody] CreatePetRelationApiRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                var petRelationId = await _petRelationService.CreatePetRelation(request);
                return CreatedAtAction(nameof(GetPetRelation), 
                    new { petRelationId = petRelationId }, petRelationId);
            }
            catch (Exception ex)
            {
                return HandleException<long>(ex);
            }
        }

        /// <summary>
        /// 更新寵物關係資訊
        /// </summary>
        [HttpPut("{petRelationId}")]
        public async Task<IActionResult> UpdatePetRelation(long petRelationId, [FromBody] UpdatePetRelationApiRequest request)
        {
            try
            {
                if (petRelationId != request.PetRelationId)
                    return BadRequest("路徑中的ID與請求資料中的ID不一致");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                await _petRelationService.UpdatePetRelation(request);
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
        /// 刪除寵物關係
        /// </summary>
        [HttpDelete("{petRelationId}")]
        public async Task<IActionResult> DeletePetRelation(long petRelationId)
        {
            try
            {
                await _petRelationService.DeletePetRelation(petRelationId);
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
    }
}