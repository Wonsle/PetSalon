using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services.CodeTypeService
{
    /// <summary>
    /// 代碼類型服務實作
    /// </summary>
    public class CodeTypeService : ICodeTypeService
    {
        private readonly PetSalonContext _context;

        public CodeTypeService(PetSalonContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 取得所有代碼類型
        /// </summary>
        /// <returns>代碼類型列表</returns>
        public async Task<IList<CodeType>> GetAllCodeTypesAsync()
        {
            return await _context.CodeType
                .OrderBy(x => x.CodeType1)
                .ToListAsync();
        }

        /// <summary>
        /// 根據ID取得代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>代碼類型</returns>
        public async Task<CodeType?> GetCodeTypeByIdAsync(int id)
        {
            return await _context.CodeType
                .FirstOrDefaultAsync(x => x.Id == id);
        }

        /// <summary>
        /// 根據代碼類型取得代碼類型
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <returns>代碼類型</returns>
        public async Task<CodeType?> GetCodeTypeByCodeAsync(string codeType)
        {
            return await _context.CodeType
                .FirstOrDefaultAsync(x => x.CodeType1 == codeType);
        }

        /// <summary>
        /// 建立新的代碼類型
        /// </summary>
        /// <param name="codeTypeDto">代碼類型資料</param>
        /// <param name="userName">操作者名稱</param>
        /// <returns>建立的代碼類型</returns>
        public async Task<CodeType> CreateCodeTypeAsync(CreateOrUpdateCodeTypeDto codeTypeDto, string userName)
        {
            var codeType = new CodeType
            {
                CodeType1 = codeTypeDto.CodeType,
                Name = codeTypeDto.Name,
                Description = codeTypeDto.Description ?? string.Empty,
                CreateUser = userName,
                CreateTime = DateTime.Now,
                ModifyUser = userName,
                ModifyTime = DateTime.Now
            };

            _context.CodeType.Add(codeType);
            await _context.SaveChangesAsync();

            return codeType;
        }

        /// <summary>
        /// 更新代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <param name="codeTypeDto">更新的代碼類型資料</param>
        /// <param name="userName">操作者名稱</param>
        /// <returns>更新的代碼類型</returns>
        public async Task<CodeType?> UpdateCodeTypeAsync(int id, CreateOrUpdateCodeTypeDto codeTypeDto, string userName)
        {
            var existingCodeType = await _context.CodeType
                .FirstOrDefaultAsync(x => x.Id == id);

            if (existingCodeType == null)
                return null;

            existingCodeType.CodeType1 = codeTypeDto.CodeType;
            existingCodeType.Name = codeTypeDto.Name;
            existingCodeType.Description = codeTypeDto.Description ?? string.Empty;
            existingCodeType.ModifyUser = userName;
            existingCodeType.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();

            return existingCodeType;
        }

        /// <summary>
        /// 刪除代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>是否刪除成功</returns>
        public async Task<bool> DeleteCodeTypeAsync(int id)
        {
            var codeType = await _context.CodeType
                .FirstOrDefaultAsync(x => x.Id == id);

            if (codeType == null)
                return false;

            // 檢查是否有相關的系統代碼使用此代碼類型
            var hasRelatedSystemCodes = await _context.SystemCode
                .AnyAsync(x => x.CodeType == codeType.CodeType1);

            if (hasRelatedSystemCodes)
                throw new InvalidOperationException("無法刪除此代碼類型，因為仍有系統代碼正在使用此類型");

            _context.CodeType.Remove(codeType);
            await _context.SaveChangesAsync();

            return true;
        }

        /// <summary>
        /// 檢查代碼類型是否存在
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <param name="excludeId">排除的ID（用於更新時檢查）</param>
        /// <returns>是否存在</returns>
        public async Task<bool> CodeTypeExistsAsync(string codeType, int? excludeId = null)
        {
            var query = _context.CodeType.Where(x => x.CodeType1 == codeType);

            if (excludeId.HasValue)
                query = query.Where(x => x.Id != excludeId.Value);

            return await query.AnyAsync();
        }
    }
}
