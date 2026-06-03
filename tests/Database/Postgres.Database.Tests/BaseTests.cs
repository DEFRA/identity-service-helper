// <copyright file="BaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Tests.Collections;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using NSubstitute;

/// <summary>
/// The base class for the database fixture driven tests.
/// </summary>
/// <param name="fixture">The test fixture used for this test.</param>
[Collection(nameof(PostgreCollection))]
public abstract partial class BaseTests(PostgreContainerFixture fixture) : IAsyncLifetime
{
    private IServiceScope? scope;

    protected PostgresDbContext Context { get; private set; } = null!;

    protected ReadOnlyPostgresDbContext ReadOnlyContext { get; private set; } = null!;

    private static Dictionary<string, string> ConnectionStringConfiguration => new()
    {
        { $"ConnectionStrings:{DatabaseConstants.ConnectionStringName}", $"{PostgreContainerFixture.ConnectionString}" },
        { $"ConnectionStrings:{DatabaseConstants.ReadOnlyConnectionStringName}", $"{PostgreContainerFixture.ConnectionString}" },
        { "Deployment:Environment", "Dev" },
    };

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        await ReadOnlyContext.DisposeAsync();
        scope?.Dispose();
    }

    public async ValueTask InitializeAsync()
    {
        await fixture.InitializeAsync();

        var configuration = new ConfigurationBuilder()
            .AddInMemoryCollection(ConnectionStringConfiguration!)
            .Build();

        var services = new ServiceCollection();
        services.AddLogging();
        services.AddPostgresDatabase(configuration);

        var serviceProvider = services.BuildServiceProvider();

        using (var initScope = serviceProvider.CreateScope())
        {
            var context = initScope.ServiceProvider.GetRequiredService<PostgresDbContext>();
            try
            {
                if (await context.Database.CanConnectAsync())
                {
                    await context.Database.OpenConnectionAsync();
                }
            }
            catch (InvalidOperationException ex) when (ex.Message.Contains("PendingModelChangesWarning"))
            {
                // Ignore pending model changes warning during tests/dev if it's treated as error
            }
        }

        scope = serviceProvider.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
        ReadOnlyContext = scope.ServiceProvider.GetRequiredService<ReadOnlyPostgresDbContext>();
    }
}
