using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace PetSalon.Services
{
    /// <summary>
    /// 檔案服務實作 - 統一檔案上傳、儲存和管理
    /// </summary>
    public class FileService : IFileService
    {
        private readonly PetSalonContext _context;
        private readonly ILogger<FileService> _logger;

        public FileService(PetSalonContext context, ILogger<FileService> logger)
        {
            _context = context;
            _logger = logger;
        }

        public async Task<FileAttachmentDto> SaveFileAsync(IFormFile file, string entityType, long entityId, string attachmentType)
        {
            try
            {
                // 1. 計算檔案 Hash 值
                string hash;
                using (var stream = file.OpenReadStream())
                {
                    hash = await CalculateFileHashAsync(stream);
                }

                // 2. 檢查是否已存在相同 Hash 的檔案
                var existing = await FindDuplicateFileAsync(hash, entityType, entityId);
                if (existing != null)
                {
                    _logger.LogInformation($"Found duplicate file with hash {hash}, returning existing file {existing.FileId}");
                    return existing;
                }

                // 3. 取得副檔名和 MIME Type
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                var mimeType = file.ContentType;

                // 4. 產生儲存檔名
                var storedFileName = $"{Guid.NewGuid()}{extension}";

                // 5. 建立上傳目錄
                var entityFolder = entityType.ToLower();
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads", entityFolder);
                Directory.CreateDirectory(uploadsFolder);

                // 6. 儲存檔案到實體位置（使用新的 stream）
                var filePath = Path.Combine(uploadsFolder, storedFileName);
                using (var fileStream = new FileStream(filePath, FileMode.Create))
                {
                    using (var uploadStream = file.OpenReadStream())
                    {
                        await uploadStream.CopyToAsync(fileStream);
                    }
                }

                // 7. 建立資料庫記錄
                var fileAttachment = new FileAttachment
                {
                    OriginalFileName = file.FileName,
                    StoredFileName = storedFileName,
                    FilePath = $"/uploads/{entityFolder}/{storedFileName}",
                    FileSize = file.Length,
                    MimeType = mimeType,
                    Extension = extension,
                    FileHash = hash,
                    EntityType = entityType,
                    EntityId = entityId,
                    AttachmentType = attachmentType,
                    DisplayOrder = 0,
                    IsActive = true,
                    CreateUser = "System", // TODO: 從認證中取得實際使用者
                    CreateTime = DateTime.Now,
                    ModifyUser = "System",
                    ModifyTime = DateTime.Now
                };

                _context.FileAttachment.Add(fileAttachment);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"File saved successfully: {file.FileName} -> {storedFileName} (ID: {fileAttachment.FileId})");

                return MapToDto(fileAttachment);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error saving file: {file.FileName}");
                throw;
            }
        }

        public async Task<List<FileAttachmentDto>> GetEntityFilesAsync(string entityType, long entityId, string? attachmentType = null)
        {
            var query = _context.FileAttachment
                .Where(f => f.EntityType == entityType && f.EntityId == entityId && f.IsActive);

            if (!string.IsNullOrEmpty(attachmentType))
            {
                query = query.Where(f => f.AttachmentType == attachmentType);
            }

            var files = await query
                .OrderBy(f => f.DisplayOrder)
                .ThenByDescending(f => f.CreateTime)
                .AsNoTracking()
                .ToListAsync();

            return files.Select(MapToDto).ToList();
        }

        public async Task<FileAttachmentDto?> GetFileAsync(long fileId)
        {
            var file = await _context.FileAttachment
                .AsNoTracking()
                .FirstOrDefaultAsync(f => f.FileId == fileId);

            return file != null ? MapToDto(file) : null;
        }

        public async Task<bool> DeleteFileAsync(long fileId)
        {
            try
            {
                var file = await _context.FileAttachment.FindAsync(fileId);
                if (file == null)
                    return false;

                // 軟刪除
                file.IsActive = false;
                file.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                file.ModifyTime = DateTime.Now;

                await _context.SaveChangesAsync();

                _logger.LogInformation($"File soft deleted: {file.FileId} - {file.OriginalFileName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file: {fileId}");
                return false;
            }
        }

        public async Task<bool> PermanentlyDeleteFileAsync(long fileId)
        {
            try
            {
                var file = await _context.FileAttachment.FindAsync(fileId);
                if (file == null)
                    return false;

                // 刪除實體檔案
                var physicalPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", file.FilePath.TrimStart('/'));
                if (File.Exists(physicalPath))
                {
                    File.Delete(physicalPath);
                    _logger.LogInformation($"Physical file deleted: {physicalPath}");
                }

                // 刪除資料庫記錄
                _context.FileAttachment.Remove(file);
                await _context.SaveChangesAsync();

                _logger.LogInformation($"File permanently deleted: {file.FileId} - {file.OriginalFileName}");
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting file: {fileId}");
                return false;
            }
        }

        public async Task<string> CalculateFileHashAsync(Stream stream)
        {
            using var sha256 = SHA256.Create();
            var hashBytes = await sha256.ComputeHashAsync(stream);
            return BitConverter.ToString(hashBytes).Replace("-", "").ToLower();
        }

        public async Task<FileAttachmentDto?> FindDuplicateFileAsync(string hash, string entityType, long entityId)
        {
            var existing = await _context.FileAttachment
                .AsNoTracking()
                .FirstOrDefaultAsync(f =>
                    f.FileHash == hash &&
                    f.EntityType == entityType &&
                    f.EntityId == entityId &&
                    f.IsActive);

            return existing != null ? MapToDto(existing) : null;
        }

        public async Task<bool> UpdateDisplayOrderAsync(long fileId, int displayOrder)
        {
            try
            {
                var file = await _context.FileAttachment.FindAsync(fileId);
                if (file == null)
                    return false;

                file.DisplayOrder = displayOrder;
                file.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                file.ModifyTime = DateTime.Now;

                await _context.SaveChangesAsync();
                return true;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating display order for file: {fileId}");
                return false;
            }
        }

        /// <summary>
        /// 將 Entity 映射為 DTO
        /// </summary>
        private FileAttachmentDto MapToDto(FileAttachment entity)
        {
            return new FileAttachmentDto
            {
                FileId = entity.FileId,
                OriginalFileName = entity.OriginalFileName,
                StoredFileName = entity.StoredFileName,
                FilePath = entity.FilePath,
                FileSize = entity.FileSize,
                MimeType = entity.MimeType,
                Extension = entity.Extension,
                FileHash = entity.FileHash,
                EntityType = entity.EntityType,
                EntityId = entity.EntityId,
                AttachmentType = entity.AttachmentType,
                DisplayOrder = entity.DisplayOrder,
                IsActive = entity.IsActive,
                CreateUser = entity.CreateUser,
                CreateTime = entity.CreateTime,
                ModifyUser = entity.ModifyUser,
                ModifyTime = entity.ModifyTime
            };
        }
    }
}
