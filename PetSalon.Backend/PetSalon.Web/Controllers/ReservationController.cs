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
        /// <param name="request">服務ID列表請求</param>
        /// <returns>總時長（分鐘）</returns>
        [HttpPost("pet/{petId}/calculate-duration", Name = nameof(CalculateServiceDuration))]
        public async Task<ActionResult> CalculateServiceDuration(long petId, [FromBody] DurationCalculationRequest request)
        {
            try
            {
                var totalDuration = await _reservationService.CalculateTotalServiceDurationAsync(petId, request.ServiceIds);
                return Ok(new { totalDuration });
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

        /// <summary>
        /// 取得行事曆事件資料
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>行事曆事件列表</returns>
        [HttpGet("calendar", Name = nameof(GetCalendarEvents))]
        public async Task<ActionResult<List<CalendarEventDto>>> GetCalendarEvents([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var events = await _context.ReserveRecord
                    .Include(r => r.Pet)
                    .Include(r => r.Pet.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                    .Where(r => r.ReserverDate >= startDate && r.ReserverDate <= endDate)
                    .Select(r => new CalendarEventDto
                    {
                        Id = r.ReserveRecordId.ToString(),
                        Title = $"{r.Pet.PetName} - 美容預約",
                        Start = r.ReserverDate.Add(r.ReserverTime),
                        End = r.ReserverDate.Add(r.ReserverTime).AddMinutes(120), // 預設 2 小時
                        AllDay = false,
                        PetName = r.Pet.PetName ?? "未知寵物",
                        ContactName = r.Pet.PetRelation
                            .Where(pr => pr.RelationshipType == "Owner")
                            .Select(pr => pr.ContactPerson.Name)
                            .FirstOrDefault() ?? "未知聯絡人",
                        ContactPhone = r.Pet.PetRelation
                            .Where(pr => pr.RelationshipType == "Owner")
                            .Select(pr => pr.ContactPerson.ContactNumber)
                            .FirstOrDefault() ?? "",
                        Status = r.Status ?? "PENDING",
                        BackgroundColor = GetStatusColor(r.Status ?? "PENDING")
                    })
                    .ToListAsync();

                return Ok(events);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得行事曆事件失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 檢查指定時段的可用性
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="time">時間(以分鐘表示，從00:00開始)</param>
        /// <param name="duration">時長(分鐘，預設120分鐘)</param>
        /// <returns>可用性檢查結果</returns>
        [HttpGet("availability", Name = nameof(CheckAvailability))]
        public async Task<ActionResult<AvailabilityCheckDto>> CheckAvailability(
            [FromQuery] DateTime date, 
            [FromQuery] int time, 
            [FromQuery] int duration = 120)
        {
            try
            {
                var conflicts = await _context.ReserveRecord
                    .Include(r => r.Pet)
                    .Where(r => r.ReserverDate.Date == date.Date &&
                               r.Status != "CANCELLED")
                    .ToListAsync();

                var filteredConflicts = conflicts
                    .Where(r => 
                    {
                        var startMinutes = (int)r.ReserverTime.TotalMinutes;
                        var endMinutes = startMinutes + 120;
                        
                        // 檢查時間重疊
                        return (startMinutes <= time && endMinutes > time) ||
                               (startMinutes < (time + duration) && endMinutes >= (time + duration)) ||
                               (startMinutes >= time && startMinutes < (time + duration));
                    })
                    .Select(r => new ConflictReservationDto
                    {
                        Id = r.ReserveRecordId,
                        PetName = r.Pet.PetName ?? "未知寵物",
                        StartTime = (int)r.ReserverTime.TotalMinutes,
                        EndTime = (int)r.ReserverTime.TotalMinutes + 120,
                        Status = r.Status ?? "PENDING"
                    })
                    .ToList();

                var result = new AvailabilityCheckDto
                {
                    Available = !filteredConflicts.Any(),
                    Conflicts = filteredConflicts,
                    SuggestedTimes = filteredConflicts.Any() ? GenerateSuggestedTimes(date, time, duration) : new List<int>()
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "檢查可用性失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得今日預約列表（Dashboard專用）
        /// </summary>
        /// <returns>今日預約列表</returns>
        [HttpGet("today", Name = nameof(GetTodayReservationsForDashboard))]
        public async Task<ActionResult<List<TodayReservationDto>>> GetTodayReservationsForDashboard()
        {
            try
            {
                var today = DateTime.Today;

                var reservations = await _context.ReserveRecord
                    .Include(r => r.Pet)
                    .Include(r => r.Pet.PetRelation)
                    .ThenInclude(pr => pr.ContactPerson)
                    .Where(r => r.ReserverDate.Date == today)
                    .OrderBy(r => r.ReserverTime)
                    .Select(r => new TodayReservationDto
                    {
                        Id = (int)r.ReserveRecordId,
                        ReserverTime = (int)r.ReserverTime.TotalMinutes,
                        PetName = r.Pet.PetName ?? "未知寵物",
                        PrimaryContactName = r.Pet.PetRelation
                            .Where(pr => pr.RelationshipType == "Owner")
                            .Select(pr => pr.ContactPerson.Name)
                            .FirstOrDefault() ?? "未知聯絡人",
                        PrimaryContactPhone = r.Pet.PetRelation
                            .Where(pr => pr.RelationshipType == "Owner")
                            .Select(pr => pr.ContactPerson.ContactNumber)
                            .FirstOrDefault() ?? "",
                        Services = new List<string> { "美容服務" }, // 暫時硬編碼
                        Status = r.Status ?? "PENDING"
                    })
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得今日預約失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 根據狀態取得顏色
        /// </summary>
        /// <param name="status">預約狀態</param>
        /// <returns>顏色代碼</returns>
        private string GetStatusColor(string status)
        {
            return status switch
            {
                "PENDING" => "#ffc107",      // 黃色 - 待確認
                "CONFIRMED" => "#28a745",    // 綠色 - 已確認
                "IN_PROGRESS" => "#17a2b8",  // 藍色 - 進行中
                "COMPLETED" => "#6c757d",    // 灰色 - 已完成
                "CANCELLED" => "#dc3545",    // 紅色 - 已取消
                "NO_SHOW" => "#fd7e14",      // 橙色 - 未出現
                _ => "#6c757d"               // 預設灰色
            };
        }

        /// <summary>
        /// 生成建議的時段
        /// </summary>
        /// <param name="date">日期</param>
        /// <param name="requestedTime">請求的時間</param>
        /// <param name="duration">時長</param>
        /// <returns>建議時段清單</returns>
        private List<int> GenerateSuggestedTimes(DateTime date, int requestedTime, int duration)
        {
            var suggestions = new List<int>();
            var workingHours = new List<int>();

            // 生成營業時間 (9:00-18:00，每 30 分鐘一個時段)
            for (int hour = 9; hour < 18; hour++)
            {
                workingHours.Add(hour * 60);      // 整點
                workingHours.Add(hour * 60 + 30); // 半點
            }

            // 找最接近的3個可用時段
            foreach (var time in workingHours)
            {
                if (suggestions.Count >= 3) break;
                if (Math.Abs(time - requestedTime) <= 180) // 3小時內的時段
                {
                    suggestions.Add(time);
                }
            }

            return suggestions;
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
