using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;

namespace PetSalon.Web.Controllers
{
    /// <summary>
    /// 帳號管理API控制器 - 提供登入認證和使用者資訊功能
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class AccountController : ControllerBase
    {
        public readonly JwtHelpers _jwt;

        public AccountController(JwtHelpers jwt)
        {
            _jwt = jwt;
        }

        /// <summary>
        /// 使用者登入認證
        /// </summary>
        /// <param name="logon">登入資訊</param>
        /// <returns>JWT Token和使用者資訊</returns>
        [HttpPost("login")]
        public ActionResult<LoginResponse> Login(Logon logon)
        {
            // Validate input parameters
            if (logon == null || string.IsNullOrEmpty(logon.UserName) || string.IsNullOrEmpty(logon.Password))
            {
                return BadRequest(new { message = "使用者名稱和密碼為必填項目" });
            }

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
                
                var response = new LoginResponse
                {
                    Token = token,
                    User = new UserInfo
                    {
                        Id = testAccounts.Keys.ToList().IndexOf(logon.UserName.ToLower()) + 1,
                        UserName = logon.UserName,
                        Name = GetDisplayName(logon.UserName),
                        Roles = account.roles,
                        LastLogin = DateTime.Now
                    },
                    ExpiresIn = 480 * 60 // seconds
                };
                
                return Ok(response);
            }
            
            return Unauthorized(new { message = "帳號或密碼錯誤" });
        }

        /// <summary>
        /// 取得目前使用者資訊檔案
        /// </summary>
        /// <returns>使用者資訊</returns>
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

        /// <summary>
        /// 根據使用者名稱取得顯示名稱
        /// </summary>
        /// <param name="userName">使用者名稱</param>
        /// <returns>顯示名稱</returns>
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
