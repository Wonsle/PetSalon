using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services.CodeTypeService;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 代碼類型管理API控制器
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class CodeTypeController : BaseController
    {
        private readonly ICodeTypeService _codeTypeService;

        public CodeTypeController(ICodeTypeService codeTypeService)
        {
            _codeTypeService = codeTypeService;
        }

        /// <summary>
        /// 取得所有代碼類型列表
        /// </summary>
        /// <returns>代碼類型列表</returns>
        [HttpGet]
        public async Task<ActionResult<IList<CodeTypeDto>>> GetAllCodeTypes()
        {
            try
            {
                var codeTypes = await _codeTypeService.GetAllCodeTypesAsync();
                var dtos = codeTypes.Select(CodeTypeDto.FromEntity).ToList();
                return Ok(dtos);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得代碼類型列表失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 根據ID取得代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>代碼類型詳細資訊</returns>
        [HttpGet("{id}")]
        public async Task<ActionResult<CodeTypeDto>> GetCodeTypeById(int id)
        {
            try
            {
                var codeType = await _codeTypeService.GetCodeTypeByIdAsync(id);
                if (codeType == null)
                {
                    return NotFound(new { message = "找不到指定的代碼類型" });
                }

                return Ok(CodeTypeDto.FromEntity(codeType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得代碼類型失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 根據代碼類型代碼取得代碼類型
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <returns>代碼類型詳細資訊</returns>
        [HttpGet("by-code/{codeType}")]
        public async Task<ActionResult<CodeTypeDto>> GetCodeTypeByCode(string codeType)
        {
            try
            {
                var codeTypeEntity = await _codeTypeService.GetCodeTypeByCodeAsync(codeType);
                if (codeTypeEntity == null)
                {
                    return NotFound(new { message = "找不到指定的代碼類型" });
                }

                return Ok(CodeTypeDto.FromEntity(codeTypeEntity));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "取得代碼類型失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 建立新的代碼類型
        /// </summary>
        /// <param name="request">代碼類型建立請求</param>
        /// <returns>建立的代碼類型</returns>
        [HttpPost]
        public async Task<ActionResult<CodeTypeDto>> CreateCodeType([FromBody] CreateOrUpdateCodeTypeDto request)
        {
            try
            {
                // 驗證必要欄位
                if (string.IsNullOrWhiteSpace(request.CodeType))
                {
                    return BadRequest(new { message = "代碼類型代碼不能為空" });
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { message = "代碼類型名稱不能為空" });
                }

                // 檢查代碼類型是否已存在
                var exists = await _codeTypeService.CodeTypeExistsAsync(request.CodeType);
                if (exists)
                {
                    return Conflict(new { message = "代碼類型代碼已存在" });
                }

                var userName = GetCurrentUserName();
                var codeType = await _codeTypeService.CreateCodeTypeAsync(request, userName);

                return CreatedAtAction(
                    nameof(GetCodeTypeById),
                    new { id = codeType.Id },
                    CodeTypeDto.FromEntity(codeType)
                );
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "建立代碼類型失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 更新代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <param name="request">代碼類型更新請求</param>
        /// <returns>更新的代碼類型</returns>
        [HttpPut("{id}")]
        public async Task<ActionResult<CodeTypeDto>> UpdateCodeType(int id, [FromBody] CreateOrUpdateCodeTypeDto request)
        {
            try
            {
                // 驗證必要欄位
                if (string.IsNullOrWhiteSpace(request.CodeType))
                {
                    return BadRequest(new { message = "代碼類型代碼不能為空" });
                }

                if (string.IsNullOrWhiteSpace(request.Name))
                {
                    return BadRequest(new { message = "代碼類型名稱不能為空" });
                }

                // 檢查代碼類型是否已存在（排除當前記錄）
                var exists = await _codeTypeService.CodeTypeExistsAsync(request.CodeType, id);
                if (exists)
                {
                    return Conflict(new { message = "代碼類型代碼已存在" });
                }

                var userName = GetCurrentUserName();
                var codeType = await _codeTypeService.UpdateCodeTypeAsync(id, request, userName);

                if (codeType == null)
                {
                    return NotFound(new { message = "找不到指定的代碼類型" });
                }

                return Ok(CodeTypeDto.FromEntity(codeType));
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "更新代碼類型失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 刪除代碼類型
        /// </summary>
        /// <param name="id">代碼類型ID</param>
        /// <returns>刪除結果</returns>
        [HttpDelete("{id}")]
        public async Task<ActionResult> DeleteCodeType(int id)
        {
            try
            {
                var result = await _codeTypeService.DeleteCodeTypeAsync(id);
                if (!result)
                {
                    return NotFound(new { message = "找不到指定的代碼類型" });
                }

                return Ok(new { message = "代碼類型刪除成功" });
            }
            catch (InvalidOperationException ex)
            {
                return BadRequest(new { message = ex.Message });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "刪除代碼類型失敗", detail = ex.Message });
            }
        }

        /// <summary>
        /// 檢查代碼類型是否存在
        /// </summary>
        /// <param name="codeType">代碼類型代碼</param>
        /// <returns>檢查結果</returns>
        [HttpGet("exists/{codeType}")]
        public async Task<ActionResult<bool>> CheckCodeTypeExists(string codeType)
        {
            try
            {
                var exists = await _codeTypeService.CodeTypeExistsAsync(codeType);
                return Ok(new { exists = exists });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = "檢查代碼類型失敗", detail = ex.Message });
            }
        }
    }
}
