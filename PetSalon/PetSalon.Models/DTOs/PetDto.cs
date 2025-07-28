using System.ComponentModel.DataAnnotations;

namespace PetSalon.Models.DTOs
{
    public class CreatePetRequest
    {
        [Required(ErrorMessage = "寵物名稱為必填")]
        [StringLength(50, ErrorMessage = "寵物名稱長度不能超過50個字符")]
        public string PetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "性別為必填")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "品種為必填")]
        public string Breed { get; set; } = string.Empty;

        public DateTime? BirthDay { get; set; }

        public decimal? NormalPrice { get; set; }

        public decimal? SubscriptionPrice { get; set; }
    }

    public class UpdatePetRequest
    {
        [Required]
        public long PetId { get; set; }

        [Required(ErrorMessage = "寵物名稱為必填")]
        [StringLength(50, ErrorMessage = "寵物名稱長度不能超過50個字符")]
        public string PetName { get; set; } = string.Empty;

        [Required(ErrorMessage = "性別為必填")]
        public string Gender { get; set; } = string.Empty;

        [Required(ErrorMessage = "品種為必填")]
        public string Breed { get; set; } = string.Empty;

        public DateTime? BirthDay { get; set; }

        public decimal? NormalPrice { get; set; }

        public decimal? SubscriptionPrice { get; set; }

        public string? PhotoUrl { get; set; }
    }
}