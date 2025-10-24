using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 檔案管理API控制器 - 提供統一的檔案上傳、查詢和刪除功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class FileController : ControllerBase
    {
        private readonly IFileService _fileService;
        private readonly ILogger<FileController> _logger;

        public FileController(IFileService fileService, ILogger<FileController> logger)
        {
            _fileService = fileService;
            _logger = logger;
        }

        /// <summary>
        /// 上傳檔案並建立關聯
        /// </summary>
        /// <param name="file">檔案</param>
        /// <param name="entityType">實體類型（如：Pet, ContactPerson）</param>
        /// <param name="entityId">實體ID</param>
        /// <param name="attachmentType">附件類型（如：Photo, Avatar, Document）</param>
        /// <returns>檔案附件資訊</returns>
        [HttpPost("upload")]
        public async Task<ActionResult<FileAttachmentDto>> UploadFile(
            [FromForm] IFormFile file,
            [FromForm] string entityType,
            [FromForm] long entityId,
            [FromForm] string attachmentType = "Photo")
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest(new { message = "No file provided" });

                if (string.IsNullOrWhiteSpace(entityType))
                    return BadRequest(new { message = "EntityType is required" });

                if (entityId <= 0)
                    return BadRequest(new { message = "Invalid EntityId" });

                var result = await _fileService.SaveFileAsync(file, entityType, entityId, attachmentType);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error uploading file");
                return StatusCode(500, new { message = "Upload failed", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得特定實體的所有檔案
        /// </summary>
        /// <param name="entityType">實體類型</param>
        /// <param name="entityId">實體ID</param>
        /// <param name="attachmentType">附件類型（可選）</param>
        /// <returns>檔案附件列表</returns>
        [HttpGet("{entityType}/{entityId}")]
        public async Task<ActionResult<List<FileAttachmentDto>>> GetEntityFiles(
            string entityType,
            long entityId,
            [FromQuery] string? attachmentType = null)
        {
            try
            {
                var files = await _fileService.GetEntityFilesAsync(entityType, entityId, attachmentType);
                return Ok(files);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting files for {entityType}/{entityId}");
                return StatusCode(500, new { message = "Failed to get files", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得特定檔案資訊
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>檔案附件資訊</returns>
        [HttpGet("{fileId:long}")]
        public async Task<ActionResult<FileAttachmentDto>> GetFile(long fileId)
        {
            try
            {
                var file = await _fileService.GetFileAsync(fileId);
                if (file == null)
                    return NotFound(new { message = "File not found" });

                return Ok(file);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error getting file {fileId}");
                return StatusCode(500, new { message = "Failed to get file", detail = ex.Message });
            }
        }

        /// <summary>
        /// 刪除檔案（軟刪除）
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{fileId}")]
        public async Task<IActionResult> DeleteFile(long fileId)
        {
            try
            {
                var success = await _fileService.DeleteFileAsync(fileId);
                if (!success)
                    return NotFound(new { message = "File not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting file {fileId}");
                return StatusCode(500, new { message = "Failed to delete file", detail = ex.Message });
            }
        }

        /// <summary>
        /// 永久刪除檔案（包含實體檔案）
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{fileId}/permanent")]
        public async Task<IActionResult> PermanentlyDeleteFile(long fileId)
        {
            try
            {
                var success = await _fileService.PermanentlyDeleteFileAsync(fileId);
                if (!success)
                    return NotFound(new { message = "File not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error permanently deleting file {fileId}");
                return StatusCode(500, new { message = "Failed to permanently delete file", detail = ex.Message });
            }
        }

        /// <summary>
        /// 更新檔案顯示順序
        /// </summary>
        /// <param name="fileId">檔案ID</param>
        /// <param name="displayOrder">新的顯示順序</param>
        /// <returns>操作結果</returns>
        [HttpPut("{fileId}/order")]
        public async Task<IActionResult> UpdateDisplayOrder(long fileId, [FromBody] int displayOrder)
        {
            try
            {
                var success = await _fileService.UpdateDisplayOrderAsync(fileId, displayOrder);
                if (!success)
                    return NotFound(new { message = "File not found" });

                return NoContent();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating display order for file {fileId}");
                return StatusCode(500, new { message = "Failed to update display order", detail = ex.Message });
            }
        }
    }
}
