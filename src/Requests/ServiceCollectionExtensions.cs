using Defra.Identity.Requests.Middleware;

namespace Defra.Identity.Requests;

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRequests(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IdentityRequestHeadersMiddleware>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<IdentityRequestHeadersMiddleware>();
    }
}
