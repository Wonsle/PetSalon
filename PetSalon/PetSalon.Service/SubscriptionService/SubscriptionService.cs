using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class SubscriptionService : ISubscriptionService
    {
        private readonly PetSalonContext _context;

        public SubscriptionService(PetSalonContext context)
        {
            _context = context;
        }

        public async Task<IList<Subscription>> GetSubscriptionList()
        {
            return await _context.Subscription
                .Include(s => s.Pet)
                .OrderByDescending(s => s.CreateTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<Subscription>> GetSubscriptionsByPet(long petId)
        {
            return await _context.Subscription
                .Where(s => s.PetId == petId)
                .OrderByDescending(s => s.StartDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<Subscription> GetSubscription(long subscriptionId)
        {
            return await _context.Subscription
                .Include(s => s.Pet)
                .Include(s => s.ReserveRecord)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);
        }

        public async Task<long> CreateSubscription(SubscriptionCreateDto subscriptionDto)
        {
            var subscription = new Subscription
            {
                PetId = subscriptionDto.PetId,
                StartDate = subscriptionDto.StartDate,
                EndDate = subscriptionDto.EndDate,
                SubscriptionDate = subscriptionDto.SubscriptionDate,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.Subscription.Add(subscription);
            await _context.SaveChangesAsync();
            return subscription.SubscriptionId;
        }

        public async Task UpdateSubscription(SubscriptionUpdateDto subscriptionDto)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionDto.SubscriptionId);
            if (subscription == null) return;

            if (subscriptionDto.StartDate.HasValue)
                subscription.StartDate = subscriptionDto.StartDate.Value;
            if (subscriptionDto.EndDate.HasValue)
                subscription.EndDate = subscriptionDto.EndDate.Value;

            subscription.ModifyUser = "SYSTEM"; // TODO: Get from current user context
            subscription.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteSubscription(long subscriptionId)
        {
            var subscription = new Subscription() { SubscriptionId = subscriptionId };
            _context.Subscription.Attach(subscription);
            _context.Subscription.Remove(subscription);
            await _context.SaveChangesAsync();
        }

        public async Task<Subscription> GetActiveSubscription(long petId, DateTime checkDate)
        {
            return await _context.Subscription
                .Where(s => s.PetId == petId
                    && s.StartDate <= checkDate
                    && s.EndDate >= checkDate)
                .OrderByDescending(s => s.StartDate)
                .FirstOrDefaultAsync();
        }

        public async Task<bool> IsSubscriptionValid(long subscriptionId, DateTime checkDate)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return false;

            // Check date range
            if (checkDate < subscription.StartDate || checkDate > subscription.EndDate)
                return false;

            return true;
        }

        public async Task<SubscriptionUsageDto> GetSubscriptionUsage(long subscriptionId)
        {
            var subscription = await _context.Subscription
                .Include(s => s.Pet)
                .Include(s => s.ReserveRecord)
                .FirstOrDefaultAsync(s => s.SubscriptionId == subscriptionId);

            if (subscription == null) return null;

            var usedCount = subscription.ReserveRecord?.Count ?? 0;
            var usageDates = subscription.ReserveRecord?.Select(r => r.ReserverDate).ToList() ?? new List<DateTime>();

            return new SubscriptionUsageDto
            {
                SubscriptionId = subscription.SubscriptionId,
                PetName = subscription.Pet?.PetName ?? "",
                StartDate = subscription.StartDate,
                EndDate = subscription.EndDate,
                TotalUsageLimit = 0, // TODO: Add usage limit field to Subscription table
                UsedCount = usedCount,
                UsageDates = usageDates
            };
        }

        public async Task<int> GetRemainingUsage(long subscriptionId)
        {
            var usage = await GetSubscriptionUsage(subscriptionId);
            return usage?.RemainingUsage ?? 0;
        }

        public async Task UseSubscription(long subscriptionId, long reservationId)
        {
            // This will be handled by the reservation service
            // when creating a reservation that uses a subscription
        }

        public async Task UpdateSubscriptionStatus(long subscriptionId, string status)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return;

            // Note: Add Status field to Subscription table in future migration
            subscription.ModifyUser = "SYSTEM";
            subscription.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<IList<Subscription>> GetExpiringSubscriptions(int daysBeforeExpiry = 7)
        {
            var cutoffDate = DateTime.Now.AddDays(daysBeforeExpiry);

            return await _context.Subscription
                .Include(s => s.Pet)
                .Where(s => s.EndDate <= cutoffDate && s.EndDate >= DateTime.Now)
                .OrderBy(s => s.EndDate)
                .AsNoTracking()
                .ToListAsync();
        }

        /// <summary>
        /// 預留包月次數（建立預約時呼叫，僅預留不扣除）
        /// </summary>
        public async Task<bool> ReserveUsageAsync(long subscriptionId, int count = 1)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return false;
            if (subscription.Status != "ACTIVE") return false;
            if (subscription.TotalUsageLimit > 0 && (subscription.UsedCount + subscription.ReservedCount + count) > subscription.TotalUsageLimit)
                return false;
            subscription.ReservedCount += count;
            subscription.ModifyUser = "SYSTEM";
            subscription.ModifyTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 釋放預留包月次數（預約取消/失敗時呼叫）
        /// </summary>
        public async Task<bool> ReleaseUsageAsync(long subscriptionId, int count = 1)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return false;
            if (subscription.ReservedCount < count) return false;
            subscription.ReservedCount -= count;
            subscription.ModifyUser = "SYSTEM";
            subscription.ModifyTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 確認包月次數（預約完成時正式扣除）
        /// </summary>
        public async Task<bool> ConfirmUsageAsync(long subscriptionId, int count = 1)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return false;
            if (subscription.ReservedCount < count) return false;
            subscription.ReservedCount -= count;
            subscription.UsedCount += count;
            subscription.ModifyUser = "SYSTEM";
            subscription.ModifyTime = DateTime.Now;
            await _context.SaveChangesAsync();
            return true;
        }

        /// <summary>
        /// 檢查包月可用性（次數、狀態、有效期）
        /// </summary>
        public async Task<bool> CheckAvailabilityAsync(long subscriptionId, int count = 1)
        {
            var subscription = await _context.Subscription.FindAsync(subscriptionId);
            if (subscription == null) return false;
            if (subscription.Status != "ACTIVE") return false;
            var now = DateTime.Now;
            if (now < subscription.StartDate || now > subscription.EndDate) return false;
            if (subscription.TotalUsageLimit > 0 && (subscription.UsedCount + subscription.ReservedCount + count) > subscription.TotalUsageLimit)
                return false;
            return true;
        }

        /// <summary>
        /// 自動更新包月狀態（如到期、用完自動轉狀態）
        /// </summary>
        public async Task AutoUpdateStatusAsync()
        {
            var now = DateTime.Now;
            var subs = await _context.Subscription.ToListAsync();
            foreach (var s in subs)
            {
                var newStatus = s.Status;
                if (now > s.EndDate)
                    newStatus = "EXPIRED";
                else if (s.TotalUsageLimit > 0 && s.UsedCount >= s.TotalUsageLimit)
                    newStatus = "EXPIRED";
                if (newStatus != s.Status)
                {
                    s.Status = newStatus;
                    s.ModifyUser = "SYSTEM";
                    s.ModifyTime = now;
                }
            }
            await _context.SaveChangesAsync();
        }
    }
}