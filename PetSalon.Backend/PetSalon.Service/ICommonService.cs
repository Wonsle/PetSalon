using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;

namespace PetSalon.Services
{
    public interface ICommonService
    {
        Task<List<SystemCodeDto>> GetAllSystemCodesAsync();
        Task<IList<SystemCode>> GetSystemCodeList(string codeType);
        Task<SystemCode?> GetSystemCode(string codeType, string code);
        Task<int> CreateSystemCode(SystemCode systemCode);
        Task UpdateSystemCode(SystemCode systemCode);
        Task DeleteSystemCode(int codeId);
        Task<IList<string>> GetSystemCodeTypes();
    }
}