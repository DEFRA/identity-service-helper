// <copyright file="DeleteTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphDelegationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Delegations;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class DeleteByIdRouteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should soft delete a delegation")]
    public async Task ShouldDeleteDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var id = Guid.NewGuid();
        var adminUser = Context.UserAccounts.First();
        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        var delegatingUserId = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1");
        var delegatedUserId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba");
        var delegatedUserRoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9");
        const string delegatedUserEmail = "test1@test.com";
        var createdAt = DateTime.UtcNow;
        var operatorId = adminUser.Id;

        var delegationToDelete = new CountyParishHoldingDelegations
        {
            Id = id,
            CountyParishHoldingId = cphId,
            DelegatingUserId = delegatingUserId,
            DelegatedUserId = delegatedUserId,
            DelegatedUserRoleId = delegatedUserRoleId,
            DelegatedUserEmail = delegatedUserEmail,
            InvitationToken = string.Empty,
            CreatedById = adminUser.Id,
            CreatedAt = createdAt,
        };

        await Context.CountyParishHoldingDelegations.AddAsync(delegationToDelete, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.Delete(x => x.Id == id, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();

        var deletedDelegation = await Context.CountyParishHoldingDelegations.IgnoreQueryFilters().SingleOrDefaultAsync(x => x.Id == delegationToDelete.Id, TestContext.Current.CancellationToken);

        deletedDelegation.ShouldSatisfyAllConditions(
            x =>
            {
                x.ShouldNotBeNull();
                x.DeletedById.ShouldBe(operatorId);
                x.DeletedAt.ShouldNotBeNull();
            });
    }

    [Fact]
    [Description("Should throw NotFoundException when delegation does not exist")]
    public async Task ShouldThrowNotFoundExceptionWhenDelegationDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();
        var operatorId = adminUser.Id;

        // Act & Assert
        await Should.ThrowAsync<NotFoundException>(
            async () =>
                await repository.Delete(x => x.Id == Guid.NewGuid(), operatorId, TestContext.Current.CancellationToken));
    }
}
