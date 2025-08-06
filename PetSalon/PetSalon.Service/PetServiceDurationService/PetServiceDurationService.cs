using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public class PetServiceDurationService : IPetServiceDurationService
    {
        private readonly PetSalonContext _context;
        private readonly ICommonService _commonService;

        public PetServiceDurationService(PetSalonContext context, ICommonService commonService)
        {
            _context = context;
            _commonService = commonService;
        }

        public async Task<IList<PetServiceDuration>> GetPetServiceDurationListAsync()
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Pet)
                .Include(psd => psd.Service)
                .AsNoTracking()
                .OrderBy(psd => psd.Pet.PetName)
                .ThenBy(psd => psd.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<PetServiceDuration>> GetPetServiceDurationsByPetAsync(long petId)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Service)
                .Where(psd => psd.PetId == petId)
                .AsNoTracking()
                .OrderBy(psd => psd.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<PetServiceDuration>> GetPetServiceDurationsByServiceAsync(long serviceId)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Pet)
                .Where(psd => psd.ServiceId == serviceId)
                .AsNoTracking()
                .OrderBy(psd => psd.Pet.PetName)
                .ToListAsync();
        }

        public async Task<PetServiceDuration?> GetPetServiceDurationAsync(long petId, long serviceId)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Pet)
                .Include(psd => psd.Service)
                .AsNoTracking()
                .FirstOrDefaultAsync(psd => psd.PetId == petId && psd.ServiceId == serviceId);
        }

        public async Task<PetServiceDuration?> GetPetServiceDurationWithDetailsAsync(long petServiceDurationId)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Pet)
                .Include(psd => psd.Service)
                .AsNoTracking()
                .FirstOrDefaultAsync(psd => psd.PetServiceDurationId == petServiceDurationId);
        }

        public async Task<IList<PetServiceDuration>> GetActivePetServiceDurationsAsync(long petId)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Service)
                .Where(psd => psd.PetId == petId && psd.IsActive && psd.Service.IsActive)
                .AsNoTracking()
                .OrderBy(psd => psd.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<long> CreatePetServiceDurationAsync(PetServiceDuration petServiceDuration)
        {
            // 檢查是否已存在相同的寵物+服務組合
            var existing = await _context.PetServiceDuration
                .FirstOrDefaultAsync(psd => psd.PetId == petServiceDuration.PetId 
                                          && psd.ServiceId == petServiceDuration.ServiceId);

            if (existing != null)
            {
                throw new InvalidOperationException($"該寵物的服務時間設定已存在");
            }

            // 設定審計欄位
            petServiceDuration.CreateUser = "System"; // TODO: 從認證中取得實際使用者
            petServiceDuration.ModifyUser = "System";
            petServiceDuration.CreateTime = DateTime.UtcNow;
            petServiceDuration.ModifyTime = DateTime.UtcNow;

            _context.PetServiceDuration.Add(petServiceDuration);
            await _context.SaveChangesAsync();

            return petServiceDuration.PetServiceDurationId;
        }

        public async Task UpdatePetServiceDurationAsync(PetServiceDuration petServiceDuration)
        {
            var existing = await _context.PetServiceDuration
                .FirstOrDefaultAsync(psd => psd.PetServiceDurationId == petServiceDuration.PetServiceDurationId);

            if (existing != null)
            {
                // 保留原始的建立資訊
                petServiceDuration.CreateUser = existing.CreateUser;
                petServiceDuration.CreateTime = existing.CreateTime;
                
                // 更新修改資訊
                petServiceDuration.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                petServiceDuration.ModifyTime = DateTime.UtcNow;

                _context.Entry(existing).CurrentValues.SetValues(petServiceDuration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task DeletePetServiceDurationAsync(long petServiceDurationId)
        {
            var petServiceDuration = await _context.PetServiceDuration
                .FirstOrDefaultAsync(psd => psd.PetServiceDurationId == petServiceDurationId);

            if (petServiceDuration != null)
            {
                _context.PetServiceDuration.Remove(petServiceDuration);
                await _context.SaveChangesAsync();
            }
        }

        public async Task TogglePetServiceDurationStatusAsync(long petServiceDurationId, bool isActive)
        {
            var petServiceDuration = await _context.PetServiceDuration
                .FirstOrDefaultAsync(psd => psd.PetServiceDurationId == petServiceDurationId);

            if (petServiceDuration != null)
            {
                petServiceDuration.IsActive = isActive;
                petServiceDuration.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                petServiceDuration.ModifyTime = DateTime.UtcNow;
                await _context.SaveChangesAsync();
            }
        }

        public async Task CreateBatchPetServiceDurationsAsync(IList<PetServiceDuration> petServiceDurations)
        {
            foreach (var petServiceDuration in petServiceDurations)
            {
                // 設定審計欄位
                petServiceDuration.CreateUser = "System"; // TODO: 從認證中取得實際使用者
                petServiceDuration.ModifyUser = "System";
                petServiceDuration.CreateTime = DateTime.UtcNow;
                petServiceDuration.ModifyTime = DateTime.UtcNow;
            }

            _context.PetServiceDuration.AddRange(petServiceDurations);
            await _context.SaveChangesAsync();
        }

        public async Task<int> BatchDeletePetServiceDurationsAsync(IList<long> petServiceDurationIds)
        {
            var durations = await _context.PetServiceDuration
                .Where(psd => petServiceDurationIds.Contains(psd.PetServiceDurationId))
                .ToListAsync();

            if (durations.Any())
            {
                // 軟刪除：設定為非活躍狀態
                foreach (var duration in durations)
                {
                    duration.IsActive = false;
                    duration.ModifyUser = "System"; // TODO: 從認證中取得實際使用者
                    duration.ModifyTime = DateTime.UtcNow;
                }

                await _context.SaveChangesAsync();
            }

            return durations.Count;
        }

        public async Task<int> GetEffectiveServiceDurationAsync(long petId, long serviceId)
        {
            // 先查找是否有客製化時間
            var customDuration = await _context.PetServiceDuration
                .Where(psd => psd.PetId == petId 
                             && psd.ServiceId == serviceId 
                             && psd.IsActive
                             && psd.CustomDuration.HasValue)
                .Select(psd => psd.CustomDuration.Value)
                .FirstOrDefaultAsync();

            if (customDuration != 0)
            {
                return customDuration;
            }

            // 如果沒有客製化時間，使用服務的預設時間
            var defaultDuration = await _context.Service
                .Where(s => s.ServiceId == serviceId && s.IsActive)
                .Select(s => s.Duration) // 使用服務的 Duration 欄位
                .FirstOrDefaultAsync();

            if (defaultDuration == 0)
            {
                defaultDuration = 60; // 如果沒有設定，預設60分鐘
            }

            return defaultDuration;
        }

        public async Task<IList<PetServiceDuration>> GetPetServiceDurationsByRangeAsync(IList<long> petIds, IList<long> serviceIds)
        {
            return await _context.PetServiceDuration
                .Include(psd => psd.Pet)
                .Include(psd => psd.Service)
                .Where(psd => petIds.Contains(psd.PetId) && serviceIds.Contains(psd.ServiceId))
                .AsNoTracking()
                .OrderBy(psd => psd.Pet.PetName)
                .ThenBy(psd => psd.Service.ServiceName)
                .ToListAsync();
        }

        public async Task<object> GetServiceDurationStatisticsAsync()
        {
            var totalCount = await _context.PetServiceDuration
                .Where(psd => psd.IsActive)
                .CountAsync();

            var avgCustomDuration = await _context.PetServiceDuration
                .Where(psd => psd.IsActive && psd.CustomDuration.HasValue)
                .AverageAsync(psd => (double?)psd.CustomDuration) ?? 0;

            var maxDuration = await _context.PetServiceDuration
                .Where(psd => psd.IsActive && psd.CustomDuration.HasValue)
                .MaxAsync(psd => (int?)psd.CustomDuration) ?? 0;

            var minDuration = await _context.PetServiceDuration
                .Where(psd => psd.IsActive && psd.CustomDuration.HasValue)
                .MinAsync(psd => (int?)psd.CustomDuration) ?? 0;

            var petCount = await _context.PetServiceDuration
                .Where(psd => psd.IsActive)
                .Select(psd => psd.PetId)
                .Distinct()
                .CountAsync();

            var serviceCount = await _context.PetServiceDuration
                .Where(psd => psd.IsActive)
                .Select(psd => psd.ServiceId)
                .Distinct()
                .CountAsync();

            return new
            {
                TotalSettings = totalCount,
                AverageCustomDuration = Math.Round(avgCustomDuration, 2),
                MaxDuration = maxDuration,
                MinDuration = minDuration,
                UniquePets = petCount,
                UniqueServices = serviceCount
            };
        }
    }
}