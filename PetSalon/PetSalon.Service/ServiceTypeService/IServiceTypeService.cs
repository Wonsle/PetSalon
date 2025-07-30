using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    /// <summary>
    /// 服務類型判斷與包月扣除邏輯服務介面
    /// </summary>
    public interface IServiceTypeService
    {
        /// <summary>
        /// 根據服務項目判斷服務類型（洗澡/美容/混合）
        /// </summary>
        Task<ServiceTypeResultDto> DetermineServiceTypeAsync(List<long> serviceIds);

        /// <summary>
        /// 計算包月扣除次數（美容服務的複合扣除邏輯：1美容+3洗澡）
        /// </summary>
        Task<int> CalculateDeductionCountAsync(string serviceType, List<long> serviceIds);

        /// <summary>
        /// 驗證服務類型與包月類型的相容性
        /// </summary>
        Task<bool> ValidateCompatibilityAsync(string subscriptionType, string serviceType);
    }
}
