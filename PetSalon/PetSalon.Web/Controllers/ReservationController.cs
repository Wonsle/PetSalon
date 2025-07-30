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
        /// 根據日期取得預約列表
        /// </summary>
        /// <param name="date">查詢日期</param>
        /// <returns>指定日期的預約列表</returns>
        [HttpGet("date/{date}", Name = nameof(GetReservationsByDate))]
        public async Task<ActionResult<IList<ReserveRecord>>> GetReservationsByDate(DateTime date)
        {
            return Ok(await _reservationService.GetReservationsByDate(date));
        }

        /// <summary>
        /// 根據寵物ID取得預約列表
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>指定寵物的預約列表</returns>
        [HttpGet("pet/{petId}", Name = nameof(GetReservationsByPet))]
        public async Task<ActionResult<IList<ReserveRecord>>> GetReservationsByPet(long petId)
        {
            return Ok(await _reservationService.GetReservationsByPet(petId));
        }

        /// <summary>
        /// 建立新預約
        /// </summary>
        /// <param name="reservation">預約建立資料</param>
        /// <returns>新建立預約的ID</returns>
        [HttpPost(Name = nameof(CreateReservation))]
        public async Task<ActionResult<long>> CreateReservation(ReservationCreateDto reservation)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            try
            {
                var reservationId = await _reservationService.CreateReservation(reservation);
                return CreatedAtAction(nameof(GetReservation),
                    new { reservationId = reservationId }, reservationId);
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(ex.Message);
            }
        }

        /// <summary>
        /// 更新預約資訊
        /// </summary>
        /// <param name="reservationId">預約ID</param>
        /// <param name="reservation">預約更新資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("{reservationId}", Name = nameof(UpdateReservation))]
        public async Task<IActionResult> UpdateReservation(long reservationId, ReservationUpdateDto reservation)
        {
            if (reservationId != reservation.ReservationId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _reservationService.UpdateReservation(reservation);
            return NoContent();
        }

        /// <summary>
        /// 刪除預約
        /// </summary>
        /// <param name="reservationId">預約ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{reservationId}", Name = nameof(DeleteReservation))]
        public async Task<IActionResult> DeleteReservation(long reservationId)
        {
            var reservation = await _reservationService.GetReservation(reservationId);
            if (reservation == null)
                return NotFound();

            await _reservationService.DeleteReservation(reservationId);
            return NoContent();
        }

        /// <summary>
        /// 更新預約狀態
        /// </summary>
        /// <param name="reservationId">預約ID</param>
        /// <param name="status">新狀態</param>
        /// <returns>操作結果</returns>
        [HttpPost("{reservationId}/status", Name = nameof(UpdateReservationStatus))]
        public async Task<IActionResult> UpdateReservationStatus(long reservationId, [FromBody] string status)
        {
            await _reservationService.UpdateReservationStatus(reservationId, status);
            return NoContent();
        }

        /// <summary>
        /// 完成預約服務
        /// </summary>
        /// <param name="reservationId">預約ID</param>
        /// <returns>操作結果</returns>
        [HttpPost("{reservationId}/complete", Name = nameof(CompleteReservation))]
        public async Task<IActionResult> CompleteReservation(long reservationId)
        {
            await _reservationService.CompleteReservation(reservationId);
            return NoContent();
        }

        /// <summary>
        /// 檢查寵物的訂閱服務資格
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="date">檢查日期（預設為目前日期）</param>
        /// <returns>是否符合訂閱資格</returns>
        [HttpGet("pet/{petId}/subscription-check", Name = nameof(CheckSubscriptionEligibility))]
        public async Task<ActionResult<bool>> CheckSubscriptionEligibility(long petId, [FromQuery] DateTime? date = null)
        {
            var checkDate = date ?? DateTime.Now;
            var isEligible = await _reservationService.CheckSubscriptionEligibility(petId, checkDate);
            return Ok(isEligible);
        }

        /// <summary>
        /// 計算預約服務費用
        /// </summary>
        /// <param name="request">費用計算請求資料</param>
        /// <returns>服務費用明細</returns>
        [HttpPost("calculate-cost", Name = nameof(CalculateReservationCost))]
        public async Task<ActionResult<ServiceCalculationDto>> CalculateReservationCost([FromBody] ReservationCalculationRequest request)
        {
            var calculation = await _reservationService.CalculateReservationCost(
                request.PetId, request.ServiceIds, request.AddonIds);
            return Ok(calculation);
        }

        /// <summary>
        /// 取得寵物的有效包月服務
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="date">檢查日期（預設為目前日期）</param>
        /// <returns>有效的包月服務</returns>
        [HttpGet("pet/{petId}/active-subscription", Name = "GetActiveSubscriptionForReservation")]
        public async Task<ActionResult<Subscription>> GetActiveSubscription(long petId, [FromQuery] DateTime? date = null)
        {
            var checkDate = date ?? DateTime.Now;
            var subscription = await _reservationService.GetActiveSubscription(petId, checkDate);
            if (subscription == null)
                return NotFound("No active subscription found");
            return Ok(subscription);
        }
    }

    /// <summary>
    /// 預約費用計算請求資料類別
    /// </summary>
    public class ReservationCalculationRequest
    {
        public long PetId { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
    }
}