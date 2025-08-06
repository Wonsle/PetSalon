using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public interface IServiceService
    {
        /// <summary>
        /// 取得所有服務清單
        /// </summary>
        /// <returns></returns>
        Task<IList<Service>> GetServiceListAsync();

        /// <summary>
        /// 取得啟用的服務清單
        /// </summary>
        /// <returns></returns>
        Task<IList<Service>> GetActiveServiceListAsync();

        /// <summary>
        /// 根據類型取得服務清單
        /// </summary>
        /// <param name="serviceType">服務類型</param>
        /// <returns></returns>
        Task<IList<Service>> GetServicesByTypeAsync(string serviceType);

        /// <summary>
        /// 取得服務詳細資訊
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task<Service?> GetServiceAsync(long serviceId);

        /// <summary>
        /// 新增服務
        /// </summary>
        /// <param name="service">服務資料</param>
        /// <returns>新增的服務ID</returns>
        Task<long> CreateServiceAsync(Service service);

        /// <summary>
        /// 更新服務
        /// </summary>
        /// <param name="service">服務資料</param>
        /// <returns></returns>
        Task UpdateServiceAsync(Service service);

        /// <summary>
        /// 刪除服務
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task DeleteServiceAsync(long serviceId);

        /// <summary>
        /// 啟用/停用服務
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns></returns>
        Task ToggleServiceStatusAsync(long serviceId, bool isActive);

        /// <summary>
        /// 更新服務排序
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <param name="newSort">新的排序值</param>
        /// <returns></returns>
        Task UpdateServiceSortAsync(long serviceId, int newSort);
    }
}