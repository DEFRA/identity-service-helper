// <copyright file="UpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphDelegationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegations;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class PutByIdRouteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update a delegation")]
    public async Task ShouldUpdateDelegation()
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

        var delegation = new CountyParishHoldingDelegations
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

        await Context.CountyParishHoldingDelegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var updatedDelegation = await repository.Update(delegation, TestContext.Current.CancellationToken);

        // Assert
        updatedDelegation.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.Id.ShouldBe(id),
            x => x.CountyParishHoldingId.ShouldBe(cphId),
            x => x.DelegatingUserId.ShouldBe(delegatingUserId),
            x => x.DelegatedUserId.ShouldBe(delegatedUserId),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUserRoleId),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUserEmail),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.CreatedById.ShouldBe(adminUser.Id));
    }
}
