using Microsoft.AspNetCore.Mvc;

namespace PetSalon.Web.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BaseController : ControllerBase
    {
        protected ActionResult<T> HandleException<T>(Exception ex)
        {
            // Log the exception here if needed
            return StatusCode(500, new { message = "系統發生錯誤", error = ex.Message });
        }

        protected IActionResult HandleException(Exception ex)
        {
            // Log the exception here if needed
            return StatusCode(500, new { message = "系統發生錯誤", error = ex.Message });
        }
    }
}