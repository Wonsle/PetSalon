using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using PetSalon.Models.DTOs;
using PetSalon.Models.EntityModels;
using PetSalon.Models.Authorization;

namespace PetSalon.Services.AuthService;

public sealed class AuthService : IAuthService
{
    private readonly PetSalonContext _context;
    private readonly IPasswordHasher<Scuser> _passwordHasher;
    private readonly TimeProvider _timeProvider;

    public AuthService(
        PetSalonContext context,
        IPasswordHasher<Scuser> passwordHasher,
        TimeProvider timeProvider)
    {
        _context = context;
        _passwordHasher = passwordHasher;
        _timeProvider = timeProvider;
    }

    public async Task<AuthenticationResult> AuthenticateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default)
    {
        var normalizedUserName = NormalizeUserName(userName);
        if (normalizedUserName.Length == 0 || string.IsNullOrEmpty(password))
        {
            return AuthenticationResult.InvalidCredentials;
        }

        var user = await _context.Scuser
            .Include(item => item.ScuserRoles)
            .ThenInclude(item => item.Role)
            .ThenInclude(item => item.ScrolePermissions)
            .ThenInclude(item => item.Permission)
            .SingleOrDefaultAsync(
                item => item.UserName == normalizedUserName,
                cancellationToken);

        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return AuthenticationResult.InvalidCredentials;
        }

        var verification = _passwordHasher.VerifyHashedPassword(
            user,
            user.PasswordHash,
            password);
        if (verification == PasswordVerificationResult.Failed)
        {
            return AuthenticationResult.InvalidCredentials;
        }

        if (verification == PasswordVerificationResult.SuccessRehashNeeded)
        {
            user.PasswordHash = _passwordHasher.HashPassword(user, password);
        }

        user.LastLogin = _timeProvider.GetUtcNow().UtcDateTime;
        await _context.SaveChangesAsync(cancellationToken);

        var roles = user.ScuserRoles
            .Select(item => item.Role.RoleName?.Trim())
            .Where(role => !string.IsNullOrWhiteSpace(role))
            .Distinct(StringComparer.Ordinal)
            .Cast<string>()
            .ToArray();
        var permissions = user.ScuserRoles
            .SelectMany(item => item.Role.ScrolePermissions)
            .Where(item => item.Permission.IsActive)
            .Select(item => item.Permission.PermissionCode?.Trim())
            .Where(code => !string.IsNullOrWhiteSpace(code))
            .Distinct(StringComparer.Ordinal)
            .Cast<string>()
            .ToArray();
        if (!user.MustChangePassword &&
            !permissions.Contains(PermissionCodes.Login, StringComparer.Ordinal))
        {
            return new AuthenticationResult(AuthenticationStatus.MissingPermission, null);
        }

        return new AuthenticationResult(
            AuthenticationStatus.Succeeded,
            new AuthenticatedUser(
                user.ScuserId,
                NormalizeUserName(user.UserName),
                user.MustChangePassword,
                roles,
                permissions,
                user.LastLogin.Value));
    }

    public async Task<PasswordChangeStatus> ChangePasswordAsync(
        long userId,
        ChangePasswordRequest request,
        CancellationToken cancellationToken = default)
    {
        if (request is null ||
            string.IsNullOrEmpty(request.CurrentPassword) ||
            string.IsNullOrEmpty(request.NewPassword) ||
            request.NewPassword.Length < 12 ||
            !string.Equals(request.NewPassword, request.ConfirmPassword, StringComparison.Ordinal) ||
            string.Equals(request.NewPassword, "password", StringComparison.OrdinalIgnoreCase))
        {
            return PasswordChangeStatus.InvalidRequest;
        }

        var user = await _context.Scuser.SingleOrDefaultAsync(
            item => item.ScuserId == userId,
            cancellationToken);
        if (user is null || !user.IsActive || string.IsNullOrWhiteSpace(user.PasswordHash))
        {
            return PasswordChangeStatus.InvalidCredentials;
        }

        if (string.Equals(request.NewPassword, user.UserName, StringComparison.OrdinalIgnoreCase))
        {
            return PasswordChangeStatus.InvalidRequest;
        }

        if (_passwordHasher.VerifyHashedPassword(
                user,
                user.PasswordHash,
                request.CurrentPassword) == PasswordVerificationResult.Failed)
        {
            return PasswordChangeStatus.InvalidCredentials;
        }

        var originalHash = user.PasswordHash;
        var originalMustChangePassword = user.MustChangePassword;
        var originalModifyUser = user.ModifyUser;
        var originalModifyTime = user.ModifyTime;
        IDbContextTransaction? transaction = null;

        try
        {
            if (_context.Database.IsRelational())
            {
                transaction = await _context.Database.BeginTransactionAsync(cancellationToken);
            }

            user.PasswordHash = _passwordHasher.HashPassword(user, request.NewPassword);
            user.MustChangePassword = false;
            user.ModifyUser = user.UserName;
            user.ModifyTime = _timeProvider.GetUtcNow().UtcDateTime;
            await _context.SaveChangesAsync(cancellationToken);

            if (transaction is not null)
            {
                await transaction.CommitAsync(cancellationToken);
            }

            return PasswordChangeStatus.Succeeded;
        }
        catch (Exception exception) when (exception is not OperationCanceledException)
        {
            if (transaction is not null)
            {
                await transaction.RollbackAsync(CancellationToken.None);
            }

            user.PasswordHash = originalHash;
            user.MustChangePassword = originalMustChangePassword;
            user.ModifyUser = originalModifyUser;
            user.ModifyTime = originalModifyTime;
            _context.Entry(user).State = EntityState.Unchanged;
            return PasswordChangeStatus.PersistenceFailed;
        }
        finally
        {
            if (transaction is not null)
            {
                await transaction.DisposeAsync();
            }
        }
    }

    private static string NormalizeUserName(string? userName) =>
        userName?.Trim().ToLowerInvariant() ?? string.Empty;
}
