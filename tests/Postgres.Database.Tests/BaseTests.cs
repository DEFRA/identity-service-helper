// <copyright file="BaseTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests;

using Defra.Identity.Postgres.Database;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Collections;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Microsoft.AspNetCore.Builder;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

[Collection(nameof(PostgreCollection))]
public abstract class BaseTests(PostgreContainerFixture fixture) : IAsyncLifetime
{
    protected AuthContext Context { get; private set; } = null!;

    private Dictionary<string, string> ConnectionStringConfiguration => new()
    {
        { $"ConnectionStrings:{DatabaseConstants.ConnectionStringName}", $"{fixture.ConnectionString}" },
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
        builder.Services.AddAuthDatabase(builder.Configuration);

        var app = builder.Build();

        app.UseAuthDatabase();
        app.UseDatabaseMigrations();
        var serviceProvider = builder.Services.BuildServiceProvider();
        Context = serviceProvider.GetRequiredService<AuthContext>();

        if (!Context.Users.Any())
        {
            await Context.StatusTypes.AddRangeAsync(
                new StatusType { Name = "New", Description = "New User" },
                new StatusType { Name = "Active", Description = "Active User" },
                new StatusType { Name = "Suspended", Description = "Suspended User" },
                new StatusType { Name = "Deleted", Description = "Deleted User" });
            await Context.SaveChangesAsync();

            var activeStatus = await Context.StatusTypes.FirstAsync(s => s.Name == "Active");

            var id = Guid.NewGuid();
            await Context.Users.AddAsync(new UserAccount()
            {
                Id = id,
                DisplayName = "Test User",
                EmailAddress = "test@test.com",
                FirstName = "test",
                LastName = "user",
                CreatedBy = id,
                StatusTypeId = activeStatus.Id,
            });
            await Context.SaveChangesAsync();
        }
    }
}
