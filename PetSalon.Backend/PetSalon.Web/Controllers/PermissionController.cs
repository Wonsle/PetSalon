using Microsoft.AspNetCore.Mvc;
using PetSalon.Models.Authorization;
using PetSalon.Models.DTOs;
using PetSalon.Services.AuthService;
using PetSalon.Web.Authorization;

namespace PetSalon.Web.Controllers;

[ApiController]
[Route("api/[controller]")]
[RequirePermission(PermissionCodes.ManagePermissions)]
public sealed class PermissionController : ControllerBase
{
    private readonly IRolePermissionService _rolePermissionService;

    public PermissionController(IRolePermissionService rolePermissionService)
    {
        _rolePermissionService = rolePermissionService;
    }

    [HttpGet]
    public async Task<ActionResult<IReadOnlyList<PermissionInfoDto>>> GetPermissions(
        CancellationToken cancellationToken) =>
        Ok(await _rolePermissionService.GetPermissionsAsync(cancellationToken));

    [HttpGet("roles")]
    public async Task<ActionResult<IReadOnlyList<RolePermissionInfoDto>>> GetRolePermissions(
        CancellationToken cancellationToken) =>
        Ok(await _rolePermissionService.GetRolePermissionsAsync(cancellationToken));

    [HttpPut("roles/{roleId:long}")]
    public async Task<IActionResult> ReplaceRolePermissions(
        long roleId,
        ReplaceRolePermissionsRequest request,
        CancellationToken cancellationToken)
    {
        var result = await _rolePermissionService.ReplaceRolePermissionsAsync(
            roleId,
            request?.PermissionCodes,
            cancellationToken);
        return result switch
        {
            RolePermissionUpdateStatus.Succeeded => NoContent(),
            RolePermissionUpdateStatus.RoleNotFound => NotFound(new { message = "找不到指定角色" }),
            RolePermissionUpdateStatus.InvalidPermissionSet => BadRequest(new { message = "權限集合無效" }),
            _ => StatusCode(StatusCodes.Status500InternalServerError, new { message = "無法更新角色權限" })
        };
    }
}
