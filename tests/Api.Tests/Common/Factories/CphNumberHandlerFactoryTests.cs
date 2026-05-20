// <copyright file="CphNumberHandlerFactoryTests.cs" company="Defra">
// Copyright (c) Defra. All rights reserved.
// </copyright>

// ReSharper disable MoveLocalFunctionAfterJumpStatement
namespace Defra.Identity.Api.Tests.Common.Factories;

using System.ComponentModel;
using Defra.Identity.Api.Common.Factories;
using Defra.Identity.Api.Tests.Common.TestData;
using Defra.Identity.Services.Cphs;
using Microsoft.AspNetCore.Http;
using NSubstitute;

public class CphNumberHandlerFactoryTests
{
    private readonly ICphNumberService cphNumberService = Substitute.For<ICphNumberService>();

    [Fact]
    [Description("Should create cph number reroute handler for target method")]
    public void Should_Create_CphNumber_Reroute_Handler()
    {
        // Arrange
        Task<IResult> FakeEndpointHandler(OperationByIdFake operationByIdFake, ServiceFake cphService)
            => Task.FromResult(Results.Ok(true));

        // Act
        var rerouteHandler = new
            CphNumberHandlerFactory<ServiceFake>(cphNumberService).CreateRerouteHandler<OperationByIdFake, OperationByCphNumberFake>(
            (Func<OperationByIdFake, ServiceFake, Task<IResult>>)FakeEndpointHandler);

        // Assert
        rerouteHandler.ShouldNotBeNull();
        rerouteHandler.ShouldBeOfType<Func<OperationByCphNumberFake, ServiceFake, Task<IResult>>>();
    }
}
