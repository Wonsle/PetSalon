using System.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetSalon.Models.Authorization;
using PetSalon.Models.EntityModels;

namespace PetSalon.Services.AuthService;

public sealed class DefaultAdminInitializer : IDefaultAdminInitializer
{
    private const string DefaultUserName = "admin";
    private const string DefaultPassword = "password";
    private const string AdministratorRoleName = "Admin";
    private const string SystemUser = "SYSTEM";
    private static readonly SemaphoreSlim InitializationLock = new(1, 1);

    private readonly PetSalonContext _context;
    private readonly IPasswordHasher<Scuser> _passwordHasher;

    public DefaultAdminInitializer(
        PetSalonContext context,
        IPasswordHasher<Scuser> passwordHasher)
    {
        _context = context;
        _passwordHasher = passwordHasher;
    }

    public async Task InitializeAsync(CancellationToken cancellationToken = default)
    {
        await InitializationLock.WaitAsync(cancellationToken);
        try
        {
            if (!await AdminExistsAsync(cancellationToken))
            {
                await CreateDefaultAdministratorAsync(cancellationToken);
            }

            await InitializePermissionMatrixAsync(cancellationToken);
        }
        finally
        {
            InitializationLock.Release();
        }
    }

    private async Task InitializePermissionMatrixAsync(CancellationToken cancellationToken)
    {
        if (await _context.Scpermission.AsNoTracking().AnyAsync(cancellationToken) ||
            await _context.ScrolePermission.AsNoTracking().AnyAsync(cancellationToken))
        {
            return;
        }

        await using var transaction = await BeginTransactionAsync(cancellationToken);
        try
        {
            var roles = await _context.Scrole.ToDictionaryAsync(
                item => item.RoleName,
                StringComparer.OrdinalIgnoreCase,
                cancellationToken);
            foreach (var roleName in new[] { "Admin", "Manager", "Designer" })
            {
                if (!roles.ContainsKey(roleName))
                {
                    var role = new Scrole { RoleName = roleName };
                    roles.Add(roleName, role);
                    _context.Scrole.Add(role);
                }
            }

            var permissions = PermissionCodes.Baseline.ToDictionary(
                code => code,
                code => new Scpermission
                {
                    PermissionCode = code,
                    Description = code,
                    IsActive = true
                },
                StringComparer.Ordinal);
            _context.Scpermission.AddRange(permissions.Values);

            AddMappings(roles["Admin"], PermissionCodes.Baseline, permissions);
            AddMappings(
                roles["Manager"],
                [PermissionCodes.Login, PermissionCodes.ReadFinancialData],
                permissions);
            AddMappings(roles["Designer"], [PermissionCodes.Login], permissions);

            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            throw;
        }
    }

    private void AddMappings(
        Scrole role,
        IEnumerable<string> permissionCodes,
        IReadOnlyDictionary<string, Scpermission> permissions)
    {
        foreach (var code in permissionCodes)
        {
            _context.ScrolePermission.Add(new ScrolePermission
            {
                Role = role,
                Permission = permissions[code]
            });
        }
    }

    private Task<bool> AdminExistsAsync(CancellationToken cancellationToken) =>
        _context.Scuser.AsNoTracking().AnyAsync(
            user => user.UserName == DefaultUserName,
            cancellationToken);

    private async Task CreateDefaultAdministratorAsync(CancellationToken cancellationToken)
    {
        await using var transaction = await BeginTransactionAsync(cancellationToken);
        try
        {
            var role = await _context.Scrole.SingleOrDefaultAsync(
                item => item.RoleName == AdministratorRoleName,
                cancellationToken);

            if (role is null)
            {
                role = new Scrole { RoleName = AdministratorRoleName };
                _context.Scrole.Add(role);
            }

            var now = DateTime.UtcNow;
            var admin = new Scuser
            {
                UserName = DefaultUserName,
                IsActive = true,
                MustChangePassword = true,
                CreateUser = SystemUser,
                CreateTime = now,
                ModifyUser = SystemUser,
                ModifyTime = now
            };
            admin.PasswordHash = _passwordHasher.HashPassword(admin, DefaultPassword);

            _context.Scuser.Add(admin);
            _context.ScuserRole.Add(new ScuserRole
            {
                Scuser = admin,
                Role = role
            });

            await _context.SaveChangesAsync(cancellationToken);
            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }
        }
        catch (DbUpdateException)
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(cancellationToken);
            }

            _context.ChangeTracker.Clear();
            if (await AdminExistsAsync(cancellationToken))
            {
                return;
            }

            throw;
        }
    }

    private async Task<IDbContextTransaction?> BeginTransactionAsync(
        CancellationToken cancellationToken)
    {
        if (!_context.Database.IsRelational())
        {
            return null;
        }

        return await _context.Database.BeginTransactionAsync(
            IsolationLevel.Serializable,
            cancellationToken);
    }
}
