using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IReservationService
    {
        Task<IList<ReserveRecord>> GetReservationList();
        Task<IList<ReserveRecord>> GetReservationsByDate(DateTime date);
        Task<IList<ReserveRecord>> GetReservationsByPet(long petId);
        Task<ReserveRecord> GetReservation(long reservationId);
        Task<ReserveRecord> CreateReservationAsync(ReservationCreateDto reservation);
        Task<ReserveRecord?> UpdateReservationAsync(long id, ReservationUpdateDto reservation);
        Task<bool> DeleteReservationAsync(long id);
        Task<ReserveRecord?> UpdateReservationStatusAsync(long id, string status);
        Task<bool> CheckSubscriptionEligibility(long petId, DateTime reservationDate);
        Task<Subscription> GetActiveSubscription(long petId, DateTime reservationDate);
        Task<ServiceCalculationDto> CalculateReservationCost(long petId, List<long> serviceIds, List<long> addonIds);
        
        
        /// <summary>
        /// 取得寵物的服務時間設定（用於時程安排）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceIds">服務ID清單</param>
        /// <returns>總服務時間（分鐘）</returns>
        Task<int> CalculateTotalServiceDurationAsync(long petId, List<long> serviceIds);
        
        Task CompleteReservation(long reservationId);
        
        /// <summary>
        /// 取得寵物的完整定價設定概覽（用於管理介面）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>定價概覽資料</returns>
        Task<PetPricingOverviewDto> GetPetPricingOverviewAsync(long petId);
    }
}