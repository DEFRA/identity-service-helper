// <copyright file="SuspendTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Users;
using Shouldly;

public class SuspendTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should suspend a user account by setting status to 3")]
    public async Task ShouldSuspendUserAccount()
    {
        // Arrange
        var repository = new UsersRepository(Context);
        var adminUser = await repository.GetSingle(x => x.EmailAddress == AdminEmailAddress, TestContext.Current.CancellationToken);
        adminUser.ShouldNotBeNull();

        var user = new UserAccount
        {
            DisplayName = "To Suspend",
            FirstName = "To",
            LastName = "Suspend",
            EmailAddress = "suspend@test.com",
            CreatedBy = adminUser.Id,
        };
        await Context.Users.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.Suspend(x => x.Id == user.Id, adminUser.Id,  TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var suspendedUser = await repository.GetSingle(x => x.Id == user.Id, TestContext.Current.CancellationToken);

        suspendedUser.ShouldSatisfyAllConditions(
            v => v.ShouldNotBeNull(),
            v => v?.StatusTypeId.ShouldBe(3),
            v => v?.UpdatedBy.ShouldBe(adminUser.Id));
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
        Func<Task> act = async () => await repository.Suspend(x => x.Id == nonExistentId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();
    }
}
