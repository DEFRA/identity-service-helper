// <copyright file="MongoContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Mongo.Database.Tests.Fixtures;

using Microsoft.EntityFrameworkCore;
using Testcontainers.MongoDb;

public class MongoContainerFixture : IAsyncLifetime
{
    private readonly MongoDbContainer container = new MongoDbBuilder()
        .WithImage("mongo:6.0")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
       await container.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();
        var options = new DbContextOptionsBuilder<AuthMongoContext>()
            .UseMongoDB(container.GetConnectionString())
            .Options;

        var context = new AuthMongoContext(options);
    }
}
