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
            return await _context.SystemCode
                .Where(x => x.CodeType == codeType && (x.EndDate == null || x.EndDate > DateTime.Now))
                .OrderBy(x => x.Sort)
                .ToListAsync();
        }
        public async Task<SystemCode> GetSystemCode(string codeType, string code)
        {
            return await _context.SystemCode.Where(x => x.CodeType == codeType && x.Code == code).SingleOrDefaultAsync();
        }

        public async Task<int> CreateSystemCode(SystemCode systemCode)
        {
            systemCode.CreateTime = DateTime.Now;
            systemCode.ModifyTime = DateTime.Now;
            systemCode.StartDate = DateTime.Now;
            
            _context.SystemCode.Add(systemCode);
            await _context.SaveChangesAsync();
            return systemCode.CodeId;
        }

        public async Task UpdateSystemCode(SystemCode systemCode)
        {
            var existingCode = await _context.SystemCode.FindAsync(systemCode.CodeId);
            if (existingCode != null)
            {
                existingCode.Name = systemCode.Name;
                existingCode.Description = systemCode.Description;
                existingCode.Sort = systemCode.Sort;
                existingCode.EndDate = systemCode.EndDate;
                existingCode.ModifyTime = DateTime.Now;
                existingCode.ModifyUser = systemCode.ModifyUser ?? "System";
                
                await _context.SaveChangesAsync();
            }
            else
            {
                throw new ArgumentException($"SystemCode with CodeId {systemCode.CodeId} not found.");
            }
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
