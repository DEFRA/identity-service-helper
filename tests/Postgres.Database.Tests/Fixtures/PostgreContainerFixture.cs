// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Testcontainers.PostgreSql;

public class PostgreContainerFixture
    : IAsyncLifetime
{
    private readonly PostgreSqlContainer container = new PostgreSqlBuilder()
        .WithImage("postgres:16")
        .Build();

    public string ConnectionString => container.GetConnectionString();

    public async ValueTask DisposeAsync()
    {
        await container.StopAsync();
    }

    public async ValueTask InitializeAsync()
    {
        await container.StartAsync();
        var options = new DbContextOptionsBuilder<PostgresDbContext>()
            .UseNpgsql(container.GetConnectionString())
            .ConfigureWarnings(w => w.Ignore(RelationalEventId.PendingModelChangesWarning))
            .Options;

        var context = new PostgresDbContext(options);
        await context.Database.MigrateAsync();
        await SeedData(context);
    }

    private async Task SeedData(PostgresDbContext context)
    {
        const string adminEmailAddress = "test@test.com";

        if (!context.UserAccounts.Any())
        {
            var id = Guid.NewGuid();
            var adminUser = await context.UserAccounts.AddAsync(
                new UserAccounts()
                    { Id = id, DisplayName = "Test User", EmailAddress = adminEmailAddress, FirstName = "test", LastName = "user", CreatedById = id },
                TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }
    }
}
