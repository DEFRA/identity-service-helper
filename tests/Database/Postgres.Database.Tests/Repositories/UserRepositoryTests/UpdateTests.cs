// <copyright file="UpdateTests.cs" company="Defra">
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
using Shouldly;

public class UpdateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    [Fact]
    [Description("Should update an existing user account")]
    public async Task ShouldUpdateUserAccount()
    {
        // Arrange
        var logger = DefraLoggerExtensions.CreateNSubstituteLogger<UserRepository>();
        var repository = new UserRepository(Context, ReadOnlyContext, logger);

        var user = new UserAccounts
        {
            Id = Guid.NewGuid(),
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test20@test.com",
            CreatedById = AdminUserId,
            CreatedAt = DateTime.UtcNow.AddDays(1),
            DeletedById = null,
            DeletedAt = null,
        };

        await Context.UserAccounts.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        user.DisplayName = "Updated Display Name";
        user.FirstName = "Updated First Name";
        user.LastName = "Updated Last Name";
        user.EmailAddress = "updated@test.com";
        user.DeletedById = AdminUserId;
        user.DeletedAt = DateTime.UtcNow.AddDays(2);

        // Act
        await repository.Update(user, TestContext.Current.CancellationToken);

        // Assert
        var updatedUser = await Context.UserAccounts.SingleAsync(
            x => x.EmailAddress.Equals(user.EmailAddress),
            TestContext.Current.CancellationToken);

        updatedUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Display Name"),
            x => x.FirstName.ShouldBe("Updated First Name"),
            x => x.LastName.ShouldBe("Updated Last Name"),
            x => x.EmailAddress.ShouldBe("updated@test.com"),
            x => x.CreatedById.ShouldBe(AdminUserId),
            x => x.CreatedAt.ShouldBeCloseToUtcNowAddDays(1),
            x => x.DeletedById.ShouldBe(AdminUserId),
            x => x.DeletedAt!.Value.ShouldBeCloseToUtcNowAddDays(2));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Updating user account with id {user.Id}");
    }
}
