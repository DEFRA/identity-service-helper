// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

using Defra.Identity.Services.Applications;
using Defra.Identity.Services.Users;
using Defra.Identity.Services.Delegations;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static string? GlobalConfigValue { get; private set; }

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<IDelegationsService, DelegationsService>();

        return services;
    }
}
