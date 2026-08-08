using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;

namespace PetSalon.Web.Authorization;

public static class PermissionPolicy
{
    public const string Prefix = "Permission:";

    public static string NameFor(string permissionCode)
    {
        ValidateCode(permissionCode);
        return Prefix + permissionCode;
    }

    public static bool TryGetCode(string? policyName, out string permissionCode)
    {
        if (policyName?.StartsWith(Prefix, StringComparison.Ordinal) != true)
        {
            permissionCode = string.Empty;
            return false;
        }

        permissionCode = policyName[Prefix.Length..];
        ValidateCode(permissionCode);
        return true;
    }

    private static void ValidateCode(string permissionCode)
    {
        if (string.IsNullOrWhiteSpace(permissionCode) ||
            permissionCode.Length > 100 ||
            permissionCode.Any(character =>
                !(character is >= 'a' and <= 'z' ||
                  character is >= '0' and <= '9' ||
                  character is '.' or '_' or '-')))
        {
            throw new ArgumentException("Permission code format is invalid.", nameof(permissionCode));
        }
    }
}

[AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, AllowMultiple = true, Inherited = true)]
public sealed class RequirePermissionAttribute : AuthorizeAttribute
{
    public RequirePermissionAttribute(string permissionCode)
        : base(PermissionPolicy.NameFor(permissionCode))
    {
        PermissionCode = permissionCode;
    }

    public string PermissionCode { get; }
}

public sealed record PermissionRequirement(string PermissionCode) : IAuthorizationRequirement;

public sealed class PermissionAuthorizationHandler : AuthorizationHandler<PermissionRequirement>
{
    protected override Task HandleRequirementAsync(
        AuthorizationHandlerContext context,
        PermissionRequirement requirement)
    {
        if (context.User.HasClaim(
                claim => claim.Type == JwtHelpers.PermissionClaimType &&
                         string.Equals(claim.Value, requirement.PermissionCode, StringComparison.Ordinal)))
        {
            context.Succeed(requirement);
        }

        return Task.CompletedTask;
    }
}

public sealed class PermissionAuthorizationPolicyProvider : IAuthorizationPolicyProvider
{
    private readonly DefaultAuthorizationPolicyProvider _defaultProvider;

    public PermissionAuthorizationPolicyProvider(IOptions<AuthorizationOptions> options)
    {
        _defaultProvider = new DefaultAuthorizationPolicyProvider(options);
    }

    public Task<AuthorizationPolicy> GetDefaultPolicyAsync() =>
        _defaultProvider.GetDefaultPolicyAsync();

    public Task<AuthorizationPolicy?> GetFallbackPolicyAsync() =>
        _defaultProvider.GetFallbackPolicyAsync();

    public Task<AuthorizationPolicy?> GetPolicyAsync(string policyName)
    {
        if (!PermissionPolicy.TryGetCode(policyName, out var permissionCode))
        {
            return _defaultProvider.GetPolicyAsync(policyName);
        }

        var policy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                !context.User.HasClaim(
                    JwtHelpers.TokenUseClaim,
                    JwtHelpers.PasswordChangeTokenUse))
            .AddRequirements(new PermissionRequirement(permissionCode))
            .Build();
        return Task.FromResult<AuthorizationPolicy?>(policy);
    }
}

public static class PermissionAuthorizationServiceCollectionExtensions
{
    public static IServiceCollection AddPermissionAuthorization(this IServiceCollection services)
    {
        services.AddSingleton<IAuthorizationPolicyProvider, PermissionAuthorizationPolicyProvider>();
        services.AddSingleton<IAuthorizationHandler, PermissionAuthorizationHandler>();
        return services;
    }
}
