// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Requests;

using Defra.Identity.Requests.Middleware;
using Defra.Identity.Requests.Users.Commands.Create;
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
        services.AddTransient<ApiKeyValidationMiddleware>(sp => new ApiKeyValidationMiddleware(ApiKey!));
        services.AddTransient<CorrellationIdMiddleware>();
        services.AddTransient<OperatorIdMiddleware>();
        services.AddValidatorsFromAssemblyContaining<CreateUser>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyValidationMiddleware>();
        app.UseMiddleware<CorrellationIdMiddleware>();
        app.UseMiddleware<OperatorIdMiddleware>();
    }
}
