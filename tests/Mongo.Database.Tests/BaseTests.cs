// <copyright file="BaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

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

    public ValueTask DisposeAsync()
    {
        throw new NotImplementedException();
    }

    public ValueTask InitializeAsync()
    {
        throw new NotImplementedException();
    }
}
