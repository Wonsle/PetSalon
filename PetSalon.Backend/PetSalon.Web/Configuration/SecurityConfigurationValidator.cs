using System.Text;

namespace PetSalon.Web.Configuration;

public static class SecurityConfigurationValidator
{
    private const int MinimumSigningKeyBytes = 32;
    private static readonly string[] PlaceholderTokens =
    [
        "change-me",
        "change_me",
        "changeme",
        "${",
        "[redacted]",
        "your-password",
        "your_password"
    ];

    public static void Validate(IConfiguration configuration)
    {
        var invalidKeys = new List<string>();
        var connectionString = configuration.GetConnectionString("DefaultConnection");
        var signingKey = configuration["JwtSettings:SignKey"];

        if (IsMissingOrPlaceholder(connectionString))
        {
            invalidKeys.Add("ConnectionStrings:DefaultConnection");
        }

        if (IsMissingOrPlaceholder(signingKey) ||
            Encoding.UTF8.GetByteCount(signingKey!) < MinimumSigningKeyBytes)
        {
            invalidKeys.Add("JwtSettings:SignKey");
        }

        if (invalidKeys.Count > 0)
        {
            throw new InvalidOperationException(
                $"Missing or invalid required configuration: {string.Join(", ", invalidKeys)}.");
        }
    }

    private static bool IsMissingOrPlaceholder(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return true;
        }

        return PlaceholderTokens.Any(token =>
            value.Contains(token, StringComparison.OrdinalIgnoreCase));
    }
}
