// <copyright file="PermissionsServiceTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Services.Tests.Permissions;

using System.ComponentModel;
using System.Linq.Expressions;
using Defra.Identity.Models.Requests.Cphs.Queries;
using Defra.Identity.Postgres.Database.Entities;
using Defra.Identity.Repositories.Assignments;
using Defra.Identity.Repositories.Common.Exceptions;
using Defra.Identity.Repositories.Cphs;
using Defra.Identity.Repositories.Delegations;
using Defra.Identity.Repositories.Users;
using Defra.Identity.Services.Common.Builders.Strategy.Factories;
using Defra.Identity.Services.Permissions;
using Defra.Identity.Services.Tests.Cphs.TestData;
using Defra.Identity.Test.Utilities.Repository;
using Microsoft.Extensions.Logging;
using NSubstitute;

public class PermissionsServiceTests
{
    private readonly IUsersRepository usersRepository = Substitute.For<IUsersRepository>();
    private readonly ICphRepository cphRepository = Substitute.For<ICphRepository>();
    private readonly ICphAssignmentsRepository cphAssignmentsRepository = Substitute.For<ICphAssignmentsRepository>();
    private readonly ICphAssignmentsForAssigneeRepository cphAssignmentsForAssigneeRepository = Substitute.For<ICphAssignmentsForAssigneeRepository>();
    private readonly ICphDelegationsForDelegateRepository cphDelegationsForDelegateRepository = Substitute.For<ICphDelegationsForDelegateRepository>();
    private readonly ICphDelegatesForCphAssigneeRepository cphDelegatesForCphAssigneeRepository = Substitute.For<ICphDelegatesForCphAssigneeRepository>();
    private readonly IStrategyBuilderFactory<PermissionsService> strategyBuilderFactory = new StrategyBuilderFactory<PermissionsService>();
    private readonly ILogger<PermissionsService> logger = Substitute.For<ILogger<PermissionsService>>();
    private readonly IPermissionsService permissionsService;

    public PermissionsServiceTests()
    {
        permissionsService = new PermissionsService(
            usersRepository,
            cphRepository,
            cphAssignmentsRepository,
            cphAssignmentsForAssigneeRepository,
            cphDelegationsForDelegateRepository,
            cphDelegatesForCphAssigneeRepository,
            strategyBuilderFactory,
            logger);
    }

    [Fact]
    [Description("GetAllCphUsersPaged Should return page one results in ascending order and does not return deleted")]
    public async Task GetAllCphUsersPaged_ShouldReturnPageOneResultsAscendingOrderAndDoesNotReturnDeleted()
    {
        // Arrange
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e")),
            (x) => x.UserId.ShouldBe(new Guid("95bdde08-b510-40e3-a09d-6d4c48f122b2")),
            (x) => x.ApplicationId.ShouldBe(new Guid("0bcd7934-4e18-414a-a6a8-d94d6a45c148")),
            (x) => x.RoleId.ShouldBe(new Guid("81b11eb8-2ac7-468f-a80a-cfeb24f70585")),
            (x) => x.Email.ShouldBe("test101@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 101"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 2, PageSize = 2,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("05425759-8e3c-4800-abba-bc1d77a97a92")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("05425759-8e3c-4800-abba-bc1d77a97a92")),
            (x) => x.UserId.ShouldBe(new Guid("a2b746a7-e733-40d3-a7e8-5f9522deae2b")),
            (x) => x.ApplicationId.ShouldBe(new Guid("f81bbbe9-8eba-4a86-8e65-a08348219f06")),
            (x) => x.RoleId.ShouldBe(new Guid("42452ec5-8393-4674-8968-f4929be60099")),
            (x) => x.Email.ShouldBe("test104@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 104"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("5d04e4fb-cbf7-4ed3-8bfc-38da192ea4ce")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("7140056b-b2ee-40d6-9be1-882bdff30cc2"), PageNumber = 2, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("560ce019-2e6e-4f76-8b86-de302bbceb2e")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), PageNumber = 1, PageSize = 2,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f")),
            (x) => x.UserId.ShouldBe(new Guid("43426677-8dba-46d0-b429-d7192dfeb6f5")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test105@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 105"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(callInfo => PredicateInterceptor.MockGetSingleEntityResult(callInfo, CphRepositoryMockingHelper.GetCphEntities()));

        cphAssignmentsRepository.GetPaged(
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

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("1cd09a5b-6b00-4f30-b03e-8de45130cad6"), PageNumber = 1, PageSize = 2, OrderByDescending = true,
        };

        // Act
        var pagedResults = await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken);

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
            (x) => x.Id.ShouldBe(new Guid("81b3624b-4c2b-4247-be3a-82ae5b76573e")),
            (x) => x.UserId.ShouldBe(new Guid("75db555e-b686-40ff-abdb-e2683b91feb1")),
            (x) => x.ApplicationId.ShouldBe(new Guid("97193f21-877d-4806-9f1b-7ba0730245e4")),
            (x) => x.RoleId.ShouldBe(new Guid("b49960ce-5c27-451b-b0f0-bdd297a933ef")),
            (x) => x.Email.ShouldBe("test106@test.com"),
            (x) => x.DisplayName.ShouldBe("Test 106"));

        secondItem.ShouldSatisfyAllConditions(
            (x) => x.Id.ShouldBe(new Guid("439c56e2-7521-4d6b-9106-b10a91805e9f")),
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(
                new CountyParishHoldings()
                {
                    Id = new Guid("cb84868b-00c1-4981-bb66-b6e45f9391f1"), DeletedAt = DateTime.Parse("2026-02-13").ToUniversalTime(),
                });

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("cb84868b-00c1-4981-bb66-b6e45f9391f1"), PageNumber = 1, PageSize = 2,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphAssignmentsRepository.DidNotReceive().GetPaged(
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
        cphRepository.GetSingle(Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(), Arg.Any<CancellationToken>())
            .Returns(Task.FromResult<CountyParishHoldings?>(null));

        var request = new GetCphAssignmentsByCphId()
        {
            Id = new Guid("52b5185d-c03e-475f-8a60-52b6b75b6d90"), PageNumber = 1, PageSize = 2,
        };

        // Act & Assert
        Should.Throw<NotFoundException>(async () => await permissionsService.GetCphAssignments(request, TestContext.Current.CancellationToken));

        logger.VerifyLogContainsOne(LogLevel.Information, $"Getting all county parish holding users for id {request.Id.ToString()} by page");
        logger.VerifyLogContainsOne(LogLevel.Warning, $"County parish holding with id {request.Id.ToString()} not found");

        await cphAssignmentsRepository.DidNotReceive().GetPaged(
            Arg.Any<Expression<Func<CountyParishHoldings, bool>>>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, bool>>>(),
            Arg.Any<int>(),
            Arg.Any<int>(),
            Arg.Any<Expression<Func<ApplicationUserAccountHoldingAssignments, string>>>(),
            Arg.Any<bool>(),
            Arg.Any<CancellationToken>());
    }
}
