// <copyright file="DeleteTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.DelegatesRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegates;
using Defra.Identity.Repositories.Exceptions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class DeleteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should soft delete a delegation")]
    public async Task ShouldDeleteDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);

        var adminUser = Context.UserAccounts.First();
        var application = new Applications
        {
            Name = "Delete Delegation Test App",
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

        var operatorId = adminUser.Id;

        // Act
        var result = await repository.Delete(x => x.Id == delegation.Id, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var deletedDelegation = await Context.Delegations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == delegation.Id, TestContext.Current.CancellationToken);
        deletedDelegation.ShouldNotBeNull();
        deletedDelegation.DeletedById.ShouldBe(operatorId);
        deletedDelegation.DeletedAt.ShouldNotBeNull();
    }

    [Fact]
    [Description("Should throw NotFoundException when delegation does not exist")]
    public async Task ShouldThrowNotFoundExceptionWhenDelegationDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<DelegatesRepository>>();
        var repository = new DelegatesRepository(Context, logger);
        var adminUser = Context.UserAccounts.First();
        var operatorId = adminUser.Id;

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(async () =>
            await repository.Delete(x => x.Id == Guid.NewGuid(), operatorId, TestContext.Current.CancellationToken));
    }
}
