namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 服務費用計算結果 DTO
    /// </summary>
    public class ServiceCalculationDto
    {
        public long PetId { get; set; }
        public List<ServiceItemDto> Services { get; set; } = new List<ServiceItemDto>();
        public List<ServiceAddonCalculationDto> Addons { get; set; } = new List<ServiceAddonCalculationDto>();
        public decimal ServiceTotal { get; set; }
        public decimal AddonTotal { get; set; }
        public decimal Discount { get; set; }
        public decimal TotalAmount => ServiceTotal + AddonTotal - Discount;
        public bool IsSubscriptionEligible { get; set; }
        public string? CalculationNote { get; set; }
    }

    /// <summary>
    /// 服務項目 DTO
    /// </summary>
    public class ServiceItemDto
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
    }


    /// <summary>
    /// 包月驗證結果 DTO
    /// </summary>
    public class SubscriptionValidationResultDto
    {
        public bool IsValid { get; set; }
        public string? Reason { get; set; }
        public long? SubscriptionId { get; set; }
        public string SubscriptionType { get; set; } = string.Empty;
        public int RemainingUsage { get; set; }
        public DateTime? ExpiryDate { get; set; }
        public bool WillExpireSoon => ExpiryDate.HasValue && (ExpiryDate.Value - DateTime.Now).Days <= 7;
    }

    /// <summary>
    /// 服務類型判斷結果 DTO
    /// </summary>
    public class ServiceTypeResultDto
    {
        public string ServiceType { get; set; } = string.Empty; // BATH/GROOM/MIXED
        public int DeductionCount { get; set; } // 扣除次數
        public string? DeductionReason { get; set; } // 扣除原因說明
        public List<string> ServiceDetails { get; set; } = new List<string>();
    }

    /// <summary>
    /// 附加服務計算 DTO
    /// </summary>
    public class ServiceAddonCalculationDto
    {
        public long AddonId { get; set; }
        public string AddonName { get; set; } = string.Empty;
        public string AddonType { get; set; } = string.Empty;
        public decimal Price { get; set; }
        public string? Description { get; set; }
    }
}
