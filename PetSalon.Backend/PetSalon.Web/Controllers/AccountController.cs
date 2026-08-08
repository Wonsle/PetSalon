using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services.AuthService;

namespace PetSalon.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class AccountController : ControllerBase
{
    private readonly IAuthService _authService;
    private readonly JwtHelpers _jwt;

    public AccountController(IAuthService authService, JwtHelpers jwt)
    {
        _authService = authService;
        _jwt = jwt;
    }

    [AllowAnonymous]
    [HttpPost("login")]
    public async Task<ActionResult<LoginResponse>> Login(
        Logon logon,
        CancellationToken cancellationToken = default)
    {
        if (logon is null || string.IsNullOrWhiteSpace(logon.UserName) || string.IsNullOrEmpty(logon.Password))
        {
            return BadRequest(new { message = "使用者名稱和密碼為必填項目" });
        }

        var result = await _authService.AuthenticateAsync(
            logon.UserName,
            logon.Password,
            cancellationToken);
        if (result.Status == AuthenticationStatus.InvalidCredentials)
        {
            return Unauthorized(new { message = "帳號或密碼錯誤" });
        }

        if (result.Status == AuthenticationStatus.MissingPermission || result.User is null)
        {
            return StatusCode(StatusCodes.Status403Forbidden, new { message = "帳號沒有系統存取權限" });
        }

        var user = result.User;
        var token = user.MustChangePassword
            ? _jwt.GeneratePasswordChangeToken(user)
            : _jwt.GenerateAccessToken(user);
        var responseRoles = user.MustChangePassword
            ? Array.Empty<string>()
            : user.Roles.ToArray();
        var responsePermissions = user.MustChangePassword
            ? Array.Empty<string>()
            : user.Permissions.ToArray();

        return Ok(new LoginResponse
        {
            Token = token.Token,
            ExpiresIn = token.ExpiresIn,
            RequiresPasswordChange = user.MustChangePassword,
            User = new UserInfo
            {
                Id = user.UserId,
                UserName = user.UserName,
                Name = user.UserName,
                Roles = responseRoles,
                Permissions = responsePermissions,
                LastLogin = user.LastLogin
            }
        });
    }

    [HttpGet("profile")]
    public ActionResult GetProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        var userName = User.Identity?.Name;
        var roles = User.FindAll(ClaimTypes.Role).Select(claim => claim.Value).ToArray();

        return Ok(new { id = userId, userName, name = userName, roles });
    }
}
