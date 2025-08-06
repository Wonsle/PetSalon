using PetSalon.Models.EntityModels;

namespace PetSalon.Models.DTOs
{
    public class SystemCodeDto
    {
        public int Id { get; set; }
        public string Type { get; set; } = string.Empty;
        public string Code { get; set; } = string.Empty;
        public string Name { get; set; } = string.Empty;
        public string Value { get; set; } = string.Empty;
        public int Sort { get; set; }
        public bool IsActive { get; set; }
        public string? CreateUser { get; set; }
        public DateTime? CreateTime { get; set; }
        public string? UpdateUser { get; set; }
        public DateTime? UpdateTime { get; set; }

        public static SystemCodeDto FromEntity(SystemCode entity)
        {
            return new SystemCodeDto
            {
                Id = entity.CodeId,
                Type = entity.CodeType ?? string.Empty,
                Code = entity.Code ?? string.Empty,
                Name = entity.Name ?? string.Empty,
                Value = entity.Code ?? string.Empty, // Use Code as Value for simplicity
                Sort = entity.Sort ?? 0,
                IsActive = entity.EndDate == null || entity.EndDate > DateTime.Now,
                CreateUser = entity.CreateUser,
                CreateTime = entity.CreateTime,
                UpdateUser = entity.ModifyUser,
                UpdateTime = entity.ModifyTime
            };
        }

        public SystemCode ToEntity()
        {
            return new SystemCode
            {
                CodeId = Id,
                CodeType = string.IsNullOrEmpty(Type) ? throw new ArgumentException("Type cannot be null or empty") : Type,
                Code = string.IsNullOrEmpty(Code) ? throw new ArgumentException("Code cannot be null or empty") : Code,
                Name = string.IsNullOrEmpty(Name) ? throw new ArgumentException("Name cannot be null or empty") : Name,
                Description = Name, // Use Name as Description
                Sort = Sort,
                StartDate = DateTime.Now,
                EndDate = IsActive ? null : DateTime.Now,
                CreateUser = CreateUser,
                CreateTime = CreateTime ?? DateTime.Now,
                ModifyUser = UpdateUser,
                ModifyTime = UpdateTime ?? DateTime.Now
            };
        }

        public void UpdateEntity(SystemCode entity)
        {
            entity.Name = Name;
            entity.Description = Name;
            entity.Sort = Sort;
            entity.EndDate = IsActive ? null : DateTime.Now;
            entity.ModifyUser = UpdateUser;
            entity.ModifyTime = DateTime.Now;
        }
    }
}