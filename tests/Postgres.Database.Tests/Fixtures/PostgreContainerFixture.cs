// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Microsoft.EntityFrameworkCore;
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
        var options = new DbContextOptionsBuilder<AuthContext>()
            .UseNpgsql(container.GetConnectionString())
            .Options;

        var context = new AuthContext(options);
        await context.Database.MigrateAsync();
        await SeedData(context);
    }

    private async Task SeedData(AuthContext context)
    {
        const string adminEmailAddress = "test@test.com";
        if (!context.StatusTypes.Any())
        {
            await context.StatusTypes.AddAsync(new StatusType() { Name = "Active" }, TestContext.Current.CancellationToken);
            await context.StatusTypes.AddAsync(new StatusType() { Name = "InActive" }, TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        if (!context.Users.Any())
        {
            var id = Guid.NewGuid();
            var adminUser = await context.Users.AddAsync(
                new UserAccount()
                    { Id = id, DisplayName = "Test User", EmailAddress = adminEmailAddress, FirstName = "test", LastName = "user", CreatedBy = id },
                TestContext.Current.CancellationToken);
        }
    }
}
