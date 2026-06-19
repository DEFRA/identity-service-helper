// <copyright file="GetTests.cs" company="Defra">
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

public class GetTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should get a single delegation")]
    public async Task ShouldGetSingleDelegation()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<CphDelegationsRepository>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var id = Guid.NewGuid();
        var adminUser = Context.UserAccounts.First();
        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        var delegatingUserId = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1");
        var delegatedUserId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba");
        var delegatedUserRoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9");
        const string delegatedUserEmail = "test1@test.com";

        var delegation = new CountyParishHoldingDelegations
        {
            Id = id,
            CountyParishHoldingId = cphId,
            DelegatingUserId = delegatingUserId,
            DelegatedUserId = delegatedUserId,
            DelegatedUserRoleId = delegatedUserRoleId,
            DelegatedUserEmail = delegatedUserEmail,
            InvitationToken = string.Empty,
            RevokedById = adminUser.Id,
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-5),
            InvitationExpiresAt = DateTime.UtcNow.AddDays(-4),
            InvitationAcceptedAt = DateTime.UtcNow.AddDays(-3),
            InvitationRejectedAt = null,
            ExpiresAt = DateTime.UtcNow.AddDays(-2),
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow.AddDays(-1),
        };

        await Context.CountyParishHoldingDelegations.AddAsync(delegation, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.GetSingle(
            x => x.Id == id,
            TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();

        result.ShouldSatisfyAllConditions(
            x => x.Id.ShouldBe(id),
            x => x.CountyParishHoldingId.ShouldBe(cphId),
            x => x.DelegatingUserId.ShouldBe(delegatingUserId),
            x => x.DelegatedUserId.ShouldBe(delegatedUserId),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUserRoleId),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUserEmail),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.RevokedById.ShouldBe(adminUser.Id),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(-5),
            x => x.InvitationExpiresAt.ShouldBeCloseToUtcNowAddDays(-4),
            x => x.InvitationAcceptedAt!.Value.ShouldBeCloseToUtcNowAddDays(-3),
            x => x.InvitationRejectedAt.ShouldBeNull(),
            x => x.ExpiresAt!.Value.ShouldBeCloseToUtcNowAddDays(-2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(-1));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting single delegation");
    }

    [Fact]
    [Description("Should get a list of delegations")]
    public async Task ShouldGetListDelegations()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<CphDelegationsRepository>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();

        var cph1 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0001");
        var cph2 = Context.CountyParishHoldings.First(x => x.Identifier == "44/000/0002");

        var delegatingUser1 = Context.UserAccounts.First(x => x.EmailAddress == "test1@test.com");
        var delegatingUser2 = Context.UserAccounts.First(x => x.EmailAddress == "test2@test.com");

        var delegatedUser1 = Context.UserAccounts.First(x => x.EmailAddress == "test3@test.com");
        var delegatedUser2 = Context.UserAccounts.First(x => x.EmailAddress == "test4@test.com");

        var delegatedUser1Role = Context.Roles.First(x => x.Name == "test-role-1");
        var delegatedUser2Role = Context.Roles.First(x => x.Name == "test-role-1");

        var delegations = new List<CountyParishHoldingDelegations>
        {
            new()
            {
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
            },
            new()
            {
                CountyParishHoldingId = cph2.Id,
                DelegatingUserId = delegatingUser2.Id,
                DelegatedUserId = delegatedUser2.Id,
                DelegatedUserRoleId = delegatedUser2Role.Id,
                DelegatedUserEmail = delegatedUser2.EmailAddress,
                InvitationToken = string.Empty,
                RevokedById = adminUser.Id,
                CreatedById = adminUser.Id,
                CreatedAt = DateTime.UtcNow.AddDays(2),
                InvitationExpiresAt = DateTime.UtcNow.AddDays(4),
                InvitationRejectedAt = DateTime.UtcNow.AddDays(3),
                InvitationAcceptedAt = null,
                ExpiresAt = DateTime.UtcNow.AddDays(5),
                DeletedById = adminUser.Id,
                DeletedAt = DateTime.UtcNow.AddDays(6),
            },
        };

        await Context.CountyParishHoldingDelegations.AddRangeAsync(delegations, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var results =
            (await repository.GetList(
                x => x.CreatedAt > DateTime.UtcNow,
                TestContext.Current.CancellationToken))
            .OrderBy(x => x.CountyParishHolding.Identifier)
            .ToList();

        // Assert
        results.ShouldNotBeNull();
        results.Count.ShouldBe(2);

        results[0].ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.CountyParishHoldingId.ShouldBe(cph1.Id),
            x => x.DelegatingUserId.ShouldBe(delegatingUser1.Id),
            x => x.DelegatedUserId.ShouldBe(delegatedUser1.Id),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUser1Role.Id),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUser1.EmailAddress),
            x => x.InvitationToken.ShouldBeNullOrWhiteSpace(),
            x => x.RevokedById.ShouldBe(adminUser.Id),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(1),
            x => x.InvitationExpiresAt.ShouldBeCloseToUtcNowAddDays(3),
            x => x.InvitationAcceptedAt!.Value.ShouldBeCloseToUtcNowAddDays(2),
            x => x.InvitationRejectedAt.ShouldBeNull(),
            x => x.ExpiresAt!.Value.ShouldBeCloseToUtcNowAddDays(4),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(5));

        results[1].ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
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

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting list of county parish holding delegations");
    }
}
