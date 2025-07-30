using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 服務類型管理API控制器 - 提供服務類型判斷與複合扣除邏輯
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class ServiceTypeController : ControllerBase
    {
        private readonly IServiceTypeService _serviceTypeService;

        public ServiceTypeController(IServiceTypeService serviceTypeService)
        {
            _serviceTypeService = serviceTypeService;
        }

        /// <summary>
        /// 判斷服務類型（洗澡/美容/混合）
        /// </summary>
        /// <param name="serviceIds">服務項目ID列表</param>
        /// <returns>服務類型判斷結果</returns>
        [HttpPost("determine", Name = nameof(DetermineServiceType))]
        public async Task<ActionResult<ServiceTypeResultDto>> DetermineServiceType([FromBody] List<long> serviceIds)
        {
            var result = await _serviceTypeService.DetermineServiceTypeAsync(serviceIds);
            return Ok(result);
        }

        /// <summary>
        /// 計算包月扣除次數（美容服務的複合扣除邏輯）
        /// </summary>
        /// <param name="serviceType">服務類型</param>
        /// <param name="serviceIds">服務項目ID列表</param>
        /// <returns>扣除次數</returns>
        [HttpPost("calculate-deduction", Name = nameof(CalculateDeductionCount))]
        public async Task<ActionResult<int>> CalculateDeductionCount([FromBody] DeductionCalculationRequest request)
        {
            var count = await _serviceTypeService.CalculateDeductionCountAsync(request.ServiceType, request.ServiceIds);
            return Ok(count);
        }

        /// <summary>
        /// 驗證服務類型與包月類型的相容性
        /// </summary>
        /// <param name="subscriptionType">包月類型</param>
        /// <param name="serviceType">服務類型</param>
        /// <returns>是否相容</returns>
        [HttpGet("validate-compatibility", Name = nameof(ValidateCompatibility))]
        public async Task<ActionResult<bool>> ValidateCompatibility([FromQuery] string subscriptionType, [FromQuery] string serviceType)
        {
            var compatible = await _serviceTypeService.ValidateCompatibilityAsync(subscriptionType, serviceType);
            return Ok(compatible);
        }
    }

    /// <summary>
    /// 扣除次數計算請求資料類別
    /// </summary>
    public class DeductionCalculationRequest
    {
        public string ServiceType { get; set; }
        public List<long> ServiceIds { get; set; } = new List<long>();
    }
}
