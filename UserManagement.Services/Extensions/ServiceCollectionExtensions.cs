using UserManagement.Services.Domain;


namespace Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddDomainServices(this IServiceCollection services)
        => services.AddScoped<IUserService, UserService>();
}
