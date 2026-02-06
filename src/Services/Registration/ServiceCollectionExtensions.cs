// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Registration;

using Defra.Identity.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static string? GlobalConfigValue { get; private set; }

    public static IServiceCollection AddServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IUserService, UserService>();

        return services;
    }
}
