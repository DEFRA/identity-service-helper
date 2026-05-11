// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Extensions;

using Defra.Identity.Api.Common.Factories;
using Defra.Identity.Api.Middleware;
using Defra.Identity.Models.Requests.Services;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Profiles;
using FluentValidation;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    public static string? ApiKey { get; private set; }

    public static IServiceCollection AddRequests(this IServiceCollection services, IConfigurationRoot config)
    {
        ApiKey = config.GetValue<string>("DefraIdentityApiKey");
        if (string.IsNullOrEmpty(ApiKey))
        {
            throw new ArgumentException("DefraIdentityApiKey configuration value is missing or empty");
        }

        services.AddTransient<ApiKeyValidationMiddleware>(sp => new ApiKeyValidationMiddleware(ApiKey, sp.GetRequiredService<ILogger<ApiKeyValidationMiddleware>>()));
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<OperatorIdMiddleware>();
        services.AddScoped<IOperatorIdService, OperatorIdService>();
        services.AddValidatorsFromAssemblyContaining<CreateUser>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyValidationMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<OperatorIdMiddleware>();
    }

    public static IServiceCollection AddCphNumberRerouting(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<ICphNumberHandlerFactory<ICphService>, CphNumberHandlerFactory<ICphService>>();
        services.AddTransient<ICphNumberHandlerFactory<IProfileService>, CphNumberHandlerFactory<IProfileService>>();

        return services;
    }
}
