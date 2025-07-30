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
                    .OrderByDescending(r => r.ReservationDate)
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
        public async Task<ActionResult<ReserveRecord>> CreateReservation([FromBody] CreateReservationDto reservation)
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
        public async Task<IActionResult> UpdateReservation(long id, [FromBody] UpdateReservationDto reservation)
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
                               s.Status == "ACTIVE" &&
                               s.EndDate > DateTime.Now &&
                               s.RemainingUsage > 0)
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
                        .CountAsync(r => r.ReservationDate.Date == today),

                    ThisMonthReservations = await _context.ReserveRecord
                        .CountAsync(r => r.ReservationDate >= thisMonth),

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
    }

    /// <summary>
    /// 建立預約請求DTO
    /// </summary>
    public class CreateReservationDto
    {
        public long PetId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public string Status { get; set; } = "PENDING";
        public string? Memo { get; set; }
        public bool UseSubscription { get; set; }
        public long? SubscriptionId { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 更新預約請求DTO
    /// </summary>
    public class UpdateReservationDto
    {
        public DateTime? ReservationDate { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public string? Status { get; set; }
        public string? Memo { get; set; }
    }
}
