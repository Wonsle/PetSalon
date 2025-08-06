using PetSalon.Models.EntityModels;

namespace PetSalon.Models.DTOs
{
    public class SystemCodeDto
    {
        public int Id { get; set; }
        public string Type { get; set; }
        public string Code { get; set; }
        public string Name { get; set; }
        public string Value { get; set; }
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
                Type = entity.CodeType,
                Code = entity.Code,
                Name = entity.Name,
                Value = entity.Code, // Use Code as Value for simplicity
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
                CodeType = Type,
                Code = Code,
                Name = Name,
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