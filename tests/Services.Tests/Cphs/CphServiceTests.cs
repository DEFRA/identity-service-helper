// <copyright file="CphServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Cphs.Users;
using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Common;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Services.Common.Exceptions;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Tests.Cphs.TestData;
using Defra.Identity.Test.Utilities.Repository;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CphServiceTests
{
    [Fact]
    [Description("GetIdFromCphNumber Should return an id given a valid cph number")]
    public async Task GetIdFromCphNumber_ShouldReturnIdGivenValidCphNumber()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = new OperationByCphNumberValidator();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var request = new OperationByCphNumberFake(1, 28, 1);
        const string formattedCphNumber = "01/028/0001";

        // Act
        var result = await cphService.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");

        result.ShouldBe(new Guid("68625a5c-7999-4394-836f-9ee55cac0a21"));
    }

    [Fact]
    [Description("GetIdFromCphNumber Should thrown not found exception when cph is deleted")]
    public void GetIdFromCphNumber_ShouldThrowNotFoundExceptionWhenItemIsDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = new OperationByCphNumberValidator();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var request = new OperationByCphNumberFake(1, 28, 2);
        const string formattedCphNumber = "01/028/0002";

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.GetIdFromCphNumber(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with cph number {formattedCphNumber} not found");
    }

    [Fact]
    [Description("GetIdFromCphNumber Should thrown not found exception when cph is not found")]
    public void GetIdFromCphNumber_ShouldThrowNotFoundExceptionWhenItemIsNotFound()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = new OperationByCphNumberValidator();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var requestWithNonExistingCphNumber = new OperationByCphNumberFake(1, 28, 100);
        const string formattedCphNumber = "01/028/0100";

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.GetIdFromCphNumber(requestWithNonExistingCphNumber, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with cph number {formattedCphNumber} not found");
    }

    [Fact]
    [Description("GetIdFromCphNumber Should throw validation exception when cph number invalid")]
    public void GetIdFromCphNumber_ShouldThrowValidationExceptionWhenCphNumberInvalid()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = new OperationByCphNumberValidator();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        var requestWithNonExistingCphNumber = new OperationByCphNumberFake(999, 9999, 99999);

        // Act & Assert
        Should.Throw<ValidationException>(async () => await cphService.GetIdFromCphNumber(requestWithNonExistingCphNumber, TestContext.Current.CancellationToken));
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in ascending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsAscendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = null, PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in ascending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsAscendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = null, PageNumber = 2, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in ascending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsAscendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = string.Empty, PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in ascending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsAscendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = string.Empty, PageNumber = 2, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("802428bd-0411-451b-b75c-2fb6c037f271")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0005"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in descending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsDescendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = null, PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in descending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsDescendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = null, PageNumber = 2, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in descending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsDescendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = string.Empty, PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("802428bd-0411-451b-b75c-2fb6c037f271")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0005"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in descending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsDescendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetAllPagedEntitiesResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphs
        {
            Expired = string.Empty, PageNumber = 2, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.Received(1).Log(LogLevel.Information, "Getting all county parish holdings by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(5),
            (x) => x.TotalPages.ShouldBe(3));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("Get Should return result when item is not expired and not deleted")]
    public async Task Get_ShouldReturnResultWhenItemIsNotExpiredAndNotDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphByCphId
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"),
        };

        // Act
        var result = await cphService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");

        result.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("Get Should return result when item is expired and not deleted")]
    public async Task Get_ShouldReturnResultWhenItemIsExpiredAndNotDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new GetCphByCphId
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"),
        };

        // Act
        var result = await cphService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");

        result.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CountyParishHoldingNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("Get Should throw not found exception when item is deleted")]
    public void Get_ShouldThrowNotFoundExceptionWhenItemIsDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
                new CountyParishHoldings()
                {
                    Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"), DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                });

        var request = new GetCphByCphId
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"),
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Get(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");
    }

    [Fact]
    [Description("Get Should throw not found exception when the entity does not exist")]
    public void Get_ShouldThrowNotFoundExceptionWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CountyParishHoldings?>(null));

        var nonExistingEntityId = new Guid("109d340f-16b7-45fc-83d4-9ea8968df112");

        var request = new GetCphByCphId
        {
            Id = nonExistingEntityId,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Get(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");
    }

    [Fact]
    [Description("Expire Should expire none expired and none deleted item")]
    public async Task Expire_ShouldExpireNoneExpiredAndNonDeletedItem()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new ExpireCphByCphId
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act
        await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");

        await cphRepository.Received(1).Update(Arg.Is<CountyParishHoldings>(v => v.ExpiredAt != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw conflict exception when already expired")]
    public async Task Expire_ShouldThrowConflictExceptionWhenAlreadyExpired()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new ExpireCphByCphId
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<ConflictException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} is already expired");

        await cphRepository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw not found exception when deleted")]
    public async Task Expire_ShouldThrowNotFoundExceptionWhenDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
                new CountyParishHoldings()
                {
                    Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"), DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                });

        var request = new ExpireCphByCphId
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphRepository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw not found exception when entity does not exist")]
    public async Task Expire_ShouldThrowNotFoundExceptionWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CountyParishHoldings?>(null));

        var nonExistingEntityId = new Guid("109d340f-16b7-45fc-83d4-9ea8968df112");

        var request = new ExpireCphByCphId
        {
            Id = nonExistingEntityId,
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphRepository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Delete Should delete none deleted item")]
    public async Task Delete_ShouldDeleteNoneDeletedItem()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        var request = new DeleteCphByCphId
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act
        await cphService.Delete(request, operatorId, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Deleting county parish holding with id {request.Id.ToString()} by operator {operatorId}");

        await cphRepository.Received(1).Update(Arg.Is<CountyParishHoldings>(v => v.DeletedAt != null && v.DeletedById == operatorId), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Delete Should throw not found exception when deleted")]
    public async Task Delete_ShouldThrowNotFoundExceptionWhenDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
                new CountyParishHoldings()
                {
                    Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"), DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                });

        var request = new DeleteCphByCphId
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Delete(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Deleting county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphRepository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw not found exception when entity does not exist")]
    public async Task Delete_ShouldThrowNotFoundExceptionWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CountyParishHoldings?>(null));

        var nonExistingEntityId = new Guid("109d340f-16b7-45fc-83d4-9ea8968df112");

        var request = new DeleteCphByCphId
        {
            Id = nonExistingEntityId,
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Delete(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Deleting county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphRepository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page one results in ascending order and does not return deleted")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageOneResultsAscendingOrderAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e")),
            (x) => x.UserId.ShouldBe(new Guid("95bdde08-b510-40e3-a09d-6d4c48f122b2")),
            (x) => x.ApplicationId.ShouldBe(new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148")),
            (x) => x.RoleId.ShouldBe(new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585")),
            (x) => x.Email.ShouldBe("test101@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 101"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce")),
            (x) => x.UserId.ShouldBe(new Guid("d686d63e-a9a0-469a-a864-a2c33436f9a7")),
            (x) => x.ApplicationId.ShouldBe(new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148")),
            (x) => x.RoleId.ShouldBe(new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585")),
            (x) => x.Email.ShouldBe("test102@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 102"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page two results in ascending order and does not return deleted")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageTwoResultsAscendingOrderAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 2, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("05425759-8e3c-4800-abba-bc1d77a97a92")),
            (x) => x.UserId.ShouldBe(new Guid("a2b746a7-e733-40d3-a7e8-5f9522deae2b")),
            (x) => x.ApplicationId.ShouldBe(new Guid("f81bbbe9-8eba-4a86-8e65-a08348219f06")),
            (x) => x.RoleId.ShouldBe(new Guid("42452ec5-8393-4674-8968-f4929be60099")),
            (x) => x.Email.ShouldBe("test104@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 104"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page one results in descending order and does not return deleted")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageOneResultsDescendingOrderAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("05425759-8e3c-4800-abba-bc1d77a97a92")),
            (x) => x.UserId.ShouldBe(new Guid("a2b746a7-e733-40d3-a7e8-5f9522deae2b")),
            (x) => x.ApplicationId.ShouldBe(new Guid("f81bbbe9-8eba-4a86-8e65-a08348219f06")),
            (x) => x.RoleId.ShouldBe(new Guid("42452ec5-8393-4674-8968-f4929be60099")),
            (x) => x.Email.ShouldBe("test104@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 104"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce")),
            (x) => x.UserId.ShouldBe(new Guid("d686d63e-a9a0-469a-a864-a2c33436f9a7")),
            (x) => x.ApplicationId.ShouldBe(new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148")),
            (x) => x.RoleId.ShouldBe(new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585")),
            (x) => x.Email.ShouldBe("test102@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 102"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page two results in descending order and does not return deleted")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageTwoResultsDescendingOrderAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 2, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(1),
            (x) => x.PageNumber.ShouldBe(2),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(3),
            (x) => x.TotalPages.ShouldBe(2));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e")),
            (x) => x.UserId.ShouldBe(new Guid("95bdde08-b510-40e3-a09d-6d4c48f122b2")),
            (x) => x.ApplicationId.ShouldBe(new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148")),
            (x) => x.RoleId.ShouldBe(new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585")),
            (x) => x.Email.ShouldBe("test101@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 101"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page one results for different cph in ascending order")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageOneResultsForDifferentCphAscendingOrder()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(2),
            (x) => x.TotalPages.ShouldBe(1));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f")),
            (x) => x.UserId.ShouldBe(new Guid("43426677-8dba-46d0-b429-d7192dfeb6f5")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test105@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 105"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e")),
            (x) => x.UserId.ShouldBe(new Guid("75db555e-b686-40ff-abdb-e2683b91feb1")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test106@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 106"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page one results for different cph in ascending order")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageOneResultsForDifferentCphDescendingOrder()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphUsersRepository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(
                callInfo => PredicateInterceptor.MockGetAllPagedAssociatedEntitiesResult(
                    callInfo,
                    CphRepositoryMockingHelper.GetCphEntities(),
                    CphRepositoryMockingHelper.GetCphUserEntities(),
                    (cph, cphUser) => cph.Id == cphUser.CountyParishHoldingId));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");

        pagedResults.ShouldSatisfyAllConditions(
            (x) => x.Items.Count().ShouldBe(2),
            (x) => x.PageNumber.ShouldBe(1),
            (x) => x.PageSize.ShouldBe(2),
            (x) => x.TotalCount.ShouldBe(2),
            (x) => x.TotalPages.ShouldBe(1));

        var pagedResultItems = pagedResults.Items.ToList();

        var firstItem = pagedResultItems[0];
        var secondItem = pagedResultItems[1];

        firstItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e")),
            (x) => x.UserId.ShouldBe(new Guid("75db555e-b686-40ff-abdb-e2683b91feb1")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test106@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 106"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.AssociationId.ShouldBe(new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f")),
            (x) => x.UserId.ShouldBe(new Guid("43426677-8dba-46d0-b429-d7192dfeb6f5")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test105@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 105"));
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should throw not found exception when entity is deleted")]
    public async Task GetAllCphUsersPaged_ShouldThrowNotFoundExceptionWhenEntityDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
                new CountyParishHoldings()
                {
                    Id = new Guid("cb84868b-00c1-4981-bb66-b6e45f9391f1"), DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                });

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("cb84868b-00c1-4981-bb66-b6e45f9391f1"), PageNumber = 1, PageSize = 2,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphUsersRepository.DidNotReceive().GetPaged(
            Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should throw not found exception when entity is deleted")]
    public async Task GetAllCphUsersPaged_ShouldThrowNotFoundExceptionWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var cphRepository = Substitute.For<ICphRepository>();
        var cphUsersRepository = Substitute.For<ICphAssociatedUsersRepository>();
        var cphNumberValidator = Substitute.For<IValidator<IOperationByCphNumber>>();
        var cphService = new CphService(cphRepository, cphUsersRepository, cphNumberValidator, logger);

        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CountyParishHoldings?>(null));

        var request = new GetCphUsersByCphId()
        {
            Id = new Guid("52b5185d-c03e-475f-8a60-52b6b75b6d90"), PageNumber = 1, PageSize = 2,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.GetAllCphUsersPaged(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphUsersRepository.DidNotReceive().GetPaged(
            Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }
}
