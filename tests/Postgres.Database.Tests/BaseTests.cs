// <copyright file="BaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Tests.Collections;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(PostgreCollection))]
public abstract partial class BaseTests(PostgreContainerFixture fixture) : IAsyncLifetime
{
    private IServiceScope? scope;

    protected PostgresDbContext Context { get; private set; } = null!;

    private Dictionary<string, string> ConnectionStringConfiguration => new()
    {
        { $"ConnectionStrings:{DatabaseConstants.ConnectionStringName}", $"{PostgreContainerFixture.ConnectionString}" },
        { "Deployment:Environment", "Dev" },
    };

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
        scope?.Dispose();
    }

    public async ValueTask InitializeAsync()
    {
        await fixture.InitializeAsync();
        var builder = WebApplication
            .CreateBuilder();
        builder.Configuration.AddInMemoryCollection(ConnectionStringConfiguration!).Build();
        builder.Services.AddPostgresDatabase(builder.Configuration);

        var app = builder.Build();

        app.UsePostgresDatabase();

        scope = app.Services.CreateScope();
        Context = scope.ServiceProvider.GetRequiredService<PostgresDbContext>();
    }
}
