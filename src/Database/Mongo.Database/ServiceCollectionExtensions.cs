// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Defra.Identity.Mongo.Database.Configuration;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;

public static class ServiceCollectionExtensions
{
    public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Mongo>(configuration.GetSection(nameof(Mongo)));
        services.AddSingleton<IClientFactory, MongoClientFactory>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<Mongo>>().Value;
            return new MongoClientFactory(cfg);
        });
    }
}
