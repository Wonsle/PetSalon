namespace PetSalon.Models.DTOs
{
    public class SubscriptionCreateDto
    {
        public long PetId { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public int TotalUsageLimit { get; set; } = 1; // 預設1次，滿足資料庫 > 0 的約束
        public decimal SubscriptionPrice { get; set; }
        public string Notes { get; set; }
    }

    public class SubscriptionUpdateDto
    {
        public long SubscriptionId { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public int? TotalUsageLimit { get; set; }
        public decimal? SubscriptionPrice { get; set; }
        public string Notes { get; set; }
    }

    public class SubscriptionDetailsDto
    {
        public long SubscriptionId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public DateTime SubscriptionDate { get; set; }
        public int TotalUsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int RemainingUsage => TotalUsageLimit > 0 ? Math.Max(0, TotalUsageLimit - UsedCount) : int.MaxValue;
        public decimal SubscriptionPrice { get; set; }
        public string Notes { get; set; }
        public bool IsExpired => DateTime.Now > EndDate;
        public bool IsActive => !IsExpired;
        public int DaysUntilExpiry => (EndDate - DateTime.Now).Days;
        public List<ReservationSummaryDto> Reservations { get; set; } = new List<ReservationSummaryDto>();
    }

    public class SubscriptionUsageDto
    {
        public long SubscriptionId { get; set; }
        public string PetName { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
        public int TotalUsageLimit { get; set; }
        public int UsedCount { get; set; }
        public int RemainingUsage => TotalUsageLimit > 0 ? Math.Max(0, TotalUsageLimit - UsedCount) : int.MaxValue;
        public bool HasUnlimitedUsage => TotalUsageLimit == 0;
        public decimal AverageUsagePerMonth { get; set; }
        public List<DateTime> UsageDates { get; set; } = new List<DateTime>();
    }

    public class ReservationSummaryDto
    {
        public long ReservationId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public decimal TotalAmount { get; set; }
        public List<string> ServiceNames { get; set; } = new List<string>();
    }

    public class SubscriptionValidationDto
    {
        public bool IsValid { get; set; }
        public string Reason { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public int? RemainingUsage { get; set; }
        public bool WillExpireSoon => ExpiryDate.HasValue && (ExpiryDate.Value - DateTime.Now).Days <= 7;
    }
}