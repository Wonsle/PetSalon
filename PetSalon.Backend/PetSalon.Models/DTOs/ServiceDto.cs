namespace PetSalon.Models.DTOs
{
    public class ServiceDto
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public string ServiceType { get; set; } = string.Empty;
        public string ServiceTypeName { get; set; } = string.Empty; // From SystemCode
        public decimal BasePrice { get; set; }
        public int Duration { get; set; }
        public string? Description { get; set; }
        public bool IsActive { get; set; }
        public int Sort { get; set; }
    }

    public class PetServicePriceDto
    {
        public long PetServicePriceId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public long ServiceId { get; set; }
        public string ServiceName { get; set; } = string.Empty;
        public decimal? CustomPrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice => CustomPrice ?? BasePrice;
        public int? Duration { get; set; }
        public int BaseDuration { get; set; }
        public int FinalDuration => Duration ?? BaseDuration;
        public string? Notes { get; set; }
        public bool IsActive { get; set; }
    }
}