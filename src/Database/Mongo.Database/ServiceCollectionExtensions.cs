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
        var cfg = configuration.GetSection(nameof(Mongo)).Get<Mongo>();
        if (cfg != null)
        {
            services.AddDbContext<AuthMongoContext>((provider, options) =>
            {
                options.UseMongoDB(cfg.DatabaseUri, cfg.DatabaseName);
            });
        }
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
