// <copyright file="CphNumberServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Repository;
using Defra.Identity.Test.Utilities.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CphNumberServiceTests
{
    private readonly ICphRepository repository = Substitute.For<ICphRepository>();

    private readonly IValidator<IOperationByCphNumber> cphNumberValidator =
        Substitute.For<IValidator<IOperationByCphNumber>>();

    private readonly ILogger<CphNumberService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<CphNumberService>();

    private readonly SutProvider<CphNumberService> sut;

    public CphNumberServiceTests()
    {
        sut = SutProvider<CphNumberService>.CreateFor(_ => new CphNumberService(
            repository,
            cphNumberValidator,
            logger));
    }

    [Fact]
    [Description("GetIdFromCphNumber returns id for cph associated cph")]
    public async Task GetIdFromCphNumber_Returns_Id_For_Associated_Cph()
    {
        // Arrange
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;

        var validatorContext = MockValidatorContext<IOperationByCphNumber>.CreateFor(cphNumberValidator);

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            cph1WithAllowedSpeciesNotExpiredOrDeleted
        ]);

        var request = new OperationByCphNumber() { County = 44, Parish = 100, Holding = 0001 };

        // Act
        var result =
            await sut.WithoutOperatorContext.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph1WithAllowedSpeciesNotExpiredOrDeleted);

        result.ShouldBe(cph1WithAllowedSpeciesNotExpiredOrDeleted.Id);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Getting county parish holding id by cph number 44/100/0001");
    }

    [Fact]
    [Description("GetIdFromCphNumber throws validation exception when request validation fails")]
    public async Task GetIdFromCphNumber_Throws_Validation_Exception_When_Request_Validation_Fails()
    {
        // Arrange
        var request = new OperationByCphNumber() { County = 9999, Parish = 9999, Holding = 9999 };

        var validatorContext = MockValidatorContext<IOperationByCphNumber>.CreateFor(cphNumberValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

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

        logger.VerifyLogDoesNotContain(
            LogLevel.Information,
            $"Getting county parish holding id by cph number 9999/9999/9999");
    }

    [Fact]
    [Description("GetIdFromCphNumber throws not found exception when cph does not exist")]
    public async Task GetIdFromCphNumber_Throws_NotFound_Exception_When_Cph_Does_Not_Exist()
    {
        // Arrange
        var request = new OperationByCphNumber() { County = 99, Parish = 999, Holding = 9999 };

        var validatorContext = MockValidatorContext<IOperationByCphNumber>.CreateFor(cphNumberValidator);
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting county parish holding id by cph number 99/999/9999");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            "County parish holding with cph number 99/999/9999 not found");
    }

    [Fact]
    [Description("GetIdFromCphNumber throws not found exception when cph is deleted")]
    public async Task GetIdFromCphNumber_Throws_NotFound_Exception_When_Cph_Deleted()
    {
        // Arrange
        var cph3DeletedButNotExpired = TestData.Cph.Cph3DeletedButNotExpired;

        var request = new OperationByCphNumber() { County = 44, Parish = 100, Holding = 0003 };

        var validatorContext = MockValidatorContext<IOperationByCphNumber>.CreateFor(cphNumberValidator);

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            cph3DeletedButNotExpired
        ]);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorContext.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.ShouldNotBeNull();
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph3DeletedButNotExpired);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Getting county parish holding id by cph number 44/100/0003");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            "County parish holding with cph number 44/100/0003 not found");
    }
}
