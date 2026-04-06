// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Services.Applications;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Delegations;
using Defra.Identity.Services.Species;
using Defra.Identity.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static string? GlobalConfigValue { get; private set; }

    public static IServiceCollection AddDataServices(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IUserService, UserService>();
        services.AddTransient<IApplicationService, ApplicationService>();
        services.AddTransient<ICphDelegationsService, CphDelegationsService>();
        services.AddTransient<ICphService, CphService>();
        services.AddTransient<IAnimalSpeciesService, AnimalSpeciesService>();

        return services;
    }

    public static IServiceCollection AddContext(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddScoped<IOperatorContext, OperatorContext>();

        return services;
    }

    public static IServiceCollection AddStrategies(this IServiceCollection services, IConfigurationRoot config)
    {
        services
            .AddTransient<IStrategyBuilderFactory<CphDelegationsService, CountyParishHoldingDelegations>,
                StrategyBuilderFactory<CphDelegationsService, CountyParishHoldingDelegations>>();

        return services;
    }
}
