// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Infrastructure.Registration;

using Defra.Identity.Infrastructure.Database.Repositories;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static IServiceCollection AddRepositories(this IServiceCollection services, IConfigurationRoot config)
    {
        services.AddTransient<IRepository<UserAccount>, UsersRepository>();

        return services;
    }
}
