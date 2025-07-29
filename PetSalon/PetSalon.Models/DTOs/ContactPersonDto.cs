using System.ComponentModel.DataAnnotations;

namespace PetSalon.Models.DTOs
{
    public class CreateContactPersonRequest
    {
        [Required(ErrorMessage = "姓名為必填")]
        [StringLength(50, ErrorMessage = "姓名長度不能超過50個字符")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "暱稱長度不能超過50個字符")]
        public string? NickName { get; set; }

        [Required(ErrorMessage = "聯絡電話為必填")]
        [StringLength(20, ErrorMessage = "聯絡電話長度不能超過20個字符")]
        public string ContactNumber { get; set; } = string.Empty;

        public List<CreatePetRelationRequest>? PetRelations { get; set; }
    }

    public class CreatePetRelationRequest
    {
        [Required(ErrorMessage = "寵物ID為必填")]
        public long PetId { get; set; }

        [Required(ErrorMessage = "關係類型為必填")]
        public string RelationshipType { get; set; } = string.Empty;

        public int Sort { get; set; } = 1;
    }

    public class UpdateContactPersonRequest
    {
        [Required]
        public long ContactPersonId { get; set; }

        [Required(ErrorMessage = "姓名為必填")]
        [StringLength(50, ErrorMessage = "姓名長度不能超過50個字符")]
        public string Name { get; set; } = string.Empty;

        [StringLength(50, ErrorMessage = "暱稱長度不能超過50個字符")]
        public string? NickName { get; set; }

        [Required(ErrorMessage = "聯絡電話為必填")]
        [StringLength(20, ErrorMessage = "聯絡電話長度不能超過20個字符")]
        public string ContactNumber { get; set; } = string.Empty;
    }

    public class ContactPersonResponse
    {
        public long ContactPersonId { get; set; }
        public string Name { get; set; } = string.Empty;
        public string? NickName { get; set; }
        public string ContactNumber { get; set; } = string.Empty;
        public string CreateUser { get; set; } = string.Empty;
        public DateTime CreateTime { get; set; }
        public string ModifyUser { get; set; } = string.Empty;
        public DateTime ModifyTime { get; set; }
        public List<PetRelationInfo>? RelatedPets { get; set; }
    }

    public class PetRelationInfo
    {
        public long PetRelationId { get; set; }
        public long PetId { get; set; }
        public string PetName { get; set; } = string.Empty;
        public string Breed { get; set; } = string.Empty;
        public string Gender { get; set; } = string.Empty;
        public string RelationshipType { get; set; } = string.Empty;
        public string RelationshipTypeName { get; set; } = string.Empty;
        public int Sort { get; set; }
    }

    public class ContactPersonSearchRequest
    {
        public string? Keyword { get; set; }
        public string? Name { get; set; }
        public string? ContactNumber { get; set; }
        public int Page { get; set; } = 1;
        public int PageSize { get; set; } = 20;
    }

    public class ContactPersonListResponse
    {
        public List<ContactPersonResponse> Data { get; set; } = new();
        public int Total { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }

    public class LinkContactToPetRequest
    {
        [Required(ErrorMessage = "關係類型為必填")]
        public string RelationshipType { get; set; } = string.Empty;
        
        public int Sort { get; set; } = 1;
    }

    public class RelationshipTypeResponse
    {
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;
        public int Sort { get; set; }
    }
}