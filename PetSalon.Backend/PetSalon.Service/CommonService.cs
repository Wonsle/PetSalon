using Microsoft.EntityFrameworkCore;
using PetSalon.Models.EntityModels;
using PetSalon.Models.DTOs;
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

        public async Task<List<SystemCodeDto>> GetAllSystemCodesAsync()
        {
            var systemCodes = await _context.SystemCode
                .Join(_context.CodeType,
                    sc => sc.CodeType,
                    ct => ct.CodeType1,
                    (sc, ct) => new SystemCodeDto
                    {
                        Id = sc.CodeId,
                        CodeType = sc.CodeType,
                        CodeTypeName = ct.Name, // 從 CodeType 表取得顯示名稱
                        Code = sc.Code,
                        Name = sc.Name,
                        Value = sc.Name, // 或者根據業務邏輯調整
                        Sort = sc.Sort ?? 0,
                        IsActive = !sc.EndDate.HasValue || sc.EndDate > DateTime.Now,
                        CreateTime = sc.CreateTime,
                        CreateUser = sc.CreateUser,
                        UpdateTime = sc.ModifyTime,
                        UpdateUser = sc.ModifyUser
                    })
                .OrderBy(x => x.CodeType)
                .ThenBy(x => x.Sort)
                .ToListAsync();

            return systemCodes;
        }

        public async Task<IList<SystemCode>> GetSystemCodeList(string codeType)
        {
            return await _context.SystemCode
                .Where(x => x.CodeType == codeType && (x.EndDate == null || x.EndDate > DateTime.Now))
                .OrderBy(x => x.Sort)
                .ToListAsync();
        }
        public async Task<SystemCode?> GetSystemCode(string codeType, string code)
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
            // 改從 CodeType 資料表取得代碼類型
            return await _context.CodeType
                .Select(x => x.CodeType1)
                .OrderBy(x => x)
                .ToListAsync();
        }
    }
}
