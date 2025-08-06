using System.ComponentModel.DataAnnotations;

namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 寵物主人資訊
    /// </summary>
    public class PetOwnerInfo
    {
        /// <summary>
        /// 聯絡人ID
        /// </summary>
        public long ContactPersonId { get; set; }

        /// <summary>
        /// 主人姓名
        /// </summary>
        public string Name { get; set; } = string.Empty;

        /// <summary>
        /// 聯絡電話
        /// </summary>
        public string ContactNumber { get; set; } = string.Empty;

        /// <summary>
        /// 關係類型代碼
        /// </summary>
        public string RelationshipType { get; set; } = string.Empty;

        /// <summary>
        /// 關係類型名稱
        /// </summary>
        public string RelationshipTypeName { get; set; } = string.Empty;

        /// <summary>
        /// 格式化的顯示文字: 姓名(電話號碼)
        /// </summary>
        public string DisplayText => $"{Name}({ContactNumber})";
    }

    /// <summary>
    /// 寵物列表回應資料
    /// </summary>
    public class PetListResponse
    {
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public DateTime? BirthDay { get; set; }
        public decimal? NormalPrice { get; set; }
        public decimal? SubscriptionPrice { get; set; }
        public string CreateUser { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public string ModifyUser { get; set; } = string.Empty;
        public DateTime ModifyTime { get; set; }

        /// <summary>
        /// 主人資訊列表（關係類型為飼主的聯絡人）
        /// </summary>
        public List<PetOwnerInfo> Owners { get; set; } = new List<PetOwnerInfo>();

        /// <summary>
        /// 主人顯示文字，多個主人以逗號分隔
        /// </summary>
        public string OwnersDisplay => Owners.Any() 
            ? string.Join(", ", Owners.Select(o => o.DisplayText))
            : "無飼主資訊";
    }

    /// <summary>
    /// 寵物詳細資訊回應資料
    /// </summary>
    public class PetDetailResponse : PetListResponse
    {
        /// <summary>
        /// 所有聯絡人關係（不只是飼主）
        /// </summary>
        public List<PetOwnerInfo> AllContacts { get; set; } = new List<PetOwnerInfo>();
    }
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