using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IPetServicePriceService
    {
        /// <summary>
        /// 取得所有寵物服務價格設定
        /// </summary>
        /// <returns></returns>
        Task<IList<PetServicePrice>> GetPetServicePriceListAsync();

        /// <summary>
        /// 根據寵物ID取得服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns></returns>
        Task<IList<PetServicePrice>> GetPetServicePricesByPetAsync(long petId);

        /// <summary>
        /// 根據服務ID取得相關的寵物價格設定
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task<IList<PetServicePrice>> GetPetServicePricesByServiceAsync(long serviceId);

        /// <summary>
        /// 取得特定寵物的特定服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task<PetServicePrice?> GetPetServicePriceAsync(long petId, long serviceId);

        /// <summary>
        /// 取得寵物服務價格設定詳細資訊（含寵物和服務資訊）
        /// </summary>
        /// <param name="petServicePriceId">寵物服務價格ID</param>
        /// <returns></returns>
        Task<PetServicePrice?> GetPetServicePriceWithDetailsAsync(long petServicePriceId);

        /// <summary>
        /// 取得寵物的有效服務價格設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns></returns>
        Task<IList<PetServicePrice>> GetActivePetServicePricesAsync(long petId);

        /// <summary>
        /// 新增寵物服務價格設定
        /// </summary>
        /// <param name="petServicePrice">寵物服務價格資料</param>
        /// <returns>新增的寵物服務價格ID</returns>
        Task<long> CreatePetServicePriceAsync(PetServicePrice petServicePrice);

        /// <summary>
        /// 更新寵物服務價格設定
        /// </summary>
        /// <param name="petServicePrice">寵物服務價格資料</param>
        /// <returns></returns>
        Task UpdatePetServicePriceAsync(PetServicePrice petServicePrice);

        /// <summary>
        /// 刪除寵物服務價格設定
        /// </summary>
        /// <param name="petServicePriceId">寵物服務價格ID</param>
        /// <returns></returns>
        Task DeletePetServicePriceAsync(long petServicePriceId);

        /// <summary>
        /// 啟用/停用寵物服務價格設定
        /// </summary>
        /// <param name="petServicePriceId">寵物服務價格ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns></returns>
        Task TogglePetServicePriceStatusAsync(long petServicePriceId, bool isActive);

        /// <summary>
        /// 批次建立寵物服務價格設定
        /// </summary>
        /// <param name="petServicePrices">寵物服務價格資料清單</param>
        /// <returns></returns>
        Task CreateBatchPetServicePricesAsync(IList<PetServicePrice> petServicePrices);

        /// <summary>
        /// 批次刪除寵物服務價格設定
        /// </summary>
        /// <param name="petServicePriceIds">寵物服務價格ID清單</param>
        /// <returns>刪除的數量</returns>
        Task<int> BatchDeletePetServicePricesAsync(IList<long> petServicePriceIds);

        /// <summary>
        /// 取得寵物的實際服務時間（客製化時間優先，否則使用預設時間）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>服務時間（分鐘）</returns>
        Task<int> GetEffectiveServiceDurationAsync(long petId, long serviceId);

        /// <summary>
        /// 取得寵物的實際服務價格（客製化價格優先，否則使用預設價格）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>服務價格</returns>
        Task<decimal> GetEffectiveServicePriceAsync(long petId, long serviceId);

        /// <summary>
        /// 根據寵物清單和服務清單取得價格設定
        /// </summary>
        /// <param name="petIds">寵物ID清單</param>
        /// <param name="serviceIds">服務ID清單</param>
        /// <returns></returns>
        Task<IList<PetServicePrice>> GetPetServicePricesByRangeAsync(IList<long> petIds, IList<long> serviceIds);

        /// <summary>
        /// 取得服務價格統計資訊
        /// </summary>
        /// <returns>統計資訊</returns>
        Task<object> GetServicePriceStatisticsAsync();

        /// <summary>
        /// 取得寵物的訂閱價格（優先使用 PetServicePrice，其次使用 Service 預設值）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns>訂閱價格，如果沒有設定則返回 null</returns>
        Task<decimal?> GetSubscriptionPriceAsync(long petId);

        /// <summary>
        /// 批次更新或插入寵物服務價格設定（Upsert）
        /// 如果記錄存在則更新，不存在則新增
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="servicePrices">服務價格設定清單</param>
        /// <returns></returns>
        Task UpsertPetServicePricesAsync(long petId, IList<PetServicePrice> servicePrices);
    }
}
