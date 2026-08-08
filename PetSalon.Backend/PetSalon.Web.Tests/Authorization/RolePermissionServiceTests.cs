using Microsoft.EntityFrameworkCore;
using PetSalon.Models.Authorization;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Services.AuthService;
using PetSalon.Web.Authorization;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Tests.Authorization;

public sealed class RolePermissionServiceTests
{
    [Fact]
    public async Task ReplacementStoresExactlyTheRequestedPermissionSet()
    {
        await using var context = CreateContext();
        var role = await SeedAsync(context);
        var service = new RolePermissionService(context);

        var result = await service.ReplaceRolePermissionsAsync(
            role.RoleId,
            [PermissionCodes.Login, PermissionCodes.ReadFinancialData]);

        Assert.Equal(RolePermissionUpdateStatus.Succeeded, result);
        Assert.Equal(
            [PermissionCodes.Login, PermissionCodes.ReadFinancialData],
            await CurrentCodesAsync(context, role.RoleId));
    }

    [Theory]
    [InlineData("unknown.permission", null)]
    [InlineData("disabled.permission", null)]
    [InlineData("auth.login", "AUTH.LOGIN")]
    public async Task InvalidPermissionInputPreservesOriginalMappings(string first, string? second)
    {
        await using var context = CreateContext();
        var role = await SeedAsync(context);
        var requested = second is null ? new[] { first } : new[] { first, second };
        var service = new RolePermissionService(context);

        var result = await service.ReplaceRolePermissionsAsync(role.RoleId, requested);

        Assert.Equal(RolePermissionUpdateStatus.InvalidPermissionSet, result);
        Assert.Equal([PermissionCodes.ManageSystemSettings], await CurrentCodesAsync(context, role.RoleId));
    }

    [Fact]
    public async Task PersistenceFailureRollsBackCompletePermissionSet()
    {
        await using var context = CreateContext();
        var role = await SeedAsync(context);
        context.FailSaves = true;
        var service = new RolePermissionService(context);

        var result = await service.ReplaceRolePermissionsAsync(
            role.RoleId,
            [PermissionCodes.Login, PermissionCodes.ReadFinancialData]);

        Assert.Equal(RolePermissionUpdateStatus.PersistenceFailed, result);
        Assert.Equal([PermissionCodes.ManageSystemSettings], await CurrentCodesAsync(context, role.RoleId));
    }

    [Fact]
    public async Task PermissionQueriesReturnCatalogAndCurrentRoleMappings()
    {
        await using var context = CreateContext();
        var role = await SeedAsync(context);
        var service = new RolePermissionService(context);

        var permissions = await service.GetPermissionsAsync();
        var roles = await service.GetRolePermissionsAsync();

        Assert.Contains(permissions, item =>
            item.PermissionCode == "disabled.permission" && !item.IsActive);
        var roleResult = Assert.Single(roles);
        Assert.Equal(role.RoleId, roleResult.RoleId);
        Assert.Equal([PermissionCodes.ManageSystemSettings], roleResult.PermissionCodes);
    }

    [Fact]
    public void PermissionControllerIsProtectedByPermissionInsteadOfRoleName()
    {
        var attribute = Assert.Single(
            typeof(PermissionController).GetCustomAttributes(typeof(RequirePermissionAttribute), true)
                .Cast<RequirePermissionAttribute>());

        Assert.Equal(PermissionCodes.ManagePermissions, attribute.PermissionCode);
        Assert.True(string.IsNullOrWhiteSpace(attribute.Roles));
    }

    private static ThrowingPetSalonContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PetSalonContext>()
            .UseInMemoryDatabase($"role-permissions-{Guid.NewGuid():N}")
            .Options;
        return new ThrowingPetSalonContext(options);
    }

    private static async Task<Scrole> SeedAsync(ThrowingPetSalonContext context)
    {
        var role = new Scrole { RoleName = "ConfigurableRole" };
        var permissions = new[]
        {
            CreatePermission(PermissionCodes.Login),
            CreatePermission(PermissionCodes.ReadFinancialData),
            CreatePermission(PermissionCodes.ManageSystemSettings),
            CreatePermission("disabled.permission", isActive: false)
        };
        context.ScrolePermission.Add(new ScrolePermission
        {
            Role = role,
            Permission = permissions[2]
        });
        context.Scpermission.AddRange(permissions.Where(item => item != permissions[2]));
        await context.SaveChangesAsync();
        return role;
    }

    private static Scpermission CreatePermission(string code, bool isActive = true) =>
        new() { PermissionCode = code, Description = code, IsActive = isActive };

    private static async Task<string[]> CurrentCodesAsync(PetSalonContext context, long roleId)
    {
        context.ChangeTracker.Clear();
        return await context.ScrolePermission
            .AsNoTracking()
            .Where(item => item.RoleId == roleId)
            .Select(item => item.Permission.PermissionCode)
            .OrderBy(item => item)
            .ToArrayAsync();
    }

    private sealed class ThrowingPetSalonContext(DbContextOptions<PetSalonContext> options)
        : PetSalonContext(options)
    {
        public bool FailSaves { get; set; }

        public override Task<int> SaveChangesAsync(CancellationToken cancellationToken = default) =>
            FailSaves
                ? Task.FromException<int>(new DbUpdateException("simulated persistence failure"))
                : base.SaveChangesAsync(cancellationToken);
    }
}
