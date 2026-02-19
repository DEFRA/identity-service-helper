// <copyright file="UpdateTests.cs" company="Defra">
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

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update a delegation")]
    public async Task ShouldUpdateDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();
        var application = new Applications
        {
            Name = "Update Delegation Test App",
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

        // In this case, we might update some property if there were any,
        // but Delegations only has ApplicationId and UserId which are usually part of the identity.
        // Let's see if there are other properties.
        // Actually, we can just call Update and ensure it returns the entity.

        // Act
        var updatedDelegation = await repository.Update(delegation, TestContext.Current.CancellationToken);

        // Assert
        updatedDelegation.ShouldNotBeNull();
        updatedDelegation.Id.ShouldBe(delegation.Id);
    }
}
