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
        Task CompleteReservation(long reservationId);
    }
}