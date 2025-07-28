namespace PetSalon.Models.DTOs
{
    public class ServiceDto
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public string ServiceType { get; set; }
        public string ServiceTypeName { get; set; } // From SystemCode
        public decimal BasePrice { get; set; }
        public int Duration { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int Sort { get; set; }
    }

    public class ServiceAddonDto
    {
        public long ServiceAddonId { get; set; }
        public string AddonName { get; set; }
        public string AddonType { get; set; }
        public string AddonTypeName { get; set; } // From SystemCode
        public decimal AddonPrice { get; set; }
        public string Description { get; set; }
        public bool IsActive { get; set; }
        public int Sort { get; set; }
    }

    public class PetServicePriceDto
    {
        public long PetServicePriceId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; }
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal? CustomPrice { get; set; }
        public decimal BasePrice { get; set; }
        public decimal FinalPrice => CustomPrice ?? BasePrice;
        public int? Duration { get; set; }
        public int BaseDuration { get; set; }
        public int FinalDuration => Duration ?? BaseDuration;
        public string Notes { get; set; }
        public bool IsActive { get; set; }
    }

    public class ServiceCalculationDto
    {
        public long PetId { get; set; }
        public string PetName { get; set; }
        public List<ServiceItemDto> Services { get; set; } = new List<ServiceItemDto>();
        public List<ServiceAddonDto> Addons { get; set; } = new List<ServiceAddonDto>();
        public decimal SubTotal => Services.Sum(s => s.Price) + Addons.Sum(a => a.AddonPrice);
        public decimal Discount { get; set; }
        public decimal Total => SubTotal - Discount;
        public int TotalDuration => Services.Sum(s => s.Duration);
        public bool IsSubscriptionEligible { get; set; }
        public decimal? SubscriptionPrice { get; set; }
    }

    public class ServiceItemDto
    {
        public long ServiceId { get; set; }
        public string ServiceName { get; set; }
        public decimal Price { get; set; }
        public int Duration { get; set; }
        public string Notes { get; set; }
    }
}