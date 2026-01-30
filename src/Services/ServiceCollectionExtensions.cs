namespace Defra.Identity.Services;

using Defra.Identity.Responses.Users;
using Defra.Identity.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    public static string? GlobalConfigValue { get; private set; }

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IUserService, UserService>();



        return services;
    }
}
