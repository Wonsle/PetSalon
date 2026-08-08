using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using Microsoft.Extensions.DependencyInjection;
using PetSalon.Models.EntityModels;
using PetSalon.Services.AuthService;

namespace PetSalon.Web.Tests.Authentication;

public sealed class DefaultAdminInitializerTests
{
    [Fact]
    public void ProvisioningServicesUseScopedInitializerAndFrameworkPasswordHasher()
    {
        var services = new ServiceCollection();

        services.AddDefaultAdministratorProvisioning();

        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IDefaultAdminInitializer) &&
            descriptor.ImplementationType == typeof(DefaultAdminInitializer) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);
        Assert.Contains(services, descriptor =>
            descriptor.ServiceType == typeof(IPasswordHasher<Scuser>) &&
            descriptor.ImplementationType == typeof(PasswordHasher<Scuser>) &&
            descriptor.Lifetime == ServiceLifetime.Scoped);
    }

    [Fact]
    public async Task FreshDatabaseCreatesRestrictedDefaultAdministrator()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var hasher = new PasswordHasher<Scuser>();
        var initializer = new DefaultAdminInitializer(context, hasher);

        await initializer.InitializeAsync();

        var admin = await context.Scuser.SingleAsync();
        var role = await context.Scrole.SingleAsync(item => item.RoleName == "Admin");
        var userRole = await context.ScuserRole.SingleAsync();
        Assert.Equal("admin", admin.UserName);
        Assert.True(admin.IsActive);
        Assert.True(admin.MustChangePassword);
        Assert.NotEqual("password", admin.PasswordHash);
        Assert.Equal(PasswordVerificationResult.Success,
            hasher.VerifyHashedPassword(admin, admin.PasswordHash, "password"));
        Assert.Equal("Admin", role.RoleName);
        Assert.Equal(admin.ScuserId, userRole.ScuserId);
        Assert.Equal(role.RoleId, userRole.RoleId);

        var permissions = await context.Scpermission
            .AsNoTracking()
            .Select(item => item.PermissionCode)
            .OrderBy(item => item)
            .ToArrayAsync();
        Assert.Equal(
        [
            "accounts.manage",
            "auth.login",
            "files.delete.permanent",
            "finance.read",
            "permissions.manage",
            "system.settings.manage"
        ], permissions);

        var mappings = await context.ScrolePermission
            .AsNoTracking()
            .Include(item => item.Role)
            .Include(item => item.Permission)
            .Select(item => new { item.Role.RoleName, item.Permission.PermissionCode })
            .ToListAsync();
        Assert.Equal(6, mappings.Count(item => item.RoleName == "Admin"));
        Assert.Equal(
            ["auth.login", "finance.read"],
            mappings.Where(item => item.RoleName == "Manager")
                .Select(item => item.PermissionCode)
                .OrderBy(item => item));
        Assert.Equal(
            ["auth.login"],
            mappings.Where(item => item.RoleName == "Designer")
                .Select(item => item.PermissionCode));
    }

    [Fact]
    public async Task ExistingAdministratorIsNeverOverwritten()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var hasher = new PasswordHasher<Scuser>();
        var existing = new Scuser
        {
            UserName = "admin",
            PasswordHash = "custom-hash",
            IsActive = false,
            MustChangePassword = false,
            CreateUser = "owner",
            ModifyUser = "owner"
        };
        context.Scuser.Add(existing);
        await context.SaveChangesAsync();

        await new DefaultAdminInitializer(context, hasher).InitializeAsync();

        var admin = await context.Scuser.AsNoTracking().SingleAsync();
        Assert.Equal("custom-hash", admin.PasswordHash);
        Assert.False(admin.IsActive);
        Assert.False(admin.MustChangePassword);
        Assert.Empty(await context.ScuserRole.AsNoTracking().ToListAsync());
    }

    [Fact]
    public async Task ExistingPermissionMatrixIsNeverOverwritten()
    {
        await using var database = await TestDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var role = new Scrole { RoleName = "OwnerConfigured" };
        var permission = new Scpermission
        {
            PermissionCode = "custom.permission",
            Description = "Owner-managed permission",
            IsActive = true
        };
        context.ScrolePermission.Add(new ScrolePermission
        {
            Role = role,
            Permission = permission
        });
        await context.SaveChangesAsync();

        await new DefaultAdminInitializer(context, new PasswordHasher<Scuser>()).InitializeAsync();

        Assert.Equal(["custom.permission"],
            await context.Scpermission.AsNoTracking().Select(item => item.PermissionCode).ToArrayAsync());
        var mapping = await context.ScrolePermission.AsNoTracking().SingleAsync();
        Assert.Equal(role.RoleId, mapping.RoleId);
        Assert.Equal(permission.PermissionId, mapping.PermissionId);
    }

    [Fact]
    public async Task ConcurrentInitializationConvergesOnOneAdministrator()
    {
        await using var database = await TestDatabase.CreateAsync();
        var hasher = new PasswordHasher<Scuser>();
        await using var firstContext = database.CreateContext();
        await using var secondContext = database.CreateContext();
        var first = new DefaultAdminInitializer(firstContext, hasher);
        var second = new DefaultAdminInitializer(secondContext, hasher);

        await Task.WhenAll(first.InitializeAsync(), second.InitializeAsync());

        await using var verificationContext = database.CreateContext();
        Assert.Equal(1, await verificationContext.Scuser.CountAsync(user => user.UserName == "admin"));
        Assert.Equal(1, await verificationContext.Scrole.CountAsync(role => role.RoleName == "Admin"));
        Assert.Equal(1, await verificationContext.ScuserRole.CountAsync());
        Assert.Equal(6, await verificationContext.Scpermission.CountAsync());
        Assert.Equal(9, await verificationContext.ScrolePermission.CountAsync());
    }

    private sealed class TestDatabase : IAsyncDisposable
    {
        private readonly string _databaseName = $"default-admin-{Guid.NewGuid():N}";
        private readonly InMemoryDatabaseRoot _databaseRoot = new();

        private TestDatabase()
        {
        }

        public static async Task<TestDatabase> CreateAsync()
        {
            var database = new TestDatabase();
            await using var context = database.CreateContext();
            await context.Database.EnsureCreatedAsync();
            return database;
        }

        public PetSalonContext CreateContext()
        {
            var options = new DbContextOptionsBuilder<PetSalonContext>()
                .UseInMemoryDatabase(_databaseName, _databaseRoot)
                .Options;
            return new PetSalonContext(options);
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
