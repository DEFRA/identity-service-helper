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
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new delegation")]
    public async Task ShouldCreateDelegation()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphDelegationsRepository>>();
        var repository = new CphDelegationsRepository(Context, ReadOnlyContext, logger);

        var userLogger = Substitute.For<ILogger<UsersRepository>>();
        var userRepository = new UsersRepository(Context, ReadOnlyContext, userLogger);

        var adminUser = await userRepository.GetSingle(
            x => x.EmailAddress == AdminEmailAddress,
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var application = new Applications
        {
            Name = "Create Delegation Test App",
            ClientId = Guid.NewGuid(),
            TenantName = "Test Tenant",
            CreatedById = adminUser.Id,
        };
        await Context.Applications.AddAsync(application, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        var newDelegation = new CountyParishHoldingDelegations
        {
            DelegatingUserId = adminUser.Id,
            CreatedById = adminUser.Id,
            DelegatedUserEmail = AdminEmailAddress,
            InvitationToken = string.Empty,
        };

        // Act
        var createdDelegation = await repository.Create(newDelegation, TestContext.Current.CancellationToken);

        // Assert
        createdDelegation.ShouldSatisfyAllConditions(
            x => x.DelegatingUserId.ShouldBe(adminUser.Id),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.DelegatedUserEmail.ShouldBe(AdminEmailAddress));

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
