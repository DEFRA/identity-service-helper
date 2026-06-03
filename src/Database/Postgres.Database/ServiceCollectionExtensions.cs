// <copyright file="ServiceCollectionExtensions.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database;

using Amazon;
using Amazon.RDS.Util;
using Amazon.Runtime;
using Amazon.Runtime.Credentials;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Npgsql;

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
    private static readonly string[] ErrorCodes =
    [
        "40001", "40P01", "55P03", "53300", "57014", "57P01", "57P02", "57P03", "58030",
        "08000", "08001", "08003", "08004", "08006", "08007", "08P01",
    ];

    public static IServiceCollection AddPostgresDatabase(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddDbContext<PostgresDbContext>((sp, options) => ConfigureNpgsql(sp, options, configuration));

        services.AddDbContext<ReadOnlyPostgresDbContext>((sp, options) =>
        {
            ConfigureNpgsql(sp, options, configuration, true);
            options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
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

    private static void ConfigureNpgsql(
        IServiceProvider sp,
        DbContextOptionsBuilder options,
        IConfiguration configuration,
        bool isReadOnly = false)
    {
        string? connectionString;
        var postgresConfig = GetPostgresConfiguration(configuration);
        if (postgresConfig.UseIamAuthentication)
        {
            var credentials = sp.GetService<AWSCredentials>()
                              ?? DefaultAWSCredentialsIdentityResolver.GetCredentials();
            var region = RegionEndpoint.GetBySystemName(
                configuration.GetValue<string>("AWS:Region") ?? "eu-west-2");

            var host = isReadOnly ? postgresConfig.ReadOnlyHost : postgresConfig.DefaultHost;
            connectionString = BuildConnectionString(credentials, region, host, postgresConfig);
        }
        else
        {
            connectionString = configuration.GetConnectionString(
                isReadOnly ? DatabaseConstants.ReadOnlyConnectionStringName : DatabaseConstants.ConnectionStringName);

            if (isReadOnly && string.IsNullOrEmpty(connectionString))
            {
                connectionString = configuration.GetConnectionString(DatabaseConstants.ReadOnlyConnectionStringName);
            }
        }

        if (string.IsNullOrEmpty(connectionString))
        {
            throw new InvalidOperationException(
                $"Connection string for {(isReadOnly ? "read-only " : string.Empty)}Postgres is missing");
        }

        CreateConnection(sp, options, connectionString, postgresConfig.UseIamAuthentication);
    }

    private static PostgresConfiguration GetPostgresConfiguration(IConfiguration configuration)
    {
        var postgresConfig = configuration
                                 .GetSection(nameof(PostgresConfiguration))
                                 .Get<PostgresConfiguration>()
                             ?? new PostgresConfiguration();
        return postgresConfig;
    }

    private static string BuildConnectionString(
        AWSCredentials credentials,
        RegionEndpoint region,
        string host,
        PostgresConfiguration config)
    {
        var token = RDSAuthTokenGenerator.GenerateAuthToken(credentials, region,   host, config.Port, config.User);
        var builder = new NpgsqlConnectionStringBuilder
        {
            Host = host,
            Port = config.Port,
            Username = config.User,
            Database = config.Name,
            Password = token,
            SslMode = SslMode.Require,
        };

        return builder.ConnectionString;
    }

    private static void CreateConnection(
        IServiceProvider sp,
        DbContextOptionsBuilder options,
        string connectionString,
        bool isProd)
    {
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
    }
}
