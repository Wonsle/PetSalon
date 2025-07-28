using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public interface ICommonService
    {
        Task<IList<SystemCode>> GetSystemCodeList(string codeType);
        Task<SystemCode> GetSystemCode(string codeType, string code);
        Task<int> CreateSystemCode(SystemCode systemCode);
        Task UpdateSystemCode(SystemCode systemCode);
        Task DeleteSystemCode(int codeId);
        Task<IList<string>> GetSystemCodeTypes();
    }
}