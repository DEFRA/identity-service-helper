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
        var delegation = new CountyParishHoldingDelegations
        {
            DelegatedUserId = adminUser.Id,
            CreatedById = adminUser.Id,
            DelegatedUserEmail = AdminEmailAddress,
        };
        await Context.CountyParishHoldingDelegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(
            x => x.DelegatedUserId == adminUser.Id,
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.DelegatedUserId.ShouldBe(adminUser.Id);
        result.DelegatedUserId.ShouldBe(adminUser.Id);
        result.DelegatedUserEmail.ShouldBe(AdminEmailAddress);
    }

    [Fact]
    [Description("Should get a list of delegations")]
    public async Task ShouldGetListDelegations()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();
        var delegations = new List<CountyParishHoldingDelegations>
        {
            new()
            {
                DelegatedUserId = adminUser.Id,
                CreatedById = adminUser.Id,
                DelegatedUserEmail = AdminEmailAddress,
            },
        };

        // We need another user for another delegation to the same app, or another app for the same user
        var anotherUser = new UserAccounts { Id = Guid.NewGuid(), DisplayName = "Another User", EmailAddress = "another@test.com", CreatedById = adminUser.Id };
        await Context.UserAccounts.AddAsync(anotherUser, TestContext.Current.CancellationToken);

        delegations.Add(new()
        {
            DelegatedUserId = DelegatedUserId, CreatedById = adminUser.Id, DelegatedUserEmail = DelegatedEmailAddress,
        });

        await Context.CountyParishHoldingDelegations.AddRangeAsync(delegations, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetList(x => x.CountyParishHolding.Identifier == "11/222/3333", TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBeGreaterThanOrEqualTo(2);
    }
}
