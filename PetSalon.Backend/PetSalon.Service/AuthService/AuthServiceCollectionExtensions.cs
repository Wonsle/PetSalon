using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.DependencyInjection;
using PetSalon.Models.EntityModels;

namespace PetSalon.Services.AuthService;

public static class AuthServiceCollectionExtensions
{
    public static IServiceCollection AddDefaultAdministratorProvisioning(
        this IServiceCollection services)
    {
        services.AddScoped<IPasswordHasher<Scuser>, PasswordHasher<Scuser>>();
        services.AddScoped<IDefaultAdminInitializer, DefaultAdminInitializer>();
        services.AddScoped<IAuthService, AuthService>();
        services.AddScoped<IRolePermissionService, RolePermissionService>();
        services.AddSingleton(TimeProvider.System);
        return services;
    }
}
