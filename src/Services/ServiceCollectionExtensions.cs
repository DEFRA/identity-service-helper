// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

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

    extension(IServiceCollection services)
    {
        public IServiceCollection AddDataServices(IConfigurationRoot config)
        {
            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<ICphDelegationsService, CphDelegationsService>();
            services.AddTransient<ICphService, CphService>();
            services.AddTransient<IAnimalSpeciesService, AnimalSpeciesService>();

            return services;
        }

        public IServiceCollection AddContext(IConfigurationRoot config)
        {
            services.AddScoped<IOperatorContext, OperatorContext>();

            return services;
        }

        public IServiceCollection AddStrategies(IConfigurationRoot config)
        {
            services.AddTransient<IStrategyBuilderFactory<UserService>, StrategyBuilderFactory<UserService>>();
            services.AddTransient<IStrategyBuilderFactory<CphDelegationsService>, StrategyBuilderFactory<CphDelegationsService>>();

            return services;
        }
    }
}
