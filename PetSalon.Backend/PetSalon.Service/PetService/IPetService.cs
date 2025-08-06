using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface IPetService
    {
        /// <summary>
        /// 取得寵物清單
        /// </summary>
        /// <returns></returns>
        Task<IList<Pet>> GetPetList();

        /// <summary>
        /// 取得寵物清單（含主人資訊）
        /// </summary>
        /// <returns></returns>
        Task<IList<PetListResponse>> GetPetListWithOwners();

        /// <summary>
        /// 取得寵物(單筆)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Pet> GetPet(long id);

        /// <summary>
        /// 取得寵物詳細資訊（含所有聯絡人關係）
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<PetDetailResponse?> GetPetDetailWithContacts(long id);

        /// <summary>
        /// 新增寵物
        /// </summary>
        Task<long> CreatePet(Pet pet);

        /// <summary>
        /// 更新寵物
        /// </summary>
        /// <param name="pet"></param>
        Task UpdatePet(Pet pet);

        /// <summary>
        /// 取得聯絡人的寵物清單
        /// </summary>
        /// <param name="contactPersonId"></param>
        /// <returns></returns>
        Task<IList<Pet>> GetPetsByContactPerson(long contactPersonId);

        /// <summary>
        /// 刪除寵物
        /// </summary>
        /// <param name="petID"></param>
        Task DeletePet(long petID);

    }
}
