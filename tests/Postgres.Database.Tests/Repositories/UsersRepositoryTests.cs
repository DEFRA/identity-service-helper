// <copyright file="UsersRepositoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories;
using Microsoft.EntityFrameworkCore;
using Shouldly;

public class UsersRepositoryTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should create a new user account")]
    public async Task ShouldCreateUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);

        var adminEmail = AdminEmailAddress;

        var adminUser = await repository.Get(x =>
            x.EmailAddress.Equals(adminEmail));

        adminUser.ShouldNotBeNull("Seeded admin user was not found; check test data initialization.");

        var newUser = new UserAccount
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test1@test.com",
            CreatedBy = adminUser.Id,
        };

        // Act
        var createdUser = await repository.Create(newUser);

        // Assert
        createdUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Test User"),
            x => x.FirstName.ShouldBe("Test"),
            x => x.LastName.ShouldBe("User"),
            x => x.StatusTypeId.ShouldBe(1));
    }

    [Fact, Trait("Category", "Integration"), Description("Should update an existing user account")]
    public async Task ShouldUpdateUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var adminUser = await repository.Get(x =>
            x.EmailAddress.Equals(AdminEmailAddress));

        adminUser.ShouldNotBeNull();
        var user = new UserAccount
        {
            DisplayName = "Test User",
            FirstName = "Test",
            LastName = "User",
            EmailAddress = "test2@test.com",
            CreatedBy = adminUser.Id,
        };
        await Context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        user.DisplayName = "Updated Name";

        // Act
        var result = await repository.Update(user);

        // Assert
        result.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"));

        var savedUser = await Context.Users.FirstAsync(x => x.EmailAddress.Equals(user.EmailAddress), TestContext.Current.CancellationToken);

        savedUser.ShouldSatisfyAllConditions(
            x => x.DisplayName.ShouldBe("Updated Name"));
        savedUser.ShouldNotBeNull();
        savedUser.DisplayName.ShouldBe("Updated Name");
    }

    [Fact]
    [Trait("Category", "Integration")]
    [Description("Should throw duplicate exception")]
    public async Task ShouldThrowDuplicateException()
    {
        // Arrange
        var repository = new UsersRepository(Context);

        var adminUser = await repository.Get(x => x.EmailAddress == AdminEmailAddress);
        adminUser.ShouldNotBeNull();

        var duplicateUser = new UserAccount
        {
            DisplayName = "Dup User",
            FirstName = "Dup",
            LastName = "User",
            EmailAddress = AdminEmailAddress,   // <-- violates unique constraint om email address field
            CreatedBy = adminUser.Id,
        };

        // Act
        Func<Task> act = async () => await repository.Create(duplicateUser);

       // Assert (Shouldly)
        var ex = await act.ShouldThrowAsync<DbUpdateException>();
        ex.InnerException.ShouldNotBeNull(); // often contains provider-specific details
    }
}
