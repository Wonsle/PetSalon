using Microsoft.AspNetCore.Mvc;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 基礎控制器，提供通用的異常處理功能
    /// </summary>
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        /// <summary>
        /// 處理異常並返回統一的錯誤響應格式 (泛型版本)
        /// </summary>
        /// <typeparam name="T">返回數據類型</typeparam>
        /// <param name="ex">異常對象</param>
        /// <returns>統一格式的錯誤響應</returns>
        protected ActionResult<T> HandleException<T>(Exception ex)
        {
            // Log the exception here if needed
            return StatusCode(500, new { message = "系統發生錯誤", error = ex.Message });
        }

        /// <summary>
        /// 處理異常並返回統一的錯誤響應格式
        /// </summary>
        /// <param name="ex">異常對象</param>
        /// <returns>統一格式的錯誤響應</returns>
        protected IActionResult HandleException(Exception ex)
        {
            // Log the exception here if needed
            return StatusCode(500, new { message = "系統發生錯誤", error = ex.Message });
        }
    }
}