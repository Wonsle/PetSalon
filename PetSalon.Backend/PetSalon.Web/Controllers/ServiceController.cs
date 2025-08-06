using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Services;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ServiceController : BaseController
    {
        private readonly IServiceService _serviceService;

        public ServiceController(IServiceService serviceService)
        {
            _serviceService = serviceService;
        }

        /// <summary>
        /// 取得所有服務清單
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        public async Task<ActionResult<IList<ServiceDto>>> GetServices()
        {
            try
            {
                var services = await _serviceService.GetServiceListAsync();
                var result = services.Select(s => new ServiceDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    ServiceType = s.ServiceType,
                    BasePrice = s.BasePrice,
                    Duration = s.Duration,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    Sort = s.Sort
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"取得服務清單失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得啟用的服務清單
        /// </summary>
        /// <returns></returns>
        [HttpGet("active")]
        public async Task<ActionResult<IList<ServiceDto>>> GetActiveServices()
        {
            try
            {
                var services = await _serviceService.GetActiveServiceListAsync();
                var result = services.Select(s => new ServiceDto
                {
                    ServiceId = s.ServiceId,
                    ServiceName = s.ServiceName,
                    ServiceType = s.ServiceType,
                    BasePrice = s.BasePrice,
                    Duration = s.Duration,
                    Description = s.Description,
                    IsActive = s.IsActive,
                    Sort = s.Sort
                }).ToList();

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"取得啟用服務清單失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 取得指定服務詳細資訊
        /// </summary>
        /// <param name="id">服務ID</param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<ServiceDto>> GetService(long id)
        {
            try
            {
                var service = await _serviceService.GetServiceAsync(id);
                if (service == null)
                {
                    return NotFound($"找不到 ID = {id} 的服務");
                }

                var result = new ServiceDto
                {
                    ServiceId = service.ServiceId,
                    ServiceName = service.ServiceName,
                    ServiceType = service.ServiceType,
                    BasePrice = service.BasePrice,
                    Duration = service.Duration,
                    Description = service.Description,
                    IsActive = service.IsActive,
                    Sort = service.Sort
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest($"取得服務資料失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 新增服務
        /// </summary>
        /// <param name="serviceDto">服務資料</param>
        /// <returns></returns>
        [HttpPost]
        public async Task<ActionResult<long>> CreateService([FromBody] ServiceDto serviceDto)
        {
            try
            {
                var service = new Service
                {
                    ServiceName = serviceDto.ServiceName,
                    ServiceType = serviceDto.ServiceType,
                    BasePrice = serviceDto.BasePrice,
                    Duration = serviceDto.Duration,
                    Description = serviceDto.Description,
                    IsActive = serviceDto.IsActive,
                    Sort = serviceDto.Sort
                };

                var serviceId = await _serviceService.CreateServiceAsync(service);
                return Ok(serviceId);
            }
            catch (Exception ex)
            {
                return BadRequest($"新增服務失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新服務
        /// </summary>
        /// <param name="id">服務ID</param>
        /// <param name="serviceDto">服務資料</param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<ActionResult> UpdateService(long id, [FromBody] ServiceDto serviceDto)
        {
            try
            {
                var service = new Service
                {
                    ServiceId = id,
                    ServiceName = serviceDto.ServiceName,
                    ServiceType = serviceDto.ServiceType,
                    BasePrice = serviceDto.BasePrice,
                    Duration = serviceDto.Duration,
                    Description = serviceDto.Description,
                    IsActive = serviceDto.IsActive,
                    Sort = serviceDto.Sort
                };

                await _serviceService.UpdateServiceAsync(service);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"更新服務失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 刪除服務
        /// </summary>
        /// <param name="id">服務ID</param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteService(long id)
        {
            try
            {
                await _serviceService.DeleteServiceAsync(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"刪除服務失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 啟用/停用服務
        /// </summary>
        /// <param name="id">服務ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns></returns>
        [HttpPut("{id}/status")]
        public async Task<ActionResult> ToggleServiceStatus(long id, [FromBody] bool isActive)
        {
            try
            {
                await _serviceService.ToggleServiceStatusAsync(id, isActive);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"更新服務狀態失敗: {ex.Message}");
            }
        }

        /// <summary>
        /// 更新服務排序
        /// </summary>
        /// <param name="id">服務ID</param>
        /// <param name="newSort">新的排序值</param>
        /// <returns></returns>
        [HttpPut("{id}/sort")]
        public async Task<ActionResult> UpdateServiceSort(long id, [FromBody] int newSort)
        {
            try
            {
                await _serviceService.UpdateServiceSortAsync(id, newSort);
                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest($"更新服務排序失敗: {ex.Message}");
            }
        }
    }
}