using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Users.Commands.Create;

namespace Defra.Identity.Requests;

using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequests(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IdentityRequestHeadersMiddleware>();
        services.AddValidatorsFromAssemblyContaining<CreateUser>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<IdentityRequestHeadersMiddleware>();
    }
}
