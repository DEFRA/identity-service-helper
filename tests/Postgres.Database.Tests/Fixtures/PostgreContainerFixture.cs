// <copyright file="PostgreContainerFixture.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Fixtures;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures.TestData;
using Defra.Identity.Postgres.Database.Tests.Fixtures.TestData.Helpers;
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
        await TearDownData(context);
        await SeedData(context);
    }

    private static async Task TearDownData(PostgresDbContext context)
    {
        var adminUser = await CreateAdminUser(context);

        await DeleteCphs(context);
    }

    private static async Task SeedData(PostgresDbContext context)
    {
        var adminUser = await CreateAdminUser(context);

        await CreateCphs(adminUser, context);
    }

    private static async Task<UserAccounts> CreateAdminUser(PostgresDbContext context)
    {
        if (!await context.UserAccounts.AnyAsync(user => user.EmailAddress == TestDataHelper.AdminEmailAddress))
        {
            var id = Guid.NewGuid();

            await context.UserAccounts.AddAsync(
                new UserAccounts()
                {
                    Id = id,
                    DisplayName = "Test User",
                    EmailAddress = TestDataHelper.AdminEmailAddress,
                    FirstName = "test",
                    LastName = "user",
                    CreatedById = id,
                },
                TestContext.Current.CancellationToken);
            await context.SaveChangesAsync(TestContext.Current.CancellationToken);
        }

        var adminUser = await TestDataHelper.GetAdminUser(context);

        return adminUser;
    }

    private static async Task CreateCphs(UserAccounts adminUser, PostgresDbContext context)
    {
        var cphEntities = CphTestData.CreateCphEntities(adminUser.CreatedById);

        foreach (var entity in cphEntities)
        {
            await context.CountyParishHoldings.AddAsync(entity, TestContext.Current.CancellationToken);
        }

        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }

    private static async Task DeleteCphs(PostgresDbContext context)
    {
        var cphEntities = await context.CountyParishHoldings.ToListAsync();
        context.CountyParishHoldings.RemoveRange(cphEntities);
        await context.SaveChangesAsync(TestContext.Current.CancellationToken);
    }
}
