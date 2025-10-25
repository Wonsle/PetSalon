using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class PetServicePriceService : IPetServicePriceService
    {
        private readonly PetSalonContext _context;
        private readonly ICommonService _commonService;

        public PetServicePriceService(PetSalonContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<IList<PetServicePrice>> GetPetServicePriceListAsync()
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Pet)
                .Include(psp => psp.Service)
                .AsNoTracking()
                .OrderBy(psp => psp.Pet.PetName)
                .ThenBy(psp => psp.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<PetServicePrice>> GetPetServicePricesByPetAsync(long petId)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Service)
                .Where(psp => psp.PetId == petId)
                .AsNoTracking()
                .OrderBy(psp => psp.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<PetServicePrice>> GetPetServicePricesByServiceAsync(long serviceId)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Pet)
                .Where(psp => psp.ServiceId == serviceId)
                .AsNoTracking()
                .OrderBy(psp => psp.Pet.PetName)
                .ToListAsync();
        }

        public async Task<PetServicePrice?> GetPetServicePriceAsync(long petId, long serviceId)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Pet)
                .Include(psp => psp.Service)
                .AsNoTracking()
                .FirstOrDefaultAsync(psp => psp.PetId == petId && psp.ServiceId == serviceId);
        }

        public async Task<PetServicePrice?> GetPetServicePriceWithDetailsAsync(long petServicePriceId)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Pet)
                .Include(psp => psp.Service)
                .AsNoTracking()
                .FirstOrDefaultAsync(psp => psp.PetServicePriceId == petServicePriceId);
        }

        public async Task<IList<PetServicePrice>> GetActivePetServicePricesAsync(long petId)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Service)
                .Where(psp => psp.PetId == petId && psp.IsActive && psp.Service.IsActive)
                .AsNoTracking()
                .OrderBy(psp => psp.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<long> CreatePetServicePriceAsync(PetServicePrice petServicePrice)
        {
            // 檢查是否已存在相同的寵物+服務組合
            var existing = await _context.PetServicePrice
                .FirstOrDefaultAsync(psp => psp.PetId == petServicePrice.PetId
                                          && psp.ServiceId == petServicePrice.ServiceId
                                          && psp.IsActive);

            if (existing != null)
            {
                throw new InvalidOperationException($"該寵物的服務價格設定已存在");
            }

            // 設定審計欄位 (如果使用 SaveChangesInterceptor 會自動處理，這裡作為備用)
            if (string.IsNullOrEmpty(petServicePrice.CreateUser))
            {
                petServicePrice.CreateUser = "System";
            }
            if (string.IsNullOrEmpty(petServicePrice.ModifyUser))
            {
                petServicePrice.ModifyUser = "System";
            }
            petServicePrice.CreateTime = DateTime.UtcNow;
            petServicePrice.ModifyTime = DateTime.UtcNow;

            _context.PetServicePrice.Add(petServicePrice);
            await _context.SaveChangesAsync();

            return petServicePrice.PetServicePriceId;
        }

        public async Task UpdatePetServicePriceAsync(PetServicePrice petServicePrice)
        {
            var existing = await _context.PetServicePrice
                .FirstOrDefaultAsync(psp => psp.PetServicePriceId == petServicePrice.PetServicePriceId);

            if (existing != null)
            {
                // 保留原始的建立資訊
                petServicePrice.CreateUser = existing.CreateUser;
                petServicePrice.CreateTime = existing.CreateTime;

                // 更新修改資訊
                petServicePrice.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                petServicePrice.ModifyTime = DateTime.UtcNow;

                _context.Entry(existing).CurrentValues.SetValues(petServicePrice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePetServicePriceAsync(long petServicePriceId)
        {
            var petServicePrice = await _context.PetServicePrice
                .FirstOrDefaultAsync(psp => psp.PetServicePriceId == petServicePriceId);

            if (petServicePrice != null)
            {
                _context.PetServicePrice.Remove(petServicePrice);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TogglePetServicePriceStatusAsync(long petServicePriceId, bool isActive)
        {
            var petServicePrice = await _context.PetServicePrice
                .FirstOrDefaultAsync(psp => psp.PetServicePriceId == petServicePriceId);

            if (petServicePrice != null)
            {
                petServicePrice.IsActive = isActive;
                petServicePrice.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                petServicePrice.ModifyTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateBatchPetServicePricesAsync(IList<PetServicePrice> petServicePrices)
        {
            foreach (var petServicePrice in petServicePrices)
            {
                // 設定審計欄位
                if (string.IsNullOrEmpty(petServicePrice.CreateUser))
                {
                    petServicePrice.CreateUser = "System";
                }
                if (string.IsNullOrEmpty(petServicePrice.ModifyUser))
                {
                    petServicePrice.ModifyUser = "System";
                }
                petServicePrice.CreateTime = DateTime.UtcNow;
                petServicePrice.ModifyTime = DateTime.UtcNow;
            }

            _context.PetServicePrice.AddRange(petServicePrices);
            await _context.SaveChangesAsync();
        }

        public async Task<int> BatchDeletePetServicePricesAsync(IList<long> petServicePriceIds)
        {
            var prices = await _context.PetServicePrice
                .Where(psp => petServicePriceIds.Contains(psp.PetServicePriceId))
                .ToListAsync();

            if (prices.Any())
            {
                // 軟刪除：設定為非活躍狀態
                foreach (var price in prices)
                {
                    price.IsActive = false;
                    price.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                    price.ModifyTime = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return prices.Count;
        }

        public async Task<int> GetEffectiveServiceDurationAsync(long petId, long serviceId)
        {
            // 先查找是否有客製化時間
            var customDuration = await _context.PetServicePrice
                .Where(psp => psp.PetId == petId
                             && psp.ServiceId == serviceId
                             && psp.IsActive
                             && psp.Duration.HasValue)
                .Select(psp => psp.Duration.Value)
                .FirstOrDefaultAsync();

            if (customDuration != 0)
            {
                return customDuration;
            }

            // 如果沒有客製化時間，使用服務的預設時間
            var defaultDuration = await _context.Service
                .Where(s => s.ServiceId == serviceId && s.IsActive)
                .Select(s => s.Duration)
                .FirstOrDefaultAsync();

            if (defaultDuration == 0)
            {
                defaultDuration = 60; // 如果沒有設定，預設60分鐘
            }

            return defaultDuration;
        }

        public async Task<decimal> GetEffectiveServicePriceAsync(long petId, long serviceId)
        {
            // 先查找是否有客製化價格
            var customPrice = await _context.PetServicePrice
                .Where(psp => psp.PetId == petId
                             && psp.ServiceId == serviceId
                             && psp.IsActive
                             && psp.CustomPrice.HasValue)
                .Select(psp => psp.CustomPrice.Value)
                .FirstOrDefaultAsync();

            if (customPrice != 0)
            {
                return customPrice;
            }

            // 如果沒有客製化價格，使用服務的預設價格
            var defaultPrice = await _context.Service
                .Where(s => s.ServiceId == serviceId && s.IsActive)
                .Select(s => s.BasePrice)
                .FirstOrDefaultAsync();

            return defaultPrice;
        }

        public async Task<IList<PetServicePrice>> GetPetServicePricesByRangeAsync(IList<long> petIds, IList<long> serviceIds)
        {
            return await _context.PetServicePrice
                .Include(psp => psp.Pet)
                .Include(psp => psp.Service)
                .Where(psp => petIds.Contains(psp.PetId) && serviceIds.Contains(psp.ServiceId))
                .AsNoTracking()
                .OrderBy(psp => psp.Pet.PetName)
                .ThenBy(psp => psp.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<object> GetServicePriceStatisticsAsync()
        {
            var totalCount = await _context.PetServicePrice
                .Where(psp => psp.IsActive)
                .CountAsync();

            var avgCustomPrice = await _context.PetServicePrice
                .Where(psp => psp.IsActive && psp.CustomPrice.HasValue)
                .AverageAsync(psp => (decimal?)psp.CustomPrice) ?? 0;

            var maxPrice = await _context.PetServicePrice
                .Where(psp => psp.IsActive && psp.CustomPrice.HasValue)
                .MaxAsync(psp => (decimal?)psp.CustomPrice) ?? 0;

            var minPrice = await _context.PetServicePrice
                .Where(psp => psp.IsActive && psp.CustomPrice.HasValue)
                .MinAsync(psp => (decimal?)psp.CustomPrice) ?? 0;

            var avgDuration = await _context.PetServicePrice
                .Where(psp => psp.IsActive && psp.Duration.HasValue)
                .AverageAsync(psp => (double?)psp.Duration) ?? 0;

            var petCount = await _context.PetServicePrice
                .Where(psp => psp.IsActive)
                .Select(psp => psp.PetId)
                .Distinct()
                .CountAsync();

            var serviceCount = await _context.PetServicePrice
                .Where(psp => psp.IsActive)
                .Select(psp => psp.ServiceId)
                .Distinct()
                .CountAsync();

            return new
            {
                TotalSettings = totalCount,
                AverageCustomPrice = Math.Round(avgCustomPrice, 2),
                MaxPrice = maxPrice,
                MinPrice = minPrice,
                AverageDuration = Math.Round(avgDuration, 2),
                UniquePets = petCount,
                UniqueServices = serviceCount
            };
        }

        public async Task<decimal?> GetSubscriptionPriceAsync(long petId)
        {
            // 1. 先嘗試從 PetServicePrice 取得該寵物的訂閱服務價格
            var subscriptionService = await _context.Service
                .Where(s => s.ServiceType == "SUBSCRIPTION" && s.IsActive)
                .FirstOrDefaultAsync();

            if (subscriptionService == null)
            {
                // 如果系統中沒有訂閱服務，返回 null
                return null;
            }

            var petServicePrice = await _context.PetServicePrice
                .Where(psp => psp.PetId == petId
                    && psp.ServiceId == subscriptionService.ServiceId
                    && psp.IsActive)
                .AsNoTracking()
                .FirstOrDefaultAsync();

            // 2. 如果有客製化價格，返回客製化價格
            if (petServicePrice?.CustomPrice != null && petServicePrice.CustomPrice > 0)
            {
                return petServicePrice.CustomPrice;
            }

            // 3. 否則返回 Service 表的預設訂閱價格
            if (subscriptionService.BasePrice > 0)
            {
                return subscriptionService.BasePrice;
            }

            // 4. 如果都沒有，返回 null
            return null;
        }

        public async Task UpsertPetServicePricesAsync(long petId, IList<PetServicePrice> servicePrices)
        {
            // 取得現有的活躍服務價格記錄
            var existingPrices = await _context.PetServicePrice
                .Where(psp => psp.PetId == petId && psp.IsActive)
                .ToListAsync();

            var existingDict = existingPrices.ToDictionary(p => p.ServiceId, p => p);
            var processedServiceIds = new HashSet<long>();

            foreach (var newPrice in servicePrices)
            {
                processedServiceIds.Add(newPrice.ServiceId);

                if (existingDict.TryGetValue(newPrice.ServiceId, out var existingPrice))
                {
                    // 更新現有記錄
                    existingPrice.CustomPrice = newPrice.CustomPrice;
                    existingPrice.Duration = newPrice.Duration;
                    existingPrice.Notes = newPrice.Notes;
                    existingPrice.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                    existingPrice.ModifyTime = DateTime.UtcNow;
                }
                else
                {
                    // 新增新記錄
                    var priceEntity = new PetServicePrice
                    {
                        PetId = petId,
                        ServiceId = newPrice.ServiceId,
                        CustomPrice = newPrice.CustomPrice,
                        Duration = newPrice.Duration,
                        Notes = newPrice.Notes,
                        IsActive = true,
                        CreateUser = "System", // TODO: 從認證中取得實際使用者
                        ModifyUser = "System",
                        CreateTime = DateTime.UtcNow,
                        ModifyTime = DateTime.UtcNow
                    };
                    _context.PetServicePrice.Add(priceEntity);
                }
            }

            // 將未處理的現有記錄設為非活躍（軟刪除）
            foreach (var existingPrice in existingPrices)
            {
                if (!processedServiceIds.Contains(existingPrice.ServiceId))
                {
                    existingPrice.IsActive = false;
                    existingPrice.ModifyUser = "System";
                    existingPrice.ModifyTime = DateTime.UtcNow;
                }
            }

            await _context.SaveChangesAsync();
        }
    }
}
