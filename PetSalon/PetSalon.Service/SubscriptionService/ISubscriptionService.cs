using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface ISubscriptionService
    {
        Task<IList<Subscription>> GetSubscriptionList();
        Task<IList<Subscription>> GetSubscriptionsByPet(long petId);
        Task<Subscription> GetSubscription(long subscriptionId);
        Task<long> CreateSubscription(SubscriptionCreateDto subscription);
        Task UpdateSubscription(SubscriptionUpdateDto subscription);
        Task DeleteSubscription(long subscriptionId);
        Task<Subscription> GetActiveSubscription(long petId, DateTime checkDate);
        Task<bool> IsSubscriptionValid(long subscriptionId, DateTime checkDate);
        Task<SubscriptionUsageDto> GetSubscriptionUsage(long subscriptionId);
        Task<int> GetRemainingUsage(long subscriptionId);
        Task UseSubscription(long subscriptionId, long reservationId);
        Task<IList<Subscription>> GetExpiringSubscriptions(int daysBeforeExpiry = 7);

        /// <summary>
        /// 預留包月次數（建立預約時呼叫，僅預留不扣除）
        /// </summary>
        Task<bool> ReserveUsageAsync(long subscriptionId, int count = 1);

        /// <summary>
        /// 釋放預留包月次數（預約取消/失敗時呼叫）
        /// </summary>
        Task<bool> ReleaseUsageAsync(long subscriptionId, int count = 1);

        /// <summary>
        /// 確認包月次數（預約完成時正式扣除）
        /// </summary>
        Task<bool> ConfirmUsageAsync(long subscriptionId, int count = 1);

        /// <summary>
        /// 檢查包月可用性（次數、有效期）
        /// </summary>
        Task<bool> CheckAvailabilityAsync(long subscriptionId, int count = 1);

        /// <summary>
        /// 更新訂閱狀態
        /// </summary>
        /// <param name="subscriptionId">訂閱ID</param>
        /// <param name="status">新狀態</param>
        Task UpdateSubscriptionStatus(long subscriptionId, string status);

        /// <summary>
        /// 自動更新訂閱狀態（根據日期自動判斷過期等）
        /// </summary>
        Task AutoUpdateStatusAsync();

    }
}