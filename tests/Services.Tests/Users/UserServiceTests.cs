// <copyright file="UserServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Users;

using System;
using System.Collections.Generic;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories;
using Defra.Identity.Requests.Users.Commands.Update;
using Defra.Identity.Requests.Users.Queries;
using Defra.Identity.Services.Users;
using NSubstitute;
using Shouldly;
using Xunit;

public class UserServiceTests
{
    private readonly IRepository<UserAccount> _repository = Substitute.For<IRepository<UserAccount>>();
    private readonly UserService _userService;

    public UserServiceTests()
    {
        _userService = new UserService(_repository);
    }

    [Fact]
    public async Task GetAll_ReturnsUsers()
    {
        // Arrange
        var request = new GetUsers();
        var userAccounts = new List<UserAccount>
        {
            new UserAccount { Id = Guid.NewGuid(), EmailAddress = "user1@example.com", FirstName = "User", LastName = "One" },
            new UserAccount { Id = Guid.NewGuid(), EmailAddress = "user2@example.com", FirstName = "User", LastName = "Two" }
        };

        _repository.GetList(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(userAccounts);

        // Act
        var result = await _userService.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Count.ShouldBe(2);
        result[0].Email.ShouldBe("user1@example.com");
        result[1].Email.ShouldBe("user2@example.com");
    }

    [Fact]
    public async Task Get_UserExists_ReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var request = new GetUserById { Id = userId };
        var userAccount = new UserAccount
        {
            Id = userId,
            EmailAddress = "test@example.com",
            FirstName = "John",
            LastName = "Doe",
        };

        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(userAccount);

        // Act
        var result = await _userService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Id.ShouldBe(userId);
        result.Email.ShouldBe("test@example.com");
        result.FirstName.ShouldBe("John");
        result.LastName.ShouldBe("Doe");
    }

    [Fact]
    public async Task Get_UserDoesNotExist_ReturnsNull()
    {
        // Arrange
        var request = new GetUserById { Id = Guid.NewGuid() };
        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((UserAccount)null);

        // Act
        var result = await _userService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldBeNull();
    }

    [Fact]
    public async Task Upsert_UserExists_UpdatesAndReturnsUser()
    {
        // Arrange
        var updateUser = new UpdateUser
        {
            Email = "test@example.com",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
        };

        var existingUser = new UserAccount
        {
            Id = Guid.NewGuid(),
            EmailAddress = "test@example.com",
            FirstName = "OldFirstName",
            LastName = "OldLastName",
        };

        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);

        _repository.Update(Arg.Any<UserAccount>(), Arg.Any<CancellationToken>())
            .Returns(x => (UserAccount)x[0]);

        // Act
        var result = await _userService.Upsert(updateUser, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(updateUser.Email);
        result.FirstName.ShouldBe(updateUser.FirstName);
        result.LastName.ShouldBe(updateUser.LastName);

        await _repository.Received(1).Update(
            Arg.Is<UserAccount>(ua =>
            ua.EmailAddress == updateUser.Email &&
            ua.FirstName == updateUser.FirstName &&
            ua.LastName == updateUser.LastName),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Upsert_UserDoesNotExist_CreatesAndReturnsUser()
    {
        // Arrange
        var updateUser = new UpdateUser
        {
            Email = "new@example.com",
            FirstName = "NewFirstName",
            LastName = "NewLastName",
        };

        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((UserAccount)null);

        _repository.Create(Arg.Any<UserAccount>(), Arg.Any<CancellationToken>())
            .Returns(new UserAccount
            {
                Id = Guid.NewGuid(),
                EmailAddress = updateUser.Email,
                FirstName = updateUser.FirstName,
                LastName = updateUser.LastName,
            });

        // Act
        var result = await _userService.Upsert(updateUser, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(updateUser.Email);
        result.FirstName.ShouldBe(updateUser.FirstName);
        result.LastName.ShouldBe(updateUser.LastName);

        await _repository.Received(1).Create(
            Arg.Is<UserAccount>(ua =>
            ua.EmailAddress == updateUser.Email &&
            ua.FirstName == updateUser.FirstName &&
            ua.LastName == updateUser.LastName),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_UserExists_UpdatesAndReturnsUser()
    {
        // Arrange
        var userId = Guid.NewGuid();
        var operatorId = Guid.NewGuid();
        var updateUser = new UpdateUser
        {
            Id = userId,
            Email = "test@example.com",
            FirstName = "UpdatedFirstName",
            LastName = "UpdatedLastName",
            DisplayName = "Updated Display Name",
            OperatorId = operatorId.ToString(),
        };

        var existingUser = new UserAccount
        {
            Id = userId,
            EmailAddress = "test@example.com",
            FirstName = "OldFirstName",
            LastName = "OldLastName",
            DisplayName = "Old Display Name",
        };

        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(existingUser);

        _repository.Update(Arg.Any<UserAccount>(), Arg.Any<CancellationToken>())
            .Returns(x => (UserAccount)x[0]);

        // Act
        var result = await _userService.Update(updateUser, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldNotBeNull();
        result.Email.ShouldBe(updateUser.Email);
        result.FirstName.ShouldBe(updateUser.FirstName);
        result.LastName.ShouldBe(updateUser.LastName);

        await _repository.Received(1).Update(
            Arg.Is<UserAccount>(ua =>
            ua.Id == userId &&
            ua.EmailAddress == updateUser.Email &&
            ua.FirstName == updateUser.FirstName &&
            ua.LastName == updateUser.LastName &&
            ua.DisplayName == updateUser.DisplayName &&
            ua.UpdatedBy == operatorId),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Update_UserDoesNotExist_ThrowsNullReferenceException()
    {
        // Arrange
        var updateUser = new UpdateUser
        {
            Id = Guid.NewGuid(),
            Email = "new@example.com",
            FirstName = "NewFirstName",
            LastName = "NewLastName",
        };

        _repository.GetSingle(Arg.Any<Expression<Func<UserAccount, bool>>>(), Arg.Any<CancellationToken>())
            .Returns((UserAccount)null);

        // Act & Assert
        await Should.ThrowAsync<NullReferenceException>(async () =>
            await _userService.Update(updateUser, TestContext.Current.CancellationToken));

        await _repository.DidNotReceiveWithAnyArgs().Update(null!, default);
    }
}
