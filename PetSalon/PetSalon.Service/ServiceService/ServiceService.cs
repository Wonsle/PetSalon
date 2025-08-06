using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public class ServiceService : IServiceService
    {
        private readonly PetSalonContext _context;

        public ServiceService(PetSalonContext context)
        {
            _context = context;
        }

        public async Task<IList<Service>> GetServiceListAsync()
        {
            return await _context.Service
                .AsNoTracking()
                .OrderBy(s => s.Sort)
                .ThenBy(s => s.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<Service>> GetActiveServiceListAsync()
        {
            return await _context.Service
                .Where(s => s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.Sort)
                .ThenBy(s => s.ServiceName)
                .ToListAsync();
        }

        public async Task<IList<Service>> GetServicesByTypeAsync(string serviceType)
        {
            return await _context.Service
                .Where(s => s.ServiceType == serviceType && s.IsActive)
                .AsNoTracking()
                .OrderBy(s => s.Sort)
                .ThenBy(s => s.ServiceName)
                .ToListAsync();
        }

        public async Task<Service?> GetServiceAsync(long serviceId)
        {
            return await _context.Service
                .AsNoTracking()
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);
        }

        public async Task<long> CreateServiceAsync(Service service)
        {
            // 設定審計欄位
            service.CreateUser = "System"; // 實際應用中應從用戶上下文取得
            service.CreateTime = DateTime.Now;
            service.ModifyUser = service.CreateUser;
            service.ModifyTime = service.CreateTime;

            _context.Service.Add(service);
            await _context.SaveChangesAsync();
            
            return service.ServiceId;
        }

        public async Task UpdateServiceAsync(Service service)
        {
            var existingService = await _context.Service
                .FirstOrDefaultAsync(s => s.ServiceId == service.ServiceId);

            if (existingService == null)
            {
                throw new InvalidOperationException($"找不到 ServiceId = {service.ServiceId} 的服務");
            }

            // 更新欄位
            existingService.ServiceName = service.ServiceName;
            existingService.ServiceType = service.ServiceType;
            existingService.BasePrice = service.BasePrice;
            existingService.Duration = service.Duration;
            existingService.Description = service.Description;
            existingService.IsActive = service.IsActive;
            existingService.Sort = service.Sort;

            // 更新審計欄位
            existingService.ModifyUser = "System"; // 實際應用中應從用戶上下文取得
            existingService.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteServiceAsync(long serviceId)
        {
            var service = await _context.Service
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null)
            {
                throw new InvalidOperationException($"找不到 ServiceId = {serviceId} 的服務");
            }

            // 軟刪除：設為非啟用狀態
            service.IsActive = false;
            service.ModifyUser = "System"; // 實際應用中應從用戶上下文取得
            service.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task ToggleServiceStatusAsync(long serviceId, bool isActive)
        {
            var service = await _context.Service
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null)
            {
                throw new InvalidOperationException($"找不到 ServiceId = {serviceId} 的服務");
            }

            service.IsActive = isActive;
            service.ModifyUser = "System"; // 實際應用中應從用戶上下文取得
            service.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }

        public async Task UpdateServiceSortAsync(long serviceId, int newSort)
        {
            var service = await _context.Service
                .FirstOrDefaultAsync(s => s.ServiceId == serviceId);

            if (service == null)
            {
                throw new InvalidOperationException($"找不到 ServiceId = {serviceId} 的服務");
            }

            service.Sort = newSort;
            service.ModifyUser = "System"; // 實際應用中應從用戶上下文取得
            service.ModifyTime = DateTime.Now;

            await _context.SaveChangesAsync();
        }
    }
}