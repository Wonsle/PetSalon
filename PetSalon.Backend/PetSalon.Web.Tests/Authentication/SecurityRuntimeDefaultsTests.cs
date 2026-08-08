namespace PetSalon.Web.Tests.Authentication;

public sealed class SecurityRuntimeDefaultsTests
{
    private static readonly string RepositoryRoot = FindRepositoryRoot();

    [Fact]
    public void RuntimeDefaultsDoNotLogSensitiveEntityValues()
    {
        var program = ReadProgramSource();

        Assert.DoesNotContain("EnableSensitiveDataLogging", program);
    }

    [Fact]
    public void TokenLifetimeHasNoAdditionalClockSkewGracePeriod()
    {
        var program = ReadProgramSource();

        Assert.Contains("ClockSkew = TimeSpan.Zero", program);
    }

    private static string ReadProgramSource() => File.ReadAllText(Path.Combine(
        RepositoryRoot,
        "PetSalon.Backend",
        "PetSalon.Web",
        "Program.cs"));

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
