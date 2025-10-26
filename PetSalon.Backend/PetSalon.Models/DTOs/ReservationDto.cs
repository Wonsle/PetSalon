namespace PetSalon.Models.DTOs
{
    public class ReservationCreateDto
    {
        public long PetId { get; set; }
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
        public bool UseSubscription { get; set; }
        public long? SubscriptionId { get; set; }
        public int ServiceDurationMinutes { get; set; }
        public string Memo { get; set; } = string.Empty;
        public string Status { get; set; } = "PENDING";
    }

    public class ReservationUpdateDto
    {
        public long ReservationId { get; set; }
        public DateTime? ReservationDate { get; set; }
        public TimeSpan? ReservationTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public List<long> ServiceIds { get; set; } = new List<long>();
        public List<long> AddonIds { get; set; } = new List<long>();
        public int? ServiceDurationMinutes { get; set; }
    }

    public class ReservationDetailsDto
    {
        public long ReservationId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public DateTime ReservationDate { get; set; }
        public TimeSpan ReservationTime { get; set; }
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty;
        public bool UsedSubscription { get; set; }
        public long? SubscriptionId { get; set; }
        public List<ServiceItemDto> Services { get; set; } = new List<ServiceItemDto>();
        public decimal TotalAmount { get; set; }
        public int TotalDuration { get; set; }
        public DateTime CreatedTime { get; set; }
        public List<ContactPersonDto> ContactPersons { get; set; } = new List<ContactPersonDto>();
    }

    public class ContactPersonDto
    {
        public long ContactPersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string NickName { get; set; } = string.Empty;
        public string ContactNumber { get; set; } = string.Empty;
        public string RelationshipType { get; set; } = string.Empty;
        public string RelationshipName { get; set; } = string.Empty;
    }

    public class ReservationCalendarDto
    {
        public DateTime Date { get; set; }
        public List<ReservationSlotDto> Reservations { get; set; } = new List<ReservationSlotDto>();
        public int TotalReservations => Reservations.Count;
        public bool IsFullyBooked { get; set; }
    }

    public class ReservationSlotDto
    {
        public long ReservationId { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string ContactName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string StatusName { get; set; } = string.Empty;
        public List<string> ServiceNames { get; set; } = new List<string>();
        public bool IsSubscription { get; set; }
    }

    /// <summary>
    /// 計算服務時長請求 DTO
    /// </summary>
    public class DurationCalculationRequest
    {
        public List<long> ServiceIds { get; set; } = new List<long>();
    }

    /// <summary>
    /// 預約列表項目 DTO - 用於預約列表顯示
    /// </summary>
    public class ReservationListItemDto
    {
        public long Id { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty; // Alias for PetName for compatibility
        public long? OwnerId { get; set; }
        public string OwnerName { get; set; } = string.Empty;
        public string ContactPhone { get; set; } = string.Empty;
        public long? SubscriptionId { get; set; }
        public string SubscriptionName { get; set; } = string.Empty;
        public bool UseSubscription { get; set; }
        public int SubscriptionDeductionCount { get; set; }
        public string ReserveDate { get; set; } = string.Empty; // ISO date string
        public string ReserveTime { get; set; } = string.Empty; // HH:mm format
        public string ServiceType { get; set; } = string.Empty;
        public string Designer { get; set; } = string.Empty;
        public string Status { get; set; } = string.Empty;
        public string Note { get; set; } = string.Empty;
        public string Memo { get; set; } = string.Empty; // Alias for Note
        public decimal TotalAmount { get; set; }
        public int ServiceDurationMinutes { get; set; }
        public string CreateTime { get; set; } = string.Empty; // ISO datetime string
        public string UpdateTime { get; set; } = string.Empty; // ISO datetime string
    }

    /// <summary>
    /// 預約列表回應 DTO - 包含分頁資訊
    /// </summary>
    public class ReservationListResponseDto
    {
        public List<ReservationListItemDto> Data { get; set; } = new List<ReservationListItemDto>();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}