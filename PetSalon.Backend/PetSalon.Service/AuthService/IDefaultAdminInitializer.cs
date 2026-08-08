namespace PetSalon.Services.AuthService;

public interface IDefaultAdminInitializer
{
    Task InitializeAsync(CancellationToken cancellationToken = default);
}
