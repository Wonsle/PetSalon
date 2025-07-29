using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 公用API控制器 - 提供系統代碼管理和檔案上傳功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {

        private readonly ICommonService _commonService;
        private readonly PetSalonContext _context;
        private readonly FileUploadSettings _fileUploadSettings;
        
        public CommonController(ICommonService commonService, PetSalonContext context, IOptions<FileUploadSettings> fileUploadSettings)
        {
            _commonService = commonService;
            _context = context;
            _fileUploadSettings = fileUploadSettings.Value;
        }

        /// <summary>
        /// 取得系統代碼列表（可依類型篩選）
        /// </summary>
        /// <param name="type">代碼類型（可選）</param>
        /// <returns>系統代碼列表</returns>
        [HttpGet("systemcodes/list")]
        public async Task<ActionResult<IList<SystemCodeDto>>> GetSystemCodes([FromQuery] string? type = null)
        {
            try
            {
                if (string.IsNullOrEmpty(type))
                {
                    // Return all system codes grouped by type
                    var allCodes = await _context.SystemCode
                        .Where(x => x.EndDate == null || x.EndDate > DateTime.Now)
                        .OrderBy(x => x.CodeType).ThenBy(x => x.Sort)
                        .ToListAsync();
                        
                    var dtos = allCodes.Select(SystemCodeDto.FromEntity).ToList();
                    return Ok(dtos);
                }
                else
                {
                    var codes = await _commonService.GetSystemCodeList(type);
                    var dtos = codes.Select(SystemCodeDto.FromEntity).ToList();
                    return Ok(dtos);
                }
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// 根據代碼類型取得系統代碼
        /// </summary>
        /// <param name="codeType">代碼類型</param>
        /// <returns>指定類型的系統代碼列表</returns>
        [HttpGet("systemcodes/{codeType}")]
        public async Task<ActionResult<IList<SystemCodeDto>>> GetSystemCodesByType(string codeType)
        {
            try
            {
                var codes = await _commonService.GetSystemCodeList(codeType);
                var dtos = codes.Select(SystemCodeDto.FromEntity).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Internal server error", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得特定系統代碼
        /// </summary>
        /// <param name="codeType">代碼類型</param>
        /// <param name="code">代碼值</param>
        /// <returns>特定系統代碼</returns>
        [HttpGet("systemcodes/{codeType}/{code}")]
        public async Task<ActionResult<SystemCode>> GetSystemCode(string codeType, string code)
        {
            var systemCode = await _commonService.GetSystemCode(codeType, code);
            if (systemCode == null)
                return NotFound();
            return systemCode;
        }

        /// <summary>
        /// 取得所有系統代碼類型
        /// </summary>
        /// <returns>系統代碼類型列表</returns>
        [HttpGet("systemcode-types")]
        public async Task<ActionResult<IList<string>>> GetSystemCodeTypes()
        {
            return Ok(await _commonService.GetSystemCodeTypes());
        }

        /// <summary>
        /// 建立新系統代碼
        /// </summary>
        /// <param name="systemCodeDto">系統代碼資料</param>
        /// <returns>新建立系統代碼</returns>
        [HttpPost("systemcodes")]
        public async Task<ActionResult<SystemCodeDto>> CreateSystemCode(SystemCodeDto systemCodeDto)
        {
            try
            {
                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Set audit fields
                systemCodeDto.CreateUser = GetCurrentUser();
                systemCodeDto.CreateTime = DateTime.Now;
                systemCodeDto.UpdateUser = GetCurrentUser();
                systemCodeDto.UpdateTime = DateTime.Now;

                var systemCode = systemCodeDto.ToEntity();
                var codeId = await _commonService.CreateSystemCode(systemCode);
                
                systemCodeDto.Id = codeId;
                return Ok(systemCodeDto);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to create system code", detail = ex.Message });
            }
        }

        /// <summary>
        /// 更新系統代碼
        /// </summary>
        /// <param name="codeId">代碼ID</param>
        /// <param name="systemCodeDto">系統代碼資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("systemcodes/{codeId}")]
        public async Task<IActionResult> UpdateSystemCode(int codeId, SystemCodeDto systemCodeDto)
        {
            try
            {
                if (codeId != systemCodeDto.Id)
                    return BadRequest("CodeId mismatch");

                if (!ModelState.IsValid)
                    return BadRequest(ModelState);

                // Set audit fields
                systemCodeDto.UpdateUser = GetCurrentUser();
                systemCodeDto.UpdateTime = DateTime.Now;

                var systemCode = systemCodeDto.ToEntity();
                systemCode.CodeId = codeId; // Ensure the CodeId is set correctly
                await _commonService.UpdateSystemCode(systemCode);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to update system code", detail = ex.Message });
            }
        }

        /// <summary>
        /// 取得目前使用者名稱（從JWT Token中取得）
        /// </summary>
        /// <returns>目前使用者名稱</returns>
        private string GetCurrentUser()
        {
            // Try to get user from JWT claims or return default
            return User?.Identity?.Name ?? "System";
        }

        /// <summary>
        /// 刪除系統代碼
        /// </summary>
        /// <param name="codeId">代碼ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("systemcodes/{codeId}")]
        public async Task<IActionResult> DeleteSystemCode(int codeId)
        {
            try
            {
                await _commonService.DeleteSystemCode(codeId);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "Failed to delete system code", detail = ex.Message });
            }
        }

        /// <summary>
        /// 上傳照片檔案
        /// </summary>
        /// <param name="prefix">檔案存放目錄前綴</param>
        /// <param name="file">照片檔案</param>
        /// <returns>上傳結果和檔案URL</returns>
        [HttpPost("upload-photo")]
        public async Task<IActionResult> UploadPhoto([FromForm] string prefix, [FromForm] IFormFile file)
        {
            try
            {
                if (file == null || file.Length == 0)
                    return BadRequest("No file provided");

                if (string.IsNullOrWhiteSpace(prefix))
                    return BadRequest("Prefix is required");

                // Validate file extension
                var extension = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (!_fileUploadSettings.AllowedExtensions.Contains(extension))
                    return BadRequest($"Invalid file type. Allowed extensions: {string.Join(", ", _fileUploadSettings.AllowedExtensions)}");

                // Validate file size
                var maxFileSize = _fileUploadSettings.MaxFileSizeInMB * 1024 * 1024; // Convert to bytes
                if (file.Length > maxFileSize)
                    return BadRequest($"File size exceeds maximum limit of {_fileUploadSettings.MaxFileSizeInMB}MB");

                // Create upload folder path with prefix
                var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), _fileUploadSettings.BaseUploadPath, prefix);
                Directory.CreateDirectory(uploadsFolder);

                // Generate unique filename
                var fileName = $"{Guid.NewGuid()}{extension}";
                var filePath = Path.Combine(uploadsFolder, fileName);

                // Save file
                using (var stream = new FileStream(filePath, FileMode.Create))
                {
                    await file.CopyToAsync(stream);
                }

                // Return relative URL path
                var relativeUrl = $"/uploads/{prefix}/{fileName}";
                return Ok(new { 
                    success = true, 
                    photoUrl = relativeUrl,
                    fileName = fileName,
                    originalFileName = file.FileName,
                    fileSize = file.Length
                });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = "Upload failed", detail = ex.Message });
            }
        }
    }
}
