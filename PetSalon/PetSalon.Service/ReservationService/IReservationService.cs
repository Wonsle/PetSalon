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
        Task<long> CreateReservation(ReservationCreateDto reservation);
        Task UpdateReservation(ReservationUpdateDto reservation);
        Task DeleteReservation(long reservationId);
        Task<bool> CheckSubscriptionEligibility(long petId, DateTime reservationDate);
        Task<Subscription> GetActiveSubscription(long petId, DateTime reservationDate);
        Task UpdateReservationStatus(long reservationId, string status);
        Task<ServiceCalculationDto> CalculateReservationCost(long petId, List<long> serviceIds, List<long> addonIds);
        Task CompleteReservation(long reservationId);
    }
}