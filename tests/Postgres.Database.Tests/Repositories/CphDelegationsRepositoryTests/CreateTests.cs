// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.CphDelegationsRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should create a new delegation")]
    public async Task ShouldCreateDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var adminUser = Context.UserAccounts.First();
        var cphId = new Guid("4435a146-d0ac-4260-8a27-c550e0ed9563");
        var delegatingUserId = new Guid("0a629f9f-2d25-4ac5-afbf-e821f5c6e7d1");
        var delegatedUserId = new Guid("42bde7a0-9efe-402a-a7c3-9161be7b00ba");
        var delegatedUserRoleId = new Guid("0c15ba2f-b4ba-406a-a0ae-213de64600a9");
        const string delegatedUserEmail = "test1@test.com";
        var createdAt = DateTime.Now.ToUniversalTime();

        var newDelegation = new CountyParishHoldingDelegations
        {
            CountyParishHoldingId = cphId,
            DelegatingUserId = delegatingUserId,
            DelegatedUserId = delegatedUserId,
            DelegatedUserRoleId = delegatedUserRoleId,
            DelegatedUserEmail = delegatedUserEmail,
            InvitationToken = string.Empty,
            CreatedById = adminUser.Id,
            CreatedAt = createdAt,
        };

        // Act
        var createdDelegation = await repository.Create(newDelegation, TestContext.Current.CancellationToken);

        // Assert
        createdDelegation.ShouldSatisfyAllConditions(
            x => x.ShouldNotBeNull(),
            x => x.CountyParishHoldingId.ShouldBe(cphId),
            x => x.DelegatingUserId.ShouldBe(delegatingUserId),
            x => x.DelegatedUserId.ShouldBe(delegatedUserId),
            x => x.DelegatedUserRoleId.ShouldBe(delegatedUserRoleId),
            x => x.DelegatedUserEmail.ShouldBe(delegatedUserEmail),
            x => x.InvitationToken.ShouldBeEmpty(),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBe(createdAt));

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
