// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.ApplicationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Applications;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single application")]
    public async Task ShouldGetSingleApplication()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApplicationsRepository>>();
        var repository = new ApplicationsRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();

        var application = new Applications
        {
            Name = "Get Single Test",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };
        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(x => x.Name == "Get Single Test", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Name.ShouldBe("Get Single Test");
    }

    [Fact]
    [Description("Should get a list of applications")]
    public async Task ShouldGetListApplications()
    {
        // Arrange
        var logger = Substitute.For<ILogger<ApplicationsRepository>>();
        var repository = new ApplicationsRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();

        var apps = new List<Applications>
        {
            new() { Name = "App 1", ClientId = Guid.NewGuid(), TenantName = "Tenant 1", CreatedById = adminUser.Id },
            new() { Name = "App 2", ClientId = Guid.NewGuid(), TenantName = "Tenant 2", CreatedById = adminUser.Id },
        };
        await Context.Applications.AddRangeAsync(apps, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetList(x => x.CreatedById == adminUser.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
        result.Any(x => x.Name == "App 1").ShouldBeTrue();
        result.Any(x => x.Name == "App 2").ShouldBeTrue();
    }
}
