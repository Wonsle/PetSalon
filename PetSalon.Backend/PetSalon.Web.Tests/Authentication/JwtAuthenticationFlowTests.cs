using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using PetSalon.Models.DTOs;
using PetSalon.Models.Authorization;
using PetSalon.Services.AuthService;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Tests.Authentication;

public sealed class JwtAuthenticationFlowTests
{
    private static readonly DateTime Now = new(2026, 8, 7, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public void GeneralTokenContainsDatabaseIdentityAndEveryRole()
    {
        var user = CreateUser(
            mustChangePassword: false,
            roles: ["Admin", "Manager"],
            permissions: [PermissionCodes.Login, PermissionCodes.ReadFinancialData]);

        var token = ReadToken(CreateJwt().GenerateAccessToken(user).Token);

        Assert.Equal("7", token.Subject);
        Assert.Contains(token.Claims, claim => claim.Type == ClaimTypes.Name && claim.Value == "admin");
        Assert.Equal(
            ["Admin", "Manager"],
            token.Claims.Where(claim => claim.Type == ClaimTypes.Role)
                .Select(claim => claim.Value)
                .OrderBy(role => role));
        Assert.Equal(
            [PermissionCodes.Login, PermissionCodes.ReadFinancialData],
            token.Claims.Where(claim => claim.Type == JwtHelpers.PermissionClaimType)
                .Select(claim => claim.Value)
                .OrderBy(code => code));
        Assert.DoesNotContain(token.Claims, claim => claim.Type == "token_use");
    }

    [Fact]
    public void PasswordChangeTokenIsShortLivedAndContainsNoBusinessRoles()
    {
        var user = CreateUser(
            mustChangePassword: true,
            roles: ["Admin"],
            permissions: [PermissionCodes.Login, PermissionCodes.ManagePermissions]);

        var token = ReadToken(CreateJwt().GeneratePasswordChangeToken(user).Token);

        Assert.Equal("7", token.Subject);
        Assert.Contains(token.Claims, claim => claim.Type == "token_use" && claim.Value == "password_change");
        Assert.DoesNotContain(token.Claims, claim => claim.Type == ClaimTypes.Role);
        Assert.DoesNotContain(token.Claims, claim => claim.Type == JwtHelpers.PermissionClaimType);
        Assert.InRange(token.ValidTo - token.ValidFrom, TimeSpan.Zero, TimeSpan.FromMinutes(15));
    }

    [Fact]
    public async Task InitialAdministratorLoginReturnsOnlyRestrictedSession()
    {
        var user = CreateUser(
            mustChangePassword: true,
            roles: ["Admin"],
            permissions: [PermissionCodes.Login, PermissionCodes.ManagePermissions]);
        var controller = new AccountController(
            new StubAuthService(new AuthenticationResult(AuthenticationStatus.Succeeded, user)),
            CreateJwt());

        var action = await controller.Login(new Logon { UserName = "admin", Password = "password" });

        var ok = Assert.IsType<OkObjectResult>(action.Result);
        var response = Assert.IsType<LoginResponse>(ok.Value);
        Assert.True(response.RequiresPasswordChange);
        Assert.Empty(response.User.Roles);
        Assert.Empty(response.User.Permissions);
        var token = ReadToken(response.Token);
        Assert.Contains(token.Claims, claim => claim.Type == "token_use" && claim.Value == "password_change");
        Assert.DoesNotContain(token.Claims, claim => claim.Type == ClaimTypes.Role);
    }

    [Fact]
    public async Task GeneralLoginResponseContainsResolvedPermissions()
    {
        var user = CreateUser(
            mustChangePassword: false,
            roles: ["RenamedRole"],
            permissions: [PermissionCodes.Login, PermissionCodes.ReadFinancialData]);
        var controller = new AccountController(
            new StubAuthService(new AuthenticationResult(AuthenticationStatus.Succeeded, user)),
            CreateJwt());

        var action = await controller.Login(new Logon { UserName = "admin", Password = "changed-password" });

        var response = Assert.IsType<LoginResponse>(Assert.IsType<OkObjectResult>(action.Result).Value);
        Assert.Equal([PermissionCodes.Login, PermissionCodes.ReadFinancialData], response.User.Permissions);
    }

    private static AuthenticatedUser CreateUser(
        bool mustChangePassword,
        string[] roles,
        string[] permissions) =>
        new(7, "admin", mustChangePassword, roles, permissions, Now);

    private static JwtHelpers CreateJwt()
    {
        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["JwtSettings:Issuer"] = "PetSalon.Tests",
                ["JwtSettings:SignKey"] = "test-only-signing-key-at-least-32-bytes-long"
            })
            .Build();
        return new JwtHelpers(configuration, new FixedTimeProvider(Now));
    }

    private static JwtSecurityToken ReadToken(string token) =>
        new JwtSecurityTokenHandler().ReadJwtToken(token);

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utcNow);
    }

    private sealed class StubAuthService(AuthenticationResult result) : IAuthService
    {
        public Task<AuthenticationResult> AuthenticateAsync(
            string userName,
            string password,
            CancellationToken cancellationToken = default) =>
            Task.FromResult(result);

        public Task<PasswordChangeStatus> ChangePasswordAsync(
            long userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();
    }
}
