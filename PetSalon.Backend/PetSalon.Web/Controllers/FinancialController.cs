using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 財務統計 API 控制器 - 提供收入、支出、利潤等財務統計功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FinancialController : ControllerBase
    {
        private readonly PetSalonContext _context;

        public FinancialController(PetSalonContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得月收入統計
        /// </summary>
        /// <param name="month">月份（可選）</param>
        /// <param name="year">年份（可選）</param>
        /// <returns>月收入金額</returns>
        [HttpGet("monthly-revenue")]
        public async Task<ActionResult<decimal>> GetMonthlyRevenue([FromQuery] int? month, [FromQuery] int? year)
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

                // 計算包月收入
                var subscriptionRevenue = await _context.Subscription
                    .Where(s => s.CreateTime >= monthStart && s.CreateTime <= monthEnd)
                    .SumAsync(s => s.SubscriptionPrice);

                // 計算預約收入（暫時使用估算）
                var completedReservations = await _context.ReserveRecord
                    .Where(r => r.ReserverDate >= monthStart && r.ReserverDate <= monthEnd && 
                               r.Status == "COMPLETED")
                    .CountAsync();

                var reservationRevenue = completedReservations * 800; // 假設平均每筆預約 800 元

                var totalRevenue = subscriptionRevenue + reservationRevenue;

                return Ok(totalRevenue);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得月收入統計失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得財務儀表板統計資料
        /// </summary>
        /// <returns>財務儀表板統計資料</returns>
        [HttpGet("dashboard-stats")]
        public async Task<ActionResult<FinancialDashboardStatsDto>> GetDashboardStats()
        {
            try
            {
                var today = DateTime.Today;
                var weekStart = today.AddDays(-(int)today.DayOfWeek);
                var monthStart = new DateTime(today.Year, today.Month, 1);
                var yearStart = new DateTime(today.Year, 1, 1);

                // 今日收入
                var todayRevenue = await CalculateDailyRevenue(today);

                // 本週收入
                var weeklyRevenue = 0m;
                for (var date = weekStart; date <= today; date = date.AddDays(1))
                {
                    weeklyRevenue += await CalculateDailyRevenue(date);
                }

                // 本月收入
                var monthlyRevenue = await CalculateMonthlyRevenue(monthStart);

                // 本年收入
                var yearlyRevenue = 0m;
                for (var month = yearStart; month < today.AddMonths(1); month = month.AddMonths(1))
                {
                    yearlyRevenue += await CalculateMonthlyRevenue(month);
                }

                // 待收款項（暫時設為0，待實際實現）
                var pendingPayments = 0m;

                // 本月支出（暫時使用估算）
                var monthlyExpenses = monthlyRevenue * 0.3m; // 假設支出為收入的30%

                // 本月淨利
                var monthlyProfit = monthlyRevenue - monthlyExpenses;

                var stats = new FinancialDashboardStatsDto
                {
                    TodayRevenue = todayRevenue,
                    WeeklyRevenue = weeklyRevenue,
                    MonthlyRevenue = monthlyRevenue,
                    YearlyRevenue = yearlyRevenue,
                    PendingPayments = pendingPayments,
                    MonthlyExpenses = monthlyExpenses,
                    MonthlyProfit = monthlyProfit
                };

                return Ok(stats);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得財務儀表板統計失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得收入趨勢統計
        /// </summary>
        /// <param name="period">時間週期：7days, 30days, 90days, 1year</param>
        /// <returns>收入趨勢資料</returns>
        [HttpGet("revenue-trend")]
        public async Task<ActionResult<RevenueTrendDto>> GetRevenueTrend([FromQuery] string period = "30days")
        {
            try
            {
                var labels = new List<string>();
                var revenueData = new List<decimal>();
                var subscriptionData = new List<decimal>();
                var reservationData = new List<decimal>();

                switch (period)
                {
                    case "7days":
                        for (int i = 6; i >= 0; i--)
                        {
                            var date = DateTime.Today.AddDays(-i);
                            labels.Add(date.ToString("MM/dd"));

                            var dailyRevenue = await CalculateDailyRevenue(date);
                            var dailySubscription = await CalculateDailySubscriptionRevenue(date);
                            var dailyReservation = dailyRevenue - dailySubscription;

                            revenueData.Add(dailyRevenue);
                            subscriptionData.Add(dailySubscription);
                            reservationData.Add(dailyReservation);
                        }
                        break;

                    case "30days":
                        for (int i = 29; i >= 0; i--)
                        {
                            var date = DateTime.Today.AddDays(-i);
                            labels.Add(date.ToString("MM/dd"));

                            var dailyRevenue = await CalculateDailyRevenue(date);
                            var dailySubscription = await CalculateDailySubscriptionRevenue(date);
                            var dailyReservation = dailyRevenue - dailySubscription;

                            revenueData.Add(dailyRevenue);
                            subscriptionData.Add(dailySubscription);
                            reservationData.Add(dailyReservation);
                        }
                        break;

                    case "90days":
                        // 每3天一個數據點
                        for (int i = 90; i >= 0; i -= 3)
                        {
                            var endDate = DateTime.Today.AddDays(-i);
                            var startDate = endDate.AddDays(-2);
                            labels.Add(endDate.ToString("MM/dd"));

                            decimal periodRevenue = 0;
                            decimal periodSubscription = 0;
                            for (var date = startDate; date <= endDate; date = date.AddDays(1))
                            {
                                periodRevenue += await CalculateDailyRevenue(date);
                                periodSubscription += await CalculateDailySubscriptionRevenue(date);
                            }

                            revenueData.Add(periodRevenue);
                            subscriptionData.Add(periodSubscription);
                            reservationData.Add(periodRevenue - periodSubscription);
                        }
                        break;

                    case "1year":
                        for (int i = 11; i >= 0; i--)
                        {
                            var monthStart = DateTime.Today.AddMonths(-i).AddDays(-DateTime.Today.Day + 1);
                            labels.Add(monthStart.ToString("MM月"));

                            var monthlyRevenue = await CalculateMonthlyRevenue(monthStart);
                            var monthlySubscription = await CalculateMonthlySubscriptionRevenue(monthStart);
                            var monthlyReservation = monthlyRevenue - monthlySubscription;

                            revenueData.Add(monthlyRevenue);
                            subscriptionData.Add(monthlySubscription);
                            reservationData.Add(monthlyReservation);
                        }
                        break;
                }

                var trend = new RevenueTrendDto
                {
                    Labels = labels,
                    TotalRevenueData = revenueData,
                    SubscriptionRevenueData = subscriptionData,
                    ReservationRevenueData = reservationData
                };

                return Ok(trend);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得收入趨勢失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得收入來源分布
        /// </summary>
        /// <returns>收入來源分布統計</returns>
        [HttpGet("revenue-sources")]
        public async Task<ActionResult<RevenueSourcesDto>> GetRevenueSources()
        {
            try
            {
                var thisMonth = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
                var nextMonth = thisMonth.AddMonths(1);

                // 包月收入
                var subscriptionRevenue = await _context.Subscription
                    .Where(s => s.CreateTime >= thisMonth && s.CreateTime < nextMonth)
                    .SumAsync(s => s.SubscriptionPrice);

                // 預約收入（估算）
                var completedReservations = await _context.ReserveRecord
                    .Where(r => r.ReserverDate >= thisMonth && r.ReserverDate < nextMonth && 
                               r.Status == "COMPLETED")
                    .CountAsync();
                var reservationRevenue = completedReservations * 800m;

                // 其他收入（暫時設為0）
                var otherRevenue = 0m;

                var sources = new RevenueSourcesDto
                {
                    SubscriptionRevenue = subscriptionRevenue,
                    ReservationRevenue = reservationRevenue,
                    OtherRevenue = otherRevenue,
                    TotalRevenue = subscriptionRevenue + reservationRevenue + otherRevenue
                };

                return Ok(sources);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得收入來源分布失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 計算每日收入
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>當日收入</returns>
        private async Task<decimal> CalculateDailyRevenue(DateTime date)
        {
            var nextDay = date.AddDays(1);

            // 包月收入
            var subscriptionRevenue = await _context.Subscription
                .Where(s => s.CreateTime >= date && s.CreateTime < nextDay)
                .SumAsync(s => s.SubscriptionPrice);

            // 預約收入（估算）
            var completedReservations = await _context.ReserveRecord
                .Where(r => r.ReserverDate >= date && r.ReserverDate < nextDay && 
                           r.Status == "COMPLETED")
                .CountAsync();
            var reservationRevenue = completedReservations * 800m;

            return subscriptionRevenue + reservationRevenue;
        }

        /// <summary>
        /// 計算每日包月收入
        /// </summary>
        /// <param name="date">日期</param>
        /// <returns>當日包月收入</returns>
        private async Task<decimal> CalculateDailySubscriptionRevenue(DateTime date)
        {
            var nextDay = date.AddDays(1);

            return await _context.Subscription
                .Where(s => s.CreateTime >= date && s.CreateTime < nextDay)
                .SumAsync(s => s.SubscriptionPrice);
        }

        /// <summary>
        /// 計算月收入
        /// </summary>
        /// <param name="monthStart">月初日期</param>
        /// <returns>當月收入</returns>
        private async Task<decimal> CalculateMonthlyRevenue(DateTime monthStart)
        {
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            // 包月收入
            var subscriptionRevenue = await _context.Subscription
                .Where(s => s.CreateTime >= monthStart && s.CreateTime <= monthEnd)
                .SumAsync(s => s.SubscriptionPrice);

            // 預約收入（估算）
            var completedReservations = await _context.ReserveRecord
                .Where(r => r.ReserverDate >= monthStart && r.ReserverDate <= monthEnd && 
                           r.Status == "COMPLETED")
                .CountAsync();
            var reservationRevenue = completedReservations * 800m;

            return subscriptionRevenue + reservationRevenue;
        }

        /// <summary>
        /// 計算月包月收入
        /// </summary>
        /// <param name="monthStart">月初日期</param>
        /// <returns>當月包月收入</returns>
        private async Task<decimal> CalculateMonthlySubscriptionRevenue(DateTime monthStart)
        {
            var monthEnd = monthStart.AddMonths(1).AddDays(-1);

            return await _context.Subscription
                .Where(s => s.CreateTime >= monthStart && s.CreateTime <= monthEnd)
                .SumAsync(s => s.SubscriptionPrice);
        }
    }

    /// <summary>
    /// 收入趨勢 DTO
    /// </summary>
    public class RevenueTrendDto
    {
        /// <summary>
        /// 標籤清單
        /// </summary>
        public List<string> Labels { get; set; } = new();

        /// <summary>
        /// 總收入資料
        /// </summary>
        public List<decimal> TotalRevenueData { get; set; } = new();

        /// <summary>
        /// 包月收入資料
        /// </summary>
        public List<decimal> SubscriptionRevenueData { get; set; } = new();

        /// <summary>
        /// 預約收入資料
        /// </summary>
        public List<decimal> ReservationRevenueData { get; set; } = new();
    }

    /// <summary>
    /// 收入來源 DTO
    /// </summary>
    public class RevenueSourcesDto
    {
        /// <summary>
        /// 包月收入
        /// </summary>
        public decimal SubscriptionRevenue { get; set; }

        /// <summary>
        /// 預約收入
        /// </summary>
        public decimal ReservationRevenue { get; set; }

        /// <summary>
        /// 其他收入
        /// </summary>
        public decimal OtherRevenue { get; set; }

        /// <summary>
        /// 總收入
        /// </summary>
        public decimal TotalRevenue { get; set; }
    }
}