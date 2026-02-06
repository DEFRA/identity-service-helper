// <copyright file="ActivateTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;
using Shouldly;

public class ActivateTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should activate a user account by setting status to 2")]
    public async Task ShouldActivateUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var adminUser = await repository.GetSingle(x => x.EmailAddress == AdminEmailAddress, TestContext.Current.CancellationToken);
        adminUser.ShouldNotBeNull();

        var userId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        var user = new UserAccount
        {
            Id = userId,
            DisplayName = "To Activate",
            FirstName = "To",
            LastName = "Activate",
            EmailAddress = "activate@test.com",
            CreatedBy = adminUser.Id,
            StatusTypeId = 3, // Suspended
        };
        await Context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.Activate(x => x.Id == userId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var activatedUser = await repository.GetSingle(x => x.Id == userId, TestContext.Current.CancellationToken);
        activatedUser.ShouldNotBeNull();
        activatedUser.StatusTypeId.ShouldBe(2);
    }

    [Fact]
    [Description("Should throw ArgumentException when activating non-existent user account")]
    public async Task ShouldThrowWhenActivatingNonExistentUser()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var nonExistentId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await repository.Activate(x => x.Id == nonExistentId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<ArgumentException>();
    }
}
