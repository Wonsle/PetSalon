using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 儀表板 API 控制器 - 提供儀表板統計資料和概覽功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly PetSalonContext _context;
        private readonly IPetService _petService;
        private readonly ISubscriptionService _subscriptionService;

        public DashboardController(
            PetSalonContext context,
            IPetService petService,
            ISubscriptionService subscriptionService)
        {
            _context = context;
            _petService = petService;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// 測試資料庫連線
        /// </summary>
        /// <returns>連線狀態</returns>
        [HttpGet("test-connection")]
        public async Task<ActionResult> TestConnection()
        {
            try
            {
                // 簡單的資料庫連線測試
                var canConnect = await _context.Database.CanConnectAsync();
                if (canConnect)
                {
                    return Ok(new { status = "success", message = "資料庫連線成功" });
                }
                else
                {
                    return StatusCode(500, new { status = "error", message = "無法連線到資料庫" });
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { status = "error", message = "資料庫連線測試失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得儀表板統計資料
        /// </summary>
        /// <returns>儀表板統計資料</returns>
        [HttpGet("statistics")]
        public async Task<ActionResult<DashboardStatisticsDto>> GetStatistics()
        {
            try
            {
                var today = DateTime.Today;
                var thisMonth = new DateTime(today.Year, today.Month, 1);

                // 使用簡化的查詢，避免複雜的導航屬性問題
                int todayReservations = 0;
                int totalPets = 0;
                decimal monthlyRevenue = 0;
                int activeSubscriptions = 0;

                // 嘗試查詢各項統計數據，如果失敗則使用預設值
                try
                {
                    todayReservations = await _context.ReserveRecord
                        .Where(r => r.ReserverDate.Date == today)
                        .CountAsync();
                }
                catch (Exception ex)
                {
                    // 如果查詢失敗，記錄錯誤但不中斷整個請求
                    Console.WriteLine($"今日預約查詢失敗: {ex.Message}");
                    todayReservations = 0;
                }

                try
                {
                    totalPets = await _context.Pet.CountAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"寵物數量查詢失敗: {ex.Message}");
                    totalPets = 0;
                }

                try
                {
                    activeSubscriptions = await _context.Subscription
                        .Where(s => s.StartDate <= today && s.EndDate >= today)
                        .CountAsync();
                }
                catch (Exception ex)
                {
                    Console.WriteLine($"包月數量查詢失敗: {ex.Message}");
                    activeSubscriptions = 0;
                }

                // 使用簡化的收入計算
                monthlyRevenue = (todayReservations * 800) + (activeSubscriptions * 1500);

                var statistics = new DashboardStatisticsDto
                {
                    TodayReservations = todayReservations,
                    TotalPets = totalPets,
                    MonthlyRevenue = monthlyRevenue,
                    ActiveSubscriptions = activeSubscriptions
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得統計資料失敗", detail = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// 取得今日預約列表
        /// </summary>
        /// <returns>今日預約列表</returns>
        [HttpGet("today-reservations")]
        public async Task<ActionResult<List<TodayReservationDto>>> GetTodayReservations()
        {
            try
            {
                var today = DateTime.Today;

                // 使用簡化的查詢，先只取得基本的預約資料
                var reservations = await _context.ReserveRecord
                    .Where(r => r.ReserverDate.Date == today)
                    .OrderBy(r => r.ReserverTime)
                    .Select(r => new TodayReservationDto
                    {
                        Id = (int)r.ReserveRecordId,
                        ReserverTime = (int)r.ReserverTime.TotalMinutes,
                        PetName = "寵物" + r.PetId, // 簡化顯示，避免複雜的導航屬性
                        PrimaryContactName = "聯絡人",
                        PrimaryContactPhone = "",
                        Services = new List<string> { "美容服務" },
                        Status = r.Status ?? "PENDING"
                    })
                    .ToListAsync();

                return Ok(reservations);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得今日預約失敗", detail = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// 取得月收入統計
        /// </summary>
        /// <param name="month">月份（可選）</param>
        /// <param name="year">年份（可選）</param>
        /// <returns>月收入統計</returns>
        [HttpGet("monthly-revenue")]
        public async Task<ActionResult<MonthlyRevenueDto>> GetMonthlyRevenue([FromQuery] int? month, [FromQuery] int? year)
        {
            try
            {
                var targetDate = DateTime.Today;
                if (year.HasValue && month.HasValue)
                {
                    targetDate = new DateTime(year.Value, month.Value, 1);
                }

                var monthStart = new DateTime(targetDate.Year, targetDate.Month, 1);
                var monthEnd = monthStart.AddMonths(1).AddDays(-1);

                // 計算預約收入 (暫時使用模擬數據)
                var reservationRevenue = await CalculateReservationRevenue(monthStart, monthEnd);

                // 計算包月收入
                var subscriptionRevenue = await CalculateSubscriptionRevenue(monthStart, monthEnd);

                var monthlyRevenue = new MonthlyRevenueDto
                {
                    Year = targetDate.Year,
                    Month = targetDate.Month,
                    ReservationRevenue = reservationRevenue,
                    SubscriptionRevenue = subscriptionRevenue,
                    OtherRevenue = 0, // 待實現
                    TotalRevenue = reservationRevenue + subscriptionRevenue
                };

                return Ok(monthlyRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得月收入統計失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得有效包月數量
        /// </summary>
        /// <returns>有效包月數量</returns>
        [HttpGet("active-subscriptions-count")]
        public async Task<ActionResult<int>> GetActiveSubscriptionsCount()
        {
            try
            {
                var today = DateTime.Today;
                var count = await _context.Subscription
                    .Where(s => s.StartDate <= today && s.EndDate >= today)
                    .CountAsync();

                return Ok(count);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得有效包月數量失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 計算月收入（暫時實現）
        /// </summary>
        /// <param name="monthStart">月初日期</param>
        /// <returns>月收入金額</returns>
        private async Task<decimal> CalculateMonthlyRevenue(DateTime monthStart)
        {
            // 這裡先使用簡單的模擬計算
            // 實際應該整合 PaymentRecord 或其他收入記錄
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);
            
            var reservationCount = await _context.ReserveRecord
                .Where(r => r.ReserverDate >= monthStart && r.ReserverDate <= monthEnd)
                .CountAsync();

            var subscriptionCount = await _context.Subscription
                .Where(s => s.StartDate >= monthStart && s.StartDate <= monthEnd)
                .CountAsync();

            // 簡單估算：每筆預約平均 800 元，每筆包月 1500 元
            return (reservationCount * 800) + (subscriptionCount * 1500);
        }

        /// <summary>
        /// 計算預約收入
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>預約收入</returns>
        private async Task<decimal> CalculateReservationRevenue(DateTime startDate, DateTime endDate)
        {
            // 暫時使用模擬計算
            var reservationCount = await _context.ReserveRecord
                .Where(r => r.ReserverDate >= startDate && r.ReserverDate <= endDate && 
                           r.Status == "COMPLETED")
                .CountAsync();

            return reservationCount * 800; // 假設平均每筆預約 800 元
        }

        /// <summary>
        /// 計算包月收入
        /// </summary>
        /// <param name="startDate">開始日期</param>
        /// <param name="endDate">結束日期</param>
        /// <returns>包月收入</returns>
        private async Task<decimal> CalculateSubscriptionRevenue(DateTime startDate, DateTime endDate)
        {
            // 計算該月份內新建的包月收入
            var newSubscriptions = await _context.Subscription
                .Where(s => s.StartDate >= startDate && s.StartDate <= endDate)
                .CountAsync();

            return newSubscriptions * 1500; // 假設平均每筆包月 1500 元
        }
    }
}