// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database;

using Defra.Identity.Mongo.Database.Configuration;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using MongoDB.Driver;

public static class ServiceCollectionExtensions
{
    public static void AddMongoDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<Mongo>(configuration.GetSection(nameof(Mongo)));
        var poo = configuration.GetSection(nameof(Mongo)).Get<Mongo>();
        var mongoCLient = new MongoClient(poo.DatabaseUri);
        services.AddSingleton<IMongoClient>(mongoCLient);
        /*services
            .AddPooledDbContextFactory<AuthMongoContext>((sp, options) =>
            {
                var env = sp.GetRequiredService<IHostEnvironment>();
                var isProd = env.IsProduction();
                var cfg = sp.GetRequiredService<IOptions<Mongo>>().Value;
                var client = new MongoClient(cfg.DatabaseUri);
                options
                    .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                    .UseMongoDB(
                        client, cfg.DatabaseName)
                    .EnableSensitiveDataLogging(isProd);
            });
        services.AddSingleton<IClientFactory, MongoClientFactory>(sp =>
        {
            var cfg = sp.GetRequiredService<IOptions<Mongo>>().Value;
            return new MongoClientFactory(cfg);
        });*/

        services.AddDbContext<AuthMongoContext>((provider, options) =>
        {
            var client = provider.GetRequiredService<IMongoClient>();
            options.UseMongoDB(client, poo.DatabaseName);
        });
    }

    public static void UseMongoDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<AuthMongoContext>();
        if (context is not null)
        {
            Console.WriteLine("Mongo DB is ready");
        }
    }
}
