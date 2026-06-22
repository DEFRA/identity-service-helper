// <copyright file="UpdateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphDelegationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.Extensions.Logging;
using Shouldly;

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update a delegation")]
    public async Task ShouldUpdateDelegation()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<CphDelegationsRepository>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var id = Guid.NewGuid();

        var cph1 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0001");
        var cph2 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0002");

        var delegatingUser1 = Context.UserAccounts.First(x => x.EmailAddress == "test1@test.com");
        var delegatingUser2 = Context.UserAccounts.First(x => x.EmailAddress == "test2@test.com");

        var delegatedUser1 = Context.UserAccounts.First(x => x.EmailAddress == "test3@test.com");
        var delegatedUser2 = Context.UserAccounts.First(x => x.EmailAddress == "test4@test.com");

        var delegatedUser1Role = Context.Roles.First(x => x.Name == "test-role-1");
        var delegatedUser2Role = Context.Roles.First(x => x.Name == "test-role-1");

        var delegation = new CountyParishHoldingDelegations()
        {
            Id = id,
            CountyParishHoldingId = cph1.Id,
            DelegatingUserId = delegatingUser1.Id,
            DelegatedUserId = delegatedUser1.Id,
            DelegatedUserRoleId = delegatedUser1Role.Id,
            DelegatedUserEmail = delegatedUser1.EmailAddress,
            InvitationToken = string.Empty,
            RevokedById = adminUser.Id,
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(1),
            InvitationExpiresAt = DateTime.UtcNow.AddDays(3),
            InvitationAcceptedAt = DateTime.UtcNow.AddDays(2),
            InvitationRejectedAt = null,
            ExpiresAt = DateTime.UtcNow.AddDays(4),
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow.AddDays(5),
        };

        await Context.CountyParishHoldingDelegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        delegation.CountyParishHoldingId = cph2.Id;
        delegation.DelegatingUserId = delegatingUser2.Id;
        delegation.DelegatedUserId = delegatedUser2.Id;
        delegation.DelegatedUserRoleId = delegatedUser2Role.Id;
        delegation.DelegatedUserEmail = delegatedUser2.EmailAddress;
        delegation.InvitationToken = string.Empty;
        delegation.RevokedById = adminUser.Id;
        delegation.CreatedById = adminUser.Id;
        delegation.CreatedAt = DateTime.UtcNow.AddDays(2);
        delegation.InvitationExpiresAt = DateTime.UtcNow.AddDays(4);
        delegation.InvitationRejectedAt = DateTime.UtcNow.AddDays(3);
        delegation.InvitationAcceptedAt = null;
        delegation.ExpiresAt = DateTime.UtcNow.AddDays(5);
        delegation.DeletedById = adminUser.Id;
        delegation.DeletedAt = DateTime.UtcNow.AddDays(6);

        // Act
        var updatedDelegation = await repository.Update(delegation, TestContext.Current.CancellationToken);

        // Assert
        updatedDelegation.ShouldSatisfyAllConditions(
            x => x.Id.ShouldBe(id),
            x => x.CountyParishHoldingId.ShouldBe(cph2.Id),
            x => x.DelegatingUserId.ShouldBe(delegatingUser2.Id),
            x => x.DelegatedUserId.ShouldBe(delegatedUser2.Id),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUser2Role.Id),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUser2.EmailAddress),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.RevokedById.ShouldBe(adminUser.Id),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(2),
            x => x.InvitationExpiresAt.ShouldBeCloseToUtcNowAddDays(4),
            x => x.InvitationRejectedAt!.Value.ShouldBeCloseToUtcNowAddDays(3),
            x => x.InvitationAcceptedAt.ShouldBeNull(),
            x => x.ExpiresAt!.Value.ShouldBeCloseToUtcNowAddDays(5),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(6));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Updating delegation with id {id}");
    }
}
