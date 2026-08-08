using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;

namespace PetSalon.Services.AuthService;

public sealed class RolePermissionService : IRolePermissionService
{
    private readonly PetSalonContext _context;

    public RolePermissionService(PetSalonContext context)
    {
        _context = context;
    }

    public async Task<IReadOnlyList<PermissionInfoDto>> GetPermissionsAsync(
        CancellationToken cancellationToken = default) =>
        await _context.Scpermission
            .AsNoTracking()
            .OrderBy(item => item.PermissionCode)
            .Select(item => new PermissionInfoDto(
                item.PermissionId,
                item.PermissionCode,
                item.Description,
                item.IsActive))
            .ToListAsync(cancellationToken);

    public async Task<IReadOnlyList<RolePermissionInfoDto>> GetRolePermissionsAsync(
        CancellationToken cancellationToken = default)
    {
        var roles = await _context.Scrole
            .AsNoTracking()
            .Include(item => item.ScrolePermissions)
            .ThenInclude(item => item.Permission)
            .OrderBy(item => item.RoleName)
            .ToListAsync(cancellationToken);

        return roles
            .Select(role => new RolePermissionInfoDto(
                role.RoleId,
                role.RoleName,
                role.ScrolePermissions
                    .Select(item => item.Permission.PermissionCode)
                    .OrderBy(code => code, StringComparer.Ordinal)
                    .ToArray()))
            .ToArray();
    }

    public async Task<RolePermissionUpdateStatus> ReplaceRolePermissionsAsync(
        long roleId,
        IReadOnlyCollection<string>? permissionCodes,
        CancellationToken cancellationToken = default)
    {
        if (permissionCodes is null)
        {
            return RolePermissionUpdateStatus.InvalidPermissionSet;
        }

        var normalizedCodes = permissionCodes
            .Select(code => code?.Trim())
            .ToArray();
        if (normalizedCodes.Any(string.IsNullOrWhiteSpace) ||
            normalizedCodes.Distinct(StringComparer.OrdinalIgnoreCase).Count() != normalizedCodes.Length)
        {
            return RolePermissionUpdateStatus.InvalidPermissionSet;
        }

        if (!await _context.Scrole.AsNoTracking().AnyAsync(
                role => role.RoleId == roleId,
                cancellationToken))
        {
            return RolePermissionUpdateStatus.RoleNotFound;
        }

        var activePermissions = await _context.Scpermission
            .Where(permission => permission.IsActive)
            .ToListAsync(cancellationToken);
        var permissionsByCode = activePermissions.ToDictionary(
            permission => permission.PermissionCode,
            StringComparer.OrdinalIgnoreCase);
        if (normalizedCodes.Any(code => !permissionsByCode.ContainsKey(code!)))
        {
            return RolePermissionUpdateStatus.InvalidPermissionSet;
        }

        IDbContextTransaction? transaction = null;
        try
        {
            if (_context.Database.IsRelational())
            {
                transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }

            var existing = await _context.ScrolePermission
                .Where(item => item.RoleId == roleId)
                .ToListAsync(cancellationToken);
            _context.ScrolePermission.RemoveRange(existing);
            _context.ScrolePermission.AddRange(normalizedCodes.Select(code => new ScrolePermission
            {
                RoleId = roleId,
                PermissionId = permissionsByCode[code!].PermissionId
            }));
            await _context.SaveChangesAsync(cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return RolePermissionUpdateStatus.Succeeded;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }

            _context.ChangeTracker.Clear();
            return RolePermissionUpdateStatus.PersistenceFailed;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }
}
