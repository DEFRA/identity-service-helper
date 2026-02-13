// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IUsersRepository, UsersRepository>();
        services.AddTransient<IApplicationsRepository, ApplicationsRepository>();

        return services;
    }
}
