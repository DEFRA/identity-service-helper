// <copyright file="CphServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Cphs;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Exceptions;
using Defra.Identity.Requests.Cphs.Commands;
using Defra.Identity.Requests.Cphs.Queries;
using Defra.Identity.Services.Cphs;
using Defra.Identity.Services.Exceptions;
using Defra.Identity.Services.Tests.Cphs.TestData;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class CphServiceTests
{
    [Fact]
    [Description("GetAllPaged Should return page one results in ascending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsAscendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CphNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in ascending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsAscendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in ascending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsAscendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CphNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in ascending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsAscendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("802428bd-0411-451b-b75c-2fb6c037f271")),
            (x) => x.CphNumber.ShouldBe("44/100/0005"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in descending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsDescendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CphNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in descending order and does not return expired or deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsDescendingOrderAndDoesNotReturnExpiredOrDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0001"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("GetAllPaged Should return page one results in descending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageOneResultsDescendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0007"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("802428bd-0411-451b-b75c-2fb6c037f271")),
            (x) => x.CphNumber.ShouldBe("44/100/0005"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-10").ToUniversalTime()));
    }

    [Fact]
    [Description("GetAllPaged Should return page two results in descending order with expired and does not return deleted")]
    public async Task GetAllPaged_ShouldReturnPageTwoResultsDescendingOrderWithExpiredAndDoesNotReturnDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetPaged(
                Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
                Arg.Any<int>(),
                Arg.Any<int>(),
                Arg.Any<Expression<Func<CountyParishHoldings, string>>>(),
                Arg.Any<bool>(),
                Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.MockGetAllPagedEntitiesResultFromCallInfo);

        var request = new GetCphs()
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
            (x) => x.CphNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CphNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("Get Should return result when item is not expired and not deleted")]
    public async Task Get_ShouldReturnResultWhenItemIsNotExpiredAndNotDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new GetCph()
        {
            Id = new Guid("77b9c956-2780-4b48-9abc-71bf505466f9"),
        };

        // Act
        var result = await cphService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");

        result.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("77b9c956-2780-4b48-9abc-71bf505466f9")),
            (x) => x.CphNumber.ShouldBe("44/100/0004"),
            (x) => x.Expired.ShouldBe(false),
            (x) => x.ExpiredAt.ShouldBeNull());
    }

    [Fact]
    [Description("Get Should return result when item is expired and not deleted")]
    public async Task Get_ShouldReturnResultWhenItemIsExpiredAndNotDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new GetCph()
        {
            Id = new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46"),
        };

        // Act
        var result = await cphService.Get(request, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");

        result.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("e7009d6d-0a29-4e3f-ac0b-7bf0c7497f46")),
            (x) => x.CphNumber.ShouldBe("44/100/0002"),
            (x) => x.Expired.ShouldBe(true),
            (x) => x.ExpiredAt.ShouldBe(DateTime.Parse("2026-02-12").ToUniversalTime()));
    }

    [Fact]
    [Description("Get Should throw not found exception when item is deleted")]
    public void Get_ShouldThrowNotFoundExceptionWhenItemIsDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new GetCph()
        {
            Id = new Guid("5bc8f1a5-2d44-40b5-93e4-52b613bf099f"),
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Get(request, TestContext.Current.CancellationToken));

        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");
        logger.VerifyLogReceivedOnce(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");
    }

    [Fact]
    [Description("Get Should throw not found exception when item is not in the repository")]
    public void Get_ShouldThrowNotFoundExceptionWhenItemIsNotInRepository()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var nonExistingEntityId = new Guid("109d340f-16b7-45fc-83d4-9ea8968df112");

        var request = new GetCph()
        {
            Id = nonExistingEntityId,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Get(request, TestContext.Current.CancellationToken));

        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Getting county parish holding by id {request.Id.ToString()}");
        logger.VerifyLogReceivedOnce(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");
    }

    [Fact]
    [Description("Expire Should expire none expired and none deleted item")]
    public async Task Expire_ShouldExpireNoneExpiredAndNonDeletedItem()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new ExpireCph()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act
        await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken);

        // Assert
        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");

        await repository.Received(1).Update(Arg.Is<CountyParishHoldings>(v => v.ExpiredAt != null), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw conflict exception when already expired")]
    public async Task Expire_ShouldThrowConflictExceptionWhenAlreadyExpired()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new ExpireCph()
        {
            Id = new Guid("802428bd-0411-451b-b75c-2fb6c037f271"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<ConflictException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogReceivedOnce(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} is already expired");

        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw not found exception when deleted")]
    public async Task Expire_ShouldThrowNotFoundExceptionWhenDeleted()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var request = new ExpireCph()
        {
            Id = new Guid("a4343f59-011c-46dc-a9fe-553923338e0a"),
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogReceivedOnce(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }

    [Fact]
    [Description("Expire Should throw not found exception when entity does not exist")]
    public async Task Expire_ShouldThrowNotFoundExceptionWhenEntityDoesNotExist()
    {
        // Arrange
        var logger = Substitute.For<ILogger<CphService>>();
        var repository = Substitute.For<ICphRepository>();
        var cphService = new CphService(repository, logger);

        repository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(CphServiceTestDataHelper.GetSingleMockEntityResultFromCallInfo);

        var nonExistingEntityId = new Guid("109d340f-16b7-45fc-83d4-9ea8968df112");

        var request = new ExpireCph()
        {
            Id = nonExistingEntityId,
        };

        var operatorId = new Guid("a4ae3558-90b7-48a4-90c4-a32c086ff769");

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await cphService.Expire(request, operatorId, TestContext.Current.CancellationToken));

        logger.VerifyLogReceivedOnce(LogLevel.Information, $"Expiring county parish holding with id {request.Id.ToString()} by operator {operatorId}");
        logger.VerifyLogReceivedOnce(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await repository.DidNotReceive().Update(Arg.Any<CountyParishHoldings>(), Arg.Any<CancellationToken>());
    }
}
