using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Services;
using PetSalon.Web.Controllers;
using System.Linq;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 寵物服務時間客製化管理API控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PetServiceDurationController : BaseController
    {
        private readonly IPetServiceDurationService _petServiceDurationService;

        public PetServiceDurationController(IPetServiceDurationService petServiceDurationService)
        {
            _petServiceDurationService = petServiceDurationService;
        }

        /// <summary>
        /// 取得所有寵物服務時間設定
        /// </summary>
        /// <returns>時間設定列表</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetServiceDuration>>> GetPetServiceDurations()
        {
            try
            {
                var durations = await _petServiceDurationService.GetPetServiceDurationListAsync();
                return Ok(durations);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServiceDuration>>(ex);
            }
        }

        /// <summary>
        /// 根據寵物ID取得服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>該寵物的服務時間設定列表</returns>
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<PetServiceDuration>>> GetPetServiceDurationsByPetId(long petId)
        {
            try
            {
                var durations = await _petServiceDurationService.GetPetServiceDurationsByPetAsync(petId);
                return Ok(durations);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServiceDuration>>(ex);
            }
        }

        /// <summary>
        /// 根據服務ID取得所有寵物的時間設定
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns>該服務的所有寵物時間設定列表</returns>
        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<IEnumerable<PetServiceDuration>>> GetPetServiceDurationsByServiceId(long serviceId)
        {
            try
            {
                var durations = await _petServiceDurationService.GetPetServiceDurationsByServiceAsync(serviceId);
                return Ok(durations);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServiceDuration>>(ex);
            }
        }

        /// <summary>
        /// 取得特定寵物的特定服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>時間設定詳細資訊</returns>
        [HttpGet("pet/{petId}/service/{serviceId}")]
        public async Task<ActionResult<PetServiceDuration?>> GetPetServiceDuration(long petId, long serviceId)
        {
            try
            {
                var duration = await _petServiceDurationService.GetPetServiceDurationAsync(petId, serviceId);
                if (duration == null)
                {
                    return NotFound(new { message = "找不到指定的時間設定" });
                }
                return Ok(duration);
            }
            catch (Exception ex)
            {
                return HandleException<PetServiceDuration?>(ex);
            }
        }

        /// <summary>
        /// 根據ID取得時間設定詳細資訊
        /// </summary>
        /// <param name="id">時間設定ID</param>
        /// <returns>時間設定詳細資訊</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PetServiceDuration?>> GetPetServiceDurationById(long id)
        {
            try
            {
                var duration = await _petServiceDurationService.GetPetServiceDurationWithDetailsAsync(id);
                if (duration == null)
                {
                    return NotFound(new { message = "找不到指定的時間設定" });
                }
                return Ok(duration);
            }
            catch (Exception ex)
            {
                return HandleException<PetServiceDuration?>(ex);
            }
        }

        /// <summary>
        /// 建立新的寵物服務時間設定
        /// </summary>
        /// <param name="request">時間設定資料</param>
        /// <returns>建立結果</returns>
        [HttpPost]
        public async Task<ActionResult<long>> CreatePetServiceDuration([FromBody] CreatePetServiceDurationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new PetServiceDuration
                {
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomDuration = request.CustomDuration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                };
                
                var durationId = await _petServiceDurationService.CreatePetServiceDurationAsync(entity);
                return CreatedAtAction(nameof(GetPetServiceDurationById), new { id = durationId }, durationId);
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException<long>(ex);
            }
        }

        /// <summary>
        /// 更新寵物服務時間設定
        /// </summary>
        /// <param name="id">時間設定ID</param>
        /// <param name="request">更新的時間設定資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePetServiceDuration(long id, [FromBody] CreatePetServiceDurationRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new PetServiceDuration
                {
                    PetServiceDurationId = id,
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomDuration = request.CustomDuration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                };
                
                await _petServiceDurationService.UpdatePetServiceDurationAsync(entity);

                return NoContent();
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 刪除寵物服務時間設定
        /// </summary>
        /// <param name="id">時間設定ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetServiceDuration(long id)
        {
            try
            {
                await _petServiceDurationService.DeletePetServiceDurationAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 切換寵物服務時間設定的啟用狀態
        /// </summary>
        /// <param name="id">時間設定ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns>操作結果</returns>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> TogglePetServiceDurationStatus(long id, [FromBody] bool isActive)
        {
            try
            {
                await _petServiceDurationService.TogglePetServiceDurationStatusAsync(id, isActive);

                return Ok(new { message = $"時間設定已{(isActive ? "啟用" : "停用")}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 批次建立寵物服務時間設定
        /// </summary>
        /// <param name="requests">時間設定列表</param>
        /// <returns>建立結果</returns>
        [HttpPost("batch")]
        public async Task<ActionResult<List<long>>> BatchCreatePetServiceDurations([FromBody] List<CreatePetServiceDurationRequest> requests)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (requests == null || !requests.Any())
                {
                    return BadRequest(new { message = "請提供至少一個時間設定" });
                }

                var entities = requests.Select(request => new PetServiceDuration
                {
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomDuration = request.CustomDuration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                }).ToList();
                
                await _petServiceDurationService.CreateBatchPetServiceDurationsAsync(entities);
                return Ok(new { message = "批次建立成功", count = requests.Count });
            }
            catch (ArgumentException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return HandleException<List<long>>(ex);
            }
        }

        /// <summary>
        /// 批次刪除寵物服務時間設定
        /// </summary>
        /// <param name="ids">時間設定ID列表</param>
        /// <returns>操作結果</returns>
        [HttpDelete("batch")]
        public async Task<IActionResult> BatchDeletePetServiceDurations([FromBody] List<long> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(new { message = "請提供至少一個時間設定ID" });
                }

                var deletedCount = await _petServiceDurationService.BatchDeletePetServiceDurationsAsync(ids);
                return Ok(new { message = $"成功刪除 {deletedCount} 個時間設定" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 取得有效的寵物服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>有效的服務時間設定列表</returns>
        [HttpGet("active/{petId}")]
        public async Task<ActionResult<IEnumerable<PetServiceDuration>>> GetActivePetServiceDurations(long petId)
        {
            try
            {
                var durations = await _petServiceDurationService.GetActivePetServiceDurationsAsync(petId);
                return Ok(durations);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServiceDuration>>(ex);
            }
        }

        /// <summary>
        /// 取得服務時間統計資料
        /// </summary>
        /// <returns>統計資料</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetServiceDurationStatistics()
        {
            try
            {
                var statistics = await _petServiceDurationService.GetServiceDurationStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }
    }
}