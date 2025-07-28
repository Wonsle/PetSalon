using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {

        private readonly ICommonService _commonService;
        private readonly PetSalonContext _context;
        
        public CommonController(ICommonService commonService, PetSalonContext context)
        {
            _commonService = commonService;
            _context = context;
        }

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

        [HttpGet("systemcodes/{codeType}/{code}")]
        public async Task<ActionResult<SystemCode>> GetSystemCode(string codeType, string code)
        {
            var systemCode = await _commonService.GetSystemCode(codeType, code);
            if (systemCode == null)
                return NotFound();
            return systemCode;
        }

        [HttpGet("systemcode-types")]
        public async Task<ActionResult<IList<string>>> GetSystemCodeTypes()
        {
            return Ok(await _commonService.GetSystemCodeTypes());
        }

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

        private string GetCurrentUser()
        {
            // Try to get user from JWT claims or return default
            return User?.Identity?.Name ?? "System";
        }

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
    }
}
