// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Roles;
using Defra.Identity.Repositories.Species;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IAnimalSpeciesRepository, AnimalSpeciesRepository>();
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<ICphAssignmentsForAssigneeRepository, CphAssignmentsForAssigneeRepository>();
        services.AddTransient<ICphDelegationsForDelegateRepository, CphDelegationsForDelegateRepository>();
        services.AddTransient<ICphDelegationsForDelegatorRepository, CphDelegationsForDelegatorRepository>();
        services.AddTransient<ICphDelegatesForDelegatorRepository, CphDelegatesForDelegatorRepository>();
        services.AddTransient<IRoleRepository, RoleRepository>();
        services.AddTransient<IApplicationsRepository, ApplicationsRepository>();
        services.AddTransient<ICphRepository, CphRepository>();
        services.AddTransient<ICphDelegationsRepository, CphDelegationsRepository>();
        services.AddTransient<ICphAssignmentsRepository, CphAssignmentsRepository>();

        return services;
    }
}
