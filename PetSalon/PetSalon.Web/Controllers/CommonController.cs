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

        [HttpGet("GetSystemCodeList", Name = nameof(GetSystemCodeList))]
        public async Task<IList<SystemCode>> GetSystemCodeList(string codeType)
        {
            return await _commonService.GetSystemCodeList(codeType);

        }

        [HttpGet("GetSystemCode", Name = nameof(GetSystemCode))]
        public async Task<SystemCode> GetSystemCode(string codeType, string code)
        {
            return await _commonService.GetSystemCode(codeType, code);

        }
    }
}
