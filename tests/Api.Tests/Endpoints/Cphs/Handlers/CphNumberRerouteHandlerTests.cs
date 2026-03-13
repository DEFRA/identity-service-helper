// <copyright file="CphNumberRerouteHandlerTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

namespace Defra.Identity.Api.Tests.Endpoints.Cphs.Handlers;

using System.ComponentModel;
using Defra.Identity.Api.Endpoints.Cphs.Handlers;
using Defra.Identity.Api.Tests.Endpoints.Cphs.Handlers.TestData;
using Defra.Identity.Requests.Cphs.Common;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class CphNumberRerouteHandlerTests
{
    [Fact]
    [Description("Should not throw an exception when the reroute target handler is provided")]
    public void Should_Not_Throw_Exception_When_Target_Handler_Provided()
    {
        // Arrange
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));

        // Act & Assert
        Should.NotThrow(
            () => new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, HeadersFake>(
                CphEndpointFake<OperationByIdFake>.Create(Results.Ok(fakeEndpointHandlerResult)).FakeHandlerMethod));
    }

    [Fact]
    [Description("Should throw an exception when the reroute target handler is null")]
    public void Should_Throw_Exception_When_Target_Handler_Is_Null()
    {
        // Arrange, Act & Assert
        Should.Throw<ArgumentNullException>(() => new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, HeadersFake>(null!));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id request and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberFake(44, 001, 9999);
        var fakeHeaders = new HeadersFake();
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));
        var mockCphService = Substitute.For<ICphService>();

        var cphEndpointsFake = CphEndpointFake<OperationByIdFake>.Create(fakeEndpointHandlerResult);
        var cphEndpointsFakeHandler = cphEndpointsFake.FakeHandlerMethod;

        mockCphService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("63bd6b64-9d67-4076-9855-507289ba4067"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake, HeadersFake>(cphEndpointsFakeHandler).Handler(
                fakeHeaders,
                sourceRequestToReroute,
                mockCphService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await mockCphService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        cphEndpointsFake.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("63bd6b64-9d67-4076-9855-507289ba4067")),
            (x) => x.CapturedHeaders.ShouldNotBeNull(),
            (x) => x.CapturedHeaders.ShouldBe(fakeHeaders),
            (x) => x.CapturedCphService.ShouldNotBeNull(),
            (x) => x.CapturedCphService.ShouldBe(mockCphService));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id, ascending paging values and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_AscendingPaging_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberWithPagingFake(44, 001, 9999, 2, 10, false);
        var fakeHeaders = new HeadersFake();
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));
        var mockCphService = Substitute.For<ICphService>();

        var cphEndpointsFake = CphEndpointFake<OperationByIdWithPagingFake>.Create(fakeEndpointHandlerResult);
        var cphEndpointsFakeHandler = cphEndpointsFake.FakeHandlerMethod;

        mockCphService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("cafcdc34-150e-478c-81e0-9d247188ba15"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdWithPagingFake, OperationByCphNumberWithPagingFake, HeadersFake>(cphEndpointsFakeHandler).Handler(
                fakeHeaders,
                sourceRequestToReroute,
                mockCphService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await mockCphService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        cphEndpointsFake.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("cafcdc34-150e-478c-81e0-9d247188ba15")),
            (x) => x.CapturedRequest!.PageNumber.ShouldBe(2),
            (x) => x.CapturedRequest!.PageSize.ShouldBe(10),
            (x) => x.CapturedRequest!.OrderByDescending.ShouldBe(false),
            (x) => x.CapturedHeaders.ShouldNotBeNull(),
            (x) => x.CapturedHeaders.ShouldBe(fakeHeaders),
            (x) => x.CapturedCphService.ShouldNotBeNull(),
            (x) => x.CapturedCphService.ShouldBe(mockCphService));
    }

    [Fact]
    [Description("Should reroute the call to the target handler with a resolved id, ascending paging values and parameters")]
    public async Task Should_Reroute_And_Call_TargetHandler_With_ResolvedId_DescindingPaging_And_Parameters()
    {
        // Arrange
        var sourceRequestToReroute = new OperationByCphNumberWithPagingFake(55, 002, 8888, 3, 5, true);
        var fakeHeaders = new HeadersFake();
        var fakeEndpointHandlerResult = Results.Ok(new OperationByIdResultFake(new Guid("56b21ff8-03e7-4a0d-a923-75d83905310f"), "Testing"));
        var mockCphService = Substitute.For<ICphService>();

        var cphEndpointsFake = CphEndpointFake<OperationByIdWithPagingFake>.Create(fakeEndpointHandlerResult);
        var cphEndpointsFakeHandler = cphEndpointsFake.FakeHandlerMethod;

        mockCphService.GetIdFromCphNumber(Arg.Any<IOperationByCphNumber>(), Arg.Any<CancellationToken>())
            .Returns(new Guid("8cd5ae10-4567-4213-b865-ba301ac9e0e6"));

        // Act
        var rerouteHandlerResult =
            await new CphNumberRerouteHandler<OperationByIdWithPagingFake, OperationByCphNumberWithPagingFake, HeadersFake>(cphEndpointsFakeHandler).Handler(
                fakeHeaders,
                sourceRequestToReroute,
                mockCphService);

        // Assert
        rerouteHandlerResult.ShouldBe(fakeEndpointHandlerResult);
        await mockCphService.Received(1).GetIdFromCphNumber(Arg.Is(sourceRequestToReroute), Arg.Any<CancellationToken>());

        cphEndpointsFake.ShouldSatisfyAllConditions(
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedCallCount.ShouldBe(1),
            (x) => x.CapturedRequest.ShouldNotBeNull(),
            (x) => x.CapturedRequest!.Id.ShouldBe(new Guid("8cd5ae10-4567-4213-b865-ba301ac9e0e6")),
            (x) => x.CapturedRequest!.PageNumber.ShouldBe(3),
            (x) => x.CapturedRequest!.PageSize.ShouldBe(5),
            (x) => x.CapturedRequest!.OrderByDescending.ShouldBe(true),
            (x) => x.CapturedHeaders.ShouldNotBeNull(),
            (x) => x.CapturedHeaders.ShouldBe(fakeHeaders),
            (x) => x.CapturedCphService.ShouldNotBeNull(),
            (x) => x.CapturedCphService.ShouldBe(mockCphService));
    }
}
