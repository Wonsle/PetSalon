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
        Task UpdateSubscriptionStatus(long subscriptionId, string status);
        Task<IList<Subscription>> GetExpiringSubscriptions(int daysBeforeExpiry = 7);
    }
}