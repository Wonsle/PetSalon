using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Services.AuthService;
using PetSalon.Web.Controllers;

namespace PetSalon.Web.Tests.Authentication;

public sealed class PasswordChangeTests
{
    private static readonly DateTime Now = new(2026, 8, 7, 12, 0, 0, DateTimeKind.Utc);

    [Fact]
    public async Task ValidPasswordChangeAtomicallyReplacesHashAndClearsRestriction()
    {
        await using var context = CreateContext();
        var user = await AddRestrictedAdminAsync(context);
        var service = CreateService(context);

        var result = await service.ChangePasswordAsync(user.ScuserId, new ChangePasswordRequest
        {
            CurrentPassword = "password",
            NewPassword = "Petsalon-2026!",
            ConfirmPassword = "Petsalon-2026!"
        });

        Assert.Equal(PasswordChangeStatus.Succeeded, result);
        Assert.False(user.MustChangePassword);
        var hasher = new PasswordHasher<Scuser>();
        Assert.Equal(
            PasswordVerificationResult.Success,
            hasher.VerifyHashedPassword(user, user.PasswordHash, "Petsalon-2026!"));
        Assert.Equal(
            PasswordVerificationResult.Failed,
            hasher.VerifyHashedPassword(user, user.PasswordHash, "password"));
    }

    [Theory]
    [InlineData("short", "short")]
    [InlineData("password", "password")]
    [InlineData("ADMIN", "ADMIN")]
    [InlineData("Petsalon-2026!", "different-value")]
    public async Task WeakOrMismatchedPasswordDoesNotChangeState(string next, string confirmation)
    {
        await using var context = CreateContext();
        var user = await AddRestrictedAdminAsync(context);
        var originalHash = user.PasswordHash;

        var result = await CreateService(context).ChangePasswordAsync(user.ScuserId, new ChangePasswordRequest
        {
            CurrentPassword = "password",
            NewPassword = next,
            ConfirmPassword = confirmation
        });

        Assert.Equal(PasswordChangeStatus.InvalidRequest, result);
        Assert.Equal(originalHash, user.PasswordHash);
        Assert.True(user.MustChangePassword);
    }

    [Fact]
    public async Task WrongCurrentPasswordDoesNotChangeState()
    {
        await using var context = CreateContext();
        var user = await AddRestrictedAdminAsync(context);
        var originalHash = user.PasswordHash;

        var result = await CreateService(context).ChangePasswordAsync(user.ScuserId, new ChangePasswordRequest
        {
            CurrentPassword = "wrong",
            NewPassword = "Petsalon-2026!",
            ConfirmPassword = "Petsalon-2026!"
        });

        Assert.Equal(PasswordChangeStatus.InvalidCredentials, result);
        Assert.Equal(originalHash, user.PasswordHash);
        Assert.True(user.MustChangePassword);
    }

    [Fact]
    public async Task PersistenceFailureRestoresBothPasswordFields()
    {
        await using var context = CreateContext();
        var user = await AddRestrictedAdminAsync(context);
        var originalHash = user.PasswordHash;
        context.FailSaves = true;

        var result = await CreateService(context).ChangePasswordAsync(user.ScuserId, new ChangePasswordRequest
        {
            CurrentPassword = "password",
            NewPassword = "Petsalon-2026!",
            ConfirmPassword = "Petsalon-2026!"
        });

        Assert.Equal(PasswordChangeStatus.PersistenceFailed, result);
        Assert.Equal(originalHash, user.PasswordHash);
        Assert.True(user.MustChangePassword);
        Assert.Equal(
            PasswordVerificationResult.Success,
            new PasswordHasher<Scuser>().VerifyHashedPassword(user, user.PasswordHash, "password"));
    }

    [Fact]
    public async Task PasswordEndpointUsesAuthenticatedUserAndRequiresRelogin()
    {
        var auth = new RecordingAuthService();
        var controller = new PasswordController(auth)
        {
            ControllerContext = new ControllerContext
            {
                HttpContext = new DefaultHttpContext
                {
                    User = new ClaimsPrincipal(new ClaimsIdentity(
                    [
                        new Claim(ClaimTypes.NameIdentifier, "7"),
                        new Claim(JwtHelpers.TokenUseClaim, JwtHelpers.PasswordChangeTokenUse)
                    ], "Test"))
                }
            }
        };
        var request = new ChangePasswordRequest
        {
            CurrentPassword = "password",
            NewPassword = "Petsalon-2026!",
            ConfirmPassword = "Petsalon-2026!"
        };

        var result = await controller.Change(request);

        Assert.IsType<NoContentResult>(result);
        Assert.Equal(7, auth.ChangedUserId);
        Assert.Same(request, auth.Request);
    }

    private static AuthService CreateService(PetSalonContext context) =>
        new(context, new PasswordHasher<Scuser>(), new FixedTimeProvider(Now));

    private static ThrowingPetSalonContext CreateContext()
    {
        var options = new DbContextOptionsBuilder<PetSalonContext>()
            .UseInMemoryDatabase($"password-change-{Guid.NewGuid():N}")
            .Options;
        return new ThrowingPetSalonContext(options);
    }

    private static async Task<Scuser> AddRestrictedAdminAsync(ThrowingPetSalonContext context)
    {
        var user = new Scuser
        {
            UserName = "admin",
            IsActive = true,
            MustChangePassword = true,
            CreateUser = "TEST",
            ModifyUser = "TEST"
        };
        user.PasswordHash = new PasswordHasher<Scuser>().HashPassword(user, "password");
        context.Scuser.Add(user);
        await context.SaveChangesAsync();
        return user;
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

    private sealed class FixedTimeProvider(DateTime utcNow) : TimeProvider
    {
        public override DateTimeOffset GetUtcNow() => new(utcNow);
    }

    private sealed class RecordingAuthService : IAuthService
    {
        public long? ChangedUserId { get; private set; }
        public ChangePasswordRequest? Request { get; private set; }

        public Task<AuthenticationResult> AuthenticateAsync(
            string userName,
            string password,
            CancellationToken cancellationToken = default) =>
            throw new NotSupportedException();

        public Task<PasswordChangeStatus> ChangePasswordAsync(
            long userId,
            ChangePasswordRequest request,
            CancellationToken cancellationToken = default)
        {
            ChangedUserId = userId;
            Request = request;
            return Task.FromResult(PasswordChangeStatus.Succeeded);
        }
    }
}
