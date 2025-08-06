using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IPetServiceDurationService
    {
        /// <summary>
        /// 取得所有寵物服務時間設定
        /// </summary>
        /// <returns></returns>
        Task<IList<PetServiceDuration>> GetPetServiceDurationListAsync();

        /// <summary>
        /// 根據寵物ID取得服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns></returns>
        Task<IList<PetServiceDuration>> GetPetServiceDurationsByPetAsync(long petId);

        /// <summary>
        /// 根據服務ID取得相關的寵物時間設定
        /// </summary>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task<IList<PetServiceDuration>> GetPetServiceDurationsByServiceAsync(long serviceId);

        /// <summary>
        /// 取得特定寵物的特定服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns></returns>
        Task<PetServiceDuration?> GetPetServiceDurationAsync(long petId, long serviceId);

        /// <summary>
        /// 取得寵物服務時間設定詳細資訊（含寵物和服務資訊）
        /// </summary>
        /// <param name="petServiceDurationId">寵物服務時間ID</param>
        /// <returns></returns>
        Task<PetServiceDuration?> GetPetServiceDurationWithDetailsAsync(long petServiceDurationId);

        /// <summary>
        /// 取得寵物的有效服務時間設定
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <returns></returns>
        Task<IList<PetServiceDuration>> GetActivePetServiceDurationsAsync(long petId);

        /// <summary>
        /// 新增寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDuration">寵物服務時間資料</param>
        /// <returns>新增的寵物服務時間ID</returns>
        Task<long> CreatePetServiceDurationAsync(PetServiceDuration petServiceDuration);

        /// <summary>
        /// 更新寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDuration">寵物服務時間資料</param>
        /// <returns></returns>
        Task UpdatePetServiceDurationAsync(PetServiceDuration petServiceDuration);

        /// <summary>
        /// 刪除寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDurationId">寵物服務時間ID</param>
        /// <returns></returns>
        Task DeletePetServiceDurationAsync(long petServiceDurationId);

        /// <summary>
        /// 啟用/停用寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDurationId">寵物服務時間ID</param>
        /// <param name="isActive">是否啟用</param>
        /// <returns></returns>
        Task TogglePetServiceDurationStatusAsync(long petServiceDurationId, bool isActive);

        /// <summary>
        /// 批次建立寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDurations">寵物服務時間資料清單</param>
        /// <returns></returns>
        Task CreateBatchPetServiceDurationsAsync(IList<PetServiceDuration> petServiceDurations);

        /// <summary>
        /// 批次刪除寵物服務時間設定
        /// </summary>
        /// <param name="petServiceDurationIds">寵物服務時間ID清單</param>
        /// <returns>刪除的數量</returns>
        Task<int> BatchDeletePetServiceDurationsAsync(IList<long> petServiceDurationIds);

        /// <summary>
        /// 取得寵物的實際服務時間（客製化時間優先，否則使用預設時間）
        /// </summary>
        /// <param name="petId">寵物ID</param>
        /// <param name="serviceId">服務ID</param>
        /// <returns>服務時間（分鐘）</returns>
        Task<int> GetEffectiveServiceDurationAsync(long petId, long serviceId);

        /// <summary>
        /// 根據寵物清單和服務清單取得時間設定
        /// </summary>
        /// <param name="petIds">寵物ID清單</param>
        /// <param name="serviceIds">服務ID清單</param>
        /// <returns></returns>
        Task<IList<PetServiceDuration>> GetPetServiceDurationsByRangeAsync(IList<long> petIds, IList<long> serviceIds);

        /// <summary>
        /// 取得服務時間統計資訊
        /// </summary>
        /// <returns>統計資訊</returns>
        Task<object> GetServiceDurationStatisticsAsync();
    }
}