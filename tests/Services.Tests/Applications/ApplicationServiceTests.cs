// <copyright file="ApplicationServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Applications;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Applications.Commands;
using Defra.Identity.Models.Requests.Applications.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Applications;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Services.Applications;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Comparison;
using Defra.Identity.Test.Utilities.Repository;
using Defra.Identity.Test.Utilities.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class ApplicationServiceTests
{
    private readonly IApplicationsRepository repository = Substitute.For<IApplicationsRepository>();

    private readonly IStrategyBuilderFactory<ApplicationService> strategyBuilderFactory =
        new StrategyBuilderFactory<ApplicationService>();

    private readonly IValidator<CreateApplication> createApplicationValidator =
        Substitute.For<IValidator<CreateApplication>>();

    private readonly IValidator<UpdateApplicationByClientId> updateApplicationValidator =
        Substitute.For<IValidator<UpdateApplicationByClientId>>();

    private readonly ILogger<ApplicationService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<ApplicationService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<ApplicationService> sut;

    public ApplicationServiceTests()
    {
        sut = SutProvider<ApplicationService>.CreateFor(
            context => new ApplicationService(
                repository,
                context!,
                strategyBuilderFactory,
                createApplicationValidator,
                updateApplicationValidator,
                logger),
            operatorContext);
    }

    [Fact]
    [Description("GetAll returns all applications excluding deleted")]
    public async Task GetAll_Returns_All_Applications_Excluding_Deleted()
    {
        // Arrange
        MockRepositoryContext<Applications>.CreateFor(repository).WithData(
        [
            TestData.Application.Application1NotDeleted,
            TestData.Application.Application2NotDeleted,
            TestData.Application.Application3Deleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAll(TestContext.Current.CancellationToken);

        // Assert
        result.Count.ShouldBe(2);

        result[0].ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(TestData.Application.Application1NotDeleted));

        result[1].ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(TestData.Application.Application2NotDeleted));

        logger.VerifyLogContainsOne(LogLevel.Information, "Executing get all applications [application]");
        logger.VerifyLogContainsOne(LogLevel.Information, "Successfully executed get all applications [application]");
    }

    [Fact]
    [Description("Get returns the requested application")]
    public async Task Get_Returns_Requested_Application()
    {
        // Arrange
        var request = new GetApplicationByClientId() { Id = TestData.Application.Application2NotDeleted.ClientId, };

        MockRepositoryContext<Applications>.CreateFor(repository).WithData(
        [
            TestData.Application.Application1NotDeleted,
            TestData.Application.Application2NotDeleted,
            TestData.Application.Application3Deleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Application.Application2NotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get application [application] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get application [application] with id {request.Id}");
    }

    [Fact]
    [Description("Get throws not found exception when requested application does not exist")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_Application_Does_Not_Exist()
    {
        // Arrange
        var request = new GetApplicationByClientId() { Id = Guid.NewGuid(), };
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get application [application] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Get throws not found exception when requested application is deleted")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_Application_Deleted()
    {
        // Arrange
        var application3Deleted = TestData.Application.Application3Deleted;

        var request = new GetApplicationByClientId() { Id = application3Deleted.ClientId, };

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithData([application3Deleted]);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(application3Deleted);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get application [application] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Create returns new application")]
    public async Task Create_Returns_New_Application()
    {
        // Arrange
        var idForNewEntity = Guid.NewGuid();

        var request = new CreateApplication
        {
            Id = Guid.NewGuid(),
            Name = "New App",
            TenantName = "New Tenant",
            Description = "New Description",
            Scopes = ["scope1", "scope2"],
            RedirectUris = ["https://localhost/callback"],
            Secret = "secret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithNextCreateEntityId(idForNewEntity);

        // Act
        var result = await sut.WithOperatorId.Create(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.CreateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastCreateResult.ShouldNotBeNull();

        repositoryContext.Calls.LastCreateResult.ShouldSatisfyAllConditions(createdEntity =>
        {
            createdEntity.Id.ShouldBe(idForNewEntity);
            createdEntity.ClientId.ShouldBe(request.Id);
            createdEntity.Name.ShouldBe(request.Name);
            createdEntity.Description.ShouldBe(request.Description);
            createdEntity.TenantName.ShouldBe(request.TenantName);
            createdEntity.Scopes.ShouldBe(string.Join(";", request.Scopes));
            createdEntity.RedirectUris.ShouldBe(string.Join(";", request.RedirectUris));
            createdEntity.Secret.ShouldBe(request.Secret);
            createdEntity.DeletedById.ShouldBeNull();
            createdEntity.DeletedAt.ShouldBeNull();
            createdEntity.CreatedById.ShouldBe(operatorContext!.OperatorId);
            createdEntity.CreatedAt.ShouldBeCloseToUtcNow();
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastCreateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create application [application] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed create application [application] by operator {operatorContext.OperatorId}");
    }

    [Fact]
    [Description("Create throws validation exception when request validation fails")]
    public async Task Create_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new CreateApplication
        {
            Id = Guid.NewGuid(),
            Name = "New App",
            TenantName = "New Tenant",
            Description = "New Description",
            Scopes = ["scope1", "scope2"],
            RedirectUris = ["https://localhost/callback"],
            Secret = "secret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing create application [application] by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(LogLevel.Warning, $"Execute create application [application] failed validation");
    }

    [Fact]
    [Description("Create throws invalid operation exception when operator context is not provided")]
    public async Task Create_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new CreateApplication
        {
            Id = Guid.NewGuid(),
            Name = "New App",
            TenantName = "New Tenant",
            Description = "New Description",
            Scopes = ["scope1", "scope2"],
            RedirectUris = ["https://localhost/callback"],
            Secret = "secret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator);
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
        var request = new CreateApplication
        {
            Id = Guid.NewGuid(),
            Name = "New App",
            TenantName = "New Tenant",
            Description = "New Description",
            Scopes = ["scope1", "scope2"],
            RedirectUris = ["https://localhost/callback"],
            Secret = "secret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator);
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
    [Description("Update returns updated application")]
    public async Task Update_Returns_Updated_Application()
    {
        // Arrange
        var application1NotDeleted = TestData.Application.Application1NotDeleted;
        var originalApplication1NotDeleted = TestData.Application.Application1NotDeleted;

        var request = new UpdateApplicationByClientId()
        {
            Id = application1NotDeleted.ClientId,
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        var validatorContext = MockValidatorContext<UpdateApplicationByClientId>.CreateFor(updateApplicationValidator);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithData([application1NotDeleted]);

        // Act
        var result = await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(application1NotDeleted);

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(updatedEntity =>
        {
            updatedEntity.Id.ShouldNotBe(request.Id);
            updatedEntity.Id.ShouldNotBe(Guid.Empty);
            updatedEntity.ClientId.ShouldBe(request.Id);
            updatedEntity.Name.ShouldBe(request.Name);
            updatedEntity.Description.ShouldBe(request.Description);
            updatedEntity.TenantName.ShouldBe(request.TenantName);
            updatedEntity.Scopes.ShouldBe(string.Join(";", request.Scopes));
            updatedEntity.RedirectUris.ShouldBe(string.Join(";", request.RedirectUris));
            updatedEntity.Secret.ShouldBe(request.Secret);
            updatedEntity.DeletedById.ShouldBeNull();
            updatedEntity.DeletedAt.ShouldBeNull();
            updatedEntity.CreatedById.ShouldBe(originalApplication1NotDeleted.CreatedById);
            updatedEntity.CreatedAt.ShouldBe(originalApplication1NotDeleted.CreatedAt);
        });

        result.ShouldSatisfyAllConditions(
            Assertions.ShouldMapFromEntity(repositoryContext.Calls.LastUpdateResult));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed update application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Update throws validation exception when request validation fails")]
    public async Task Update_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new UpdateApplicationByClientId()
        {
            Id = Guid.NewGuid(),
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        var validatorContext = MockValidatorContext<UpdateApplicationByClientId>.CreateFor(updateApplicationValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.ShouldHaveNoCalls();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(LogLevel.Warning, $"Execute update application [application] failed validation");
    }

    [Fact]
    [Description("Update throws not found exception when application does not exist")]
    public async Task Update_Throws_NotFound_Exception_When_Application_Does_Not_Exist()
    {
        // Arrange
        var request = new UpdateApplicationByClientId()
        {
            Id = Guid.NewGuid(),
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        MockValidatorContext<UpdateApplicationByClientId>.CreateFor(updateApplicationValidator);
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Update throws not found exception when application is deleted")]
    public async Task Update_Throws_NotFound_Exception_When_Application_Deleted()
    {
        // Arrange
        var application3Deleted = TestData.Application.Application3Deleted;

        var request = new UpdateApplicationByClientId()
        {
            Id = application3Deleted.ClientId,
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        MockValidatorContext<UpdateApplicationByClientId>.CreateFor(updateApplicationValidator);

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithData([application3Deleted]);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.Update(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(application3Deleted);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing update application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Update throws invalid operation exception when operator context is not provided")]
    public async Task Update_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new UpdateApplicationByClientId()
        {
            Id = Guid.NewGuid(),
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator);
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
        var request = new UpdateApplicationByClientId()
        {
            Id = Guid.NewGuid(),
            Name = "Updated App",
            TenantName = "Updated Tenant",
            Description = "Updated Description",
            Scopes = ["UpdatedScope1", "UpdatedScope2"],
            RedirectUris = ["https://localhost/updated-callback"],
            Secret = "UpdatedSecret123",
        };

        var validatorContext = MockValidatorContext<CreateApplication>.CreateFor(createApplicationValidator);
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
    [Description("Delete soft deletes application")]
    public async Task Delete_Soft_Deletes_Application()
    {
        // Arrange
        var application1NotDeleted = TestData.Application.Application1NotDeleted;
        var originalApplication1NotDeleted = TestData.Application.Application1NotDeleted;

        var request = new DeleteApplicationByClientId() { Id = application1NotDeleted.ClientId, };

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithData([application1NotDeleted]);

        // Act
        await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(application1NotDeleted);

        EntityComparer.CreateFor(originalApplication1NotDeleted, repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(Applications.DeletedById), nameof(Applications.DeletedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(softDeletedEntity =>
        {
            softDeletedEntity.DeletedById.ShouldBe(operatorContext!.OperatorId);
            softDeletedEntity.DeletedAt.ShouldNotBeNull();
            softDeletedEntity.DeletedAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed delete application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");
    }

    [Fact]
    [Description("Delete throws not found exception when application does not exist")]
    public async Task Delete_Throws_NotFound_Exception_When_Application_Does_Not_Exist()
    {
        // Arrange
        var request = new DeleteApplicationByClientId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws not found exception when application is deleted")]
    public async Task Delete_Throws_NotFound_Exception_When_Application_Deleted()
    {
        // Arrange
        var application3Deleted = TestData.Application.Application3Deleted;

        var request = new DeleteApplicationByClientId() { Id = application3Deleted.ClientId };

        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository)
            .WithData([application3Deleted]);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("Application not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(application3Deleted);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete application [application] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Application with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws invalid operation exception when operator context is not provided")]
    public async Task Delete_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new DeleteApplicationByClientId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<Applications>.CreateFor(repository);

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
        var request = new DeleteApplicationByClientId() { Id = Guid.NewGuid() };
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
