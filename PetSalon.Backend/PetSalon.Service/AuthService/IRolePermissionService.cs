using PetSalon.Models.DTOs;

namespace PetSalon.Services.AuthService;

public interface IRolePermissionService
{
    Task<IReadOnlyList<PermissionInfoDto>> GetPermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<IReadOnlyList<RolePermissionInfoDto>> GetRolePermissionsAsync(
        CancellationToken cancellationToken = default);

    Task<RolePermissionUpdateStatus> ReplaceRolePermissionsAsync(
        long roleId,
        IReadOnlyCollection<string>? permissionCodes,
        CancellationToken cancellationToken = default);
}

public enum RolePermissionUpdateStatus
{
    Succeeded,
    RoleNotFound,
    InvalidPermissionSet,
    PersistenceFailed
}
