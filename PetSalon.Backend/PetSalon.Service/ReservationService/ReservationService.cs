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
        private readonly IPetServicePriceService _petServicePriceService;

        public ReservationService(
            PetSalonContext context,
            ISubscriptionService subscriptionService,
            IPetServicePriceService petServicePriceService)
        {
            _context = context;
            _subscriptionService = subscriptionService;
            _petServicePriceService = petServicePriceService;
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

            // 計算總金額和扣除次數
            decimal totalAmount = 0;
            int deductionCount = 1;

            if (reservationDto.ServiceIds?.Count > 0 || reservationDto.AddonIds?.Count > 0)
            {
                var calculation = await CalculateReservationCost(
                    reservationDto.PetId,
                    reservationDto.ServiceIds ?? new List<long>(),
                    reservationDto.AddonIds ?? new List<long>()
                );

                totalAmount = calculation.TotalAmount;
                deductionCount = await CalculateDeductionCountAsync(reservationDto.ServiceIds ?? new List<long>());
            }

            var reservation = new ReserveRecord
            {
                PetId = reservationDto.PetId,
                ReserverDate = reservationDto.ReservationDate,
                ReserverTime = reservationDto.ReservationTime,
                Status = reservationDto.Status ?? "PENDING",
                TotalAmount = reservationDto.UseSubscription ? 0 : totalAmount,
                UseSubscription = reservationDto.UseSubscription,
                ServiceDurationMinutes = reservationDto.ServiceDurationMinutes,
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

            // 寫入預約明細記錄
            if (reservationDto.ServiceIds?.Count > 0)
            {
                var services = await _context.Service
                    .Where(s => reservationDto.ServiceIds.Contains(s.ServiceId) && s.IsActive)
                    .ToListAsync();

                foreach (var service in services)
                {
                    // 取得該服務的實際價格（客製化價格或預設價格）
                    var customPrice = await _context.PetServicePrice
                        .Where(psp => psp.PetId == reservationDto.PetId && psp.ServiceId == service.ServiceId && psp.IsActive)
                        .Select(psp => psp.CustomPrice)
                        .FirstOrDefaultAsync();

                    var servicePrice = customPrice ?? service.BasePrice;

                    // 包月不計價
                    if (reservationDto.UseSubscription)
                    {
                        servicePrice = 0;
                    }

                    var detail = new ReserveRecordDetail
                    {
                        ReserveRecordId = reservation.ReserveRecordId,
                        ServiceType = service.ServiceType ?? "GENERAL",
                        Price = servicePrice,
                        CreateUser = "SYSTEM",
                        ModifyUser = "SYSTEM",
                        CreateTime = DateTime.Now,
                        ModifyTime = DateTime.Now
                    };

                    _context.Set<ReserveRecordDetail>().Add(detail);
                }

                await _context.SaveChangesAsync();
            }

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

            if (reservationDto.ServiceDurationMinutes.HasValue)
                reservation.ServiceDurationMinutes = reservationDto.ServiceDurationMinutes.Value;

            // 重新計算服務項目和費用（如果有變更）
            if (reservationDto.ServiceIds?.Any() == true || reservationDto.AddonIds?.Any() == true)
            {
                var calculation = await CalculateReservationCost(
                    reservation.PetId,
                    reservationDto.ServiceIds ?? new List<long>(),
                    reservationDto.AddonIds ?? new List<long>()
                );

                reservation.TotalAmount = reservation.UseSubscription ? 0 : calculation.TotalAmount;
                reservation.SubscriptionDeductionCount = reservation.UseSubscription ? await CalculateDeductionCountAsync(reservationDto.ServiceIds ?? new List<long>()) : 0;

                // 刪除舊的明細記錄
                var existingDetails = await _context.Set<ReserveRecordDetail>()
                    .Where(d => d.ReserveRecordId == id)
                    .ToListAsync();
                _context.Set<ReserveRecordDetail>().RemoveRange(existingDetails);

                // 建立新的明細記錄
                if (reservationDto.ServiceIds?.Count > 0)
                {
                    var services = await _context.Service
                        .Where(s => reservationDto.ServiceIds.Contains(s.ServiceId) && s.IsActive)
                        .ToListAsync();

                    foreach (var service in services)
                    {
                        // 取得該服務的實際價格（客製化價格或預設價格）
                        var customPrice = await _context.PetServicePrice
                            .Where(psp => psp.PetId == reservation.PetId && psp.ServiceId == service.ServiceId && psp.IsActive)
                            .Select(psp => psp.CustomPrice)
                            .FirstOrDefaultAsync();

                        var servicePrice = customPrice ?? service.BasePrice;

                        // 包月不計價
                        if (reservation.UseSubscription)
                        {
                            servicePrice = 0;
                        }

                        var detail = new ReserveRecordDetail
                        {
                            ReserveRecordId = id,
                            ServiceType = service.ServiceType ?? "GENERAL",
                            Price = servicePrice,
                            CreateUser = "SYSTEM",
                            ModifyUser = "SYSTEM",
                            CreateTime = DateTime.Now,
                            ModifyTime = DateTime.Now
                        };

                        _context.Set<ReserveRecordDetail>().Add(detail);
                    }
                }
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
            decimal serviceTotal = 0;
            decimal addonTotal = 0;
            var services = new List<ServiceItemDto>();
            var isSubscriptionEligible = await CheckSubscriptionEligibility(petId, DateTime.Now);

            // 計算服務項目費用
            if (serviceIds?.Count > 0)
            {
                var serviceEntities = await _context.Service
                    .Where(s => serviceIds.Contains(s.ServiceId) && s.IsActive)
                    .ToListAsync();

                foreach (var service in serviceEntities)
                {
                    // 取得有效時長（客製化時長優先）
                    var duration = await _petServicePriceService.GetEffectiveServiceDurationAsync(petId, service.ServiceId);

                    // 服務費用計算邏輯：
                    // 需求1：包月不帶入洗澡美容金額
                    // 需求2：非包月帶入寵物單次金額
                    decimal servicePrice = 0;
                    if (!isSubscriptionEligible)
                    {
                        // 非包月：使用寵物的客製化價格或服務預設價格
                        var customPrice = await _context.PetServicePrice
                            .Where(psp => psp.PetId == petId && psp.ServiceId == service.ServiceId && psp.IsActive)
                            .Select(psp => psp.CustomPrice)
                            .FirstOrDefaultAsync();

                        servicePrice = customPrice ?? service.BasePrice;
                    }

                    serviceTotal += servicePrice;

                    services.Add(new ServiceItemDto
                    {
                        ServiceId = service.ServiceId,
                        ServiceName = service.ServiceName,
                        ServiceType = service.ServiceType ?? "GENERAL",
                        Price = servicePrice,
                        Duration = duration,
                        Description = service.Description
                    });
                }
            }

            // 附加服務功能已移除，addonIds 參數保留但不處理
            // 未來如需附加服務，可在此重新實作

            // 包月優惠計算（僅針對附加服務，主服務已經是0）
            var discount = 0m;
            if (isSubscriptionEligible)
            {
                // 包月客戶的附加服務可能有折扣，目前不設折扣
                discount = 0m;
            }

            var calculation = new ServiceCalculationDto
            {
                PetId = petId,
                Services = services,
                Addons = new List<ServiceAddonCalculationDto>(), // 空清單，功能已移除
                ServiceTotal = serviceTotal,
                AddonTotal = 0, // 附加服務已移除，固定為0
                Discount = discount,
                IsSubscriptionEligible = isSubscriptionEligible,
                CalculationNote = isSubscriptionEligible ? "包月服務，主服務項目不計費" : "一般計費"
            };

            return calculation;
        }

        // 舊的硬編碼價格方法已移除，改用資料庫查詢和客製化定價邏輯

        private async Task<int> CalculateDeductionCountAsync(List<long> serviceIds)
        {
            // 根據實際服務內容計算包月扣除次數
            // 目前業務規則：每次預約扣除1次，未來可根據服務類型調整

            if (serviceIds?.Count > 0)
            {
                // 可以根據具體的服務項目來計算扣除次數
                // 例如：基礎洗澡1次，美容2次等
                var services = await _context.Service
                    .Where(s => serviceIds.Contains(s.ServiceId) && s.IsActive)
                    .ToListAsync();

                // 目前簡化為固定扣除1次，未來可根據業務需求調整
                return 1;
            }

            return 1;
        }


        public async Task<int> CalculateTotalServiceDurationAsync(long petId, List<long> serviceIds)
        {
            if (serviceIds?.Count == 0) return 0;

            int totalDuration = 0;

            foreach (var serviceId in serviceIds ?? new List<long>())
            {
                // 取得有效服務時間（客製化或預設）
                var duration = await _petServicePriceService.GetEffectiveServiceDurationAsync(petId, serviceId);
                totalDuration += duration;
            }

            return totalDuration;
        }

        public async Task CompleteReservation(long reservationId)
        {
            await UpdateReservationStatusAsync(reservationId, "COMPLETED");
            // TODO: Generate payment record automatically
            // This would integrate with the payment/income system
        }

        /// <summary>
        /// 取得寵物的完整定價設定概覽（用於管理介面）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>定價概覽資料</returns>
        public async Task<PetPricingOverviewDto> GetPetPricingOverviewAsync(long petId)
        {
            var pet = await _context.Pet.FirstOrDefaultAsync(p => p.PetId == petId);
            if (pet == null)
                throw new ArgumentException($"找不到寵物 ID: {petId}");

            var breed = await _context.SystemCode
                .Where(sc => sc.CodeType == "Breed" && sc.Code == pet.Breed)
                .Select(sc => sc.CodeName)
                .FirstOrDefaultAsync() ?? pet.Breed;


            // 服務價格和時間設定（使用 PetServicePrice 整合價格和時長）
            var servicePricesEntities = await _petServicePriceService.GetActivePetServicePricesAsync(petId);
            var serviceDurations = servicePricesEntities.Select(sp => new PetServiceDurationDto
            {
                PetServiceDurationId = sp.PetServicePriceId, // 使用 PetServicePriceId 作為識別碼
                PetId = sp.PetId,
                PetName = pet.PetName,
                ServiceId = sp.ServiceId,
                ServiceName = sp.Service.ServiceName,
                ServiceType = sp.Service.ServiceType,
                DefaultDuration = sp.Service.Duration,
                CustomDuration = sp.Duration, // PetServicePrice 的 Duration 欄位
                Notes = sp.Notes,
                IsActive = sp.IsActive,
                CreateUser = sp.CreateUser,
                CreateTime = sp.CreateTime,
                ModifyUser = sp.ModifyUser,
                ModifyTime = sp.ModifyTime
            }).ToList();

            return new PetPricingOverviewDto
            {
                PetId = petId,
                PetName = pet.PetName,
                Breed = breed,
                ServiceDurations = serviceDurations
            };
        }
    }
}