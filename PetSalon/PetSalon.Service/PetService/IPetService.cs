using PetSalon.Models.EntityModels;

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
        /// 取得寵物(單筆)
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        Task<Pet> GetPet(long id);

        /// <summary>
        /// 新增寵物
        /// </summary>
        Task<long> CreatePet(Pet pet);

        /// <summary>
        /// 更新寵物
        /// </summary>
        /// <param name="pet"></param>
        void UpdatePet(Pet pet);

        /// <summary>
        /// 刪除寵物
        /// </summary>
        /// <param name="petID"></param>
        Task DeletePet(long petID);

    }
}
