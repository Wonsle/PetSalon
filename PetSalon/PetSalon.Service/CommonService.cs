using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Internal;
using PetSalon.Models.EntityModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PetSalon.Services
{
    public class CommonService : ICommonService
    {
        PetSalonContext _context { get; set; }

        public CommonService(PetSalonContext context)
        {
            _context = context;
        }

        public async Task<IList<SystemCode>> GetSystemCodeList(string codeType)
        {
            return await _context.SystemCode.Where(x => x.CodeType == codeType).ToListAsync();
        }
        public async Task<SystemCode> GetSystemCode(string codeType, string code)
        {
            return await _context.SystemCode.Where(x => x.CodeType == codeType && x.Code == code).SingleOrDefaultAsync();
        }

        public async Task<int> CreateSystemCode(SystemCode systemCode)
        {
            _context.SystemCode.Add(systemCode);
            await _context.SaveChangesAsync();
            return systemCode.CodeId;
        }

        public async Task UpdateSystemCode(SystemCode systemCode)
        {
            _context.SystemCode.Update(systemCode);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteSystemCode(int codeId)
        {
            var systemCode = new SystemCode() { CodeId = codeId };
            _context.SystemCode.Attach(systemCode);
            _context.SystemCode.Remove(systemCode);
            await _context.SaveChangesAsync();
        }

        public async Task<IList<string>> GetSystemCodeTypes()
        {
            return await _context.SystemCode
                .Select(x => x.CodeType)
                .Distinct()
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}
