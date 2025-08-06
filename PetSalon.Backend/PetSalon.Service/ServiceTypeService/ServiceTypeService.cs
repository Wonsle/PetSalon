using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    /// <summary>
    /// 服務類型判斷與包月扣除邏輯服務實作
    /// </summary>
    public class ServiceTypeService : IServiceTypeService
    {
        private readonly PetSalonContext _context;

        public ServiceTypeService(PetSalonContext context)
        {
            _context = context;
        }

        /// <summary>
        /// 根據服務項目判斷服務類型（洗澡/美容/混合）
        /// </summary>
        public async Task<ServiceTypeResultDto> DetermineServiceTypeAsync(List<long> serviceIds)
        {
            // TODO: 從 Service 表取得服務資訊並判斷類型
            // 這裡需要實際的 Service 表結構來實作

            var result = new ServiceTypeResultDto();

            // 模擬邏輯：根據服務項目判斷
            // 實際實作需要從資料庫查詢服務類型
            var bathCount = 0;
            var groomCount = 0;

            // TODO: 查詢實際服務資料
            // var services = await _context.Service.Where(s => serviceIds.Contains(s.ServiceId)).ToListAsync();

            // 暫時模擬判斷邏輯
            if (serviceIds.Count > 0)
            {
                // 假設前半部分是洗澡，後半部分是美容
                bathCount = serviceIds.Count / 2;
                groomCount = serviceIds.Count - bathCount;
            }

            if (groomCount > 0 && bathCount > 0)
            {
                result.ServiceType = "MIXED";
                result.DeductionCount = groomCount * 4 + bathCount; // 美容1次=4次洗澡
                result.DeductionReason = $"混合服務：{groomCount}次美容 + {bathCount}次洗澡";
            }
            else if (groomCount > 0)
            {
                result.ServiceType = "GROOM";
                result.DeductionCount = groomCount * 4; // 1次美容 = 4次洗澡次數
                result.DeductionReason = $"美容服務：{groomCount}次美容";
            }
            else
            {
                result.ServiceType = "BATH";
                result.DeductionCount = bathCount;
                result.DeductionReason = $"洗澡服務：{bathCount}次洗澡";
            }

            return result;
        }

        /// <summary>
        /// 計算包月扣除次數（美容服務的複合扣除邏輯：1美容+3洗澡）
        /// </summary>
        public async Task<int> CalculateDeductionCountAsync(string serviceType, List<long> serviceIds)
        {
            var result = await DetermineServiceTypeAsync(serviceIds);
            return result.DeductionCount;
        }

        /// <summary>
        /// 驗證服務類型與包月類型的相容性
        /// </summary>
        public async Task<bool> ValidateCompatibilityAsync(string subscriptionType, string serviceType)
        {
            // 包月類型與服務類型相容性檢查
            return subscriptionType switch
            {
                "BATH" => serviceType == "BATH", // 洗澡包月只能用洗澡服務
                "GROOM" => serviceType == "GROOM" || serviceType == "BATH", // 美容包月可用美容或洗澡
                "MIXED" => true, // 混合包月可用任何服務
                _ => false
            };
        }
    }
}
