// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Extensions;

using Defra.Identity.Api.Middleware;
using Defra.Identity.Models.Requests.Services;
using Defra.Identity.Models.Requests.Users.Commands;
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
        ApiKey = config.GetValue<string>("DefraIndentityApiKey");
        services.AddTransient<ApiKeyValidationMiddleware>(sp => new ApiKeyValidationMiddleware(ApiKey!, sp.GetRequiredService<ILogger<ApiKeyValidationMiddleware>>()));
        services.AddTransient<CorrelationIdMiddleware>();
        services.AddTransient<OperatorIdMiddleware>();
        services.AddValidatorsFromAssemblyContaining<CreateUser>();

        services.AddScoped<IOperatorIdService, OperatorIdService>();

        return services;
    }

    public static void UseRequests(this WebApplication app)
    {
        app.UseMiddleware<ApiKeyValidationMiddleware>();
        app.UseMiddleware<CorrelationIdMiddleware>();
        app.UseMiddleware<OperatorIdMiddleware>();
    }
}
