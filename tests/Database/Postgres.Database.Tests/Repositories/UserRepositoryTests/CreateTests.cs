// <copyright file="CreateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories.UserRepositoryTests;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Test.Utilities.Assertions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

public class CreateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string EmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new user account")]
    public async Task ShouldCreateUserAccount()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var repository = new UserRepository(Context, ReadOnlyContext, logger);
        var adminUser = await repository.GetSingle(
            x =>
                x.EmailAddress == EmailAddress,
            TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newUser = new UserAccounts
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test1@example.com",
            CreatedById = adminUser.Id,
            CreatedAt = DateTime.UtcNow.AddDays(-2),
            DeletedById = adminUser.Id,
            DeletedAt = DateTime.UtcNow.AddDays(-1),
        };

        // Act
        var createdUser = await repository.Create(newUser, TestContext.Current.CancellationToken);

        // Assert
        createdUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Test User"),
            x => x.FirstName.ShouldBe("Test"),
            x => x.LastName.ShouldBe("User"),
            x => x.EmailAddress.ShouldBe("test1@example.com"),
            x => x.CreatedById.ShouldBe(adminUser.Id),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(-2),
            x => x.DeletedById.ShouldBe(adminUser.Id),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(-1));

        logger.VerifyLogContainsOne(LogLevel.Information, "Creating user account");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Description("Should throw duplicate exception")]
    public async Task ShouldThrowDuplicateException()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var repository = new UserRepository(Context, ReadOnlyContext, logger);

        var adminUser = await repository
            .GetSingle(x => x.EmailAddress == EmailAddress, TestContext.Current.CancellationToken);

        adminUser.ShouldNotBeNull();

        var duplicateUser = new UserAccounts
        {
            DisplayName = "Dup User",
            FirstName = "Dup",
            LastName = "User",
            EmailAddress = EmailAddress, // <-- violates unique constraint om email address field
            CreatedById = adminUser.Id,
        };

        // Act
        Func<Task> act = async () => await repository
            .Create(duplicateUser, TestContext.Current.CancellationToken);

        // Assert
        var ex = await act.ShouldThrowAsync<DbUpdateException>();
        ex.InnerException.ShouldNotBeNull();

        logger.VerifyLogContainsOne(LogLevel.Information, "Creating user account");
    }
}
