// <copyright file="BaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace Defra.Identity.Mongo.Database.Tests;

using Defra.Identity.Mongo.Database.Tests.Collections;
using Defra.Identity.Mongo.Database.Tests.Fixtures;

[Collection(nameof(MongoCollection))]
public abstract class BaseTests(MongoContainerFixture fixture) : IAsyncLifetime
{
    protected AuthMongoContext Context { get; private set; } = null!;

    private Dictionary<string, string> ConnectionStringConfiguration => new()
    {
        { $"Mongo:DatabaseUri", $"{fixture.ConnectionString}" },
        { $"Mongo:DatabaseName", $"{MongoConstants.DatabaseName}" },
        { "Deployment:Environment", "Dev" },
    };

    public async ValueTask DisposeAsync()
    {
        await Context.DisposeAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await fixture.InitializeAsync();
        var builder = WebApplication
            .CreateBuilder();
        builder.Configuration.AddInMemoryCollection(ConnectionStringConfiguration!).Build();
        builder.Services.AddMongoDatabase(builder.Configuration);

        var app = builder.Build();

        app.UseMongoDatabase();
        var serviceProvider = builder.Services.BuildServiceProvider();
        Context = serviceProvider.GetRequiredService<AuthMongoContext>();
    }
}
