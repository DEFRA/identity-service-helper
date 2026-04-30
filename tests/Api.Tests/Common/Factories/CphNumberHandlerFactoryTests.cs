// <copyright file="CphNumberHandlerFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable MoveLocalFunctionAfterJumpStatement
namespace Defra.Identity.Api.Tests.Common.Factories;

using System.ComponentModel;
using Defra.Identity.Api.Common.Factories;
using Defra.Identity.Api.Middleware.Headers;
using Defra.Identity.Api.Tests.Common.TestData;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class CphNumberHandlerFactoryTests
{
    private readonly ICphNumberService cphNumberService = Substitute.For<ICphNumberService>();

    [Fact]
    [Description("Should create cph number reroute handler for target method with query request headers")]
    public void Should_Create_CphNumber_Reroute_Handler_For_QueryRequestHeaders()
    {
        // Arrange
        Task<IResult> FakeEndpointHandler(QueryRequestHeaders queryRequestHeaders, OperationByIdFake operationByIdFake, ServiceFake cphService)
            => Task.FromResult(Results.Ok(true));

        // Act
        var rerouteHandler = new
            CphNumberHandlerFactory<ServiceFake>(cphNumberService).CreateRerouteHandler<OperationByIdFake, OperationByCphNumberFake>(
            (Func<QueryRequestHeaders, OperationByIdFake, ServiceFake, Task<IResult>>)FakeEndpointHandler);

        // Assert
        rerouteHandler.ShouldNotBeNull();
        rerouteHandler.ShouldBeOfType<Func<QueryRequestHeaders, OperationByCphNumberFake, ServiceFake, Task<IResult>>>();
    }

    [Fact]
    [Description("Should create cph number reroute handler for target method with command request headers")]
    public void Should_Create_CphNumber_Reroute_Handler_For_CommandRequestHeaders()
    {
        // Arrange
        Task<IResult> FakeEndpointHandler(CommandRequestHeaders queryRequestHeaders, OperationByIdFake operationByIdFake, ServiceFake cphService)
            => Task.FromResult(Results.Ok(true));

        // Act
        var rerouteHandler = new
            CphNumberHandlerFactory<ServiceFake>(cphNumberService).CreateRerouteHandler<OperationByIdFake, OperationByCphNumberFake>(
            (Func<CommandRequestHeaders, OperationByIdFake, ServiceFake, Task<IResult>>)FakeEndpointHandler);

        // Assert
        rerouteHandler.ShouldNotBeNull();
        rerouteHandler.ShouldBeOfType<Func<CommandRequestHeaders, OperationByCphNumberFake, ServiceFake, Task<IResult>>>();
    }
}
