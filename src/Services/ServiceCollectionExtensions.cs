// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services;

using Defra.Identity.Services.Applications;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Delegations;
using Defra.Identity.Services.Delegations.Injection;
using Defra.Identity.Services.Profiles;
using Defra.Identity.Services.Roles;
using Defra.Identity.Services.Species;
using Defra.Identity.Services.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    extension(IServiceCollection services)
    {
        public IServiceCollection AddDataServices(IConfigurationRoot config)
        {
            services.AddTransient<ICphDelegationSvcRepoContext, CphDelegationSvcRepoContext>();

            services.AddTransient<IUserService, UserService>();
            services.AddTransient<IRoleService, RoleService>();
            services.AddTransient<IProfileService, ProfileService>();
            services.AddTransient<IApplicationService, ApplicationService>();
            services.AddTransient<ICphDelegationService, CphDelegationService>();
            services.AddTransient<ICphService, CphService>();
            services.AddTransient<ICphNumberService, CphNumberService>();
            services.AddTransient<IAnimalSpeciesService, AnimalSpeciesService>();

            return services;
        }

        public IServiceCollection AddOperatorContext(IConfigurationRoot config)
        {
            services.AddScoped<IOperatorContext, OperatorContext>();

            return services;
        }

        public IServiceCollection AddStrategies(IConfigurationRoot config)
        {
            services
                .AddTransient<IStrategyBuilderFactory<ApplicationService>,
                    StrategyBuilderFactory<ApplicationService>>();
            services.AddTransient<IStrategyBuilderFactory<UserService>, StrategyBuilderFactory<UserService>>();
            services.AddTransient<IStrategyBuilderFactory<RoleService>, StrategyBuilderFactory<RoleService>>();
            services.AddTransient<IStrategyBuilderFactory<CphService>, StrategyBuilderFactory<CphService>>();
            services
                .AddTransient<IStrategyBuilderFactory<CphDelegationService>,
                    StrategyBuilderFactory<CphDelegationService>>();
            services.AddTransient<IStrategyBuilderFactory<ProfileService>, StrategyBuilderFactory<ProfileService>>();
            services
                .AddTransient<IStrategyBuilderFactory<AnimalSpeciesService>,
                    StrategyBuilderFactory<AnimalSpeciesService>>();

            return services;
        }
    }
}
