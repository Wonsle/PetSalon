using PetSalon.Models;
using Microsoft.AspNetCore.Mvc;

namespace PetSalon.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly JwtHelpers _jwt;

        public AccountController(JwtHelpers jwt)
        {
            _jwt = jwt;
        }

        [HttpPost("login")]
        public ActionResult Login(Logon logon)
        {
            // 驗證測試帳號
            var testAccounts = new Dictionary<string, (string password, string[] roles)>
            {
                { "admin", ("admin123", new[] { "Admin", "Manager", "Designer" }) },
                { "manager", ("manager123", new[] { "Manager", "Designer" }) },
                { "stylist", ("stylist123", new[] { "Designer" }) }
            };

            if (testAccounts.TryGetValue(logon.UserName.ToLower(), out var account) && 
                account.password == logon.Password)
            {
                var token = _jwt.GenerateToken(logon.UserName, 480); // 8 hours
                
                return Ok(new 
                {
                    token = token,
                    user = new 
                    {
                        id = testAccounts.Keys.ToList().IndexOf(logon.UserName.ToLower()) + 1,
                        userName = logon.UserName,
                        name = GetDisplayName(logon.UserName),
                        roles = account.roles,
                        lastLogin = DateTime.Now
                    },
                    expiresIn = 480 * 60 // seconds
                });
            }
            
            return Unauthorized(new { message = "帳號或密碼錯誤" });
        }

        [HttpGet("profile")]
        public ActionResult GetProfile()
        {
            // TODO: 從 JWT token 中取得使用者資訊
            var userName = User.Identity?.Name ?? "admin";
            
            return Ok(new 
            {
                id = 1,
                userName = userName,
                name = GetDisplayName(userName),
                roles = new[] { "Admin" },
                lastLogin = DateTime.Now
            });
        }

        private string GetDisplayName(string userName)
        {
            return userName.ToLower() switch
            {
                "admin" => "系統管理員",
                "manager" => "店長",
                "stylist" => "設計師",
                _ => userName
            };
        }
    }
}
