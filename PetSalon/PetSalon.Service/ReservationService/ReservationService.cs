using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
using System.Linq;

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

        public async Task<ReserveRecord> CreateReservationAsync(ReservationCreateDto reservationDto)
        {
            // Check if using subscription
            Subscription subscription = null;
            if (reservationDto.UseSubscription)
            {
                subscription = await _subscriptionService.GetActiveSubscription(
                    reservationDto.PetId, reservationDto.ReservationDate);
                if (subscription == null)
                    throw new InvalidOperationException("No active subscription found for this pet");
                if (!await _subscriptionService.CheckAvailabilityAsync(subscription.SubscriptionId, 1))
                    throw new InvalidOperationException("Subscription is not available for this date or usage limit exceeded");
                // 預留包月次數
                var reserved = await _subscriptionService.ReserveUsageAsync(subscription.SubscriptionId, 1);
                if (!reserved)
                    throw new InvalidOperationException("Failed to reserve subscription usage");
            }

            // 計算服務類型和總金額
            string serviceType = "MIXED";
            decimal totalAmount = 0;
            int deductionCount = 1;
            
            if (reservationDto.ServiceIds?.Count > 0 || reservationDto.AddonIds?.Count > 0)
            {
                var calculation = await CalculateReservationCost(
                    reservationDto.PetId, 
                    reservationDto.ServiceIds ?? new List<long>(), 
                    reservationDto.AddonIds ?? new List<long>()
                );
                
                serviceType = DetermineServiceType(reservationDto.ServiceIds ?? new List<long>());
                totalAmount = calculation.TotalAmount;
                deductionCount = CalculateDeductionCount(serviceType);
            }

            var reservation = new ReserveRecord
            {
                PetId = reservationDto.PetId,
                ReserverDate = reservationDto.ReservationDate,
                ReserverTime = reservationDto.ReservationTime,
                Status = reservationDto.Status ?? "PENDING",
                TotalAmount = reservationDto.UseSubscription ? 0 : totalAmount,
                UseSubscription = reservationDto.UseSubscription,
                ServiceType = serviceType,
                SubscriptionDeductionCount = reservationDto.UseSubscription ? deductionCount : 0,
                Memo = reservationDto.Memo ?? "",
                SubscriptionId = subscription?.SubscriptionId,
                CreateUser = "SYSTEM", // TODO: Get from current user context
                ModifyUser = "SYSTEM",
                CreateTime = DateTime.Now,
                ModifyTime = DateTime.Now
            };

            _context.ReserveRecord.Add(reservation);
            await _context.SaveChangesAsync();

            // TODO: 未來可擴展 - 將服務項目詳細記錄到 ReservationService 和 ReservationAddon 表
            // 目前先記錄在 ReserveRecord 的 ServiceType 和相關欄位中

            return reservation;
        }

        public async Task<ReserveRecord?> UpdateReservationAsync(long id, ReservationUpdateDto reservationDto)
        {
            var reservation = await _context.ReserveRecord.FindAsync(id);
            if (reservation == null) return null;

            if (reservationDto.ReservationDate.HasValue)
                reservation.ReserverDate = reservationDto.ReservationDate.Value;

            if (reservationDto.ReservationTime.HasValue)
                reservation.ReserverTime = reservationDto.ReservationTime.Value;

            if (!string.IsNullOrEmpty(reservationDto.Status))
                reservation.Status = reservationDto.Status;

            if (!string.IsNullOrEmpty(reservationDto.Memo))
                reservation.Memo = reservationDto.Memo;

            // 重新計算服務項目和費用（如果有變更）
            if (reservationDto.ServiceIds?.Any() == true || reservationDto.AddonIds?.Any() == true)
            {
                var calculation = await CalculateReservationCost(
                    reservation.PetId,
                    reservationDto.ServiceIds ?? new List<long>(),
                    reservationDto.AddonIds ?? new List<long>()
                );
                
                reservation.ServiceType = DetermineServiceType(reservationDto.ServiceIds ?? new List<long>());
                reservation.TotalAmount = reservation.UseSubscription ? 0 : calculation.TotalAmount;
                reservation.SubscriptionDeductionCount = reservation.UseSubscription ? CalculateDeductionCount(reservation.ServiceType) : 0;
            }

            reservation.ModifyUser = "SYSTEM";
            reservation.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
            return reservation;
        }

        public async Task<bool> DeleteReservationAsync(long reservationId)
        {
            var reservation = await _context.ReserveRecord.FindAsync(reservationId);
            if (reservation == null) return false;
            
            // 若有預留包月次數，釋放
            if (reservation.UseSubscription && reservation.SubscriptionId.HasValue)
            {
                await _subscriptionService.ReleaseUsageAsync(reservation.SubscriptionId.Value, reservation.SubscriptionDeductionCount);
            }
            
            _context.ReserveRecord.Remove(reservation);
            await _context.SaveChangesAsync();
            return true;
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

        public async Task<ReserveRecord?> UpdateReservationStatusAsync(long reservationId, string status)
        {
            var reservation = await _context.ReserveRecord.FindAsync(reservationId);
            if (reservation == null) return null;

            // 狀態流轉處理
            var prevStatus = reservation.Status;
            reservation.Status = status;
            reservation.ModifyUser = "SYSTEM";
            reservation.ModifyTime = DateTime.Now;
            await _context.SaveChangesAsync();

            // 狀態變更後處理包月次數流轉
            if (reservation.UseSubscription && reservation.SubscriptionId.HasValue && reservation.SubscriptionDeductionCount > 0)
            {
                // 預約完成時正式扣除包月次數
                if (prevStatus != "COMPLETED" && status == "COMPLETED")
                {
                    await _subscriptionService.ConfirmUsageAsync(reservation.SubscriptionId.Value, reservation.SubscriptionDeductionCount);
                }
                // 預約取消時釋放預留次數
                if (prevStatus != "CANCELLED" && status == "CANCELLED")
                {
                    await _subscriptionService.ReleaseUsageAsync(reservation.SubscriptionId.Value, reservation.SubscriptionDeductionCount);
                }
            }
            
            return reservation;
        }

        public async Task<ServiceCalculationDto> CalculateReservationCost(long petId, List<long> serviceIds, List<long> addonIds)
        {
            // 基本費用計算（暫時使用固定價格，未來可從 Service 和 ServiceAddon 表取得）
            decimal serviceTotal = 0;
            decimal addonTotal = 0;
            
            // 服務項目基本費用（根據 SystemCode 的 ServiceType）
            if (serviceIds?.Count > 0)
            {
                var serviceCodes = await _context.SystemCode
                    .Where(s => s.CodeType == "ServiceType" && serviceIds.Contains(s.CodeId))
                    .ToListAsync();
                    
                foreach (var service in serviceCodes)
                {
                    serviceTotal += GetServiceBasePrice(service.Code);
                }
            }
            
            // 附加服務費用 - 暫時移除，等待 ServiceAddon 表格完成後重新實作
            // TODO: 使用 ServiceAddon 表格查詢附加服務項目和價格
            if (addonIds?.Count > 0)
            {
                // 暫時使用固定價格計算，避免依賴 SystemCode AddonType
                addonTotal = addonIds.Count * 100m; // 每項附加服務 100 元
            }
            
            var discount = 0m;
            var isSubscriptionEligible = await CheckSubscriptionEligibility(petId, DateTime.Now);
            
            // 包月優惠計算
            if (isSubscriptionEligible)
            {
                discount = serviceTotal * 0.1m; // 10% 包月優惠
            }

            var calculation = new ServiceCalculationDto
            {
                PetId = petId,
                ServiceTotal = serviceTotal,
                AddonTotal = addonTotal,
                Discount = discount,
                IsSubscriptionEligible = isSubscriptionEligible
            };

            return calculation;
        }
        
        private decimal GetServiceBasePrice(string serviceType)
        {
            return serviceType switch
            {
                "BATH" => 300m,
                "GROOM" => 800m,
                "NAIL" => 150m,
                "STYLING" => 500m,
                _ => 300m
            };
        }
        
        // GetAddonBasePrice 方法已移除 - 等待 ServiceAddon 表格完成後重新實作
        
        private string DetermineServiceType(List<long> serviceIds)
        {
            if (serviceIds == null || !serviceIds.Any()) return "MIXED";
            
            // 這裡可以根據服務項目組合判斷類型
            // 暫時簡化處理
            if (serviceIds.Count == 1) return "BATH";
            if (serviceIds.Count >= 2) return "GROOM";
            return "MIXED";
        }
        
        private int CalculateDeductionCount(string serviceType)
        {
            return serviceType switch
            {
                "BATH" => 1,
                "GROOM" => 1,
                "MIXED" => 1,
                _ => 1
            };
        }

        public async Task CompleteReservation(long reservationId)
        {
            await UpdateReservationStatusAsync(reservationId, "COMPLETED");
            // TODO: Generate payment record automatically
            // This would integrate with the payment/income system
        }
    }
}