using System.ComponentModel.DataAnnotations;

namespace PetSalon.Models.DTOs
{

    /// <summary>
    /// 寵物服務時間設定相關 DTO
    /// </summary>
    public class PetServiceDurationDto
    {
        public long PetServiceDurationId { get; set; }
        
        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }
        
        public string? PetName { get; set; }
        
        [Required(ErrorMessage = "服務ID必填")]
        public long ServiceId { get; set; }
        
        public string? ServiceName { get; set; }
        
        public string? ServiceType { get; set; }
        
        public int DefaultDuration { get; set; }
        
        [Range(1, 600, ErrorMessage = "客製化時長必須在1-600分鐘之間")]
        public int? CustomDuration { get; set; }
        
        /// <summary>
        /// 有效時長（客製化時長優先，否則使用預設時長）
        /// </summary>
        public int EffectiveDuration => CustomDuration ?? DefaultDuration;
        
        [StringLength(500, ErrorMessage = "備註不可超過500個字元")]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
        
        public string? CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime ModifyTime { get; set; }
    }


    /// <summary>
    /// 新增寵物服務時間請求 DTO
    /// </summary>
    public class CreatePetServiceDurationRequest
    {
        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }
        
        [Required(ErrorMessage = "服務ID必填")]
        public long ServiceId { get; set; }
        
        [Range(1, 600, ErrorMessage = "客製化時長必須在1-600分鐘之間")]
        public int? CustomDuration { get; set; }
        
        [StringLength(500, ErrorMessage = "備註不可超過500個字元")]
        public string? Notes { get; set; }
        
        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 寵物定價總覽 DTO
    /// </summary>
    public class PetPricingOverviewDto
    {
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        
        /// <summary>
        /// 服務時間設定
        /// </summary>
        public IList<PetServiceDurationDto> ServiceDurations { get; set; } = new List<PetServiceDurationDto>();
        
        /// <summary>
        /// 總客製化項目數量
        /// </summary>
        public int TotalCustomSettings => ServiceDurations.Count;
    }

    /// <summary>
    /// 批次建立寵物定價請求 DTO
    /// </summary>
    public class BatchCreatePetPricingRequest
    {
        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }

        public IList<CreatePetServiceDurationRequest>? ServiceDurations { get; set; }
    }

    // ============================================
    // PetServicePrice 相關 DTO (整合價格和時長)
    // ============================================

    /// <summary>
    /// 寵物服務價格設定 DTO
    /// </summary>
    public class PetServicePriceDto
    {
        public long PetServicePriceId { get; set; }

        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }

        public string? PetName { get; set; }

        [Required(ErrorMessage = "服務ID必填")]
        public long ServiceId { get; set; }

        public string? ServiceName { get; set; }

        public string? ServiceType { get; set; }

        public decimal DefaultPrice { get; set; }

        public int DefaultDuration { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "客製化價格不可小於零")]
        public decimal? CustomPrice { get; set; }

        [Range(1, 600, ErrorMessage = "客製化時長必須在1-600分鐘之間")]
        public int? Duration { get; set; }

        /// <summary>
        /// 有效價格（客製化價格優先，否則使用預設價格）
        /// </summary>
        public decimal EffectivePrice => CustomPrice ?? DefaultPrice;

        /// <summary>
        /// 有效時長（客製化時長優先，否則使用預設時長）
        /// </summary>
        public int EffectiveDuration => Duration ?? DefaultDuration;

        [StringLength(500, ErrorMessage = "備註不可超過500個字元")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;

        public string? CreateUser { get; set; }
        public DateTime CreateTime { get; set; }
        public string? ModifyUser { get; set; }
        public DateTime ModifyTime { get; set; }
    }

    /// <summary>
    /// 新增/更新寵物服務價格請求 DTO
    /// </summary>
    public class CreatePetServicePriceRequest
    {
        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }

        [Required(ErrorMessage = "服務ID必填")]
        public long ServiceId { get; set; }

        [Range(0, double.MaxValue, ErrorMessage = "客製化價格不可小於零")]
        public decimal? CustomPrice { get; set; }

        [Range(1, 600, ErrorMessage = "客製化時長必須在1-600分鐘之間")]
        public int? Duration { get; set; }

        [StringLength(500, ErrorMessage = "備註不可超過500個字元")]
        public string? Notes { get; set; }

        public bool IsActive { get; set; } = true;
    }

    /// <summary>
    /// 批次建立寵物服務價格請求 DTO
    /// </summary>
    public class BatchCreatePetServicePricesRequest
    {
        [Required(ErrorMessage = "寵物ID必填")]
        public long PetId { get; set; }

        public IList<CreatePetServicePriceRequest>? ServicePrices { get; set; }
    }
}