// <copyright file="DeleteTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Postgres.Database.Tests.Repositories;

using System.ComponentModel;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Postgres.Database.Tests.Fixtures;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Repositories.Users;
using Microsoft.Extensions.Logging;
using NSubstitute;
using Shouldly;

public class DeleteTests(PostgreContainerFixture fixture) : BaseTests(fixture)
{
    private const string AdminEmailAddress = "test@test.com";

    [Fact]
    [Description("Should soft delete a user account by setting status to 4")]
    public async Task ShouldDeleteUserAccount()
    {
        // Arrange
        var logger = Substitute.For<ILogger<UsersRepository>>();
        var repository = new UsersRepository(Context, logger);
        var adminUser = await repository.GetSingle(x => x.EmailAddress == AdminEmailAddress, TestContext.Current.CancellationToken);
        adminUser.ShouldNotBeNull();

        var userId = Guid.NewGuid();
        var user = new UserAccounts
        {
            Id = userId,
            DisplayName = "To Delete",
            FirstName = "To",
            LastName = "Delete",
            EmailAddress = "delete@test.com",
            CreatedById = adminUser.Id,
        };
        await Context.UserAccounts.AddAsync(user, TestContext.Current.CancellationToken);
        await Context.SaveChangesAsync(TestContext.Current.CancellationToken);

        // Act
        var result = await repository.Delete(x => x.Id == userId, adminUser.Id, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeTrue();
        var deletedUser = await repository.GetSingle(x => x.Id == userId, TestContext.Current.CancellationToken);
        deletedUser.ShouldNotBeNull();

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Information,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }

    [Fact]
    [Description("Should throw ArgumentException when deleting non-existent user account")]
    public async Task ShouldThrowWhenDeletingNonExistentUser()
    {
        // Arrange
        var logger = Substitute.For<ILogger<UsersRepository>>();
        var repository = new UsersRepository(Context, logger);
        var nonExistentId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();

        // Act
        Func<Task> act = async () => await repository.Delete(x => x.Id == nonExistentId, operatorId, TestContext.Current.CancellationToken);

        // Assert
        await act.ShouldThrowAsync<NotFoundException>();

        logger.ReceivedWithAnyArgs().Log(
            LogLevel.Warning,
            Arg.Any<EventId>(),
            Arg.Any<object>(),
            Arg.Any<Exception>(),
            Arg.Any<Func<object, Exception?, string>>());
    }
}
