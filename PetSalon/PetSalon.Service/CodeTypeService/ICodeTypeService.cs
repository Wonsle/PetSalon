using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services.CodeTypeService
{
    /// <summary>
    /// 代碼類型服務介面
    /// </summary>
    public interface ICodeTypeService
    {
        /// <summary>
        /// 取得所有代碼類型
        /// </summary>
        /// <returns>代碼類型列表</returns>
        Task<IList<CodeType>> GetAllCodeTypesAsync();

        /// <summary>
        /// 根據ID取得代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>代碼類型</returns>
        Task<CodeType?> GetCodeTypeByIdAsync(int id);

        /// <summary>
        /// 根據代碼類型取得代碼類型
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <returns>代碼類型</returns>
        Task<CodeType?> GetCodeTypeByCodeAsync(string codeType);

        /// <summary>
        /// 建立新的代碼類型
        /// </summary>
        /// <param name="codeType">代碼類型資料</param>
        /// <param name="userName">操作者名稱</param>
        /// <returns>建立的代碼類型</returns>
        Task<CodeType> CreateCodeTypeAsync(CreateOrUpdateCodeTypeDto codeType, string userName);

        /// <summary>
        /// 更新代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <param name="codeType">更新的代碼類型資料</param>
        /// <param name="userName">操作者名稱</param>
        /// <returns>更新的代碼類型</returns>
        Task<CodeType?> UpdateCodeTypeAsync(int id, CreateOrUpdateCodeTypeDto codeType, string userName);

        /// <summary>
        /// 刪除代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>是否刪除成功</returns>
        Task<bool> DeleteCodeTypeAsync(int id);

        /// <summary>
        /// 檢查代碼類型是否存在
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <param name="excludeId">排除的ID（用於更新時檢查）</param>
        /// <returns>是否存在</returns>
        Task<bool> CodeTypeExistsAsync(string codeType, int? excludeId = null);
    }
}
