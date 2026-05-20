// <copyright file="CphNumberRerouteHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Common.Handlers;

using System.ComponentModel;
using Defra.Identity.Api.Common.Handlers;
using Defra.Identity.Api.Tests.Common.TestData;
using Defra.Identity.Models.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class CphNumberRerouteHandlerTests
{
    private readonly ICphNumberService cphNumberService = Substitute.For<ICphNumberService>();

    [Fact]
    [Description("Should not throw an exception when cph number service and the reroute target handler is provided")]
    public void Should_Not_Throw_Exception_When_Cph_Number_Service_And_Target_Handler_Provided()
    {
        // Arrange
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));

        // Act & Assert
        Should.NotThrow(
            () => new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, ServiceFake>(
                cphNumberService,
                EndpointFake<OperationByIdFake>.Create(Results.Ok(fakeEndpointHandlerResult)).FakeHandlerMethod));
    }

    [Fact]
    [Description("Should throw an exception when cph number service is null")]
    public void Should_Throw_Exception_When_Cph_Number_Service_Is_Null()
    {
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(Guid.NewGuid(), string.Empty));
        var fakeEndpoints = EndpointFake<OperationByIdFake>.Create(fakeEndpointHandlerResult);
        var fakeEndpointsHandler = fakeEndpoints.FakeHandlerMethod;

        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, ServiceFake>(null!, fakeEndpointsHandler));
    }

    [Fact]
    [Description("Should throw an exception when the reroute target handler is null")]
    public void Should_Throw_Exception_When_Target_Handler_Is_Null()
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, ServiceFake>(cphNumberService, null!));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id request and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberFake(44, 001, 9999);
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));

        var fakeEndpoints = EndpointFake<OperationByIdFake>.Create(fakeEndpointHandlerResult);
        var fakeEndpointsHandler = fakeEndpoints.FakeHandlerMethod;
        var fakeService = new ServiceFake();

        cphNumberService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("63bd6b64-9d67-4076-9855-507289ba4067"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, ServiceFake>(cphNumberService, fakeEndpointsHandler).Handler(
                sourceRequestToReroute,
                fakeService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await cphNumberService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        fakeEndpoints.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("63bd6b64-9d67-4076-9855-507289ba4067")),
            (x) => x.CapturedService.ShouldNotBeNull(),
            (x) => x.CapturedService.ShouldBe(fakeService));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id, ascending paging values and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_AscendingPaging_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberWithPagingFake(44, 001, 9999, 2, 10, false);
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));
        var fakeEndpoints = EndpointFake<OperationByIdWithPagingFake>.Create(fakeEndpointHandlerResult);
        var fakeEndpointsHandler = fakeEndpoints.FakeHandlerMethod;
        var fakeService = new ServiceFake();

        cphNumberService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("cafcdc34-150e-478c-81e0-9d247188ba15"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdWithPagingFake, OperationByCphNumberWithPagingFake, ServiceFake>(cphNumberService, fakeEndpointsHandler).Handler(
                sourceRequestToReroute,
                fakeService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await cphNumberService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        fakeEndpoints.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("cafcdc34-150e-478c-81e0-9d247188ba15")),
            (x) => x.CapturedRequest!.PageNumber.ShouldBe(2),
            (x) => x.CapturedRequest!.PageSize.ShouldBe(10),
            (x) => x.CapturedRequest!.OrderByDescending.ShouldBe(false),
            (x) => x.CapturedService.ShouldNotBeNull(),
            (x) => x.CapturedService.ShouldBe(fakeService));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id, ascending paging values and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_DescendingPaging_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberWithPagingFake(55, 002, 8888, 3, 5, true);
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));

        var fakeEndpoints = EndpointFake<OperationByIdWithPagingFake>.Create(fakeEndpointHandlerResult);
        var fakeEndpointsHandler = fakeEndpoints.FakeHandlerMethod;
        var fakeService = new ServiceFake();

        cphNumberService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("8cd5ae10-4567-4213-b865-ba301ac9e0e6"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdWithPagingFake, OperationByCphNumberWithPagingFake, ServiceFake>(cphNumberService, fakeEndpointsHandler).Handler(
                sourceRequestToReroute,
                fakeService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await cphNumberService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        fakeEndpoints.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("8cd5ae10-4567-4213-b865-ba301ac9e0e6")),
            (x) => x.CapturedRequest!.PageNumber.ShouldBe(3),
            (x) => x.CapturedRequest!.PageSize.ShouldBe(5),
            (x) => x.CapturedRequest!.OrderByDescending.ShouldBe(true),
            (x) => x.CapturedService.ShouldNotBeNull(),
            (x) => x.CapturedService.ShouldBe(fakeService));
    }
}
