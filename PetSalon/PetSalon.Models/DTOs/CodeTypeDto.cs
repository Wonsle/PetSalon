using PetSalon.Models.EntityModels;

namespace PetSalon.Models.DTOs
{
    /// <summary>
    /// 代碼類型資料傳輸物件
    /// </summary>
    public class CodeTypeDto
    {
        /// <summary>
        /// 代碼類型唯一識別碼
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// 代碼類型代碼
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// 代碼類型名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述說明
        /// </summary>
        public string? Description { get; set; }

        /// <summary>
        /// 建立者
        /// </summary>
        public string? CreateUser { get; set; }

        /// <summary>
        /// 建立時間
        /// </summary>
        public DateTime? CreateTime { get; set; }

        /// <summary>
        /// 修改者
        /// </summary>
        public string? ModifyUser { get; set; }

        /// <summary>
        /// 修改時間
        /// </summary>
        public DateTime? ModifyTime { get; set; }

        /// <summary>
        /// 完整顯示名稱
        /// </summary>
        public string DisplayName => $"{CodeType} - {Name}";

        /// <summary>
        /// 從實體轉換為DTO
        /// </summary>
        /// <param name="entity">CodeType實體</param>
        /// <returns>CodeTypeDto</returns>
        public static CodeTypeDto FromEntity(CodeType entity)
        {
            return new CodeTypeDto
            {
                Id = entity.Id,
                CodeType = entity.CodeType1,
                Name = entity.Name,
                Description = entity.Description,
                CreateUser = entity.CreateUser,
                CreateTime = entity.CreateTime,
                ModifyUser = entity.ModifyUser,
                ModifyTime = entity.ModifyTime
            };
        }

        /// <summary>
        /// 轉換為實體物件
        /// </summary>
        /// <returns>CodeType實體</returns>
        public CodeType ToEntity()
        {
            return new CodeType
            {
                Id = Id,
                CodeType1 = CodeType,
                Name = Name,
                Description = Description ?? string.Empty,
                CreateUser = CreateUser ?? string.Empty,
                CreateTime = CreateTime ?? DateTime.Now,
                ModifyUser = ModifyUser ?? string.Empty,
                ModifyTime = ModifyTime ?? DateTime.Now
            };
        }
    }

    /// <summary>
    /// 建立或更新代碼類型的請求DTO
    /// </summary>
    public class CreateOrUpdateCodeTypeDto
    {
        /// <summary>
        /// 代碼類型代碼
        /// </summary>
        public string CodeType { get; set; }

        /// <summary>
        /// 代碼類型名稱
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 描述說明
        /// </summary>
        public string? Description { get; set; }
    }
}
