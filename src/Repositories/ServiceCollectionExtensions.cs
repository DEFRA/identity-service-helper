// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Repositories;

using Defra.Identity.Mongo.Database.Documents;
using Defra.Identity.Repositories.Mongo;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddRepositories(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddTransient<IRepository<Applications>, ApplicationsRepository>();
    }

    public static void UseRepositories(this WebApplication app)
    {
       // use dependencies
    }
}
