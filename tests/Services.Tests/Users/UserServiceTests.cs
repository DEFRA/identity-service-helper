// <copyright file="UserServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Users;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Users.Commands;
using Defra.Identity.Models.Requests.Users.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Users;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Comparison;
using Defra.Identity.Test.Utilities.Repository;
using Defra.Identity.Test.Utilities.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class UserServiceTests
{
    private readonly IUserRepository repository = Substitute.For<IUserRepository>();

    private readonly IStrategyBuilderFactory<UserService> strategyBuilderFactory =
        new StrategyBuilderFactory<UserService>();

    private readonly IValidator<CreateUser> createUserValidator =
        Substitute.For<IValidator<CreateUser>>();

    private readonly IValidator<UpdateUserById> updateUserValidator =
        Substitute.For<IValidator<UpdateUserById>>();

    private readonly IValidator<UpsertUserById> upsertUserValidator =
        Substitute.For<IValidator<UpsertUserById>>();

    private readonly ILogger<UserService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<UserService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<UserService> sut;

    public UserServiceTests()
    {
        sut = SutProvider<UserService>.CreateFor(
            context => new UserService(
                repository,
                context!,
                strategyBuilderFactory,
                createUserValidator,
                updateUserValidator,
                upsertUserValidator,
                logger),
            operatorContext);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description("GetAll returns all users excluding deleted with given IncludeInactive property")]
    public async Task GetAll_Returns_All_Users_Excluding_Deleted_Given_IncludeInactive(string? includeInactive)
    {
        // Arrange
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var user2NotDeleted = TestData.User.User2NotDeleted;
        var user3Deleted = TestData.User.User3Deleted;

        var request = new GetAllUsers() { IncludeInactive = includeInactive };

        MockRepositoryContext<UserAccounts>.CreateFor(repository).WithData(
        [
            user1NotDeleted,
            user2NotDeleted,
            user3Deleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorContext.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(2);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user1NotDeleted));
        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user2NotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all users, includehidden: false [user account]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all users, includehidden: false [user account]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description("GetAll returns all users including deleted with given IncludeInactive property")]
    public async Task GetAll_Returns_All_Users_Including_Deleted_Given_IncludeInactive(string? includeInactive)
    {
        // Arrange
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var user2NotDeleted = TestData.User.User2NotDeleted;
        var user3Deleted = TestData.User.User3Deleted;

        var request = new GetAllUsers() { IncludeInactive = includeInactive };

        MockRepositoryContext<UserAccounts>.CreateFor(repository).WithData(
        [
            user1NotDeleted,
            user2NotDeleted,
            user3Deleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorContext.GetAll(request, TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(3);

        result[0].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user1NotDeleted));
        result[1].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user2NotDeleted));
        result[2].ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user3Deleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all users, includehidden: true [user account]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all users, includehidden: true [user account]");
    }

    [Fact]
    [Description("Get returns the requested user")]
    public async Task Get_Returns_Requested_User()
    {
        // Arrange
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var user2NotDeleted = TestData.User.User2NotDeleted;
        var user3Deleted = TestData.User.User3Deleted;

        var request = new GetUserById() { Id = user2NotDeleted.Id };

        MockRepositoryContext<UserAccounts>.CreateFor(repository).WithData(
        [
            user1NotDeleted,
            user2NotDeleted,
            user3Deleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user2NotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user [user account] with id {request.Id}");
    }

    [Fact]
    [Description("Get returns the requested deleted user")]
    public async Task Get_Returns_Requested_Deleted_User()
    {
        // Arrange
        var user3Deleted = TestData.User.User3Deleted;

        var request = new GetUserById() { Id = user3Deleted.Id };

        MockRepositoryContext<UserAccounts>.CreateFor(repository).WithData(
        [
            user3Deleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(user3Deleted));

        result.Active.ShouldBeFalse();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get user [user account] with id {request.Id}");
    }

    [Fact]
    [Description("Get throws not found exception when requested user does not exist")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_User_Does_Not_Exist()
    {
        // Arrange
        var request = new GetUserById() { Id = Guid.NewGuid(), };
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get user [user account] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Create returns new user")]
    public async Task Create_Returns_New_User()
    {
        // Arrange
        var idForNewEntity = Guid.NewGuid();

        var request = new CreateUser()
        {
            Email = "NEWUSER@TEST.COM",
            FirstName = "New User First Name",
            LastName = "New User Last Name",
            DisplayName = "New User Display Name",
        };

        var validatorContext = MockValidatorContext<CreateUser>.CreateFor(createUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithNextCreateEntityId(idForNewEntity);

        // Act
        var result = await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.CreateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastCreateResult.ShouldNotBeNull();

        repositoryContext.Calls.LastCreateResult.ShouldSatisfyAllConditions(createdEntity =>
        {
            createdEntity.Id.ShouldBe(idForNewEntity);
            createdEntity.EmailAddress.ShouldBe(request.Email.ToLowerInvariant());
            createdEntity.FirstName.ShouldBe(request.FirstName);
            createdEntity.LastName.ShouldBe(request.LastName);
            createdEntity.DisplayName.ShouldBe(request.DisplayName);
            createdEntity.KrdsId.ShouldBeNull();
            createdEntity.SamId.ShouldBeNull();
            createdEntity.DeletedById.ShouldBeNull();
            createdEntity.DeletedAt.ShouldBeNull();
            createdEntity.CreatedById.ShouldBe(operatorContext!.OperatorId);
            createdEntity.CreatedAt.ShouldBeCloseToUtcNow();
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastCreateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed create user [user account] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Create throws validation exception when request validation fails")]
    public async Task Create_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new CreateUser()
        {
            Email = "NEWUSER@TEST.COM",
            FirstName = "New User First Name",
            LastName = "New User Last Name",
            DisplayName = "New User Display Name",
        };

        var validatorContext = MockValidatorContext<CreateUser>.CreateFor(createUserValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<ValidationException>();

        exception.Message.ShouldContain("Random Property 1: Simulated validation failure 1");
        exception.Message.ShouldContain("Random Property 2: Simulated validation failure 2");

        exception.Errors.Count().ShouldBe(2);

        exception.Errors.ToList().ShouldSatisfyAllConditions(errors =>
        {
            errors[0].PropertyName.ShouldBe("Random Property 1");
            errors[0].ErrorMessage.ShouldBe("Simulated validation failure 1");
            errors[1].PropertyName.ShouldBe("Random Property 2");
            errors[1].ErrorMessage.ShouldBe("Simulated validation failure 2");
        });

        exception.Errors.First().PropertyName.ShouldBe("Random Property 1");
        exception.Errors.First().ErrorMessage.ShouldBe("Simulated validation failure 1");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(LogLevel.Warning, $"Execute create user [user account] failed validation");
    }

    [Fact]
    [Description("Create throws invalid operation exception when operator context is not provided")]
    public async Task Create_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new CreateUser()
        {
            Email = "NEWUSER@TEST.COM",
            FirstName = "New User First Name",
            LastName = "New User Last Name",
            DisplayName = "New User Display Name",
        };

        var validatorContext = MockValidatorContext<CreateUser>.CreateFor(createUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Create throws unauthorised access exception when operator id is not provided")]
    public async Task Create_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new CreateUser()
        {
            Email = "NEWUSER@TEST.COM",
            FirstName = "New User First Name",
            LastName = "New User Last Name",
            DisplayName = "New User Display Name",
        };

        var validatorContext = MockValidatorContext<CreateUser>.CreateFor(createUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Update returns updated user")]
    public async Task Update_Returns_Updated_User()
    {
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var originalUser1NotDeleted = TestData.User.User1NotDeleted;

        // Arrange
        var request = new UpdateUserById()
        {
            Id = user1NotDeleted.Id,
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user1NotDeleted]);

        // Act
        var result = await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(user1NotDeleted);

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(updatedEntity =>
        {
            updatedEntity.Id.ShouldBe(request.Id);
            updatedEntity.EmailAddress.ShouldBe(request.Email.ToLowerInvariant());
            updatedEntity.FirstName.ShouldBe(request.FirstName);
            updatedEntity.LastName.ShouldBe(request.LastName);
            updatedEntity.DisplayName.ShouldBe(request.DisplayName);
            updatedEntity.KrdsId.ShouldBe(originalUser1NotDeleted.KrdsId);
            updatedEntity.SamId.ShouldBe(originalUser1NotDeleted.SamId);
            updatedEntity.DeletedById.ShouldBeNull();
            updatedEntity.DeletedAt.ShouldBeNull();
            updatedEntity.CreatedById.ShouldBe(originalUser1NotDeleted.CreatedById);
            updatedEntity.CreatedAt.ShouldBe(originalUser1NotDeleted.CreatedAt);
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastUpdateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed update user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Update throws validation exception when request validation fails")]
    public async Task Update_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<ValidationException>();

        exception.Message.ShouldContain("Random Property 1: Simulated validation failure 1");
        exception.Message.ShouldContain("Random Property 2: Simulated validation failure 2");

        exception.Errors.Count().ShouldBe(2);

        exception.Errors.ToList().ShouldSatisfyAllConditions(errors =>
        {
            errors[0].PropertyName.ShouldBe("Random Property 1");
            errors[0].ErrorMessage.ShouldBe("Simulated validation failure 1");
            errors[1].PropertyName.ShouldBe("Random Property 2");
            errors[1].ErrorMessage.ShouldBe("Simulated validation failure 2");
        });

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(LogLevel.Warning, $"Execute update user [user account] failed validation");
    }

    [Fact]
    [Description("Update throws not found exception when user does not exist")]
    public async Task Update_Throws_NotFound_Exception_When_User_Does_Not_Exist()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Update throws not found exception when user is deleted")]
    public async Task Update_Throws_NotFound_Exception_When_User_Deleted()
    {
        // Arrange
        var user3Deleted = TestData.User.User3Deleted;

        var request = new UpdateUserById()
        {
            Id = user3Deleted.Id,
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user3Deleted]);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(user3Deleted);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Update throws invalid operation exception when operator context is not provided")]
    public async Task Update_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Update throws unauthorised access exception when operator id is not provided")]
    public async Task Update_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new UpdateUserById()
        {
            Id = Guid.NewGuid(),
            Email = "UPDATEDUSER@TEST.COM",
            FirstName = "Updated User First Name",
            LastName = "Updated User Last Name",
            DisplayName = "Updated User Display Name",
        };

        var validatorContext = MockValidatorContext<UpdateUserById>.CreateFor(updateUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Upsert creates and returns new user when user id is not provided")]
    public async Task Upsert_Creates_And_Returns_New_User_When_User_Id_Not_Provided()
    {
        // Arrange
        var idForNewEntity = Guid.NewGuid();

        var request = new UpsertUserById()
        {
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithNextCreateEntityId(idForNewEntity);

        // Act
        var result = await sut.WithOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.CreateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastCreateResult.ShouldNotBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        repositoryContext.Calls.LastCreateResult.ShouldSatisfyAllConditions(createdEntity =>
        {
            createdEntity.Id.ShouldBe(idForNewEntity);
            createdEntity.EmailAddress.ShouldBe(request.Email.ToLowerInvariant());
            createdEntity.FirstName.ShouldBe(request.FirstName);
            createdEntity.LastName.ShouldBe(request.LastName);
            createdEntity.DisplayName.ShouldBe(request.DisplayName);
            createdEntity.KrdsId.ShouldBeNull();
            createdEntity.SamId.ShouldBeNull();
            createdEntity.DeletedById.ShouldBeNull();
            createdEntity.DeletedAt.ShouldBeNull();
            createdEntity.CreatedById.ShouldBe(operatorContext!.OperatorId);
            createdEntity.CreatedAt.ShouldBeCloseToUtcNow();
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastCreateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing upsert user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed upsert user [user account] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Upsert creates and returns new user when user id is not found")]
    public async Task Upsert_Creates_And_Returns_New_User_When_User_With_Id_Not_Found()
    {
        // Arrange
        var idForNewEntity = Guid.NewGuid();

        var request = new UpsertUserById()
        {
            Id = Guid.NewGuid(),
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithNextCreateEntityId(idForNewEntity);

        // Act
        var result = await sut.WithOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.CreateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastCreateResult.ShouldNotBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        repositoryContext.Calls.LastCreateResult.ShouldSatisfyAllConditions(createdEntity =>
        {
            createdEntity.Id.ShouldBe(idForNewEntity);
            createdEntity.Id.ShouldNotBe(request.Id.Value);
            createdEntity.EmailAddress.ShouldBe(request.Email.ToLowerInvariant());
            createdEntity.FirstName.ShouldBe(request.FirstName);
            createdEntity.LastName.ShouldBe(request.LastName);
            createdEntity.DisplayName.ShouldBe(request.DisplayName);
            createdEntity.KrdsId.ShouldBeNull();
            createdEntity.SamId.ShouldBeNull();
            createdEntity.DeletedById.ShouldBeNull();
            createdEntity.DeletedAt.ShouldBeNull();
            createdEntity.CreatedById.ShouldBe(operatorContext!.OperatorId);
            createdEntity.CreatedAt.ShouldBeCloseToUtcNow();
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastCreateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing upsert user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed upsert user [user account] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Upsert updates and returns existing user")]
    public async Task Upsert_Updates_And_Returns_Existing_User()
    {
        // Arrange
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var originalUser1NotDeleted = TestData.User.User1NotDeleted;

        var request = new UpsertUserById()
        {
            Id = user1NotDeleted.Id,
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user1NotDeleted]);

        // Act
        var result = await sut.WithOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.CreateCallCount.ShouldBe(0);

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(updatedEntity =>
        {
            updatedEntity.Id.ShouldBe(request.Id.Value);
            updatedEntity.EmailAddress.ShouldBe(request.Email.ToLowerInvariant());
            updatedEntity.FirstName.ShouldBe(request.FirstName);
            updatedEntity.LastName.ShouldBe(request.LastName);
            updatedEntity.DisplayName.ShouldBe(request.DisplayName);
            updatedEntity.KrdsId.ShouldBe(originalUser1NotDeleted.KrdsId);
            updatedEntity.SamId.ShouldBe(originalUser1NotDeleted.SamId);
            updatedEntity.DeletedById.ShouldBeNull();
            updatedEntity.DeletedAt.ShouldBeNull();
            updatedEntity.CreatedById.ShouldBe(originalUser1NotDeleted.CreatedById);
            updatedEntity.CreatedAt.ShouldBe(originalUser1NotDeleted.CreatedAt);
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastUpdateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing upsert user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed upsert user [user account] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Upsert throws validation exception when request validation fails")]
    public async Task Upsert_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new UpsertUserById()
        {
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<ValidationException>();

        exception.Message.ShouldContain("Random Property 1: Simulated validation failure 1");
        exception.Message.ShouldContain("Random Property 2: Simulated validation failure 2");

        exception.Errors.Count().ShouldBe(2);

        exception.Errors.ToList().ShouldSatisfyAllConditions(errors =>
        {
            errors[0].PropertyName.ShouldBe("Random Property 1");
            errors[0].ErrorMessage.ShouldBe("Simulated validation failure 1");
            errors[1].PropertyName.ShouldBe("Random Property 2");
            errors[1].ErrorMessage.ShouldBe("Simulated validation failure 2");
        });

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing upsert user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(LogLevel.Warning, $"Execute upsert user [user account] failed validation");
    }

    [Fact]
    [Description("Update throws not found exception when existing user is deleted")]
    public async Task Upsert_Throws_NotFound_Exception_When_Existing_User_Deleted()
    {
        // Arrange
        var user3Deleted = TestData.User.User3Deleted;

        var request = new UpsertUserById()
        {
            Id = user3Deleted.Id,
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user3Deleted]);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(user3Deleted);

        repositoryContext.Calls.CreateCallCount.ShouldBe(0);
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing upsert user [user account] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Upsert throws invalid operation exception when operator context is not provided")]
    public async Task Upsert_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new UpsertUserById()
        {
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Upsert throws unauthorised access exception when operator id is not provided")]
    public async Task Upsert_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new UpsertUserById()
        {
            Email = "UPSERTED@TEST.COM",
            FirstName = "Upserted User First Name",
            LastName = "Upserted User Last Name",
            DisplayName = "Upserted User Display Name",
        };

        var validatorContext = MockValidatorContext<UpsertUserById>.CreateFor(upsertUserValidator);
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Upsert(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        validatorContext.Calls.ShouldHaveNoCalls();
        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Delete soft deletes user")]
    public async Task Delete_Soft_Deletes_User()
    {
        var user1NotDeleted = TestData.User.User1NotDeleted;
        var originalUser1NotDeleted = TestData.User.User1NotDeleted;

        var request = new DeleteUserById() { Id = user1NotDeleted.Id };

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user1NotDeleted]);

        // Act
        await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(user1NotDeleted);

        EntityComparer.CreateFor(originalUser1NotDeleted, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(UserAccounts.DeletedById), nameof(UserAccounts.DeletedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(softDeletedEntity =>
        {
            softDeletedEntity.DeletedById.ShouldBe(operatorContext!.OperatorId);
            softDeletedEntity.DeletedAt.ShouldNotBeNull();
            softDeletedEntity.DeletedAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed delete user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Delete throws not found exception when user does not exist")]
    public async Task Delete_Throws_NotFound_Exception_When_User_Does_Not_Exist()
    {
        // Arrange
        var request = new DeleteUserById() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws not found exception when user is deleted")]
    public async Task Delete_Throws_NotFound_Exception_When_User_Deleted()
    {
        // Arrange
        var user3Deleted = TestData.User.User3Deleted;

        var request = new DeleteUserById() { Id = user3Deleted.Id };

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository)
            .WithData([user3Deleted]);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("User account not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(user3Deleted);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete user [user account] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"User account with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws invalid operation exception when operator context is not provided")]
    public async Task Delete_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new DeleteUserById() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Delete throws unauthorised access exception when operator id is not provided")]
    public async Task Delete_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new DeleteUserById() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }
}
