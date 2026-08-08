namespace PetSalon.Web.Tests;

public class SecuritySetupDocumentationTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void EnvironmentTemplate_RequiresRuntimeSecretsWithoutUsableDefaults()
    {
        var template = File.ReadAllText(Path.Combine(RepositoryRoot, ".env.example"));

        Assert.Contains("SA_PASSWORD=CHANGE_ME_BEFORE_USE", template);
        Assert.Contains("ConnectionStrings__DefaultConnection=CHANGE_ME_BEFORE_USE", template);
        Assert.Contains("JwtSettings__SignKey=CHANGE_ME_BEFORE_USE", template);
        Assert.DoesNotContain("YourStrong!Passw0rd", template);
    }

    [Fact]
    public void Readme_DocumentsUserSecretsContainerInjectionAndRotation()
    {
        var readme = File.ReadAllText(Path.Combine(RepositoryRoot, "README.md"));

        Assert.Contains("dotnet user-secrets", readme);
        Assert.Contains("ConnectionStrings:DefaultConnection", readme);
        Assert.Contains("JwtSettings:SignKey", readme);
        Assert.Contains("ConnectionStrings__DefaultConnection", readme);
        Assert.Contains("JwtSettings__SignKey", readme);
        Assert.Contains("輪替", readme);
    }

    [Fact]
    public void AppleSiliconDocumentation_DoesNotRequireIntelEmulation()
    {
        var dockerGuide = File.ReadAllText(Path.Combine(RepositoryRoot, "DOCKER_SETUP.md"));
        var macCompose = File.ReadAllText(Path.Combine(RepositoryRoot, "docker-compose.mac.yml"));

        Assert.DoesNotContain("Intel Mac", dockerGuide);
        Assert.DoesNotContain("Rosetta", dockerGuide);
        Assert.DoesNotContain("linux/amd64", macCompose);
        Assert.DoesNotContain("Intel Mac", macCompose);
        Assert.DoesNotContain("Rosetta", macCompose);
    }

    private static string FindRepositoryRoot()
    {
        var directory = new DirectoryInfo(AppContext.BaseDirectory);
        while (directory is not null && !File.Exists(Path.Combine(directory.FullName, ".env.example")))
        {
            directory = directory.Parent;
        }

        return directory?.FullName
            ?? throw new DirectoryNotFoundException("Repository root marker was not found.");
    }
}
