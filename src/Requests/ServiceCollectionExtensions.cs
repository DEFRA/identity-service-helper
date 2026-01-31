// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Users.Commands.Create;

namespace Defra.Identity.Requests;

using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static string? ApiKey { get; private set; }

    public static IServiceCollection AddRequests(this IServiceCollection services, IConfigurationRoot config)
    {
        ApiKey = config.GetValue<string>("DefraIndentityApiKey");
        services.AddTransient<IdentityRequestHeadersMiddleware>(sp => new IdentityRequestHeadersMiddleware(ApiKey!));
        services.AddValidatorsFromAssemblyContaining<CreateUser>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<IdentityRequestHeadersMiddleware>();
    }
}
