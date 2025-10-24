using Microsoft.AspNetCore.Http;
using PetSalon.Models.DTOs;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace PetSalon.Services
{
    /// <summary>
    /// 檔案服務接口 - 統一檔案上傳、儲存和管理
    /// </summary>
    public interface IFileService
    {
        /// <summary>
        /// 儲存檔案並建立檔案附件記錄
        /// </summary>
        /// <param name="file">上傳的檔案</param>
        /// <param name="entityType">實體類型（如：Pet, ContactPerson）</param>
        /// <param name="entityId">實體ID</param>
        /// <param name="attachmentType">附件類型（如：Photo, Avatar, Document）</param>
        /// <returns>檔案附件 DTO</returns>
        Task<FileAttachmentDto> SaveFileAsync(IFormFile file, string entityType, long entityId, string attachmentType);

        /// <summary>
        /// 取得特定實體的所有檔案附件
        /// </summary>
        /// <param name="entityType">實體類型</param>
        /// <param name="entityId">實體ID</param>
        /// <param name="attachmentType">附件類型（可選）</param>
        /// <returns>檔案附件列表</returns>
        Task<List<FileAttachmentDto>> GetEntityFilesAsync(string entityType, long entityId, string? attachmentType = null);

        /// <summary>
        /// 取得特定檔案附件資訊
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>檔案附件 DTO</returns>
        Task<FileAttachmentDto?> GetFileAsync(long fileId);

        /// <summary>
        /// 刪除檔案附件（軟刪除）
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>是否成功</returns>
        Task<bool> DeleteFileAsync(long fileId);

        /// <summary>
        /// 永久刪除檔案附件（包含實體檔案）
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>是否成功</returns>
        Task<bool> PermanentlyDeleteFileAsync(long fileId);

        /// <summary>
        /// 計算檔案的 SHA256 Hash 值
        /// </summary>
        /// <param name="stream">檔案串流</param>
        /// <returns>Hash 值（小寫16進位字串）</returns>
        Task<string> CalculateFileHashAsync(Stream stream);

        /// <summary>
        /// 檢查是否存在相同 Hash 的檔案
        /// </summary>
        /// <param name="hash">檔案 Hash 值</param>
        /// <param name="entityType">實體類型</param>
        /// <param name="entityId">實體ID</param>
        /// <returns>是否存在</returns>
        Task<FileAttachmentDto?> FindDuplicateFileAsync(string hash, string entityType, long entityId);

        /// <summary>
        /// 更新檔案顯示順序
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <param name="displayOrder">新的顯示順序</param>
        /// <returns>是否成功</returns>
        Task<bool> UpdateDisplayOrderAsync(long fileId, int displayOrder);
    }
}
