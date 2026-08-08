using Microsoft.Extensions.Configuration;
using PetSalon.Web.Configuration;

namespace PetSalon.Web.Tests.Authentication;

public sealed class SecurityConfigurationValidatorTests
{
    private const string ValidConnectionString =
        "Server=db.internal;Database=PetSalon;User ID=petsalon;Password=Rotated-Only-In-Test!;Encrypt=True";
    private const string ValidSigningKey = "A-secure-test-signing-key-with-32-bytes";

    [Fact]
    public void MissingSigningKeyFailsWithoutDisclosingOtherSecrets()
    {
        var configuration = BuildConfiguration(ValidConnectionString, null);

        var error = Assert.Throws<InvalidOperationException>(() =>
            SecurityConfigurationValidator.Validate(configuration));

        Assert.Contains("JwtSettings:SignKey", error.Message);
        Assert.DoesNotContain("Rotated-Only-In-Test", error.Message);
    }

    [Theory]
    [InlineData("change-me")]
    [InlineData("${DB_CONNECTION_STRING}")]
    [InlineData("Server=localhost;Database=PetSalon;User ID=sa;Password=CHANGE_ME")]
    public void PlaceholderConnectionStringFails(string placeholder)
    {
        var configuration = BuildConfiguration(placeholder, ValidSigningKey);

        var error = Assert.Throws<InvalidOperationException>(() =>
            SecurityConfigurationValidator.Validate(configuration));

        Assert.Contains("ConnectionStrings:DefaultConnection", error.Message);
        Assert.DoesNotContain(placeholder, error.Message);
    }

    [Fact]
    public void SigningKeyShorterThan32BytesFails()
    {
        var configuration = BuildConfiguration(ValidConnectionString, "too-short");

        var error = Assert.Throws<InvalidOperationException>(() =>
            SecurityConfigurationValidator.Validate(configuration));

        Assert.Contains("JwtSettings:SignKey", error.Message);
        Assert.DoesNotContain("too-short", error.Message);
    }

    [Fact]
    public void ValidEnvironmentInjectedSecretsPass()
    {
        var configuration = BuildConfiguration(ValidConnectionString, ValidSigningKey);

        SecurityConfigurationValidator.Validate(configuration);
    }

    private static IConfiguration BuildConfiguration(
        string? connectionString,
        string? signingKey)
    {
        var values = new Dictionary<string, string?>
        {
            ["ConnectionStrings:DefaultConnection"] = connectionString,
            ["JwtSettings:SignKey"] = signingKey
        };

        return new ConfigurationBuilder()
            .AddInMemoryCollection(values)
            .Build();
    }
}
