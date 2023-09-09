using PetSalon.Models;
using Microsoft.AspNetCore.Mvc;

namespace PetSalon.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly JwtHelpers _jwt;

        public AccountController(JwtHelpers jwt)
        {
            _jwt = jwt;
        }

        [HttpPost]
        public ActionResult Logon(Logon logon)
        {
            if (logon.UserName == "admin")
            {
                return Ok(new { token = _jwt.GenerateToken(logon.UserName, 20) });
            }
            else
            {
                return Unauthorized();
            }
        }
    }
}
