// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Defra.Identity.Mongo.Database.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

public static class ServiceCollectionExtensions
{
    public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<MongoDatabase>(configuration.GetSection(nameof(MongoDatabase)));
        services.AddSingleton<IClientFactory, MongoClientFactory>();
    }
}
