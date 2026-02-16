// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

public static class ServiceCollectionExtensions
{
    private const int MaxRetryCount = 5;
    private const int MaxRetryDelay = 10;

    private const int CommandTimeout = 60;

    /// <summary>
    /// Common PostgreSQL SQLSTATEs often considered transient:
    /// 40001: Serialization failure
    /// 40P01: Deadlock detected
    /// 55P03: Lock not available
    /// 53300: Too many connections
    /// 57014: Query canceled
    /// 57P01: Admin shutdown
    /// 57P02: Crash shutdown
    /// 57P03: Cannot connect now
    /// 58030: I/O error
    /// 08000/08003/08006/08001/08004/08007/08P01: Connection-related errors (connection exception class 08)
    /// </summary>
    private static readonly string[] ErrorCodes = ["40001", "40P01", "55P03", "57P03"];

    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString(DatabaseConstants.ConnectionStringName);
        services.AddDbContext<PostgresDbContext>((sp, options) =>
        {
            var env = sp.GetRequiredService<IHostEnvironment>();
            var isProd = env.IsProduction();
            options
                .UseLoggerFactory(sp.GetRequiredService<ILoggerFactory>())
                .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
                .UseNpgsql(
                    connectionString,
                    npgsqlOptions =>
                    {
                        npgsqlOptions.EnableRetryOnFailure(
                            maxRetryCount: MaxRetryCount,
                            maxRetryDelay: TimeSpan.FromSeconds(MaxRetryDelay),
                            errorCodesToAdd: ErrorCodes);
                        npgsqlOptions.CommandTimeout(CommandTimeout);
                    })
                .EnableSensitiveDataLogging(!isProd);
        });

        return services;
    }

    public static void UsePostgresDatabase(this WebApplication app)
    {
        using var scope = app.Services.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();

        try
        {
            if (context.Database.CanConnect())
            {
                context.Database.OpenConnection();
            }
        }
        catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning"))
        {
            // Ignore pending model changes warning during tests/dev if it's treated as error
        }
    }

    public static void UseDatabaseMigrations(this WebApplication app)
    {
        var env = app.Services.GetRequiredService<IHostEnvironment>();

        // Migrate the database on startup in development mode
        if (env.IsDevelopment())
        {
            using var scope = app.Services.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            context.Database.Migrate();
        }
    }
}
