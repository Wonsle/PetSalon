using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 預約管理API控制器 - 提供預約CRUD功能與包月整合
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ReservationController : ControllerBase
    {
        private readonly PetSalonContext _context;
        private readonly IReservationService _reservationService;

        public ReservationController(PetSalonContext context, IReservationService reservationService)
        {
            _context = context;
            _reservationService = reservationService;
        }

        /// <summary>
        /// 取得所有預約記錄
        /// </summary>
        /// <returns>預約記錄列表</returns>
        [HttpGet(Name = nameof(GetReservations))]
        public async Task<ActionResult<IEnumerable<ReserveRecord>>> GetReservations()
        {
            try
            {
                var reservations = await _context.ReserveRecord
                    .Include(r => r.Pet)
                    .Include(r => r.Subscription)
                    .OrderByDescending(r => r.ReserverDate)
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取預約記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 根據ID取得預約記錄
        /// </summary>
        /// <param name="id">預約記錄ID</param>
        /// <returns>預約記錄</returns>
        [HttpGet("{id}", Name = nameof(GetReservation))]
        public async Task<ActionResult<ReserveRecord>> GetReservation(long id)
        {
            try
            {
                var reservation = await _context.ReserveRecord
                    .Include(r => r.Pet)
                    .Include(r => r.Subscription)
                    .FirstOrDefaultAsync(r => r.ReserveRecordId == id);

                if (reservation == null)
                {
                    return NotFound(new { message = "找不到指定的預約記錄" });
                }

                return Ok(reservation);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取預約記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 建立新的預約記錄
        /// </summary>
        /// <param name="reservation">預約資料</param>
        /// <returns>建立的預約記錄</returns>
        [HttpPost(Name = nameof(CreateReservation))]
        public async Task<ActionResult<ReserveRecord>> CreateReservation([FromBody] ReservationCreateDto reservation)
        {
            try
            {
                var result = await _reservationService.CreateReservationAsync(reservation);
                return CreatedAtAction(nameof(GetReservation), new { id = result.ReserveRecordId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "建立預約失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新預約記錄
        /// </summary>
        /// <param name="id">預約記錄ID</param>
        /// <param name="reservation">更新的預約資料</param>
        /// <returns>更新結果</returns>
        [HttpPut("{id}", Name = nameof(UpdateReservation))]
        public async Task<IActionResult> UpdateReservation(long id, [FromBody] ReservationUpdateDto reservation)
        {
            try
            {
                var result = await _reservationService.UpdateReservationAsync(id, reservation);
                if (result == null)
                {
                    return NotFound(new { message = "找不到指定的預約記錄" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新預約失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 刪除預約記錄
        /// </summary>
        /// <param name="id">預約記錄ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}", Name = nameof(DeleteReservation))]
        public async Task<IActionResult> DeleteReservation(long id)
        {
            try
            {
                var result = await _reservationService.DeleteReservationAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "找不到指定的預約記錄" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "刪除預約失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 根據寵物ID取得該寵物的有效包月方案
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>有效的包月方案</returns>
        [HttpGet("pet/{petId}/active-subscription-for-reservation", Name = nameof(GetActiveSubscriptionForReservation))]
        public async Task<ActionResult<Subscription>> GetActiveSubscriptionForReservation(long petId)
        {
            try
            {
                var subscription = await _context.Subscription
                    .Where(s => s.PetId == petId &&
                               s.EndDate > DateTime.Now &&
                               s.UsedCount + s.ReservedCount < s.TotalUsageLimit)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    return NotFound(new { message = "該寵物沒有可用的包月方案" });
                }

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取包月方案失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新預約狀態
        /// </summary>
        /// <param name="id">預約記錄ID</param>
        /// <param name="status">新狀態</param>
        /// <returns>更新結果</returns>
        [HttpPost("{id}/status", Name = nameof(UpdateReservationStatus))]
        public async Task<IActionResult> UpdateReservationStatus(long id, [FromBody] string status)
        {
            try
            {
                var result = await _reservationService.UpdateReservationStatusAsync(id, status);
                if (result == null)
                {
                    return NotFound(new { message = "找不到指定的預約記錄" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新預約狀態失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得預約統計資料
        /// </summary>
        /// <returns>統計資料</returns>
        [HttpGet("statistics", Name = nameof(GetReservationStatistics))]
        public async Task<ActionResult> GetReservationStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                var statistics = new
                {
                    TodayReservations = await _context.ReserveRecord
                        .CountAsync(r => r.ReserverDate.Date == today),

                    ThisMonthReservations = await _context.ReserveRecord
                        .CountAsync(r => r.ReserverDate >= thisMonth),

                    PendingReservations = await _context.ReserveRecord
                        .CountAsync(r => r.Status == "PENDING"),

                    CompletedReservations = await _context.ReserveRecord
                        .CountAsync(r => r.Status == "COMPLETED"),

                    SubscriptionBasedReservations = await _context.ReserveRecord
                        .CountAsync(r => r.SubscriptionId != null)
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取統計資料失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 計算預約服務成本（含附加服務）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceIds">服務ID列表</param>
        /// <param name="addonIds">附加服務ID列表</param>
        /// <returns>計算結果</returns>
        [HttpPost("calculate-cost", Name = nameof(CalculateReservationCost))]
        public async Task<ActionResult<ServiceCalculationDto>> CalculateReservationCost([FromQuery] long petId, [FromBody] ServiceCostCalculationRequest request)
        {
            try
            {
                var result = await _reservationService.CalculateReservationCost(petId, request.ServiceIds, request.AddonIds);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "計算服務成本失敗", error = ex.Message });
            }
        }


        /// <summary>
        /// 計算服務總時長
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceIds">服務ID列表</param>
        /// <returns>總時長（分鐘）</returns>
        [HttpPost("pet/{petId}/calculate-duration", Name = nameof(CalculateServiceDuration))]
        public async Task<ActionResult<int>> CalculateServiceDuration(long petId, [FromBody] List<long> serviceIds)
        {
            try
            {
                var totalDuration = await _reservationService.CalculateTotalServiceDurationAsync(petId, serviceIds);
                return Ok(totalDuration);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "計算服務時長失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得寵物的完整定價設定概覽
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>定價概覽</returns>
        [HttpGet("pet/{petId}/pricing-overview", Name = nameof(GetPetPricingOverview))]
        public async Task<ActionResult<PetPricingOverviewDto>> GetPetPricingOverview(long petId)
        {
            try
            {
                var overview = await _reservationService.GetPetPricingOverviewAsync(petId);
                return Ok(overview);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取寵物定價概覽失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 檢查包月服務資格
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="reservationDate">預約日期</param>
        /// <returns>是否符合包月資格</returns>
        [HttpGet("pet/{petId}/check-subscription-eligibility", Name = nameof(CheckSubscriptionEligibility))]
        public async Task<ActionResult<bool>> CheckSubscriptionEligibility(long petId, [FromQuery] DateTime reservationDate)
        {
            try
            {
                var isEligible = await _reservationService.CheckSubscriptionEligibility(petId, reservationDate);
                return Ok(isEligible);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "檢查包月資格失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 完成預約（更新包月次數）
        /// </summary>
        /// <param name="reservationId">預約ID</param>
        /// <returns>操作結果</returns>
        [HttpPost("{reservationId}/complete", Name = nameof(CompleteReservation))]
        public async Task<IActionResult> CompleteReservation(long reservationId)
        {
            try
            {
                await _reservationService.CompleteReservation(reservationId);
                return Ok(new { message = "預約已完成" });
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "完成預約失敗", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// 服務成本計算請求 DTO
    /// </summary>
    public class ServiceCostCalculationRequest
    {
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
    }
}
