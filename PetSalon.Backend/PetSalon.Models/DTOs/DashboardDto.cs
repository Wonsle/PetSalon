using System.ComponentModel.DataAnnotations;

namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 儀表板統計資料 DTO
    /// </summary>
    public class DashboardStatisticsDto
    {
        /// <summary>
        /// 今日預約數量
        /// </summary>
        public int TodayReservations { get; set; }

        /// <summary>
        /// 總寵物數量
        /// </summary>
        public int TotalPets { get; set; }

        /// <summary>
        /// 本月收入
        /// </summary>
        public decimal MonthlyRevenue { get; set; }

        /// <summary>
        /// 有效包月數量
        /// </summary>
        public int ActiveSubscriptions { get; set; }
    }

    /// <summary>
    /// 今日預約 DTO
    /// </summary>
    public class TodayReservationDto
    {
        /// <summary>
        /// 預約 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 預約時間 (分鐘，從 00:00 開始計算)
        /// </summary>
        public int ReserverTime { get; set; }

        /// <summary>
        /// 寵物名稱
        /// </summary>
        public string PetName { get; set; } = string.Empty;

        /// <summary>
        /// 主要聯絡人姓名
        /// </summary>
        public string PrimaryContactName { get; set; } = string.Empty;

        /// <summary>
        /// 主要聯絡人電話
        /// </summary>
        public string PrimaryContactPhone { get; set; } = string.Empty;

        /// <summary>
        /// 服務項目清單
        /// </summary>
        public List<string> Services { get; set; } = new();

        /// <summary>
        /// 預約狀態
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }

    /// <summary>
    /// 月收入統計 DTO
    /// </summary>
    public class MonthlyRevenueDto
    {
        /// <summary>
        /// 年份
        /// </summary>
        public int Year { get; set; }

        /// <summary>
        /// 月份
        /// </summary>
        public int Month { get; set; }

        /// <summary>
        /// 總收入
        /// </summary>
        public decimal TotalRevenue { get; set; }

        /// <summary>
        /// 預約收入
        /// </summary>
        public decimal ReservationRevenue { get; set; }

        /// <summary>
        /// 包月收入
        /// </summary>
        public decimal SubscriptionRevenue { get; set; }

        /// <summary>
        /// 其他收入
        /// </summary>
        public decimal OtherRevenue { get; set; }
    }

    /// <summary>
    /// 即將到期的包月 DTO
    /// </summary>
    public class ExpiringSubscriptionDto
    {
        /// <summary>
        /// 包月 ID
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 寵物 ID
        /// </summary>
        public int PetId { get; set; }

        /// <summary>
        /// 寵物名稱
        /// </summary>
        public string PetName { get; set; } = string.Empty;

        /// <summary>
        /// 包月類型
        /// </summary>
        public string SubscriptionType { get; set; } = string.Empty;

        /// <summary>
        /// 結束日期
        /// </summary>
        public DateTime EndDate { get; set; }

        /// <summary>
        /// 剩餘天數
        /// </summary>
        public int DaysLeft { get; set; }

        /// <summary>
        /// 剩餘使用次數
        /// </summary>
        public int RemainingUsage { get; set; }

        /// <summary>
        /// 主要聯絡人姓名
        /// </summary>
        public string PrimaryContactName { get; set; } = string.Empty;

        /// <summary>
        /// 主要聯絡人電話
        /// </summary>
        public string PrimaryContactPhone { get; set; } = string.Empty;
    }

    /// <summary>
    /// 包月儀表板統計 DTO
    /// </summary>
    public class SubscriptionDashboardStatsDto
    {
        /// <summary>
        /// 啟用中的包月數量
        /// </summary>
        public int ActiveSubscriptions { get; set; }

        /// <summary>
        /// 即將到期的包月數量
        /// </summary>
        public int ExpiringSoon { get; set; }

        /// <summary>
        /// 本月包月收入
        /// </summary>
        public decimal MonthlyRevenue { get; set; }

        /// <summary>
        /// 平均使用率
        /// </summary>
        public decimal AverageUsageRate { get; set; }
    }

    /// <summary>
    /// 使用率分布 DTO
    /// </summary>
    public class UsageDistributionDto
    {
        /// <summary>
        /// 高使用率數量 (>80%)
        /// </summary>
        public int HighUsage { get; set; }

        /// <summary>
        /// 中使用率數量 (40-80%)
        /// </summary>
        public int MediumUsage { get; set; }

        /// <summary>
        /// 低使用率數量 (<40%)
        /// </summary>
        public int LowUsage { get; set; }
    }

    /// <summary>
    /// 銷售趨勢 DTO
    /// </summary>
    public class SalesTrendDto
    {
        /// <summary>
        /// 標籤清單（月份）
        /// </summary>
        public List<string> Labels { get; set; } = new();

        /// <summary>
        /// 包月銷售資料
        /// </summary>
        public List<decimal> SubscriptionData { get; set; } = new();

        /// <summary>
        /// 總銷售資料
        /// </summary>
        public List<decimal> TotalSalesData { get; set; } = new();
    }

    /// <summary>
    /// 財務儀表板統計 DTO
    /// </summary>
    public class FinancialDashboardStatsDto
    {
        /// <summary>
        /// 今日收入
        /// </summary>
        public decimal TodayRevenue { get; set; }

        /// <summary>
        /// 本週收入
        /// </summary>
        public decimal WeeklyRevenue { get; set; }

        /// <summary>
        /// 本月收入
        /// </summary>
        public decimal MonthlyRevenue { get; set; }

        /// <summary>
        /// 本年收入
        /// </summary>
        public decimal YearlyRevenue { get; set; }

        /// <summary>
        /// 待收款項
        /// </summary>
        public decimal PendingPayments { get; set; }

        /// <summary>
        /// 本月支出
        /// </summary>
        public decimal MonthlyExpenses { get; set; }

        /// <summary>
        /// 本月淨利
        /// </summary>
        public decimal MonthlyProfit { get; set; }
    }

    /// <summary>
    /// 行事曆事件 DTO
    /// </summary>
    public class CalendarEventDto
    {
        /// <summary>
        /// 事件 ID
        /// </summary>
        public string Id { get; set; } = string.Empty;

        /// <summary>
        /// 事件標題
        /// </summary>
        public string Title { get; set; } = string.Empty;

        /// <summary>
        /// 開始時間
        /// </summary>
        public DateTime Start { get; set; }

        /// <summary>
        /// 結束時間
        /// </summary>
        public DateTime End { get; set; }

        /// <summary>
        /// 是否為全天事件
        /// </summary>
        public bool AllDay { get; set; }

        /// <summary>
        /// 寵物名稱
        /// </summary>
        public string PetName { get; set; } = string.Empty;

        /// <summary>
        /// 聯絡人姓名
        /// </summary>
        public string ContactName { get; set; } = string.Empty;

        /// <summary>
        /// 聯絡人電話
        /// </summary>
        public string ContactPhone { get; set; } = string.Empty;

        /// <summary>
        /// 預約狀態
        /// </summary>
        public string Status { get; set; } = string.Empty;

        /// <summary>
        /// 背景顏色
        /// </summary>
        public string BackgroundColor { get; set; } = string.Empty;
    }

    /// <summary>
    /// 可用性檢查 DTO
    /// </summary>
    public class AvailabilityCheckDto
    {
        /// <summary>
        /// 是否可用
        /// </summary>
        public bool Available { get; set; }

        /// <summary>
        /// 衝突的預約
        /// </summary>
        public List<ConflictReservationDto> Conflicts { get; set; } = new();

        /// <summary>
        /// 建議的時段
        /// </summary>
        public List<int> SuggestedTimes { get; set; } = new();
    }

    /// <summary>
    /// 衝突預約 DTO
    /// </summary>
    public class ConflictReservationDto
    {
        /// <summary>
        /// 預約 ID
        /// </summary>
        public long Id { get; set; }

        /// <summary>
        /// 寵物名稱
        /// </summary>
        public string PetName { get; set; } = string.Empty;

        /// <summary>
        /// 開始時間 (分鐘)
        /// </summary>
        public int StartTime { get; set; }

        /// <summary>
        /// 結束時間 (分鐘)
        /// </summary>
        public int EndTime { get; set; }

        /// <summary>
        /// 預約狀態
        /// </summary>
        public string Status { get; set; } = string.Empty;
    }
}