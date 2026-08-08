using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.DTOs;
using PetSalon.Services.AuthService;
using PetSalon.Web.Authorization;

namespace PetSalon.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
public sealed class PasswordController : ControllerBase
{
    private readonly IAuthService _authService;

    public PasswordController(IAuthService authService)
    {
        _authService = authService;
    }

    [Authorize(Policy = PetSalonAuthorization.PasswordChangeOnly)]
    [HttpPost("change")]
    public async Task<IActionResult> Change(
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (!long.TryParse(User.FindFirstValue(ClaimTypes.NameIdentifier), out var userId))
        {
            return Unauthorized(new { message = "登入資訊無效" });
        }

        var result = await _authService.ChangePasswordAsync(userId, request, cancellationToken);
        return result switch
        {
            PasswordChangeStatus.Succeeded => NoContent(),
            PasswordChangeStatus.InvalidRequest => BadRequest(new { message = "新密碼不符合安全規則" }),
            PasswordChangeStatus.InvalidCredentials => Unauthorized(new { message = "目前密碼錯誤" }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = "無法更新密碼" })
        };
    }
}
