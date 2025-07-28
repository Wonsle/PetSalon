using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class ReservationService : IReservationService
    {
        private readonly PetSalonContext _context;
        private readonly ISubscriptionService _subscriptionService;

        public ReservationService(PetSalonContext context, ISubscriptionService subscriptionService)
        {
            _context = context;
            _subscriptionService = subscriptionService;
        }

        public async Task<IList<ReserveRecord>> GetReservationList()
        {
            return await _context.ReserveRecord
                .Include(r => r.Subscription)
                .ThenInclude(s => s.Pet)
                .OrderByDescending(r => r.ReserverDate)
                .ThenByDescending(r => r.ReserverTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<ReserveRecord>> GetReservationsByDate(DateTime date)
        {
            return await _context.ReserveRecord
                .Include(r => r.Subscription)
                .ThenInclude(s => s.Pet)
                .Where(r => r.ReserverDate.Date == date.Date)
                .OrderBy(r => r.ReserverTime)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<IList<ReserveRecord>> GetReservationsByPet(long petId)
        {
            return await _context.ReserveRecord
                .Include(r => r.Subscription)
                .Where(r => r.Subscription.PetId == petId)
                .OrderByDescending(r => r.ReserverDate)
                .AsNoTracking()
                .ToListAsync();
        }

        public async Task<ReserveRecord> GetReservation(long reservationId)
        {
            return await _context.ReserveRecord
                .Include(r => r.Subscription)
                .ThenInclude(s => s.Pet)
                .FirstOrDefaultAsync(r => r.ReserveRecordId == reservationId);
        }

        public async Task<long> CreateReservation(ReservationCreateDto reservationDto)
        {
            // Check if using subscription
            Subscription subscription = null;
            if (reservationDto.UseSubscription)
            {
                subscription = await _subscriptionService.GetActiveSubscription(
                    reservationDto.PetId, reservationDto.ReservationDate);
                
                if (subscription == null)
                {
                    throw new InvalidOperationException("No active subscription found for this pet");
                }

                if (!await _subscriptionService.IsSubscriptionValid(subscription.SubscriptionId, reservationDto.ReservationDate))
                {
                    throw new InvalidOperationException("Subscription is not valid for this date");
                }
            }

            var reservation = new ReserveRecord
            {
                ReserverDate = reservationDto.ReservationDate,
                ReserverTime = reservationDto.ReservationTime,
                Memo = reservationDto.Memo ?? "",
                SubscriptionId = subscription?.SubscriptionId,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM"
            };

            _context.ReserveRecord.Add(reservation);
            await _context.SaveChangesAsync();

            // TODO: Add services and addons to ReservationService and ReservationAddon tables
            // This would require implementing the service assignment logic

            return reservation.ReserveRecordId;
        }

        public async Task UpdateReservation(ReservationUpdateDto reservationDto)
        {
            var reservation = await _context.ReserveRecord.FindAsync(reservationDto.ReservationId);
            if (reservation == null) return;

            if (reservationDto.ReservationDate.HasValue)
                reservation.ReserverDate = reservationDto.ReservationDate.Value;
            
            if (reservationDto.ReservationTime.HasValue)
                reservation.ReserverTime = reservationDto.ReservationTime.Value;

            if (!string.IsNullOrEmpty(reservationDto.Memo))
                reservation.Memo = reservationDto.Memo;

            reservation.ModifyUser = "SYSTEM";
            reservation.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteReservation(long reservationId)
        {
            var reservation = new ReserveRecord() { ReserveRecordId = reservationId };
            _context.ReserveRecord.Attach(reservation);
            _context.ReserveRecord.Remove(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> CheckSubscriptionEligibility(long petId, DateTime reservationDate)
        {
            var subscription = await _subscriptionService.GetActiveSubscription(petId, reservationDate);
            return subscription != null;
        }

        public async Task<Subscription> GetActiveSubscription(long petId, DateTime reservationDate)
        {
            return await _subscriptionService.GetActiveSubscription(petId, reservationDate);
        }

        public async Task UpdateReservationStatus(long reservationId, string status)
        {
            var reservation = await _context.ReserveRecord.FindAsync(reservationId);
            if (reservation == null) return;

            // TODO: Add Status field to ReserveRecord table
            reservation.ModifyUser = "SYSTEM";
            reservation.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task<ServiceCalculationDto> CalculateReservationCost(long petId, List<long> serviceIds, List<long> addonIds)
        {
            // TODO: Implement service cost calculation
            // This would require the Service and ServiceAddon tables to be implemented
            // and integrated with the Pet entity for custom pricing

            var calculation = new ServiceCalculationDto
            {
                PetId = petId,
                // Services and addons would be populated here
                Discount = 0,
                IsSubscriptionEligible = await CheckSubscriptionEligibility(petId, DateTime.Now)
            };

            return calculation;
        }

        public async Task CompleteReservation(long reservationId)
        {
            await UpdateReservationStatus(reservationId, "COMPLETED");
            
            // TODO: Generate payment record automatically
            // This would integrate with the payment/income system
        }
    }
}