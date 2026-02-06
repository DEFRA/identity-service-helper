// <copyright file="DeleteTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Users;
using Shouldly;

public class DeleteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should soft delete a user account by setting status to 4")]
    public async Task ShouldDeleteUserAccount()
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
            DisplayName = "To Delete",
            FirstName = "To",
            LastName = "Delete",
            EmailAddress = "delete@test.com",
            CreatedBy = adminUser.Id,
            StatusTypeId = 1,
        };
        await Context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.Delete(x => x.Id == userId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var deletedUser = await repository.GetSingle(x => x.Id == userId, TestContext.Current.CancellationToken);
        deletedUser.ShouldNotBeNull();
        deletedUser.StatusTypeId.ShouldBe(4);
    }

    [Fact]
    [Description("Should return false when deleting non-existent user account")]
    public async Task ShouldReturnFalseWhenDeletingNonExistentUser()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var nonExistentId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();

        // Act
        var result = await repository.Delete(x => x.Id == nonExistentId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeFalse();
    }
}
