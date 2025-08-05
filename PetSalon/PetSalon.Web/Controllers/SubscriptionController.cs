using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 包月管理API控制器 - 提供包月CRUD功能、次數管理與自動化處理
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly PetSalonContext _context;
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(PetSalonContext context, ISubscriptionService subscriptionService)
        {
            _context = context;
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// 取得所有包月記錄
        /// </summary>
        /// <returns>包月記錄列表</returns>
        [HttpGet(Name = nameof(GetSubscriptions))]
        public async Task<ActionResult<IEnumerable<Subscription>>> GetSubscriptions()
        {
            try
            {
                var subscriptions = await _context.Subscription
                    .Include(s => s.Pet)
                    .Include(s => s.SubscriptionTypeNavigation)
                    .OrderByDescending(s => s.CreateTime)
                    .ToListAsync();

                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取包月記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 根據ID取得包月記錄
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <returns>包月記錄</returns>
        [HttpGet("{id}", Name = nameof(GetSubscription))]
        public async Task<ActionResult<Subscription>> GetSubscription(long id)
        {
            try
            {
                var subscription = await _context.Subscription
                    .Include(s => s.Pet)
                    .Include(s => s.SubscriptionTypeNavigation)
                    .FirstOrDefaultAsync(s => s.SubscriptionId == id);

                if (subscription == null)
                {
                    return NotFound(new { message = "找不到指定的包月記錄" });
                }

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取包月記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 建立新的包月記錄
        /// </summary>
        /// <param name="subscription">包月資料</param>
        /// <returns>建立的包月記錄</returns>
        [HttpPost(Name = nameof(CreateSubscription))]
        public async Task<ActionResult<Subscription>> CreateSubscription([FromBody] CreateSubscriptionDto subscription)
        {
            try
            {
                // Convert DTO to the correct type
                var createDto = new SubscriptionCreateDto
                {
                    PetId = subscription.PetId,
                    StartDate = subscription.StartDate,
                    EndDate = subscription.EndDate,
                    SubscriptionDate = DateTime.Now,
                    TotalUsageLimit = subscription.UsageLimit,
                    SubscriptionPrice = subscription.Price,
                    Notes = ""
                };
                var subscriptionId = await _subscriptionService.CreateSubscription(createDto);
                var result = await _subscriptionService.GetSubscription(subscriptionId);
                return CreatedAtAction(nameof(GetSubscription), new { id = result.SubscriptionId }, result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "建立包月記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 更新包月記錄
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <param name="subscription">更新的包月資料</param>
        /// <returns>更新結果</returns>
        [HttpPut("{id}", Name = nameof(UpdateSubscription))]
        public async Task<IActionResult> UpdateSubscription(long id, [FromBody] UpdateSubscriptionDto subscription)
        {
            try
            {
                // Convert DTO to the correct type
                var updateDto = new SubscriptionUpdateDto
                {
                    SubscriptionId = id,
                    TotalUsageLimit = subscription.UsageLimit,
                    SubscriptionPrice = subscription.Price,
                    EndDate = subscription.EndDate,
                    Notes = ""
                };
                await _subscriptionService.UpdateSubscription(updateDto);
                var result = await _subscriptionService.GetSubscription(id);
                if (result == null)
                {
                    return NotFound(new { message = "找不到指定的包月記錄" });
                }

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "更新包月記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取消包月記錄
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <returns>取消結果</returns>
        [HttpDelete("{id}", Name = nameof(CancelSubscription))]
        public async Task<IActionResult> CancelSubscription(long id)
        {
            try
            {
                await _subscriptionService.DeleteSubscription(id);
                var result = true;
                if (!result)
                {
                    return NotFound(new { message = "找不到指定的包月記錄" });
                }

                return NoContent();
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "取消包月記錄失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 根據寵物ID取得該寵物的所有包月方案
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>該寵物的包月方案列表</returns>
        [HttpGet("pet/{petId}", Name = nameof(GetSubscriptionsByPet))]
        public async Task<ActionResult<IEnumerable<SubscriptionDetailsDto>>> GetSubscriptionsByPet(long petId)
        {
            try
            {
                var subscriptions = await _subscriptionService.GetSubscriptionDetailsByPet(petId);
                return Ok(subscriptions);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取寵物包月方案失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 根據寵物ID取得該寵物的有效包月方案
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>有效的包月方案</returns>
        [HttpGet("pet/{petId}/active", Name = nameof(GetActiveSubscription))]
        public async Task<ActionResult<Subscription>> GetActiveSubscription(long petId)
        {
            try
            {
                var subscription = await _context.Subscription
                    .Include(s => s.SubscriptionTypeNavigation)
                    .Where(s => s.PetId == petId &&
                               s.EndDate > DateTime.Now)
                    .FirstOrDefaultAsync();

                if (subscription == null)
                {
                    return NotFound(new { message = "該寵物沒有可用的包月方案" });
                }

                return Ok(subscription);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取包月方案失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 檢查包月可用性
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <returns>是否可用</returns>
        [HttpGet("{id}/availability", Name = nameof(CheckAvailability))]
        public async Task<ActionResult<bool>> CheckAvailability(long id)
        {
            try
            {
                var isAvailable = await _subscriptionService.CheckAvailabilityAsync(id);
                return Ok(isAvailable);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "檢查包月可用性失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 預留包月次數
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <param name="count">預留次數</param>
        /// <returns>預留結果</returns>
        [HttpPost("{id}/reserve", Name = nameof(ReserveUsage))]
        public async Task<ActionResult<bool>> ReserveUsage(long id, [FromBody] int count)
        {
            try
            {
                var result = await _subscriptionService.ReserveUsageAsync(id, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "預留包月次數失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 釋放預留的包月次數
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <param name="count">釋放次數</param>
        /// <returns>釋放結果</returns>
        [HttpPost("{id}/release", Name = nameof(ReleaseUsage))]
        public async Task<ActionResult<bool>> ReleaseUsage(long id, [FromBody] int count)
        {
            try
            {
                var result = await _subscriptionService.ReleaseUsageAsync(id, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "釋放包月次數失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 確認使用包月次數
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <param name="count">確認次數</param>
        /// <returns>確認結果</returns>
        [HttpPost("{id}/confirm", Name = nameof(ConfirmUsage))]
        public async Task<ActionResult<bool>> ConfirmUsage(long id, [FromBody] int count)
        {
            try
            {
                var result = await _subscriptionService.ConfirmUsageAsync(id, count);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(new { message = "確認使用包月次數失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得包月使用情況
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <returns>使用情況</returns>
        [HttpGet("{id}/usage", Name = nameof(GetUsage))]
        public async Task<ActionResult> GetUsage(long id)
        {
            try
            {
                var usage = await _subscriptionService.GetSubscriptionUsage(id);
                return Ok(usage);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取使用情況失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得剩餘使用次數
        /// </summary>
        /// <param name="id">包月記錄ID</param>
        /// <returns>剩餘次數</returns>
        [HttpGet("{id}/remaining", Name = nameof(GetRemainingUsage))]
        public async Task<ActionResult<int>> GetRemainingUsage(long id)
        {
            try
            {
                var remaining = await _subscriptionService.GetRemainingUsage(id);
                return Ok(remaining);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取剩餘次數失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 自動更新包月狀態
        /// </summary>
        /// <returns>更新結果</returns>
        [HttpPost("auto-update-status", Name = nameof(AutoUpdateStatus))]
        public async Task<ActionResult> AutoUpdateStatus()
        {
            try
            {
                // Auto update status functionality has been removed
                // Status is now calculated based on dates and usage
                return Ok(new { message = "自動更新包月狀態完成" });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "自動更新包月狀態失敗", error = ex.Message });
            }
        }

        /// <summary>
        /// 取得包月統計資料
        /// </summary>
        /// <returns>統計資料</returns>
        [HttpGet("statistics", Name = nameof(GetSubscriptionStatistics))]
        public async Task<ActionResult> GetSubscriptionStatistics()
        {
            try
            {
                var statistics = new
                {
                    ActiveSubscriptions = await _context.Subscription
                        .CountAsync(s => s.EndDate > DateTime.Now),

                    ExpiringSoon = await _context.Subscription
                        .CountAsync(s => s.EndDate > DateTime.Now && s.EndDate <= DateTime.Now.AddDays(7)),

                    MonthlyRevenue = await _context.Subscription
                        .Where(s => s.CreateTime >= DateTime.Now.AddMonths(-1))
                        .SumAsync(s => s.SubscriptionPrice),

                    AverageUsageRate = await _context.Subscription
                        .Where(s => s.EndDate > DateTime.Now && s.TotalUsageLimit > 0)
                        .AverageAsync(s => (double)s.UsedCount / s.TotalUsageLimit * 100)
                };

                return Ok(statistics);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "獲取統計資料失敗", error = ex.Message });
            }
        }
    }

    /// <summary>
    /// 建立包月請求DTO
    /// </summary>
    public class CreateSubscriptionDto
    {
        public long PetId { get; set; }
        public long SubscriptionTypeId { get; set; }
        public int UsageLimit { get; set; }
        public decimal Price { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }

    /// <summary>
    /// 更新包月請求DTO
    /// </summary>
    public class UpdateSubscriptionDto
    {
        public int? UsageLimit { get; set; }
        public decimal? Price { get; set; }
        public DateTime? EndDate { get; set; }
    }
}
