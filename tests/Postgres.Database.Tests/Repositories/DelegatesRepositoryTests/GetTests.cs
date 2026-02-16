// <copyright file="GetTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.DelegatesRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegates;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single delegation")]
    public async Task ShouldGetSingleDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();
        var application = new Applications
        {
            Name = "Delegates Test App",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };
        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var delegation = new Delegations
        {
            ApplicationId = application.Id,
            UserId = adminUser.Id,
            CreatedById = adminUser.Id,
        };
        await Context.Delegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(x => x.ApplicationId == application.Id && x.UserId == adminUser.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.ApplicationId.ShouldBe(application.Id);
        result.UserId.ShouldBe(adminUser.Id);
    }

    [Fact]
    [Description("Should get a list of delegations")]
    public async Task ShouldGetListDelegations()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();
        var application = new Applications
        {
            Name = "Delegates List Test App",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };
        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var delegations = new List<Delegations>
        {
            new() { ApplicationId = application.Id, UserId = adminUser.Id, CreatedById = adminUser.Id },
        };

        // We need another user for another delegation to the same app, or another app for the same user
        var anotherUser = new UserAccounts { Id = Guid.NewGuid(), DisplayName = "Another User", EmailAddress = "another@test.com", CreatedById = adminUser.Id };
        await Context.UserAccounts.AddAsync(anotherUser);

        delegations.Add(new() { ApplicationId = application.Id, UserId = anotherUser.Id, CreatedById = adminUser.Id });

        await Context.Delegations.AddRangeAsync(delegations, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetList(x => x.ApplicationId == application.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }
}
