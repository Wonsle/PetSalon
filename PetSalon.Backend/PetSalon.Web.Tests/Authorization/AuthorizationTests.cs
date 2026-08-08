using System.Reflection;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Routing;
using Microsoft.Extensions.DependencyInjection;
using PetSalon.Web.Authorization;
using PetSalon.Web.Controllers;
using PetSalon.Models.Authorization;

namespace PetSalon.Web.Tests.Authorization;

public sealed class AuthorizationTests
{
    [Fact]
    public async Task AnonymousCallerIsChallengedByFallbackPolicy()
    {
        var policy = CreateOptions().FallbackPolicy!;

        var result = await CreateEvaluator().AuthorizeAsync(
            policy,
            AuthenticateResult.NoResult(),
            new DefaultHttpContext(),
            resource: null);

        Assert.True(result.Challenged);
        Assert.False(result.Forbidden);
    }

    [Fact]
    public async Task PasswordChangeTokenIsForbiddenFromBusinessPolicyButAcceptedByChangePolicy()
    {
        var options = CreateOptions();
        var principal = CreatePrincipal(
            new Claim(JwtHelpers.TokenUseClaim, JwtHelpers.PasswordChangeTokenUse));
        var authentication = AuthenticateResult.Success(
            new AuthenticationTicket(principal, "Test"));
        var evaluator = CreateEvaluator();
        var context = new DefaultHttpContext { User = principal };

        var businessResult = await evaluator.AuthorizeAsync(
            options.FallbackPolicy!, authentication, context, null);
        var passwordResult = await evaluator.AuthorizeAsync(
            options.GetPolicy(PetSalonAuthorization.PasswordChangeOnly)!,
            authentication,
            context,
            null);

        Assert.True(businessResult.Forbidden);
        Assert.False(businessResult.Challenged);
        Assert.True(passwordResult.Succeeded);
    }

    [Fact]
    public async Task GeneralTokenIsAcceptedByFallbackPolicy()
    {
        var principal = CreatePrincipal(new Claim(ClaimTypes.Role, "Designer"));
        var authentication = AuthenticateResult.Success(
            new AuthenticationTicket(principal, "Test"));

        var result = await CreateEvaluator().AuthorizeAsync(
            CreateOptions().FallbackPolicy!,
            authentication,
            new DefaultHttpContext { User = principal },
            null);

        Assert.True(result.Succeeded);
    }

    [Fact]
    public void LoginIsTheOnlyExplicitlyAnonymousControllerAction()
    {
        var anonymousActions = typeof(AccountController).Assembly
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetMethods(BindingFlags.Instance | BindingFlags.Public))
            .Where(method => method.GetCustomAttributes<HttpMethodAttribute>().Any())
            .Where(method => method.GetCustomAttribute<AllowAnonymousAttribute>() is not null)
            .Select(method => $"{method.DeclaringType?.Name}.{method.Name}")
            .ToArray();

        Assert.Equal(["AccountController.Login"], anonymousActions);
    }

    [Fact]
    public async Task PermissionPolicyForbidsMissingClaimAndAcceptsMatchingClaimRegardlessOfRoleName()
    {
        using var provider = CreateServiceProvider();
        var policyProvider = provider.GetRequiredService<IAuthorizationPolicyProvider>();
        var policy = await policyProvider.GetPolicyAsync(
            PermissionPolicy.NameFor(PermissionCodes.ReadFinancialData));
        Assert.NotNull(policy);
        var evaluator = provider.GetRequiredService<IPolicyEvaluator>();

        var missing = CreatePrincipal(new Claim(ClaimTypes.Role, "RenamedRole"));
        var allowed = CreatePrincipal(
            new Claim(ClaimTypes.Role, "RenamedRole"),
            new Claim(JwtHelpers.PermissionClaimType, PermissionCodes.ReadFinancialData));
        var missingResult = await EvaluateAsync(evaluator, policy, missing);
        var allowedResult = await EvaluateAsync(evaluator, policy, allowed);

        Assert.True(missingResult.Forbidden);
        Assert.True(allowedResult.Succeeded);
    }

    [Fact]
    public void ControllersNeverDeclareRoleBasedAuthorization()
    {
        var authorizeAttributes = typeof(AccountController).Assembly
            .GetTypes()
            .Where(type => typeof(ControllerBase).IsAssignableFrom(type))
            .SelectMany(type => type.GetCustomAttributes<AuthorizeAttribute>()
                .Concat(type.GetMethods(BindingFlags.Instance | BindingFlags.Public)
                    .SelectMany(method => method.GetCustomAttributes<AuthorizeAttribute>())))
            .ToArray();

        Assert.All(authorizeAttributes, attribute => Assert.True(string.IsNullOrWhiteSpace(attribute.Roles)));
        Assert.DoesNotContain(authorizeAttributes, attribute =>
            new[] { "Admin", "Manager", "Designer" }.Any(role =>
                attribute.Policy?.Contains(role, StringComparison.OrdinalIgnoreCase) == true));
    }

    [Fact]
    public void SensitiveEndpointsDeclareStablePermissionCodes()
    {
        AssertPermission<CodeTypeController>(nameof(CodeTypeController.CreateCodeType), PermissionCodes.ManageSystemSettings);
        AssertPermission<CodeTypeController>(nameof(CodeTypeController.UpdateCodeType), PermissionCodes.ManageSystemSettings);
        AssertPermission<CodeTypeController>(nameof(CodeTypeController.DeleteCodeType), PermissionCodes.ManageSystemSettings);
        AssertPermission<CommonController>(nameof(CommonController.CreateSystemCode), PermissionCodes.ManageSystemSettings);
        AssertPermission<CommonController>(nameof(CommonController.UpdateSystemCode), PermissionCodes.ManageSystemSettings);
        AssertPermission<CommonController>(nameof(CommonController.DeleteSystemCode), PermissionCodes.ManageSystemSettings);
        AssertPermission<ServiceController>(nameof(ServiceController.CreateService), PermissionCodes.ManageSystemSettings);
        AssertPermission<ServiceController>(nameof(ServiceController.UpdateService), PermissionCodes.ManageSystemSettings);
        AssertPermission<ServiceController>(nameof(ServiceController.DeleteService), PermissionCodes.ManageSystemSettings);
        AssertPermission<ServiceController>(nameof(ServiceController.ToggleServiceStatus), PermissionCodes.ManageSystemSettings);
        AssertPermission<ServiceController>(nameof(ServiceController.UpdateServiceSort), PermissionCodes.ManageSystemSettings);
        AssertPermission<FileController>(nameof(FileController.PermanentlyDeleteFile), PermissionCodes.PermanentlyDeleteFiles);
        AssertPermission<DashboardController>(nameof(DashboardController.GetStatistics), PermissionCodes.ReadFinancialData);
        AssertPermission<DashboardController>(nameof(DashboardController.GetMonthlyRevenue), PermissionCodes.ReadFinancialData);

        var financial = typeof(FinancialController).GetCustomAttribute<RequirePermissionAttribute>();
        Assert.NotNull(financial);
        Assert.Equal(PermissionCodes.ReadFinancialData, financial.PermissionCode);
    }

    private static AuthorizationOptions CreateOptions()
    {
        var options = new AuthorizationOptions();
        PetSalonAuthorization.Configure(options);
        return options;
    }

    private static ClaimsPrincipal CreatePrincipal(params Claim[] claims) =>
        new(new ClaimsIdentity(claims, "Test"));

    private static IPolicyEvaluator CreateEvaluator()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(PetSalonAuthorization.Configure);
        return services.BuildServiceProvider().GetRequiredService<IPolicyEvaluator>();
    }

    private static ServiceProvider CreateServiceProvider()
    {
        var services = new ServiceCollection();
        services.AddLogging();
        services.AddAuthorization(PetSalonAuthorization.Configure);
        services.AddPermissionAuthorization();
        return services.BuildServiceProvider();
    }

    private static async Task<PolicyAuthorizationResult> EvaluateAsync(
        IPolicyEvaluator evaluator,
        AuthorizationPolicy policy,
        ClaimsPrincipal principal)
    {
        var context = new DefaultHttpContext { User = principal };
        return await evaluator.AuthorizeAsync(
            policy,
            AuthenticateResult.Success(new AuthenticationTicket(principal, "Test")),
            context,
            null);
    }

    private static void AssertPermission<TController>(string methodName, string expectedCode)
    {
        var method = typeof(TController).GetMethod(methodName);
        Assert.NotNull(method);
        var attribute = method.GetCustomAttribute<RequirePermissionAttribute>();
        Assert.NotNull(attribute);
        Assert.Equal(expectedCode, attribute.PermissionCode);
    }
}
