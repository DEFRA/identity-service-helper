// <copyright file="CphHandlerFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable MoveLocalFunctionAfterJumpStatement
namespace Defra.Identity.Api.Tests.Endpoints.Cphs.Factories;

using System.ComponentModel;
using Defra.Identity.Api.Endpoints.Cphs.Factories;
using Defra.Identity.Api.Tests.Endpoints.Cphs.TestData;
using Defra.Identity.Models.Requests;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;

public class CphHandlerFactoryTests
{
    [Fact]
    [Description("Should create cph number reroute handler for target method with query request headers")]
    public void Should_Create_CphNumber_Reroute_Handler_For_QueryRequestHeaders()
    {
        // Arrange
        Task<IResult> FakeEndpointHandler(QueryRequestHeaders queryRequestHeaders, OperationByIdFake operationByIdFake, ICphService cphService)
            => Task.FromResult(Results.Ok(true));

        // Act
        var rerouteHandler =
            CphHandlerFactory.CreateCphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake>(
                (Func<QueryRequestHeaders, OperationByIdFake, ICphService, Task<IResult>>)FakeEndpointHandler);

        // Assert
        rerouteHandler.ShouldNotBeNull();
        rerouteHandler.ShouldBeOfType<Func<QueryRequestHeaders, OperationByCphNumberFake, ICphService, Task<IResult>>>();
    }

    [Fact]
    [Description("Should create cph number reroute handler for target method with command request headers")]
    public void Should_Create_CphNumber_Reroute_Handler_For_CommandRequestHeaders()
    {
        // Arrange
        Task<IResult> FakeEndpointHandler(CommandRequestHeaders queryRequestHeaders, OperationByIdFake operationByIdFake, ICphService cphService)
            => Task.FromResult(Results.Ok(true));

        // Act
        var rerouteHandler =
            CphHandlerFactory.CreateCphNumberRerouteHandler<OperationByIdFake, OperationByCphNumberFake>(
                (Func<CommandRequestHeaders, OperationByIdFake, ICphService, Task<IResult>>)FakeEndpointHandler);

        // Assert
        rerouteHandler.ShouldNotBeNull();
        rerouteHandler.ShouldBeOfType<Func<CommandRequestHeaders, OperationByCphNumberFake, ICphService, Task<IResult>>>();
    }
}
