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
    }
}
