using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 訂閱服務API控制器 - 提供寵物月包訂閱服務管理功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        /// <summary>
        /// 取得所有訂閱服務列表
        /// </summary>
        /// <returns>訂閱服務列表</returns>
        [HttpGet(Name = nameof(GetSubscriptionList))]
        public async Task<ActionResult<IList<Subscription>>> GetSubscriptionList()
        {
            return Ok(await _subscriptionService.GetSubscriptionList());
        }

        /// <summary>
        /// 根據ID取得特定訂閱服務
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <returns>訂閱服務詳細資訊</returns>
        [HttpGet("{subscriptionId}", Name = nameof(GetSubscription))]
        public async Task<ActionResult<Subscription>> GetSubscription(long subscriptionId)
        {
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);
            if (subscription == null)
                return NotFound();
            return subscription;
        }

        /// <summary>
        /// 根據寵物ID取得訂閱服務列表
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>指定寵物的訂閱服務列表</returns>
        [HttpGet("pet/{petId}", Name = nameof(GetSubscriptionsByPet))]
        public async Task<ActionResult<IList<Subscription>>> GetSubscriptionsByPet(long petId)
        {
            return Ok(await _subscriptionService.GetSubscriptionsByPet(petId));
        }

        /// <summary>
        /// 建立新訂閱服務
        /// </summary>
        /// <param name="subscription">訂閱服務建立資料</param>
        /// <returns>新建立訂閱服務的ID</returns>
        [HttpPost(Name = nameof(CreateSubscription))]
        public async Task<ActionResult<long>> CreateSubscription(SubscriptionCreateDto subscription)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscriptionId = await _subscriptionService.CreateSubscription(subscription);
            return CreatedAtAction(nameof(GetSubscription), 
                new { subscriptionId = subscriptionId }, subscriptionId);
        }

        /// <summary>
        /// 更新訂閱服務資訊
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <param name="subscription">訂閱服務更新資料</param>
        /// <returns>操作結果</returns>
        [HttpPut("{subscriptionId}", Name = nameof(UpdateSubscription))]
        public async Task<IActionResult> UpdateSubscription(long subscriptionId, SubscriptionUpdateDto subscription)
        {
            if (subscriptionId != subscription.SubscriptionId)
                return BadRequest();

            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            await _subscriptionService.UpdateSubscription(subscription);
            return NoContent();
        }

        /// <summary>
        /// 刪除訂閱服務
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <returns>操作結果</returns>
        [HttpDelete("{subscriptionId}", Name = nameof(DeleteSubscription))]
        public async Task<IActionResult> DeleteSubscription(long subscriptionId)
        {
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);
            if (subscription == null)
                return NotFound();

            await _subscriptionService.DeleteSubscription(subscriptionId);
            return NoContent();
        }

        /// <summary>
        /// 取得訂閱服務使用狀況
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <returns>訂閱服務使用明細</returns>
        [HttpGet("{subscriptionId}/usage", Name = nameof(GetSubscriptionUsage))]
        public async Task<ActionResult<SubscriptionUsageDto>> GetSubscriptionUsage(long subscriptionId)
        {
            var usage = await _subscriptionService.GetSubscriptionUsage(subscriptionId);
            if (usage == null)
                return NotFound();
            return usage;
        }

        /// <summary>
        /// 取得訂閱服務剩餘使用次数
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <returns>剩餘使用次数</returns>
        [HttpGet("{subscriptionId}/remaining", Name = nameof(GetRemainingUsage))]
        public async Task<ActionResult<int>> GetRemainingUsage(long subscriptionId)
        {
            var remaining = await _subscriptionService.GetRemainingUsage(subscriptionId);
            return Ok(remaining);
        }

        /// <summary>
        /// 取得寵物目前有效的訂閱服務
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="checkDate">檢查日期（預設為目前日期）</param>
        /// <returns>有效的訂閱服務</returns>
        [HttpGet("pet/{petId}/active", Name = nameof(GetActiveSubscription))]
        public async Task<ActionResult<Subscription>> GetActiveSubscription(long petId, [FromQuery] DateTime? checkDate = null)
        {
            var date = checkDate ?? DateTime.Now;
            var subscription = await _subscriptionService.GetActiveSubscription(petId, date);
            if (subscription == null)
                return NotFound("No active subscription found");
            return subscription;
        }

        /// <summary>
        /// 取得即將過期的訂閱服務
        /// </summary>
        /// <param name="days">預警天數（預設7天）</param>
        /// <returns>即將過期的訂閱服務列表</returns>
        [HttpGet("expiring", Name = nameof(GetExpiringSubscriptions))]
        public async Task<ActionResult<IList<Subscription>>> GetExpiringSubscriptions([FromQuery] int days = 7)
        {
            var expiring = await _subscriptionService.GetExpiringSubscriptions(days);
            return Ok(expiring);
        }

        /// <summary>
        /// 更新訂閱服務狀態
        /// </summary>
        /// <param name="subscriptionId">訂閱服務ID</param>
        /// <param name="status">新狀態</param>
        /// <returns>操作結果</returns>
        [HttpPost("{subscriptionId}/status", Name = nameof(UpdateSubscriptionStatus))]
        public async Task<IActionResult> UpdateSubscriptionStatus(long subscriptionId, [FromBody] string status)
        {
            await _subscriptionService.UpdateSubscriptionStatus(subscriptionId, status);
            return NoContent();
        }
    }
}