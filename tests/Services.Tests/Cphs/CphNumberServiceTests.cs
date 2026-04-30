// <copyright file="CphNumberServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Tests.Cphs.TestData;
using Defra.Identity.Test.Utilities.Repository;
using FluentValidation;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CphNumberServiceTests
{
    private readonly ICphRepository cphRepository = Substitute.For<ICphRepository>();
    private readonly OperationByCphNumberValidator cphNumberValidator = new OperationByCphNumberValidator();
    private readonly ILogger<CphNumberService> logger = Substitute.For<ILogger<CphNumberService>>();
    private readonly ICphNumberService cphNumberService;

    public CphNumberServiceTests()
    {
        cphNumberService = new CphNumberService(cphRepository, cphNumberValidator, logger);
    }

    [Fact]
    [Description("GetIdFromCphNumber Should return an id given a valid cph number")]
    public async Task GetIdFromCphNumber_ShouldReturnIdGivenValidCphNumber()
    {
        // Arrange
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var request = new OperationByCphNumberFake(1, 28, 1);
        const string formattedCphNumber = "01/028/0001";

        // Act
        var result = await cphNumberService.GetIdFromCphNumber(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");

        result.ShouldBe(new Guid("68625a5c-7999-4394-836f-9ee55cac0a21"));
    }

    [Fact]
    [Description("GetIdFromCphNumber Should thrown not found exception when cph is deleted")]
    public void GetIdFromCphNumber_ShouldThrowNotFoundExceptionWhenItemIsDeleted()
    {
        // Arrange
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var request = new OperationByCphNumberFake(1, 28, 2);
        const string formattedCphNumber = "01/028/0002";

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphNumberService.GetIdFromCphNumber(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with cph number {formattedCphNumber} not found");
    }

    [Fact]
    [Description("GetIdFromCphNumber Should thrown not found exception when cph is not found")]
    public void GetIdFromCphNumber_ShouldThrowNotFoundExceptionWhenItemIsNotFound()
    {
        // Arrange
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntitiesForSimpleFilterChecks()));

        var requestWithNonExistingCphNumber = new OperationByCphNumberFake(1, 28, 100);
        const string formattedCphNumber = "01/028/0100";

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphNumberService.GetIdFromCphNumber(requestWithNonExistingCphNumber, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting county parish holding id by cph number {formattedCphNumber}");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with cph number {formattedCphNumber} not found");
    }

    [Fact]
    [Description("GetIdFromCphNumber Should throw validation exception when cph number invalid")]
    public void GetIdFromCphNumber_ShouldThrowValidationExceptionWhenCphNumberInvalid()
    {
        // Arrange
        var requestWithNonExistingCphNumber = new OperationByCphNumberFake(999, 9999, 99999);

        // Act & Assert
        Should.Throw<ValidationException>(async () => await cphNumberService.GetIdFromCphNumber(requestWithNonExistingCphNumber, TestContext.Current.CancellationToken));
    }
}
