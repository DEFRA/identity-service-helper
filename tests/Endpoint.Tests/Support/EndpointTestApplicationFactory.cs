// <copyright file="EndpointTestApplicationFactory.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Endpoint.Tests.Support;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.AspNetCore.TestHost;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;

public class EndpointTestApplicationFactory : WebApplicationFactory<Defra.Identity.Api.Program>
{
    protected override void ConfigureWebHost(IWebHostBuilder builder)
    {
        builder.ConfigureAppConfiguration((_, config) =>
        {
            var cs = PostgreContainerFixture.ConnectionString;

            config.AddInMemoryCollection(new Dictionary<string, string?>
            {
                ["ConnectionStrings:PostgresConnection"] = cs,
                ["ConnectionStrings:ReadOnlyPostgresConnection"] = cs,
                ["DefraIdentityApiKey"] = "test",
            });
        });

        builder.ConfigureTestServices(services =>
        {
            var cs = PostgreContainerFixture.ConnectionString;

            services.RemoveAll<DbContextOptions<PostgresDbContext>>();
            services.RemoveAll<DbContextOptions<ReadOnlyPostgresDbContext>>();
            services.RemoveAll<PostgresConfiguration>();
            services.RemoveAll<IPostgresDataSourceFactory>();

            services.AddSingleton(new PostgresConfiguration
            {
                ConnectionString = cs,
                ReadOnlyConnectionString = cs,
                UseIamAuthentication = false,
            });

            services.AddSingleton<IPostgresDataSourceFactory, PostgresDataSourceFactory>();

            services.AddDbContext<PostgresDbContext>((sp, options) =>
            {
                var dataSourceFactory = sp.GetRequiredService<IPostgresDataSourceFactory>();
                var dataSource = dataSourceFactory.CreateDataSource("Default");

                options.UseNpgsql(dataSource);
            });

            services.AddDbContext<ReadOnlyPostgresDbContext>((sp, options) =>
            {
                var dataSourceFactory = sp.GetRequiredService<IPostgresDataSourceFactory>();
                var dataSource = dataSourceFactory.CreateDataSource("ReadOnly");

                options
                    .UseNpgsql(dataSource)
                    .UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
        });
    }
}
