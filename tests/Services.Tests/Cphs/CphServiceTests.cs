// <copyright file="CphServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs;

using System.ComponentModel;
using Defra.Identity.Models.Requests.Common.Queries;
using Defra.Identity.Models.Requests.Cphs.Commands;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Services.Common.Context;
using Defra.Identity.Services.Common.Exceptions;
using Defra.Identity.Services.Common.Strategy.Factories;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Test.Utilities.Assertions;
using Defra.Identity.Test.Utilities.Comparison;
using Defra.Identity.Test.Utilities.Repository;
using Defra.Identity.Test.Utilities.Validation;
using FluentValidation;
using FluentValidation.Results;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CphServiceTests
{
    private readonly ICphRepository repository = Substitute.For<ICphRepository>();

    private readonly IStrategyBuilderFactory<CphService> strategyBuilderFactory =
        new StrategyBuilderFactory<CphService>();

    private readonly IValidator<PagedQuery> pagedQueryValidator =
        Substitute.For<IValidator<PagedQuery>>();

    private readonly ILogger<CphService> logger =
        DefraLoggerExtensions.CreateNSubstituteLogger<CphService>();

    private readonly IOperatorContext? operatorContext = Substitute.For<IOperatorContext>();

    private readonly SutProvider<CphService> sut;

    public CphServiceTests()
    {
        sut = SutProvider<CphService>.CreateFor(
            context => new CphService(
                repository,
                context!,
                strategyBuilderFactory,
                pagedQueryValidator,
                logger),
            operatorContext);
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description(
        "GetAllPaged returns page one in ascending order excluding expired or deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_One_Ascending_Excluding_Expired_Or_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 1, PageSize = 2, Expired = includeExpired };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(TestData.Cph.Cph4NotExpiredOrDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description(
        "GetAllPaged returns page two in ascending order excluding expired or deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_Two_Ascending_Excluding_Expired_Or_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 2, PageSize = 2, Expired = includeExpired };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph7NotExpiredOrDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description(
        "GetAllPaged returns page one in descending order excluding expired or deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_One_Descending_Excluding_Expired_Or_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 1, PageSize = 2, Expired = includeExpired, OrderByDescending = true };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph7NotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph4NotExpiredOrDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description(
        "GetAllPaged returns page two in descending order excluding expired or deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_Two_Descending_Excluding_Expired_Or_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 2, PageSize = 2, Expired = includeExpired, OrderByDescending = true };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description(
        "GetAllPaged returns page one in ascending order including expired but excluding deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_One_Ascending_Including_Expired_Excluding_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 1, PageSize = 2, Expired = includeExpired };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph2ExpiredButNotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description(
        "GetAllPaged returns page two in ascending order including expired but excluding deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_Two_Ascending_Including_Expired_Excluding_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 2, PageSize = 2, Expired = includeExpired };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph4NotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph5ExpiredButNotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description(
        "GetAllPaged returns page one in descending order including expired but excluding deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_One_Descending_Including_Expired_Excluding_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 1, PageSize = 2, Expired = includeExpired, OrderByDescending = true };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph7NotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph5ExpiredButNotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [Description(
        "GetAllPaged returns page two in descending order including expired but excluding deleted with given IncludeExpired property")]
    public async Task GetAllPaged_Returns_Page_Two_Descending_Including_Expired_Excluding_Deleted_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 2, PageSize = 2, Expired = includeExpired, OrderByDescending = true };

        var validatorContext = MockValidatorContext<GetCphs>.CreateFor(pagedQueryValidator);

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            TestData.Cph.Cph7NotExpiredOrDeleted,
            TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted,
            TestData.Cph.Cph4NotExpiredOrDeleted,
            TestData.Cph.Cph3DeletedButNotExpired,
            TestData.Cph.Cph2ExpiredButNotDeleted,
            TestData.Cph.Cph6DeletedButNotExpired,
            TestData.Cph.Cph5ExpiredButNotDeleted,
            TestData.Cph.Cph8ExpiredAndDeleted
        ]);

        // Act
        var result = await sut.WithoutOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        validatorContext.Calls.ValidateAsyncCallCount.ShouldBe(1);
        validatorContext.Calls.LastValidateAsync.Request.ShouldBe(request);

        result.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = result.Items.ToList();

        pagedResultItems[0]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph4NotExpiredOrDeleted));

        pagedResultItems[1]
            .ShouldSatisfyAllConditions(
                Assertions.ShouldMapFromEntity(TestData.Cph.Cph2ExpiredButNotDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            "Successfully executed get all county parish holdings paged [county parish holding]");
    }

    [Theory]
    [InlineData("")]
    [InlineData("True")]
    [InlineData("true")]
    [InlineData("TRUE")]
    [InlineData(null)]
    [InlineData("False")]
    [InlineData("false")]
    [InlineData("FALSE")]
    [InlineData("InvalidValue")]
    [Description(
        "GetAllPaged throws validation exception when request validation fails with given IncludeExpired property")]
    public async Task GetAllPaged_Throws_Validation_Exception_When_Request_Validation_Fails_Given_IncludeExpired(
        string? includeExpired)
    {
        // Arrange
        var request = new GetCphs { PageNumber = 1, PageSize = 1, Expired = includeExpired };

        var validatorContext = MockValidatorContext<PagedQuery>.CreateFor(pagedQueryValidator)
            .WithValidationFailures(
            [
                new ValidationFailure("Random Property 1", "Simulated validation failure 1"),
                new ValidationFailure("Random Property 2", "Simulated validation failure 2"),
            ]);

        var repositoryContext = MockRepositoryContext<UserAccounts>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithOperatorId.GetAllPaged(request, TestContext.Current.CancellationToken);

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
            "Executing get all county parish holdings paged [county parish holding]");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute get all county parish holdings paged [county parish holding] failed validation");
    }

    [Fact]
    [Description("Get returns the requested cph")]
    public async Task Get_Returns_Requested_Cph()
    {
        // Arrange
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var cph4NotExpiredOrDeleted = TestData.Cph.Cph4NotExpiredOrDeleted;
        var cph5ExpiredButNotDeleted = TestData.Cph.Cph5ExpiredButNotDeleted;

        var request = new GetCphByCphId() { Id = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id };

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            cph1WithAllowedSpeciesNotExpiredOrDeleted,
            cph4NotExpiredOrDeleted,
            cph5ExpiredButNotDeleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(cph1WithAllowedSpeciesNotExpiredOrDeleted));

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holding [county parish holding] with id {request.Id}");
    }

    [Fact]
    [Description("Get returns the requested expired cph")]
    public async Task Get_Returns_Requested_Expired_Cph()
    {
        // Arrange
        var cph5ExpiredButNotDeleted = TestData.Cph.Cph5ExpiredButNotDeleted;

        var request = new GetCphByCphId() { Id = cph5ExpiredButNotDeleted.Id };

        MockRepositoryContext<CountyParishHoldings>.CreateFor(repository).WithData(
        [
            cph5ExpiredButNotDeleted,
        ]);

        // Act
        var result = await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        result.ShouldSatisfyAllConditions(Assertions.ShouldMapFromEntity(cph5ExpiredButNotDeleted));

        result.ExpiredAt.ShouldBe(cph5ExpiredButNotDeleted.ExpiredAt);
        result.Expired.ShouldBeTrue();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed get county parish holding [county parish holding] with id {request.Id}");
    }

    [Fact]
    [Description("Get throws not found exception when requested cph does not exist")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_Cph_Does_Not_Exist()
    {
        // Arrange
        var request = new GetCphByCphId() { Id = Guid.NewGuid(), };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Get throws not found exception when requested cph is deleted")]
    public async Task Get_Throws_NotFound_Exception_When_Requested_Cph_Deleted()
    {
        // Arrange
        var cph3DeletedButNotExpired = TestData.Cph.Cph3DeletedButNotExpired;

        var request = new GetCphByCphId() { Id = cph3DeletedButNotExpired.Id, };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData([cph3DeletedButNotExpired]);

        // Act
        Func<Task> act = async () =>
            await sut.WithoutOperatorId.Get(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph3DeletedButNotExpired);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing get county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Expire expires the requested cph")]
    public async Task Expire_Expires_Requested_Cph()
    {
        // Arrange
        var cph4NotExpiredOrDeleted = TestData.Cph.Cph4NotExpiredOrDeleted;
        var originalCph4NotExpiredOrDeleted = TestData.Cph.Cph4NotExpiredOrDeleted;

        var request = new ExpireCphByCphId() { Id = cph4NotExpiredOrDeleted.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData(
            [
                cph4NotExpiredOrDeleted
            ]);

        // Act
        await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(cph4NotExpiredOrDeleted);

        EntityComparer.CreateFor(originalCph4NotExpiredOrDeleted, cph4NotExpiredOrDeleted)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(CountyParishHoldings.ExpiredAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(expiredEntity =>
        {
            expiredEntity.ExpiredAt.ShouldNotBeNull();
            expiredEntity.ExpiredAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed expire county parish holding [county parish holding] with id {request.Id}");
    }

    [Fact]
    [Description("Expire throws not found exception when cph does not exist")]
    public async Task Expire_Throws_NotFound_Exception_When_Cph_Does_Not_Exist()
    {
        // Arrange
        var request = new ExpireCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        var act = async () => await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Expire throws conflict exception when cph is already expired")]
    public async Task Expire_Throws_Conflict_Exception_When_Cph_Already_Expired()
    {
        // Arrange
        var cph2ExpiredButNotDeleted = TestData.Cph.Cph2ExpiredButNotDeleted;

        var request = new ExpireCphByCphId() { Id = cph2ExpiredButNotDeleted.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData(
            [
                cph2ExpiredButNotDeleted
            ]);

        // Act
        var act = async () => await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<ConflictException>();
        exception.Message.ShouldBe("County parish holding must not have already expired");

        // Check that the repository returned the expired item for evaluation of the expired state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph2ExpiredButNotDeleted);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"Execute expire county parish holding [county parish holding] with id {request.Id} failed conflict rule 'County parish holding must not have already expired'");
    }

    [Fact]
    [Description("Expire throws not found exception when cph is deleted")]
    public async Task Expire_Throws_NotFound_Exception_When_Cph_Deleted()
    {
        // Arrange
        var cph3DeletedButNotExpired = TestData.Cph.Cph3DeletedButNotExpired;

        var request = new ExpireCphByCphId() { Id = cph3DeletedButNotExpired.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData(
            [
                cph3DeletedButNotExpired
            ]);

        // Act
        var act = async () => await sut.WithOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph3DeletedButNotExpired);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing expire county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Expire throws invalid operation exception when operator context is not provided")]
    public async Task Expire_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new ExpireCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorContext.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<InvalidOperationException>();

        exception.Message.ShouldContain("Operator context must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Expire throws unauthorised access exception when operator id is not provided")]
    public async Task Expire_Throws_Unauthorised_Access_Exception_When_Operator_Id_Not_Provided()
    {
        // Arrange
        var request = new ExpireCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Expire(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }

    [Fact]
    [Description("Delete soft deletes cph")]
    public async Task Delete_Soft_Deletes_Cph()
    {
        var cph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;
        var originalCph1WithAllowedSpeciesNotExpiredOrDeleted = TestData.Cph.Cph1WithAllowedSpeciesNotExpiredOrDeleted;

        var request = new DeleteCphByCphId() { Id = cph1WithAllowedSpeciesNotExpiredOrDeleted.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData([cph1WithAllowedSpeciesNotExpiredOrDeleted]);

        // Act
        await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        repositoryContext.Calls.UpdateCallCount.ShouldBe(1);
        repositoryContext.Calls.LastUpdateResult.ShouldNotBeNull();
        repositoryContext.Calls.LastUpdateResult.ShouldBe(cph1WithAllowedSpeciesNotExpiredOrDeleted);

        EntityComparer.CreateFor(
                originalCph1WithAllowedSpeciesNotExpiredOrDeleted,
                repositoryContext.Calls.LastUpdateResult)
            .VariancesAtTopLevelWithoutObjectsOrEnumerables
            .ShouldOnlyHaveChanged(nameof(CountyParishHoldings.DeletedById), nameof(CountyParishHoldings.DeletedAt));

        repositoryContext.Calls.LastUpdateResult.ShouldSatisfyAllConditions(softDeletedEntity =>
        {
            softDeletedEntity.DeletedById.ShouldBe(operatorContext!.OperatorId);
            softDeletedEntity.DeletedAt.ShouldNotBeNull();
            softDeletedEntity.DeletedAt.Value.ShouldBeCloseToUtcNow();
        });

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding [county parish holding] with id {request.Id}");

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Successfully executed delete county parish holding [county parish holding] with id {request.Id}");
    }

    [Fact]
    [Description("Delete throws not found exception when cph does not exist")]
    public async Task Delete_Throws_NotFound_Exception_When_Cph_Does_Not_Exist()
    {
        // Arrange
        var request = new DeleteCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBeNull();
        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding [county parish holding] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws not found exception when cph is deleted")]
    public async Task Delete_Throws_NotFound_Exception_When_Cph_Deleted()
    {
        // Arrange
        var cph3DeletedButNotExpired = TestData.Cph.Cph3DeletedButNotExpired;

        var request = new DeleteCphByCphId() { Id = cph3DeletedButNotExpired.Id };

        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository)
            .WithData([cph3DeletedButNotExpired]);

        // Act
        var act = async () => await sut.WithOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<NotFoundException>();
        exception.Message.ShouldBe("County parish holding not found");

        // Check that the repository returned the deleted item for evaluation of the deleted state
        repositoryContext.Calls.GetCallCount.ShouldBe(1);
        repositoryContext.Calls.LastGetResult.ShouldBe(cph3DeletedButNotExpired);

        repositoryContext.Calls.UpdateCallCount.ShouldBe(0);

        logger.VerifyLogContainsOne(
            LogLevel.Information,
            $"Executing delete county parish holding [county parish holding] with id {request.Id} by operator {operatorContext!.OperatorId}");

        logger.VerifyLogContainsOne(
            LogLevel.Warning,
            $"County parish holding with id {request.Id} not found");
    }

    [Fact]
    [Description("Delete throws invalid operation exception when operator context is not provided")]
    public async Task Delete_Throws_Invalid_Operation_Exception_When_Operator_Context_Not_Provided()
    {
        // Arrange
        var request = new DeleteCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

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
        var request = new DeleteCphByCphId() { Id = Guid.NewGuid() };
        var repositoryContext = MockRepositoryContext<CountyParishHoldings>.CreateFor(repository);

        // Act
        var act = async () =>
            await sut.WithoutOperatorId.Delete(request, TestContext.Current.CancellationToken);

        // Assert
        var exception = await act.ShouldThrowAsync<UnauthorizedAccessException>();

        exception.Message.ShouldContain("Operator id must be provided for this operation");

        repositoryContext.Calls.ShouldHaveNoCalls();
    }
}
