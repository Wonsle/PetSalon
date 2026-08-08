namespace PetSalon.Models.DTOs;

public sealed record PermissionInfoDto(
    long PermissionId,
    string PermissionCode,
    string? Description,
    bool IsActive);

public sealed record RolePermissionInfoDto(
    long RoleId,
    string RoleName,
    IReadOnlyList<string> PermissionCodes);

public sealed class ReplaceRolePermissionsRequest
{
    public string[] PermissionCodes { get; set; } = Array.Empty<string>();
}
