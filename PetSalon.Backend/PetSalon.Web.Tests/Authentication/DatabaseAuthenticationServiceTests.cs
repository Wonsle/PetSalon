using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetSalon.Models.Authorization;
using PetSalon.Models.EntityModels;
using PetSalon.Services.AuthService;

namespace PetSalon.Web.Tests.Authentication;

public sealed class DatabaseAuthenticationServiceTests
{
    private static readonly DateTime PreviousLogin = new(2026, 8, 1, 8, 0, 0, DateTimeKind.Utc);
    private static readonly DateTime CurrentLogin = new(2026, 8, 7, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ValidActiveUserVerifiesHashAndUpdatesLastLogin()
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var user = await database.AddUserAsync(context, "admin", "correct-password", isActive: true);
        await database.AddRoleWithPermissionsAsync(context, user, "Admin", PermissionCodes.Login);
        var service = CreateService(context);

        var result = await service.AuthenticateAsync(" ADMIN ", "correct-password");

        Assert.Equal(AuthenticationStatus.Succeeded, result.Status);
        Assert.Equal(user.ScuserId, result.User?.UserId);
        Assert.Equal("admin", result.User?.UserName);
        Assert.Equal(CurrentLogin, result.User?.LastLogin);
        Assert.Equal(CurrentLogin, (await context.Scuser.SingleAsync()).LastLogin);
    }

    [Theory]
    [InlineData("missing", "correct-password", true)]
    [InlineData("admin", "wrong-password", true)]
    [InlineData("admin", "correct-password", false)]
    public async Task InvalidLoginVariantsReturnSameResultAndDoNotUpdateLastLogin(
        string userName,
        string password,
        bool isActive)
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        await database.AddUserAsync(context, "admin", "correct-password", isActive);
        var service = CreateService(context);

        var result = await service.AuthenticateAsync(userName, password);

        Assert.Equal(AuthenticationStatus.InvalidCredentials, result.Status);
        Assert.Null(result.User);
        Assert.Equal(PreviousLogin, (await context.Scuser.SingleAsync()).LastLogin);
    }

    [Fact]
    public async Task SuccessfulLoginLoadsEveryDatabaseRole()
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var user = await database.AddUserAsync(context, "admin", "correct-password", isActive: true);
        await database.AddRoleWithPermissionsAsync(context, user, "Admin", PermissionCodes.Login);
        await database.AddRoleWithPermissionsAsync(context, user, "Manager", PermissionCodes.ReadFinancialData);

        var result = await CreateService(context).AuthenticateAsync("admin", "correct-password");

        Assert.Equal(AuthenticationStatus.Succeeded, result.Status);
        Assert.NotNull(result.User);
        Assert.Equal(user.ScuserId, result.User.UserId);
        Assert.Equal(["Admin", "Manager"], result.User.Roles.OrderBy(role => role));
    }

    [Fact]
    public async Task UserWithoutLoginPermissionCannotReceiveGeneralAuthenticationResult()
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var user = await database.AddUserAsync(context, "admin", "correct-password", isActive: true);
        await database.AddRoleWithPermissionsAsync(
            context,
            user,
            "Bookkeeper",
            PermissionCodes.ReadFinancialData);

        var result = await CreateService(context).AuthenticateAsync("admin", "correct-password");

        Assert.Equal(AuthenticationStatus.MissingPermission, result.Status);
        Assert.Null(result.User);
        Assert.Equal(CurrentLogin, (await context.Scuser.SingleAsync()).LastLogin);
    }

    [Fact]
    public async Task PermissionsFromMultipleRolesAreDeduplicated()
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var user = await database.AddUserAsync(context, "admin", "correct-password", isActive: true);
        await database.AddRoleWithPermissionsAsync(
            context,
            user,
            "Operations",
            PermissionCodes.Login,
            PermissionCodes.ReadFinancialData);
        await database.AddRoleWithPermissionsAsync(
            context,
            user,
            "SecondRole",
            PermissionCodes.Login);

        var result = await CreateService(context).AuthenticateAsync("admin", "correct-password");

        Assert.Equal(AuthenticationStatus.Succeeded, result.Status);
        Assert.Equal(
            [PermissionCodes.Login, PermissionCodes.ReadFinancialData],
            result.User!.Permissions.OrderBy(code => code));
    }

    [Fact]
    public async Task PermissionMappingChangeAffectsNextAuthenticationOnly()
    {
        await using var database = await AuthenticationDatabase.CreateAsync();
        await using var context = database.CreateContext();
        var user = await database.AddUserAsync(context, "admin", "correct-password", isActive: true);
        await database.AddRoleWithPermissionsAsync(context, user, "DynamicRole", PermissionCodes.Login);
        var service = CreateService(context);

        var first = await service.AuthenticateAsync("admin", "correct-password");
        var loginMapping = await context.ScrolePermission
            .Include(item => item.Permission)
            .SingleAsync(item => item.Permission.PermissionCode == PermissionCodes.Login);
        context.ScrolePermission.Remove(loginMapping);
        await context.SaveChangesAsync();

        var second = await service.AuthenticateAsync("admin", "correct-password");

        Assert.Contains(PermissionCodes.Login, first.User!.Permissions);
        Assert.Equal(AuthenticationStatus.MissingPermission, second.Status);
        Assert.Null(second.User);
        Assert.Contains(PermissionCodes.Login, first.User.Permissions);
    }

    private static AuthService CreateService(PetSalonContext context) =>
        new(context, new PasswordHasher<Scuser>(), new FixedTimeProvider(CurrentLogin));

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utcNow);
    }

    private sealed class AuthenticationDatabase : IAsyncDisposable
    {
        private readonly string _databaseName = $"authentication-{Guid.NewGuid():N}";
        private readonly InMemoryDatabaseRoot _databaseRoot = new();

        public static async Task<AuthenticationDatabase> CreateAsync()
        {
            var database = new AuthenticationDatabase();
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

        public async Task<Scuser> AddUserAsync(
            PetSalonContext context,
            string userName,
            string password,
            bool isActive)
        {
            var user = new Scuser
            {
                UserName = userName,
                IsActive = isActive,
                MustChangePassword = false,
                LastLogin = PreviousLogin,
                CreateUser = "TEST",
                ModifyUser = "TEST"
            };
            user.PasswordHash = new PasswordHasher<Scuser>().HashPassword(user, password);
            context.Scuser.Add(user);
            await context.SaveChangesAsync();
            return user;
        }

        public async Task AddRoleWithPermissionsAsync(
            PetSalonContext context,
            Scuser user,
            string roleName,
            params string[] permissionCodes)
        {
            var role = new Scrole { RoleName = roleName };
            context.ScuserRole.Add(new ScuserRole { Scuser = user, Role = role });
            foreach (var permissionCode in permissionCodes)
            {
                var permission = await context.Scpermission
                    .SingleOrDefaultAsync(item => item.PermissionCode == permissionCode)
                    ?? new Scpermission
                    {
                        PermissionCode = permissionCode,
                        Description = permissionCode,
                        IsActive = true
                    };
                context.ScrolePermission.Add(new ScrolePermission
                {
                    Role = role,
                    Permission = permission
                });
            }

            await context.SaveChangesAsync();
        }

        public ValueTask DisposeAsync() => ValueTask.CompletedTask;
    }
}
