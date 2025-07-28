using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using PetSalon.Services;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class SubscriptionController : ControllerBase
    {
        private readonly ISubscriptionService _subscriptionService;

        public SubscriptionController(ISubscriptionService subscriptionService)
        {
            _subscriptionService = subscriptionService;
        }

        [HttpGet(Name = nameof(GetSubscriptionList))]
        public async Task<ActionResult<IList<Subscription>>> GetSubscriptionList()
        {
            return Ok(await _subscriptionService.GetSubscriptionList());
        }

        [HttpGet("{subscriptionId}", Name = nameof(GetSubscription))]
        public async Task<ActionResult<Subscription>> GetSubscription(long subscriptionId)
        {
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);
            if (subscription == null)
                return NotFound();
            return subscription;
        }

        [HttpGet("pet/{petId}", Name = nameof(GetSubscriptionsByPet))]
        public async Task<ActionResult<IList<Subscription>>> GetSubscriptionsByPet(long petId)
        {
            return Ok(await _subscriptionService.GetSubscriptionsByPet(petId));
        }

        [HttpPost(Name = nameof(CreateSubscription))]
        public async Task<ActionResult<long>> CreateSubscription(SubscriptionCreateDto subscription)
        {
            if (!ModelState.IsValid)
                return BadRequest(ModelState);

            var subscriptionId = await _subscriptionService.CreateSubscription(subscription);
            return CreatedAtAction(nameof(GetSubscription), 
                new { subscriptionId = subscriptionId }, subscriptionId);
        }

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

        [HttpDelete("{subscriptionId}", Name = nameof(DeleteSubscription))]
        public async Task<IActionResult> DeleteSubscription(long subscriptionId)
        {
            var subscription = await _subscriptionService.GetSubscription(subscriptionId);
            if (subscription == null)
                return NotFound();

            await _subscriptionService.DeleteSubscription(subscriptionId);
            return NoContent();
        }

        [HttpGet("{subscriptionId}/usage", Name = nameof(GetSubscriptionUsage))]
        public async Task<ActionResult<SubscriptionUsageDto>> GetSubscriptionUsage(long subscriptionId)
        {
            var usage = await _subscriptionService.GetSubscriptionUsage(subscriptionId);
            if (usage == null)
                return NotFound();
            return usage;
        }

        [HttpGet("{subscriptionId}/remaining", Name = nameof(GetRemainingUsage))]
        public async Task<ActionResult<int>> GetRemainingUsage(long subscriptionId)
        {
            var remaining = await _subscriptionService.GetRemainingUsage(subscriptionId);
            return Ok(remaining);
        }

        [HttpGet("pet/{petId}/active", Name = nameof(GetActiveSubscription))]
        public async Task<ActionResult<Subscription>> GetActiveSubscription(long petId, [FromQuery] DateTime? checkDate = null)
        {
            var date = checkDate ?? DateTime.Now;
            var subscription = await _subscriptionService.GetActiveSubscription(petId, date);
            if (subscription == null)
                return NotFound("No active subscription found");
            return subscription;
        }

        [HttpGet("expiring", Name = nameof(GetExpiringSubscriptions))]
        public async Task<ActionResult<IList<Subscription>>> GetExpiringSubscriptions([FromQuery] int days = 7)
        {
            var expiring = await _subscriptionService.GetExpiringSubscriptions(days);
            return Ok(expiring);
        }

        [HttpPost("{subscriptionId}/status", Name = nameof(UpdateSubscriptionStatus))]
        public async Task<IActionResult> UpdateSubscriptionStatus(long subscriptionId, [FromBody] string status)
        {
            await _subscriptionService.UpdateSubscriptionStatus(subscriptionId, status);
            return NoContent();
        }
    }
}