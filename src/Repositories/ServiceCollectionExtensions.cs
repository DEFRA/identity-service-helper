namespace Defra.Identity.Repositories;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IRepository<UserAccount>, UsersRepository>();

        return services;
    }
}
