using PetSalon.Models.EntityModels;

namespace PetSalon.Services
{
    public interface ICommonService
    {
        Task<IList<SystemCode>> GetSystemCodeList(string codeType);
        Task<SystemCode> GetSystemCode(string codeType, string code);
    }
}