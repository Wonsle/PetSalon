using Microsoft.AspNetCore.Authorization;

namespace PetSalon.Web.Authorization;

public static class PetSalonAuthorization
{
    public const string PasswordChangeOnly = "PasswordChangeOnly";

    public static void Configure(AuthorizationOptions options)
    {
        options.FallbackPolicy = new AuthorizationPolicyBuilder()
            .RequireAuthenticatedUser()
            .RequireAssertion(context =>
                !context.User.HasClaim(
                    JwtHelpers.TokenUseClaim,
                    JwtHelpers.PasswordChangeTokenUse))
            .Build();

        options.AddPolicy(
            PasswordChangeOnly,
            policy => policy
                .RequireAuthenticatedUser()
                .RequireClaim(
                    JwtHelpers.TokenUseClaim,
                    JwtHelpers.PasswordChangeTokenUse));
    }
}
