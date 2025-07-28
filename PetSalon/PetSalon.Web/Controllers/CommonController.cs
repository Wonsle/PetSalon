using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class CommonController : ControllerBase
    {

        private readonly ICommonService _commonService;
        public CommonController(ICommonService commonService)
        {
            _commonService = commonService;
        }

        [HttpGet("systemcodes/{codeType}", Name = nameof(GetSystemCodeList))]
        public async Task<ActionResult<IList<SystemCode>>> GetSystemCodeList(string codeType)
        {
            return Ok(await _commonService.GetSystemCodeList(codeType));
        }

        [HttpGet("systemcodes/{codeType}/{code}", Name = nameof(GetSystemCode))]
        public async Task<ActionResult<SystemCode>> GetSystemCode(string codeType, string code)
        {
            var systemCode = await _commonService.GetSystemCode(codeType, code);
            if (systemCode == null)
                return NotFound();
            return systemCode;
        }

        [HttpGet("systemcode-types", Name = nameof(GetSystemCodeTypes))]
        public async Task<ActionResult<IList<string>>> GetSystemCodeTypes()
        {
            return Ok(await _commonService.GetSystemCodeTypes());
        }

        [HttpPost("systemcodes", Name = nameof(CreateSystemCode))]
        public async Task<ActionResult<int>> CreateSystemCode(SystemCode systemCode)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var codeId = await _commonService.CreateSystemCode(systemCode);
            return CreatedAtAction(nameof(GetSystemCode), 
                new { codeType = systemCode.CodeType, code = systemCode.Code }, codeId);
        }

        [HttpPut("systemcodes/{codeId}", Name = nameof(UpdateSystemCode))]
        public async Task<IActionResult> UpdateSystemCode(int codeId, SystemCode systemCode)
        {
            if (codeId != systemCode.CodeId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _commonService.UpdateSystemCode(systemCode);
            return NoContent();
        }

        [HttpDelete("systemcodes/{codeId}", Name = nameof(DeleteSystemCode))]
        public async Task<IActionResult> DeleteSystemCode(int codeId)
        {
            await _commonService.DeleteSystemCode(codeId);
            return NoContent();
        }
    }
}
