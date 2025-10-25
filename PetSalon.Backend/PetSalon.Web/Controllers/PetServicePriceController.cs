using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Services;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 寵物服務價格管理API控制器
    /// </summary>
    [Route("api/[controller]")]
    [ApiController]
    public class PetServicePriceController : BaseController
    {
        private readonly IPetServicePriceService _petServicePriceService;

        public PetServicePriceController(IPetServicePriceService petServicePriceService)
        {
            _petServicePriceService = petServicePriceService;
        }

        /// <summary>
        /// 取得所有寵物服務價格設定
        /// </summary>
        /// <returns>價格設定列表</returns>
        [HttpGet]
        public async Task<ActionResult<IEnumerable<PetServicePrice>>> GetPetServicePrices()
        {
            try
            {
                var prices = await _petServicePriceService.GetPetServicePriceListAsync();
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServicePrice>>(ex);
            }
        }

        /// <summary>
        /// 根據寵物ID取得服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>該寵物的服務價格設定列表</returns>
        [HttpGet("pet/{petId}")]
        public async Task<ActionResult<IEnumerable<PetServicePrice>>> GetPetServicePricesByPetId(long petId)
        {
            try
            {
                var prices = await _petServicePriceService.GetPetServicePricesByPetAsync(petId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServicePrice>>(ex);
            }
        }

        /// <summary>
        /// 根據服務ID取得所有寵物的價格設定
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns>該服務的所有寵物價格設定列表</returns>
        [HttpGet("service/{serviceId}")]
        public async Task<ActionResult<IEnumerable<PetServicePrice>>> GetPetServicePricesByServiceId(long serviceId)
        {
            try
            {
                var prices = await _petServicePriceService.GetPetServicePricesByServiceAsync(serviceId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServicePrice>>(ex);
            }
        }

        /// <summary>
        /// 取得特定寵物的特定服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>價格設定詳細資訊</returns>
        [HttpGet("pet/{petId}/service/{serviceId}")]
        public async Task<ActionResult<PetServicePrice?>> GetPetServicePrice(long petId, long serviceId)
        {
            try
            {
                var price = await _petServicePriceService.GetPetServicePriceAsync(petId, serviceId);
                if (price == null)
                {
                    return NotFound(new { message = "找不到指定的價格設定" });
                }
                return Ok(price);
            }
            catch (Exception ex)
            {
                return HandleException<PetServicePrice?>(ex);
            }
        }

        /// <summary>
        /// 根據ID取得價格設定詳細資訊
        /// </summary>
        /// <param name="id">價格設定ID</param>
        /// <returns>價格設定詳細資訊</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<PetServicePrice?>> GetPetServicePriceById(long id)
        {
            try
            {
                var price = await _petServicePriceService.GetPetServicePriceWithDetailsAsync(id);
                if (price == null)
                {
                    return NotFound(new { message = "找不到指定的價格設定" });
                }
                return Ok(price);
            }
            catch (Exception ex)
            {
                return HandleException<PetServicePrice?>(ex);
            }
        }

        /// <summary>
        /// 建立新的寵物服務價格設定
        /// </summary>
        /// <param name="request">價格設定資料</param>
        /// <returns>建立結果</returns>
        [HttpPost]
        public async Task<ActionResult<long>> CreatePetServicePrice([FromBody] CreatePetServicePriceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new PetServicePrice
                {
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomPrice = request.CustomPrice,
                    Duration = request.Duration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                };

                var priceId = await _petServicePriceService.CreatePetServicePriceAsync(entity);
                return CreatedAtAction(nameof(GetPetServicePriceById), new { id = priceId }, priceId);
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
        /// 更新寵物服務價格設定
        /// </summary>
        /// <param name="id">價格設定ID</param>
        /// <param name="request">更新的價格設定資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("{id}")]
        public async Task<IActionResult> UpdatePetServicePrice(long id, [FromBody] CreatePetServicePriceRequest request)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                var entity = new PetServicePrice
                {
                    PetServicePriceId = id,
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomPrice = request.CustomPrice,
                    Duration = request.Duration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                };

                await _petServicePriceService.UpdatePetServicePriceAsync(entity);

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
        /// 刪除寵物服務價格設定
        /// </summary>
        /// <param name="id">價格設定ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePetServicePrice(long id)
        {
            try
            {
                await _petServicePriceService.DeletePetServicePriceAsync(id);

                return NoContent();
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 切換寵物服務價格設定的啟用狀態
        /// </summary>
        /// <param name="id">價格設定ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns>操作結果</returns>
        [HttpPatch("{id}/toggle-status")]
        public async Task<IActionResult> TogglePetServicePriceStatus(long id, [FromBody] bool isActive)
        {
            try
            {
                await _petServicePriceService.TogglePetServicePriceStatusAsync(id, isActive);

                return Ok(new { message = $"價格設定已{(isActive ? "啟用" : "停用")}" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 批次建立寵物服務價格設定
        /// </summary>
        /// <param name="requests">價格設定列表</param>
        /// <returns>建立結果</returns>
        [HttpPost("batch")]
        public async Task<ActionResult<List<long>>> BatchCreatePetServicePrices([FromBody] List<CreatePetServicePriceRequest> requests)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    return BadRequest(ModelState);
                }

                if (requests == null || !requests.Any())
                {
                    return BadRequest(new { message = "請提供至少一個價格設定" });
                }

                var entities = requests.Select(request => new PetServicePrice
                {
                    PetId = request.PetId,
                    ServiceId = request.ServiceId,
                    CustomPrice = request.CustomPrice,
                    Duration = request.Duration,
                    Notes = request.Notes,
                    IsActive = request.IsActive
                }).ToList();

                await _petServicePriceService.CreateBatchPetServicePricesAsync(entities);
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
        /// 批次刪除寵物服務價格設定
        /// </summary>
        /// <param name="ids">價格設定ID列表</param>
        /// <returns>操作結果</returns>
        [HttpDelete("batch")]
        public async Task<IActionResult> BatchDeletePetServicePrices([FromBody] List<long> ids)
        {
            try
            {
                if (ids == null || !ids.Any())
                {
                    return BadRequest(new { message = "請提供至少一個價格設定ID" });
                }

                var deletedCount = await _petServicePriceService.BatchDeletePetServicePricesAsync(ids);
                return Ok(new { message = $"成功刪除 {deletedCount} 個價格設定" });
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 取得有效的寵物服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>有效的服務價格設定列表</returns>
        [HttpGet("active/{petId}")]
        public async Task<ActionResult<IEnumerable<PetServicePrice>>> GetActivePetServicePrices(long petId)
        {
            try
            {
                var prices = await _petServicePriceService.GetActivePetServicePricesAsync(petId);
                return Ok(prices);
            }
            catch (Exception ex)
            {
                return HandleException<IEnumerable<PetServicePrice>>(ex);
            }
        }

        /// <summary>
        /// 取得服務價格統計資料
        /// </summary>
        /// <returns>統計資料</returns>
        [HttpGet("statistics")]
        public async Task<IActionResult> GetServicePriceStatistics()
        {
            try
            {
                var statistics = await _petServicePriceService.GetServicePriceStatisticsAsync();
                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return HandleException(ex);
            }
        }

        /// <summary>
        /// 取得寵物的實際服務價格
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>實際服務價格</returns>
        [HttpGet("effective-price/{petId}/{serviceId}")]
        public async Task<ActionResult<decimal>> GetEffectiveServicePrice(long petId, long serviceId)
        {
            try
            {
                var price = await _petServicePriceService.GetEffectiveServicePriceAsync(petId, serviceId);
                return Ok(new { petId, serviceId, effectivePrice = price });
            }
            catch (Exception ex)
            {
                return HandleException<decimal>(ex);
            }
        }

        /// <summary>
        /// 取得寵物的實際服務時長
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>實際服務時長（分鐘）</returns>
        [HttpGet("effective-duration/{petId}/{serviceId}")]
        public async Task<ActionResult<int>> GetEffectiveServiceDuration(long petId, long serviceId)
        {
            try
            {
                var duration = await _petServicePriceService.GetEffectiveServiceDurationAsync(petId, serviceId);
                return Ok(new { petId, serviceId, effectiveDuration = duration });
            }
            catch (Exception ex)
            {
                return HandleException<int>(ex);
            }
        }

        /// <summary>
        /// 取得寵物的訂閱價格（優先使用 PetServicePrice，其次使用 Service 預設值）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>訂閱價格</returns>
        [HttpGet("subscription-price/{petId}")]
        public async Task<ActionResult<decimal?>> GetSubscriptionPrice(long petId)
        {
            try
            {
                var price = await _petServicePriceService.GetSubscriptionPriceAsync(petId);
                return Ok(new { petId, subscriptionPrice = price });
            }
            catch (Exception ex)
            {
                return HandleException<decimal?>(ex);
            }
        }
    }
}
