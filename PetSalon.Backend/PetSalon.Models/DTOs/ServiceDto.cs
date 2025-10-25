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

    // PetServicePriceDto 已移至 PetServicePricingDto.cs 以避免重複定義
}