// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;

using Microsoft.Extensions.Logging;
using NSubstitute;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new user account")]
    public async Task ShouldCreateUserAccount()
    {
        // Arrange
        var logger = Substitute.For<ILogger<UsersRepository>>();
        var repository = new UsersRepository(Context, logger);

        var adminEmail = AdminEmailAddress;

        var adminUser = await repository.GetSingle(
            x =>
                x.EmailAddress == adminEmail,
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newUser = new UserAccounts
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test1@example.com",
            CreatedById = adminUser.Id,
        };

        // Act
        var createdUser = await repository.Create(newUser, TestContext.Current.CancellationToken);

        // Assert
        createdUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Test User"),
            x => x.FirstName.ShouldBe("Test"),
            x => x.LastName.ShouldBe("User"));

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
