namespace PetSalon.Services.AuthService;

public interface IAuthService
{
    Task<AuthenticationResult> AuthenticateAsync(
        string userName,
        string password,
        CancellationToken cancellationToken = default);

    Task<PasswordChangeStatus> ChangePasswordAsync(
        long userId,
        PetSalon.Models.DTOs.ChangePasswordRequest request,
        CancellationToken cancellationToken = default);
}

public enum PasswordChangeStatus
{
    Succeeded,
    InvalidRequest,
    InvalidCredentials,
    PersistenceFailed
}

public enum AuthenticationStatus
{
    Succeeded,
    InvalidCredentials,
    MissingPermission
}

public sealed record AuthenticatedUser(
    long UserId,
    string UserName,
    bool MustChangePassword,
    IReadOnlyList<string> Roles,
    IReadOnlyList<string> Permissions,
    DateTime LastLogin);

public sealed record AuthenticationResult(
    AuthenticationStatus Status,
    AuthenticatedUser? User)
{
    public static AuthenticationResult InvalidCredentials { get; } =
        new(AuthenticationStatus.InvalidCredentials, null);
}
